﻿using FluentAssertions;
using Sfa.Tl.ResultsAndCertification.Common.Enum;
using Sfa.Tl.ResultsAndCertification.Data.Repositories;
using Sfa.Tl.ResultsAndCertification.Domain.Models;
using Sfa.Tl.ResultsAndCertification.Models.Contracts.StatementOfAchievement;
using Sfa.Tl.ResultsAndCertification.Tests.Common.Enum;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Sfa.Tl.ResultsAndCertification.IntegrationTests.Repositories.StatementOfAchievementRepositoryTests
{
    public class When_FindSoaLearnerRecordAsync_IsCalled : StatementOfAchievementRepositoryBaseTest
    {
        private Dictionary<long, RegistrationPathwayStatus> _ulns;
        private IList<TqRegistrationProfile> _profiles;
        private FindSoaLearnerRecord _actualResult;
        private List<(long uln, bool isEngishAndMathsAchieved, bool seedIndustryPlacement)> _testCriteriaData;

        public override void Given()
        {
            _profiles = new List<TqRegistrationProfile>();
            _ulns = new Dictionary<long, RegistrationPathwayStatus>
            {
                { 1111111111, RegistrationPathwayStatus.Active },
                { 1111111112, RegistrationPathwayStatus.Withdrawn },
                { 1111111113, RegistrationPathwayStatus.Active },
                { 1111111114, RegistrationPathwayStatus.Withdrawn }
            };

            _testCriteriaData = new List<(long uln, bool isEngishAndMathsAchieved, bool seedIndustryPlacement)>
            {
                (1111111111, true, true), // EnglishAndMaths + IP
                (1111111112, true, false), // EnglishAndMaths + No IP
                (1111111113, false, true), // EnglishAndMaths + IP
                (1111111114, true, true) // EnglishAndMaths + IP
            };

            SeedTestData(EnumAwardingOrganisation.Pearson, true);

            foreach (var uln in _ulns)
            {
                _profiles.Add(SeedRegistrationDataByStatus(uln.Key, uln.Value, TqProvider));
            }            

            foreach (var (uln, isEngishAndMathsAchieved, seedIndustryPlacement) in _testCriteriaData)
            {
                var profile = _profiles.FirstOrDefault(p => p.UniqueLearnerNumber == uln);
                BuildLearnerRecordCriteria(profile, isEngishAndMathsAchieved, seedIndustryPlacement);
            }

            TransferRegistration(_profiles.FirstOrDefault(p => p.UniqueLearnerNumber == 1111111113), Provider.WalsallCollege);

            DbContext.SaveChanges();

            // Test class.
            StatementOfAchievementRepository = new StatementOfAchievementRepository(DbContext, StatementOfAchievementRepositoryLogger);
        }

        public override Task When()
        {
            return Task.CompletedTask;
        }

        public async Task WhenAsync(long providerUkprn, long uln)
        {
            if (_actualResult != null)
                return;

            _actualResult = await StatementOfAchievementRepository.FindSoaLearnerRecordAsync(providerUkprn, uln);
        }

        [Theory]
        [MemberData(nameof(Data))]
        public async Task Then_Returns_Expected_Results(long uln, Provider provider, RegistrationPathwayStatus expectedStatus, bool isIpAdded, bool isRecordFound)
        {
            await WhenAsync((long)provider, uln);

            if (isRecordFound == false)
            {
                _actualResult.Should().BeNull();
                return;
            }

            var expectedProvider = TlProviders.FirstOrDefault(p => p.UkPrn == (long)provider);
            var expectedProviderName = expectedProvider != null ? $"{expectedProvider.Name} ({expectedProvider.UkPrn})" : null;
            var expectedTlevelTitle = Pathway.TlevelTitle;
            var expectedProfile = _profiles.FirstOrDefault(p => p.UniqueLearnerNumber == uln);
            var expectedIsLearnerRegistered = expectedStatus == RegistrationPathwayStatus.Active || expectedStatus == RegistrationPathwayStatus.Withdrawn;

            expectedProfile.Should().NotBeNull();
            _actualResult.Should().NotBeNull();
            _actualResult.Uln.Should().Be(uln);
            _actualResult.LearnerName.Should().Be($"{expectedProfile.Firstname} {expectedProfile.Lastname}");
            _actualResult.DateofBirth.Should().Be(expectedProfile.DateofBirth);
            _actualResult.ProviderName.Should().Be(expectedProviderName);
            _actualResult.TlevelTitle.Should().Be(expectedTlevelTitle);
            _actualResult.Status.Should().Be(expectedStatus);
            _actualResult.IsLearnerRegistered.Should().Be(expectedIsLearnerRegistered);
            _actualResult.IsIndustryPlacementAdded.Should().Be(isIpAdded);
        }

        public static IEnumerable<object[]> Data
        {
            get
            {
                return new[]
                {
                    new object[] { 9999999999, Provider.WalsallCollege, null, false, false }, // Invalid Uln
                    new object[] { 1111111111, Provider.BarsleyCollege, RegistrationPathwayStatus.Active, true, true }, // Active
                    new object[] { 1111111111, Provider.WalsallCollege, null, false, false }, // Uln not from WalsallCollege
                    new object[] { 1111111112, Provider.BarsleyCollege, RegistrationPathwayStatus.Withdrawn, false, true }, // Withdrawn
                    new object[] { 1111111113, Provider.BarsleyCollege, RegistrationPathwayStatus.Transferred, true, true }, // Transferred
                    new object[] { 1111111113, Provider.WalsallCollege, RegistrationPathwayStatus.Active, true, true }, // Active
                    new object[] { 1111111114, Provider.BarsleyCollege, RegistrationPathwayStatus.Withdrawn, true, true } // Withdrawn
                };
            }
        }
    }
}
