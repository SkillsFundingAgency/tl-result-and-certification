﻿using FluentAssertions;
using Sfa.Tl.ResultsAndCertification.Common.Enum;
using Sfa.Tl.ResultsAndCertification.Data.Repositories;
using Sfa.Tl.ResultsAndCertification.Domain.Models;
using Sfa.Tl.ResultsAndCertification.Models.Contracts.PostResultsService;
using Sfa.Tl.ResultsAndCertification.Tests.Common.Enum;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Sfa.Tl.ResultsAndCertification.IntegrationTests.Repositories.PostResultsServiceTests
{
    public class When_FindPrsLearnerRecordAsync_IsCalled : PostResultsServiceRepositoryBaseTest
    {
        private Dictionary<long, RegistrationPathwayStatus> _ulns;
        private IList<TqRegistrationProfile> _profiles;
        private FindPrsLearnerRecord _actualResult;
        private List<TqPathwayAssessment> _tqPathwayAssessmentsSeedData;

        public override void Given()
        {
            _profiles = new List<TqRegistrationProfile>();
            _ulns = new Dictionary<long, RegistrationPathwayStatus>
            {
                { 1111111111, RegistrationPathwayStatus.Active },    // No assessment
                { 1111111112, RegistrationPathwayStatus.Withdrawn }, // Assessment + Result
                { 1111111113, RegistrationPathwayStatus.Active },    // Assessment + Result
                { 1111111114, RegistrationPathwayStatus.Active }     // Multi Assessment 
            };

            SeedTestData(EnumAwardingOrganisation.Pearson, true);

            foreach (var uln in _ulns)
            {
                _profiles.Add(SeedRegistrationDataByStatus(uln.Key, uln.Value, TqProvider));
            }

            TransferRegistration(_profiles.FirstOrDefault(p => p.UniqueLearnerNumber == 1111111113), Provider.WalsallCollege);
            DbContext.SaveChanges();

            // Seed Assessments And Results
            _tqPathwayAssessmentsSeedData = new List<TqPathwayAssessment>();

            var profilesWithAssessment = new List<long> { 1111111112, 1111111113, 1111111114 };
            foreach (var profile in _profiles.Where(x => profilesWithAssessment.Contains(x.UniqueLearnerNumber)))
            {
                var isLatestActive = _ulns[profile.UniqueLearnerNumber] != RegistrationPathwayStatus.Withdrawn;
                var pathwayAssessments = GetPathwayAssessmentsDataToProcess(profile.TqRegistrationPathways.ToList(), isLatestActive);
                _tqPathwayAssessmentsSeedData.AddRange(pathwayAssessments);

                // Seed Pathway results
                var profilesWithResults = new List<(long, PrsStatus?)> { (1111111112, null), (1111111113, null), (1111111114, null) };
                foreach (var assessment in pathwayAssessments.Where(x => profilesWithResults.Any(p => p.Item1 == x.TqRegistrationPathway.TqRegistrationProfile.UniqueLearnerNumber)))
                {
                    var hasHitoricData = new List<long> { 1111111113 };
                    var hasHistoricResult = hasHitoricData.Any(x => x == profile.UniqueLearnerNumber);
                    var prsStatus = profilesWithResults.FirstOrDefault(p => p.Item1 == assessment.TqRegistrationPathway.TqRegistrationProfile.UniqueLearnerNumber).Item2;
                    var seedPathwayResultsAsActive = assessment.TqRegistrationPathway.TqRegistrationProfile.UniqueLearnerNumber != 1111111112;
                    GetPathwayResultDataToProcess(assessment, seedPathwayResultsAsActive, hasHistoricResult, prsStatus);
                }
            }

            // Additional assessment to 1111111114
            var prof =  _profiles.FirstOrDefault(x => x.UniqueLearnerNumber == 1111111114);
            var asmnt = GetPathwayAssessmentsDataToProcess(prof.TqRegistrationPathways.ToList(), true);
            _tqPathwayAssessmentsSeedData.AddRange(asmnt);

            // Seed Assessments
            SeedPathwayAssessmentsData(_tqPathwayAssessmentsSeedData, true);

            // Test class.
            PostResultsServiceRepository = new PostResultsServiceRepository(DbContext);
        }

        public override Task When()
        {
            return Task.CompletedTask;
        }

        public async Task WhenAsync(long aoUkprn, long uln)
        {
            if (_actualResult != null)
                return;

            _actualResult = await PostResultsServiceRepository.FindPrsLearnerRecordAsync(aoUkprn, uln);
        }

        [Theory]
        [MemberData(nameof(Data))]
        public async Task Then_Returns_Expected_Results(long uln, AwardingOrganisation ao, bool isRecordFound)
        {
            await WhenAsync((long)ao, uln);

            if (isRecordFound == false)
            {
                _actualResult.Should().BeNull();
                return;
            }

            var expectedProfile = _profiles.FirstOrDefault(x => x.UniqueLearnerNumber == uln);
            _actualResult.Should().NotBeNull();
            _actualResult.Uln.Should().Be(expectedProfile.UniqueLearnerNumber);
            _actualResult.Firstname.Should().Be(expectedProfile.Firstname);
            _actualResult.Lastname.Should().Be(expectedProfile.Lastname);
            _actualResult.DateofBirth.Should().Be(expectedProfile.DateofBirth);

            var expectedPathway = expectedProfile.TqRegistrationPathways.OrderByDescending(x => x.CreatedOn).FirstOrDefault(x => x.Status == RegistrationPathwayStatus.Active || x.Status == RegistrationPathwayStatus.Withdrawn);
            _actualResult.Status.Should().Be(expectedPathway.Status);

            var expectedTqAwardingOrganisation = expectedPathway.TqProvider.TqAwardingOrganisation;
            _actualResult.TlevelTitle.Should().Be(expectedTqAwardingOrganisation.TlPathway.TlevelTitle);

            var expctedProvider = expectedTqAwardingOrganisation.TqProviders.FirstOrDefault().TlProvider;

            if (uln == 1111111113)
            {
                _actualResult.ProviderUkprn.Should().Be((int)Provider.WalsallCollege);
                _actualResult.ProviderName.Should().Be("Walsall College");
            }
            else 
            {
                _actualResult.ProviderUkprn.Should().Be(expctedProvider.UkPrn);
                _actualResult.ProviderName.Should().Be(expctedProvider.Name);
            }

            var expectedAssessments = _profiles.Where(x => x.UniqueLearnerNumber == uln)
                .SelectMany(x => x.TqRegistrationPathways.Last().TqPathwayAssessments.Where(x => x.EndDate == null && x.IsOptedin))
                .OrderBy(o => o.Id).ToList();

            var actualAssessments = _actualResult.PathwayAssessments.OrderBy(o => o.AssessmentId);
            actualAssessments.Count().Should().Be(expectedAssessments.Count());

            for (int i = 0; i < expectedAssessments.Count(); i++)
            {
                var hasResult = expectedAssessments[i].TqPathwayResults.Any(x => x.IsOptedin && x.EndDate == null);
                actualAssessments.ElementAt(i).AssessmentId.Should().Be(expectedAssessments[i].Id);
                actualAssessments.ElementAt(i).SeriesName.Should().Be(expectedAssessments[i].AssessmentSeries.Name);
                actualAssessments.ElementAt(i).HasResult.Should().Be(hasResult);
            }
        }

        public static IEnumerable<object[]> Data
        {
            get
            {
                return new[]
                {
                    new object[] { 9999999999, AwardingOrganisation.Pearson, false }, // Invalid Uln
                    new object[] { 1111111111, AwardingOrganisation.Pearson, true }, // Active + No Assessments
                    new object[] { 1111111111, AwardingOrganisation.Ncfe, false },
                    new object[] { 1111111112, AwardingOrganisation.Pearson, true }, // Withdrawn
                    new object[] { 1111111113, AwardingOrganisation.Pearson, true }, // Active + Single Assessment
                    new object[] { 1111111114, AwardingOrganisation.Pearson, true } // Active + Multiple Assessments
                };
            }
        }
    }
}
