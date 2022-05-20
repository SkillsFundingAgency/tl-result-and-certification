﻿using FluentAssertions;
using Sfa.Tl.ResultsAndCertification.Common.Enum;
using Sfa.Tl.ResultsAndCertification.Data.Repositories;
using Sfa.Tl.ResultsAndCertification.Domain.Models;
using Sfa.Tl.ResultsAndCertification.Models.Contracts.Common;
using Sfa.Tl.ResultsAndCertification.Models.Contracts.TrainingProvider;
using Sfa.Tl.ResultsAndCertification.Tests.Common.Enum;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Sfa.Tl.ResultsAndCertification.IntegrationTests.Repositories.TrainingProviderRepositoryTests
{
    public class When_SearchLearnerDetailsAsync_IsCalled : TrainingProviderRepositoryBaseTest
    {
        private Dictionary<long, RegistrationPathwayStatus> _ulns;
        private List<(long uln, bool isRcFeed, bool seedQualificationAchieved, bool isSendQualification, bool isEngishAndMathsAchieved, bool seedIndustryPlacement, bool? isSendLearner)> _testCriteriaData;
        private IList<TqRegistrationProfile> _profiles;
        private PagedResponse<SearchLearnerDetail> _actualResult;

        public override void Given()
        {
            _profiles = new List<TqRegistrationProfile>();
            _ulns = new Dictionary<long, RegistrationPathwayStatus>
            {
                { 1111111111, RegistrationPathwayStatus.Active },
                { 1111111112, RegistrationPathwayStatus.Withdrawn },
                { 1111111113, RegistrationPathwayStatus.Active },
                { 1111111114, RegistrationPathwayStatus.Active },
                { 1111111115, RegistrationPathwayStatus.Active }
            };

            _testCriteriaData = new List<(long uln, bool isRcFeed, bool seedQualificationAchieved, bool isSendQualification, bool isEngishAndMathsAchieved, bool seedIndustryPlacement, bool? isSendLearner)>
            {
                (1111111111, false, true, true, true, true, true), // Lrs data with Send Qualification + IP
                (1111111112, true, false, false, false, false, null), // Not from Lrs
                (1111111113, false, true, false, true, false, false), // Lrs data without Send Qualification
                (1111111114, true, false, false, true, false, null), // from provider
                (1111111115, true, false, false, false, true, null), // from provider
            };

            SeedTestData(EnumAwardingOrganisation.Pearson, true);
            Qualifications = SeedQualificationData();

            foreach (var uln in _ulns)
            {
                _profiles.Add(SeedRegistrationDataByStatus(uln.Key, uln.Value, TqProvider));
            }

            TransferRegistration(_profiles.FirstOrDefault(p => p.UniqueLearnerNumber == 1111111113), Provider.WalsallCollege);

            foreach (var (uln, isRcFeed, seedQualificationAchieved, isSendQualification, isEngishAndMathsAchieved, seedIndustryPlacement, isSendLearner) in _testCriteriaData)
            {
                var profile = _profiles.FirstOrDefault(p => p.UniqueLearnerNumber == uln);
                BuildLearnerRecordCriteria(profile, isRcFeed, seedQualificationAchieved, isSendQualification, isEngishAndMathsAchieved, seedIndustryPlacement, isSendLearner);
            }

            DbContext.SaveChanges();

            // Test class.
            TrainingProviderRepository = new TrainingProviderRepository(DbContext, TraningProviderRepositoryLogger);
        }

        public override Task When()
        {
            return Task.CompletedTask;
        }

        public async Task WhenAsync(SearchLearnerRequest request)
        {
            if (_actualResult != null)
                return;

            _actualResult = await TrainingProviderRepository.SearchLearnerDetailsAsync(request);
        }

        [Theory]
        [MemberData(nameof(Data))]
        public async Task Then_Returns_Expected_Results(SearchLearnerRequest request)
        {
            await WhenAsync(request);

            var pathways = request.AcademicYear.Any()
                ? _profiles.SelectMany(p => p.TqRegistrationPathways.Where(p => request.AcademicYear.Contains(p.AcademicYear) && p.TqProvider.TlProvider.UkPrn == request.Ukprn && (p.Status == RegistrationPathwayStatus.Active || p.Status == RegistrationPathwayStatus.Withdrawn))).ToList()
                : _profiles.SelectMany(p => p.TqRegistrationPathways.Where(p => p.TqProvider.TlProvider.UkPrn == request.Ukprn && (p.Status == RegistrationPathwayStatus.Active || p.Status == RegistrationPathwayStatus.Withdrawn))).ToList();

            var expectedLearnerDeails = pathways.Select(x => new SearchLearnerDetail
            {
                ProfileId = x.TqRegistrationProfile.Id,
                Uln = x.TqRegistrationProfile.UniqueLearnerNumber,
                Firstname = x.TqRegistrationProfile.Firstname,
                Lastname = x.TqRegistrationProfile.Lastname,
                AcademicYear = x.AcademicYear,
                TlevelName = x.TqProvider.TqAwardingOrganisation.TlPathway.Name,
                EnglishStatus = x.TqRegistrationProfile.EnglishStatus,
                MathsStatus = x.TqRegistrationProfile.MathsStatus,
                IndustryPlacementStatus = x.IndustryPlacements.Any() ? x.IndustryPlacements.FirstOrDefault().Status : null,
                CreatedOn = x.CreatedOn
            }).ToList();

            var expectedPagedResponse = new PagedResponse<SearchLearnerDetail>
            {
                TotalRecords = expectedLearnerDeails.Count(),
                Records = expectedLearnerDeails
            };

            _actualResult.Should().BeEquivalentTo(expectedPagedResponse);
        }

        public static IEnumerable<object[]> Data
        {
            get
            {
                return new[]
                {
                    new object[] { new SearchLearnerRequest { AcademicYear = new List<int> { 2020 }, Ukprn = (int)Provider.TestCollege } }, // No learners registered for test college
                    new object[] { new SearchLearnerRequest { AcademicYear = new List<int> { 2020 }, Ukprn = (int)Provider.WalsallCollege } }, // learner transfered to walsall college
                    new object[] { new SearchLearnerRequest { AcademicYear = new List<int> { 2021 }, Ukprn = (int)Provider.BarnsleyCollege } }, // No learners registered for Barnsley college for 2021
                    new object[] { new SearchLearnerRequest { AcademicYear = new List<int> { 2020 }, Ukprn = (int)Provider.BarnsleyCollege } }, // Learners registered for Barnsley college for 2020
                    new object[] { new SearchLearnerRequest { AcademicYear = new List<int>(), Ukprn = (int)Provider.BarnsleyCollege } }, // Learners registered for Barnsley college for 2020 but not passing academic year
                };
            }
        }
    }
}