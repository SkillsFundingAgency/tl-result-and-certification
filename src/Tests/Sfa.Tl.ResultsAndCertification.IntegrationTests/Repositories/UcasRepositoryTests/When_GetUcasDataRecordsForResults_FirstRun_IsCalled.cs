﻿using FluentAssertions;
using Sfa.Tl.ResultsAndCertification.Common.Enum;
using Sfa.Tl.ResultsAndCertification.Data.Repositories;
using Sfa.Tl.ResultsAndCertification.Domain.Models;
using Sfa.Tl.ResultsAndCertification.Tests.Common.Enum;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Sfa.Tl.ResultsAndCertification.IntegrationTests.Repositories.UcasRepositoryTests
{
    public class When_GetUcasDataRecordsForResults_FirstRun_IsCalled : UcasRepositoryBaseTest
    {
        private Dictionary<long, RegistrationPathwayStatus> _ulns;
        private List<TqRegistrationProfile> _registrations;
        private IList<OverallResult> _actualOverallResults;
        private List<OverallResult> _overallResults;

        public override void Given()
        {
            _ulns = new Dictionary<long, RegistrationPathwayStatus>
            {
                { 1111111111, RegistrationPathwayStatus.Active },
                { 1111111112, RegistrationPathwayStatus.Active },
                { 1111111113, RegistrationPathwayStatus.Withdrawn },
                { 1111111114, RegistrationPathwayStatus.Active }
            };

            SeedTestData(EnumAwardingOrganisation.Pearson, true);
            _registrations = SeedRegistrationsDataByStatus(_ulns, null);

            SetAcademicYear(_registrations, 2021, new List<long> { 1111111114 });

            var ulnsWithOverallResult = new List<long> { 1111111111, 1111111112, 1111111113, 1111111114 };
            _overallResults = SeedOverallResultData(_registrations, ulnsWithOverallResult);

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
            _actualOverallResults = await UcasRepository.GetUcasDataRecordsForResultsAsync();
        }

        [Fact]
        public async Task Then_Expected_Results_Are_Returned()
        {
            await WhenAsync();
            _actualOverallResults.Should().NotBeNull();
            _actualOverallResults.Count.Should().Be(2);

            foreach (var actualOverallResult in _actualOverallResults)
            {
                var expectedOverallResult = _overallResults.FirstOrDefault(x => x.TqRegistrationPathwayId == actualOverallResult.TqRegistrationPathwayId);
                actualOverallResult.Should().BeEquivalentTo(expectedOverallResult);

                actualOverallResult.TqRegistrationPathway.Should().BeEquivalentTo(expectedOverallResult.TqRegistrationPathway);
                actualOverallResult.TqRegistrationPathway.TqRegistrationProfile.Should().BeEquivalentTo(expectedOverallResult.TqRegistrationPathway.TqRegistrationProfile);
            }
        }

        private void SetAcademicYear(List<TqRegistrationProfile> _registrations, int academicYear, List<long> ulns)
        {
            _registrations.Where(x => ulns.Contains(x.UniqueLearnerNumber)).ToList()
                .ForEach(x => { x.TqRegistrationPathways.FirstOrDefault().AcademicYear = academicYear; });
        }
    }
}
