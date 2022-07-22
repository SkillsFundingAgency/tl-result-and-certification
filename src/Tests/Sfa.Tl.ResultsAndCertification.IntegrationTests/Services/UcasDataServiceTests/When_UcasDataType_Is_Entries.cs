﻿using FluentAssertions;
using Sfa.Tl.ResultsAndCertification.Common.Enum;
using Sfa.Tl.ResultsAndCertification.Domain.Models;
using Sfa.Tl.ResultsAndCertification.Models.Configuration;
using Sfa.Tl.ResultsAndCertification.Tests.Common.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Sfa.Tl.ResultsAndCertification.IntegrationTests.Services.UcasDataServiceTests
{
    public class When_UcasDataType_Is_Entries : UcasDataServiceBaseTest
    {
        private Dictionary<long, RegistrationPathwayStatus> _ulns;
        private List<TqRegistrationProfile> _registrations;

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

            var componentWithAssessments = new List<long> { 1111111111, 1111111112, 1111111113, 1111111114, 1111111115 };
            var componentWithResults = new List<long> { 1111111111, 1111111112, 1111111113 };
            SeedAssessmentsAndResults(_registrations, componentWithAssessments, componentWithResults, $"Summer {currentAcademicYear}");

            componentWithAssessments = new List<long> { 1111111111, 1111111112, 1111111113 };
            componentWithResults = new List<long> { 1111111111, 1111111112, 1111111113 };
            SeedAssessmentsAndResults(_registrations, componentWithAssessments, componentWithResults, $"Autumn {currentAcademicYear}");

            SetAssessmentResult(1111111111, $"Summer {currentAcademicYear}", "B", "Merit");
            SetAssessmentResult(1111111112, $"Autumn {currentAcademicYear}", "B", "Pass");
            
            CreateService();
        }

        public override Task When()
        {
            return Task.CompletedTask;
        }

        public async Task WhenAsync()
        {
            await Task.CompletedTask;
            ActualResult = await UcasDataService.ProcessUcasDataRecordsAsync(UcasDataType.Entries);

            ActualResult.Should().NotBeNull();

            AssertHeaderRecord(UcasDataType.Entries);

            ActualResult.UcasDataRecords.Should().HaveCount(5);

            var expectedDataRecords = new List<ExepectedUcasDataRecord>
            {
                new ExepectedUcasDataRecord { Uln = 1111111111, Name = "Last 1:First 1", Sex = "M", DateOfBirth = "10101980", ComponentRecord = "_|10123456|||_|10123456|||_|TLEVEL|||_|" },
                new ExepectedUcasDataRecord { Uln = 1111111112, Name = "Last 2:First 2", Sex = "F", DateOfBirth = "07051981", ComponentRecord = "_|10123456|||_|10123456|||_|TLEVEL|||_|" },
                new ExepectedUcasDataRecord { Uln = 1111111113, Name = "Last 3:First 3", Sex = "F", DateOfBirth = "03071982", ComponentRecord = "_|10123456|||_|10123456|||_|TLEVEL|||_|" },
                new ExepectedUcasDataRecord { Uln = 1111111114, Name = "Last 4:First 4", Sex = "M", DateOfBirth = "03071982", ComponentRecord = "_|10123456|||_|10123456|||_|TLEVEL|||_|"},
                new ExepectedUcasDataRecord { Uln = 1111111115, Name = "Last 5:First 5", Sex = "", DateOfBirth = "03071982", ComponentRecord = "_|10123456|||_|10123456|||_|TLEVEL|||_|" }
            };

            foreach (var expectedRecord in expectedDataRecords)
            {
                var actualRecord = ActualResult.UcasDataRecords.FirstOrDefault(x => x.CandidateNumber == expectedRecord.Uln.ToString());
                actualRecord.Should().NotBeNull();

                actualRecord.UcasRecordType.Should().Be((char)UcasRecordType.Subject);
                actualRecord.SendingOrganisation.Should().Be(ResultsAndCertificationConfiguration.UcasDataSettings.SendingOrganisation);
                actualRecord.ReceivingOrganisation.Should().Be(ResultsAndCertificationConfiguration.UcasDataSettings.ReceivingOrganisation);
                actualRecord.CentreNumber.Should().Be(ResultsAndCertificationConfiguration.UcasDataSettings.CentreNumber);

                actualRecord.CandidateNumber.Should().Be(expectedRecord.Uln.ToString());
                actualRecord.CandidateName.Should().Be(expectedRecord.Name);
                actualRecord.CandidateDateofBirth.Should().Be(expectedRecord.DateOfBirth);
                actualRecord.Sex.Should().Be(expectedRecord.Sex);
                actualRecord.UcaDataComponentRecord.Should().Be(expectedRecord.ComponentRecord);

            }
            AssertTrailerRecord();
        }

        [Fact]
        public async Task Then_Expected_Results_Are_Returned()
        {
            await WhenAsync();
        }

        private void SetAssessmentResult(long uln, string seriesName, string coreGrade, string specialismGrade)
        {
            var currentResult = DbContext.TqPathwayResult.FirstOrDefault(x => x.TqPathwayAssessment.TqRegistrationPathway.TqRegistrationProfile.UniqueLearnerNumber == uln && x.TqPathwayAssessment.AssessmentSeries.Name.Equals(seriesName, StringComparison.InvariantCultureIgnoreCase));
            currentResult.TlLookup = PathwayComponentGrades.FirstOrDefault(x => x.Value.Equals(coreGrade, StringComparison.InvariantCultureIgnoreCase));

            var currentSpecialismResult = DbContext.TqSpecialismResult.FirstOrDefault(x => x.TqSpecialismAssessment.TqRegistrationSpecialism.TqRegistrationPathway.TqRegistrationProfile.UniqueLearnerNumber == uln &&
            x.TqSpecialismAssessment.AssessmentSeries.Name.Equals(seriesName, StringComparison.InvariantCultureIgnoreCase));
            var x = DbContext.TqSpecialismResult.Where(x => x.TqSpecialismAssessment.TqRegistrationSpecialism.TqRegistrationPathway.TqRegistrationProfile.UniqueLearnerNumber == uln);
            var y = DbContext.TqSpecialismResult.FirstOrDefault(x => x.TqSpecialismAssessment.AssessmentSeries.Name.Equals(seriesName, StringComparison.InvariantCultureIgnoreCase));
            currentSpecialismResult.TlLookup = SpecialismComponentGrades.FirstOrDefault(x => x.Value.Equals(specialismGrade, StringComparison.InvariantCultureIgnoreCase)); 
            
            DbContext.SaveChanges();
        }
    }
}