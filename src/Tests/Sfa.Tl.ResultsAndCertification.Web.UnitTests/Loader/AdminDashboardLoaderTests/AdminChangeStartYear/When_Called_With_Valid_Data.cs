﻿using AutoMapper;
using NSubstitute;
using Sfa.Tl.ResultsAndCertification.Api.Client.Interfaces;
using Sfa.Tl.ResultsAndCertification.Web.Loader;
using Sfa.Tl.ResultsAndCertification.Web.ViewModel.AdminDashboard.LearnerRecord;
using Sfa.Tl.ResultsAndCertification.Web.ViewModel.AdminDashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Sfa.Tl.ResultsAndCertification.Tests.Common.BaseTest;
using Sfa.Tl.ResultsAndCertification.Web.Mapper;
using FluentAssertions;

namespace Sfa.Tl.ResultsAndCertification.Web.UnitTests.Loader.AdminDashboardLoaderTests.AdminChangeStartYear
{
    public class When_Called_With_Valid_Data : BaseTest<AdminDashboardLoader>
    {
        private IResultsAndCertificationInternalApiClient _internalApiClient;
        private AdminDashboardLoader Loader;
        private int PathwayId;

        public override void Setup()
        {
            _internalApiClient = Substitute.For<IResultsAndCertificationInternalApiClient>();

            var mapperConfig = new MapperConfiguration(c => c.AddMaps(typeof(AdminDashboardMapper).Assembly));
            var mapper = new AutoMapper.Mapper(mapperConfig);

            Loader = new AdminDashboardLoader(_internalApiClient, mapper);
        }


        private Models.OverallResults.OverallResultDetail _expectedOverallResult;
        private Models.Contracts.AdminDashboard.AdminLearnerRecord _expectedApiResult;

        protected AdminChangeStartYearViewModel ActualResult { get; set; }

        public override void Given()
        {
            PathwayId = 1;



            _expectedApiResult = new Models.Contracts.AdminDashboard.AdminLearnerRecord
            {
                ProfileId = PathwayId,
                RegistrationPathwayId = 222,
                Uln = 786787689,
                Name = "John smith",
                DateofBirth = DateTime.UtcNow.AddYears(-15),
                ProviderName = "Barnsley College",
                TlevelName = "Education and Early Years(60358294)",
                AcademicYear = 2023,
                AwardingOrganisationName = "NCFE",
                MathsStatus = Common.Enum.SubjectStatus.Achieved,
                EnglishStatus = Common.Enum.SubjectStatus.Achieved,
                IsLearnerRegistered = true,
                IndustryPlacementId = 1,
                IndustryPlacementStatus = Common.Enum.IndustryPlacementStatus.Completed,
                AcademicStartYearsToBe = new List<int> { 2021, 2022 }
            };
            _internalApiClient.GetAdminLearnerRecordAsync(PathwayId).Returns(_expectedApiResult);
        }

        public async override Task When()
        {
            ActualResult = await Loader.GetAdminLearnerRecordAsync<AdminChangeStartYearViewModel>(PathwayId);
        }

        [Fact]
        public void Then_Expected_Methods_AreCalled()
        {
            _internalApiClient.Received(1).GetAdminLearnerRecordAsync(PathwayId);
        }

        [Fact]
        public void Then_Returns_Expected_Results()
        {
            ActualResult.Should().NotBeNull();
            ActualResult.ProfileId.Should().Be(_expectedApiResult.ProfileId);
            ActualResult.RegistrationPathwayId.Should().Be(_expectedApiResult.RegistrationPathwayId);
            ActualResult.Uln.Should().Be(_expectedApiResult.Uln);
            ActualResult.Learner.Should().Be($"{_expectedApiResult.FirstName} {_expectedApiResult.LastName}");
            ActualResult.ProviderName.Should().Be(_expectedApiResult.ProviderName);
            ActualResult.TlevelName.Should().Be(_expectedApiResult.TlevelName);
            ActualResult.AcademicStartYearsToBe.Should().BeEquivalentTo(_expectedApiResult.AcademicStartYearsToBe);           
        }
    }
}