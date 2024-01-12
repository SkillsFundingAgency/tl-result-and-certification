﻿using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging;
using Sfa.Tl.ResultsAndCertification.Application.Services;
using Sfa.Tl.ResultsAndCertification.Common.Enum;
using Sfa.Tl.ResultsAndCertification.Data.Repositories;
using Sfa.Tl.ResultsAndCertification.Domain.Models;
using Sfa.Tl.ResultsAndCertification.Models.Contracts.AdminDashboard;
using Sfa.Tl.ResultsAndCertification.Models.Contracts.IndustryPlacement;
using Sfa.Tl.ResultsAndCertification.Models.Contracts.Learner;
using Sfa.Tl.ResultsAndCertification.Tests.Common.DataProvider;
using Sfa.Tl.ResultsAndCertification.Tests.Common.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using AutoMapper;
using Sfa.Tl.ResultsAndCertification.Application.Mappers;
using Notify.Interfaces;
using NSubstitute;
using Sfa.Tl.ResultsAndCertification.Models.Configuration;
using Sfa.Tl.ResultsAndCertification.Common.Services.System.Service;
using Sfa.Tl.ResultsAndCertification.Tests.Common.DataBuilders;

namespace Sfa.Tl.ResultsAndCertification.IntegrationTests.Services.AdminDashboardServiceTests
{
    public class When_ProcessReviewChangeStartYear_IsCalled: AdminDashboardServiceBaseTest
    {
        private IList<TqRegistrationPathway> _tqRegistrationPathways;
        private Dictionary<long, RegistrationPathwayStatus> _ulns;
        private LearnerRecord _result;

        private List<TqRegistrationProfile> _registrations;

        private static ReviewChangeStartYearRequest reviewChangeStartYearRequest;

        public override void Given()
        {
            // Parameters
            AoUkprn = 10011881;
            _ulns = new Dictionary<long, RegistrationPathwayStatus>
            {
                { 1111111111, RegistrationPathwayStatus.Active },
                { 1111111112, RegistrationPathwayStatus.Active },                
            };

             reviewChangeStartYearRequest = new ReviewChangeStartYearRequest()
            {
                AcademicYear = 2022,
                AcademicYearTo = 2021,
                ChangeReason = "Test Reason",
                ContactName = "Test User",
                PathwayId = 1,
                changeStartYearDetails = new ChangeStartYearDetails() { StartYearFrom = 2022, StartYearTo = 2021 },
                RequestDate = DateTime.Now.ToShortDateString(),
                ZendeskId = "1234567890",
                CreatedBy = "System"
            };

            // Create mapper
            CreateMapper();
            CreateCommonMapper();

            // Registrations seed
            SeedTestData(EnumAwardingOrganisation.Pearson, true);
            _registrations = SeedRegistrationsData(_ulns, TqProvider);

            var currentYearUln = new List<long> { 1111111111 };
            RegisterUlnForNextAcademicYear(_registrations, currentYearUln);

            // Assessments seed
            var tqPathwayAssessmentsSeedData = new List<TqPathwayAssessment>();
            var tqPathwayResultsSeedData = new List<TqPathwayResult>();

            var tqSpecialismAssessmentsSeedData = new List<TqSpecialismAssessment>();
            var tqSpecialismResultsSeedData = new List<TqSpecialismResult>();
            var industryPlacementUln = 1111111111;
            var profilesWithPrsStatus = new List<(long, PrsStatus?)> { (1111111111, null), (1111111112, null), (1111111113, null), (1111111114, PrsStatus.BeingAppealed), (1111111115, null) };
           

            DbContext.SaveChanges();
            DetachAll();

            Configuration = new ResultsAndCertificationConfiguration
            {
                TlevelQueriedSupportEmailAddress = "test@test.com"
            };

            CommonServiceLogger = new Logger<CommonService>(new NullLoggerFactory());
            TlLookupRepositoryLogger = new Logger<GenericRepository<TlLookup>>(new NullLoggerFactory());
            TlLookupRepository = new GenericRepository<TlLookup>(TlLookupRepositoryLogger, DbContext);
            FunctionLogRepositoryLogger = new Logger<GenericRepository<FunctionLog>>(new NullLoggerFactory());
            FunctionLogRepository = new GenericRepository<FunctionLog>(FunctionLogRepositoryLogger, DbContext);
            CommonRepository = new CommonRepository(DbContext);
             

            NotificationsClient = Substitute.For<IAsyncNotificationClient>();
            NotificationLogger = new Logger<NotificationService>(new NullLoggerFactory());
            NotificationTemplateRepositoryLogger = new Logger<GenericRepository<NotificationTemplate>>(new NullLoggerFactory());
            NotificationTemplateRepository = new GenericRepository<NotificationTemplate>(NotificationTemplateRepositoryLogger, DbContext);
            NotificationService = new NotificationService(NotificationTemplateRepository, NotificationsClient, NotificationLogger);
            ChangeLogRepositoryLogger = new Logger<GenericRepository<ChangeLog>>(new NullLoggerFactory());
            ChangeLogRepository = new GenericRepository<ChangeLog>(ChangeLogRepositoryLogger, DbContext);
            commonService = new CommonService(CommonServiceLogger, CommonMapper, TlLookupRepository, FunctionLogRepository, CommonRepository, NotificationService, Configuration, ChangeLogRepository);
            SystemProvider = new SystemProvider();

            AdminDashboardRepository = new AdminDashboardRepository(DbContext);
            RegistrationPathwayRepositoryLogger = new Logger<GenericRepository<TqRegistrationPathway>>(new NullLoggerFactory());
            RegistrationPathwayRepository = new GenericRepository<TqRegistrationPathway>(RegistrationPathwayRepositoryLogger, DbContext);
           

            AdminDashboardService = new AdminDashboardService(AdminDashboardRepository, SystemProvider, Mapper, RegistrationPathwayRepository, commonService);

        }

        private bool _actualResult;

        protected virtual void CreateMapper()
        {
            var mapperConfig = new MapperConfiguration(c => c.AddMaps(typeof(AdminDashboardMapper).Assembly));
            Mapper = new Mapper(mapperConfig);
        }

        public override Task When()
        {
            return Task.CompletedTask;
        }
        public async Task WhenAsync(ReviewChangeStartYearRequest request)
        {
            _actualResult = await AdminDashboardService.ProcessChangeStartYearAsync(request);
        }

        [Theory()]
        [MemberData(nameof(Data))]
        public async Task Then_Expected_Results_Are_Returned(ReviewChangeStartYearRequest request, bool expectedResponse,long uln)
        {
            await WhenAsync(request);

            if (expectedResponse == false)
            {
                _actualResult.Should().BeFalse();
                return;
            }

            var expectedRegistrationPathway = _registrations.SingleOrDefault(x => x.UniqueLearnerNumber == uln).TqRegistrationPathways.FirstOrDefault();

            expectedRegistrationPathway.Should().NotBeNull();

            var actualIndustryPlacement = DbContext.TqRegistrationPathway.FirstOrDefault(ip => ip.Id == request.PathwayId);


            // Assert
            request.PathwayId.Should().Be(actualIndustryPlacement.Id);
            request.AcademicYearTo.Should().Be(actualIndustryPlacement.AcademicYear);
            
        }


        public static IEnumerable<object[]> Data
        {
            get
            {
                return new[]
                {
                    // Uln not found
                    new object[] { new ReviewChangeStartYearRequest()
                    {
                AcademicYear = 2022,
                AcademicYearTo = 2021,
                ChangeReason = "Test Reason",
                ContactName = "Test User",
                PathwayId = 1,
                changeStartYearDetails = new ChangeStartYearDetails() { StartYearFrom = 2022, StartYearTo = 2021 },
                RequestDate = DateTime.Now.ToShortDateString(),
                ZendeskId = "1234567890",
                CreatedBy = "System"
                    },
                        true,1111111111 }                 

                };
            }
        }

        private void SeedIndustyPlacementData(int uln)
        {
            var pathway = _registrations.FirstOrDefault(x => x.UniqueLearnerNumber == uln).TqRegistrationPathways.FirstOrDefault();
            IndustryPlacementProvider.CreateIndustryPlacement(DbContext, pathway.Id, IndustryPlacementStatus.Completed);
        }

        public List<TqRegistrationProfile> SeedRegistrationsData(Dictionary<long, RegistrationPathwayStatus> ulns, TqProvider tqProvider = null, bool isCouplet = false)
        {
            var profiles = new List<TqRegistrationProfile>();

            foreach (var uln in ulns)
            {
                profiles.Add(SeedRegistrationData(uln.Key, uln.Value, tqProvider, isCouplet));
            }
            return profiles;
        }

        public TqRegistrationProfile SeedRegistrationData(long uln, RegistrationPathwayStatus status = RegistrationPathwayStatus.Active, TqProvider tqProvider = null, bool isCouplet = false)
        {
            var profile = new TqRegistrationProfileBuilder().BuildList().FirstOrDefault(p => p.UniqueLearnerNumber == uln);
            var tqRegistrationProfile = RegistrationsDataProvider.CreateTqRegistrationProfile(DbContext, profile);
            var tqRegistrationPathway = RegistrationsDataProvider.CreateTqRegistrationPathway(DbContext, tqRegistrationProfile, tqProvider ?? TqProvider);
            var tqRegistrationSpecialisms = isCouplet ? RegistrationsDataProvider.CreateTqRegistrationSpecialisms(DbContext, tqRegistrationPathway)
                : new List<TqRegistrationSpecialism> { RegistrationsDataProvider.CreateTqRegistrationSpecialism(DbContext, tqRegistrationPathway, Specialisms.First()) };

            if (status == RegistrationPathwayStatus.Withdrawn)
            {
                tqRegistrationPathway.Status = status;
                tqRegistrationPathway.EndDate = DateTime.UtcNow.AddDays(-1);
                foreach (var tqRegistrationSpecialism in tqRegistrationSpecialisms)
                {
                    tqRegistrationSpecialism.IsOptedin = true;
                    tqRegistrationSpecialism.EndDate = DateTime.UtcNow.AddDays(-1);
                }
            }

            DbContext.SaveChanges();
            return profile;
        }


    }
}