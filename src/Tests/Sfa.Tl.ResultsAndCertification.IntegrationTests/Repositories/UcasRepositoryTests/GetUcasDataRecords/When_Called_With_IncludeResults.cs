﻿using FluentAssertions;
using Sfa.Tl.ResultsAndCertification.Common.Enum;
using Sfa.Tl.ResultsAndCertification.Data.Repositories;
using Sfa.Tl.ResultsAndCertification.Domain.Models;
using Sfa.Tl.ResultsAndCertification.Tests.Common.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Sfa.Tl.ResultsAndCertification.IntegrationTests.Repositories.UcasRepositoryTests.GetUcasDataRecords
{
    public class When_Called_With_IncludeResults : UcasRepositoryBaseTest
    {
        private Dictionary<long, RegistrationPathwayStatus> _ulns;
        private List<TqRegistrationProfile> _registrations;
        private IList<TqRegistrationPathway> _result;

        public override void Given()
        {
            _ulns = new Dictionary<long, RegistrationPathwayStatus>
            {
                { 1111111111, RegistrationPathwayStatus.Active },
                { 1111111112, RegistrationPathwayStatus.Active },
                { 1111111113, RegistrationPathwayStatus.Active },
                { 1111111114, RegistrationPathwayStatus.Active },
                { 1111111115, RegistrationPathwayStatus.Active },
                { 1111111116, RegistrationPathwayStatus.Active },
            };
            SeedTestData(EnumAwardingOrganisation.Pearson, true);
            _registrations = SeedRegistrationsDataByStatus(_ulns, null);

            var currentAcademicYear = GetAcademicYear();
            _registrations.ForEach(x =>
            {
                x.TqRegistrationPathways.ToList().ForEach(p => p.AcademicYear = currentAcademicYear - 1);
            });

            var pathwaysWithAssessments = new List<long> { 1111111111, 1111111112, 1111111113, 1111111114, 1111111115 };
            var pathwaysWithResults = new List<long> { 1111111111, 1111111112, 1111111113 };
            SeedAssessmentsAndResults(_registrations, pathwaysWithAssessments, pathwaysWithResults, $"Summer {currentAcademicYear}");

            pathwaysWithAssessments = new List<long> { 1111111111, 1111111112, 1111111113 };
            pathwaysWithResults = new List<long> { 1111111111, 1111111112, 1111111113 };
            SeedAssessmentsAndResults(_registrations, pathwaysWithAssessments, pathwaysWithResults, $"Autumn {currentAcademicYear}");

            SetAssessmentResult(1111111111, $"Summer {currentAcademicYear}", "B");
            SetAssessmentResult(1111111112, $"Autumn {currentAcademicYear}", "B");

            CommonRepository = new CommonRepository(DbContext);
            UcasRepository = new UcasRepository(DbContext, CommonRepository);
        }

        public override Task When()
        {
            return Task.CompletedTask;
        }

        public async Task WhenAsync()
        {
            await Task.CompletedTask;

            var includeResults = true;
            _result = await UcasRepository.GetUcasDataRecordsAsync(includeResults);
        }

        [Theory()]
        [MemberData(nameof(Data))]
        public async Task Then_Expected_Results_Are_Returned(long uln, int expectedAssessmentsCount, int expectedResultsCount, string expectedSeriesName, string expectedGrade)
        {
            await WhenAsync();
            _result.Should().NotBeNull();

            var actualPathwayRegistration = _result.FirstOrDefault(x => x.TqRegistrationProfile.UniqueLearnerNumber == uln);
            actualPathwayRegistration.Should().NotBeNull();

            var actualPathwayAssessments = actualPathwayRegistration.TqPathwayAssessments;
            actualPathwayAssessments.Count().Should().Be(expectedAssessmentsCount);

            if (expectedAssessmentsCount > 0)
            {
                var hasExpectedAssessmentSeries = actualPathwayAssessments.FirstOrDefault(x => x.AssessmentSeries.Name.Equals(expectedSeriesName));
                hasExpectedAssessmentSeries.Should().NotBeNull();
            }

            var actualResults = actualPathwayAssessments.SelectMany(x => x.TqPathwayResults);
            actualResults.Count().Should().Be(expectedResultsCount);

            var actualResult = actualResults.FirstOrDefault(x => x.TqPathwayAssessment.AssessmentSeries.Name.Equals(expectedSeriesName, StringComparison.InvariantCultureIgnoreCase));
            if (expectedGrade != null)
                actualResult.TlLookup.Value.Should().Be(expectedGrade);
        }

        private void SetAssessmentResult(long uln, string seriesName, string grade)
        {
            var currentResult = DbContext.TqPathwayResult.FirstOrDefault(x => x.TqPathwayAssessment.TqRegistrationPathway.TqRegistrationProfile.UniqueLearnerNumber == uln && x.TqPathwayAssessment.AssessmentSeries.Name.Equals(seriesName, StringComparison.InvariantCultureIgnoreCase));
            currentResult.TlLookup = PathwayComponentGrades.FirstOrDefault(x => x.Value.Equals(grade, StringComparison.InvariantCultureIgnoreCase));
            DbContext.SaveChanges();
        }

        public static IEnumerable<object[]> Data
        {
            get
            {
                return new[]
                {
                    new object[] { 1111111111, 2, 2, "Summer 2021", "B" },
                    new object[] { 1111111111, 2, 2, "Autumn 2021", "A*" },

                    new object[] { 1111111112, 2, 2, "Summer 2021", "A*" },
                    new object[] { 1111111112, 2, 2, "Autumn 2021", "B" },

                    new object[] { 1111111113, 2, 2, "Summer 2021", "A*" },
                    new object[] { 1111111113, 2, 2, "Autumn 2021", "A*" },

                    new object[] { 1111111114, 1, 0, "Summer 2021", null },
                    new object[] { 1111111115, 1, 0, "Summer 2021", null },
                    new object[] { 1111111116, 0, 0, null, null },
                };
            }
        }
    }
}
