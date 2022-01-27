﻿using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Sfa.Tl.ResultsAndCertification.Common.Enum;
using Sfa.Tl.ResultsAndCertification.Data.Repositories;
using Sfa.Tl.ResultsAndCertification.Domain.Models;
using Sfa.Tl.ResultsAndCertification.Tests.Common.Enum;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Sfa.Tl.ResultsAndCertification.IntegrationTests.Repositories.ResultRepositoryBaseTests
{
    public class When_GetResultsAsync_IsCalled : ResultRepositoryBaseTest
    {
        private Dictionary<long, RegistrationPathwayStatus> _ulns;
        private List<TqRegistrationProfile> _registrations;
        private List<TqPathwayAssessment> _pathwayAssessments;
        private List<TqPathwayResult> _pathwayResults;
        private TqRegistrationPathway _result;
        private List<TqSpecialismAssessment> _specialismAssessments;
        private List<TqSpecialismResult> _specialismResults;

        public override void Given()
        {
            AoUkprn = 10011881;
            _ulns = new Dictionary<long, RegistrationPathwayStatus> 
            { 
                { 1111111111, RegistrationPathwayStatus.Active },
                { 1111111112, RegistrationPathwayStatus.Active },
                { 1111111113, RegistrationPathwayStatus.Withdrawn }
            };

            /// Registrations seed
            SeedTestData(EnumAwardingOrganisation.Pearson, true);
            _registrations = SeedRegistrationsDataByStatus(_ulns, TqProvider);

            // Assessments seed
            var tqPathwayAssessmentsSeedData = new List<TqPathwayAssessment>();
            var tqPathwayResultsSeedData = new List<TqPathwayResult>();
            var tqSpecialismAssessmentsSeedData = new List<TqSpecialismAssessment>();
            var tqSpecialismResultsSeedData = new List<TqSpecialismResult>();

            foreach (var registration in _registrations.Where(x => x.UniqueLearnerNumber != 1111111111))
            {
                var hasHitoricData = new List<long> { 1111111112 };
                var isHistoricAssessent = hasHitoricData.Any(x => x == registration.UniqueLearnerNumber);
                var isLatestActive = _ulns[registration.UniqueLearnerNumber] != RegistrationPathwayStatus.Withdrawn;

                var pathwayAssessments = GetPathwayAssessmentsDataToProcess(registration.TqRegistrationPathways.ToList(), isLatestActive, isHistoricAssessent);
                tqPathwayAssessmentsSeedData.AddRange(pathwayAssessments);

                // Build Pathway results
                tqPathwayResultsSeedData.AddRange(GetPathwayResultsDataToProcess(pathwayAssessments, isLatestActive, isHistoricAssessent));

                var specialismAssessments = GetSpecialismAssessmentsDataToProcess(registration.TqRegistrationPathways.SelectMany(p => p.TqRegistrationSpecialisms).ToList(), isLatestActive, isHistoricAssessent);
                tqSpecialismAssessmentsSeedData.AddRange(specialismAssessments);

                tqSpecialismResultsSeedData.AddRange(GetSpecialismResultsDataToProcess(specialismAssessments, isLatestActive, isHistoricAssessent));
            }

            _pathwayAssessments = SeedPathwayAssessmentsData(tqPathwayAssessmentsSeedData, false);
            _pathwayResults = SeedPathwayResultsData(tqPathwayResultsSeedData, false);

            _specialismAssessments = SeedSpecialismAssessmentsData(tqSpecialismAssessmentsSeedData, false);
            _specialismResults = SeedSpecialismResultsData(tqSpecialismResultsSeedData, false);

            DbContext.SaveChanges();

            DetachAll();

            ResultRepositoryLogger = new Logger<ResultRepository>(new NullLoggerFactory());
            ResultRepository = new ResultRepository(ResultRepositoryLogger, DbContext);
        }

        public override Task When()
        {
            return Task.CompletedTask;
        }

        public async Task WhenAsync(long aoUkprn, int profileId)
        {
            if (_result != null)
                return;

            _result = await ResultRepository.GetResultsAsync(aoUkprn, profileId);
        }

        [Theory()]
        [MemberData(nameof(Data))]
        public async Task Then_Expected_Results_Are_Returned(long aoUkprn, long uln, int profileId, RegistrationPathwayStatus status, bool hasAssessments, bool expectedResponse)
        {
            await WhenAsync(aoUkprn, profileId);

            if (_result == null)
            {
                expectedResponse.Should().BeFalse();
                return;
            }

            // Expected result
            var expectedRegistration = _registrations.SingleOrDefault(x => x.UniqueLearnerNumber == uln);

            expectedRegistration.Should().NotBeNull();

            TqRegistrationPathway expectedPathway = null;

            if (status == RegistrationPathwayStatus.Withdrawn)
            {
                expectedPathway = expectedRegistration.TqRegistrationPathways.FirstOrDefault(p => p.Status == status && p.EndDate != null);
            }
            else
            {
                expectedPathway = expectedRegistration.TqRegistrationPathways.FirstOrDefault(p => p.Status == status && p.EndDate == null);
            }

            expectedPathway.Should().NotBeNull();

            TqRegistrationSpecialism expectedSpecialim = null;
            TqSpecialismAssessment expectedSpecialismAssessment = null;

            if (status == RegistrationPathwayStatus.Withdrawn)
            {
                expectedSpecialim = expectedPathway.TqRegistrationSpecialisms.FirstOrDefault(s => s.IsOptedin && s.EndDate != null);
                expectedSpecialismAssessment = expectedSpecialim.TqSpecialismAssessments.FirstOrDefault(sa => sa.IsOptedin && sa.EndDate != null);

            }
            else
            {
                expectedSpecialim = expectedPathway.TqRegistrationSpecialisms.FirstOrDefault(s => s.IsOptedin && s.EndDate == null);
                expectedSpecialismAssessment = expectedSpecialim.TqSpecialismAssessments.FirstOrDefault(sa => sa.IsOptedin && sa.EndDate == null);
            }

            TqPathwayAssessment expectedPathwayAssessment = null;

            if (status == RegistrationPathwayStatus.Withdrawn)
            {
                expectedPathwayAssessment = _pathwayAssessments.FirstOrDefault(x => x.TqRegistrationPathway.TqRegistrationProfile.UniqueLearnerNumber == uln && x.IsOptedin && x.EndDate != null);
                expectedSpecialismAssessment = _specialismAssessments.FirstOrDefault(x => x.TqRegistrationSpecialism.TqRegistrationPathway.TqRegistrationProfile.UniqueLearnerNumber == uln && x.IsOptedin && x.EndDate != null);
            }
            else
            {
                expectedPathwayAssessment = _pathwayAssessments.FirstOrDefault(x => x.TqRegistrationPathway.TqRegistrationProfile.UniqueLearnerNumber == uln && x.IsOptedin && x.EndDate == null);
                expectedSpecialismAssessment = _specialismAssessments.FirstOrDefault(x => x.TqRegistrationSpecialism.TqRegistrationPathway.TqRegistrationProfile.UniqueLearnerNumber == uln && x.IsOptedin && x.EndDate == null);
            }

            TqPathwayResult expectedPathwayResult = null;
            TqSpecialismResult expectedSpecialismResult = null;

            if (status == RegistrationPathwayStatus.Withdrawn)
            {
                expectedPathwayResult = _pathwayResults.FirstOrDefault(x => expectedPathwayAssessment != null && x.TqPathwayAssessmentId == expectedPathwayAssessment.Id && x.IsOptedin && x.EndDate != null);
                expectedSpecialismResult = _specialismResults.FirstOrDefault(x => expectedPathwayAssessment != null && x.TqSpecialismAssessmentId == expectedSpecialismAssessment.Id && x.IsOptedin && x.EndDate != null);
            }
            else
            {
                expectedPathwayResult = _pathwayResults.FirstOrDefault(x => expectedPathwayAssessment != null && x.TqPathwayAssessmentId == expectedPathwayAssessment.Id && x.IsOptedin && x.EndDate == null);
                expectedSpecialismResult = _specialismResults.FirstOrDefault(x => expectedPathwayAssessment != null && x.TqSpecialismAssessmentId == expectedSpecialismAssessment.Id && x.IsOptedin && x.EndDate == null);
            }

            _result.Should().NotBeNull();

            // Actual result
            var actualPathway = _result;
            var actualSpecialism = actualPathway.TqRegistrationSpecialisms.FirstOrDefault();
            var actualPathwayAssessment = actualPathway.TqPathwayAssessments.FirstOrDefault();            

            // Assert Registration Pathway
            actualPathway.TqRegistrationProfileId.Should().Be(expectedPathway.TqRegistrationProfileId);
            actualPathway.TqProviderId.Should().Be(expectedPathway.TqProviderId);
            actualPathway.AcademicYear.Should().Be(expectedPathway.AcademicYear);
            actualPathway.StartDate.Should().Be(expectedPathway.StartDate);
            actualPathway.EndDate.Should().Be(expectedPathway.EndDate);
            actualPathway.Status.Should().Be(expectedPathway.Status);

            // Assert Registration Specialism
            actualSpecialism.TqRegistrationPathwayId.Should().Be(expectedSpecialim.TqRegistrationPathwayId);
            actualSpecialism.TlSpecialismId.Should().Be(expectedSpecialim.TlSpecialismId);
            actualSpecialism.StartDate.Should().Be(expectedSpecialim.StartDate);
            actualSpecialism.EndDate.Should().Be(expectedSpecialim.EndDate);
            actualSpecialism.IsOptedin.Should().Be(expectedSpecialim.IsOptedin);

            // Assert Assessments
            if (hasAssessments)
            {
                AssertPathwayAssessment(actualPathwayAssessment, expectedPathwayAssessment);
                AssertPathwayResult(expectedPathwayResult, expectedPathwayResult);

                var actualSpecialismAssessment = actualSpecialism.TqSpecialismAssessments.FirstOrDefault();
                var actualSpecialismResult = actualSpecialismAssessment.TqSpecialismResults.FirstOrDefault();

                AssertSpecialismAssessment(actualSpecialismAssessment, expectedSpecialismAssessment);
                AssertSpecialismResult(actualSpecialismResult, expectedSpecialismResult);
            }
        }

        public static IEnumerable<object[]> Data
        {
            get
            {
                return new[]
                {
                    // Uln not found
                    new object[] { 10011881, 0000000000, 0, RegistrationPathwayStatus.Active, false, false },

                    // Uln not found for registered AoUkprn
                    new object[] { 00000000, 1111111111, 1, RegistrationPathwayStatus.Active, false, false },
                    
                    // Uln: 1111111111 - Registration(Active) but no asessment entries for pathway and specialism
                    new object[] { 10011881, 1111111111, 1, RegistrationPathwayStatus.Active, false, true },

                    // Uln: 1111111112 - Registration(Active), TqPathwayAssessments(Active + History) and TqSpecialismAssessments(Active + History)
                    new object[] { 10011881, 1111111112, 2, RegistrationPathwayStatus.Active, true, true },

                    // Uln: 1111111113 - Registration(Withdrawn), TqPathwayAssessments(Withdrawn) and TqSpecialismAssessments(Withdrawn)
                    new object[] { 10011881, 1111111113, 3, RegistrationPathwayStatus.Withdrawn, true, true },
                };
            }
        }
    }
}
