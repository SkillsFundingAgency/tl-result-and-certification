﻿using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Sfa.Tl.ResultsAndCertification.Common.Enum;
using Sfa.Tl.ResultsAndCertification.Common.Helpers;
using Sfa.Tl.ResultsAndCertification.Domain.Models;
using Sfa.Tl.ResultsAndCertification.Models.Registration;
using Sfa.Tl.ResultsAndCertification.Tests.Common.DataProvider;
using Sfa.Tl.ResultsAndCertification.Tests.Common.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Sfa.Tl.ResultsAndCertification.IntegrationTests.Services.RegistrationServiceTests
{
    [Collection("BulkProcessTests")]
    public class When_CompareAndProcessRegistrationsAsync_Called_With_Transfer_Registrations : IClassFixture<BulkRegistrationsTextFixture>
    {
        private RegistrationProcessResponse _result;
        private BulkRegistrationsTextFixture _bulkRegistrationTestFixture;
        private List<OverallGradeLookup> _overallGradeLookup;

        public When_CompareAndProcessRegistrationsAsync_Called_With_Transfer_Registrations(BulkRegistrationsTextFixture bulkRegistrationTestFixture)
        {
            _bulkRegistrationTestFixture = bulkRegistrationTestFixture;

            // Given
            _bulkRegistrationTestFixture.Uln = 1111111111;
            _bulkRegistrationTestFixture.SeedTestData(EnumAwardingOrganisation.Pearson);
            var barnsleyCollegeTqProvider = _bulkRegistrationTestFixture.TqProviders.FirstOrDefault(p => p.TlProvider.UkPrn == 10000536);
            var walsallCollegeTqProvider = _bulkRegistrationTestFixture.TqProviders.FirstOrDefault(p => p.TlProvider.UkPrn == 10007315);

            _bulkRegistrationTestFixture.TqRegistrationProfileBeforeSeed = _bulkRegistrationTestFixture.SeedRegistrationData(_bulkRegistrationTestFixture.Uln, barnsleyCollegeTqProvider, seedIndustryPlacement: true);

            // PathwayAssessments seed
            var tqPathwayAssessmentsSeedData = new List<TqPathwayAssessment>();
            var seededRegistrationPathways = _bulkRegistrationTestFixture.TqRegistrationProfileBeforeSeed.TqRegistrationPathways.ToList();
            tqPathwayAssessmentsSeedData.AddRange(_bulkRegistrationTestFixture.GetPathwayAssessmentsDataToProcess(seededRegistrationPathways));
            var pathwayAssessments = _bulkRegistrationTestFixture.SeedPathwayAssessmentsData(tqPathwayAssessmentsSeedData);

            // PathwayResults seed
            var results = _bulkRegistrationTestFixture.GetPathwayResultsDataToProcess(pathwayAssessments);
            var pathwayResults = _bulkRegistrationTestFixture.SeedPathwayResultsData(results);

            // SpecialismAssessments seed
            var tqSpecialismAssessmentsSeedData = new List<TqSpecialismAssessment>();
            var seededRegistrationSpecialisms = _bulkRegistrationTestFixture.TqRegistrationProfileBeforeSeed.TqRegistrationPathways.SelectMany(x => x.TqRegistrationSpecialisms);
            tqSpecialismAssessmentsSeedData.AddRange(_bulkRegistrationTestFixture.GetSpecialismAssessmentsDataToProcess(seededRegistrationSpecialisms));
            var specialismAssessments = _bulkRegistrationTestFixture.SeedSpecialismAssessmentsData(tqSpecialismAssessmentsSeedData);

            // SpecialismResults seed
            var specialismResultsDataToProcess = _bulkRegistrationTestFixture.GetSpecialismResultsDataToProcess(specialismAssessments);
            var specialismResults = _bulkRegistrationTestFixture.SeedSpecialismResultsData(specialismResultsDataToProcess);

            // Seed OverallResultLookup
            _overallGradeLookup = new List<OverallGradeLookup>();
            var pathwayId = seededRegistrationPathways.FirstOrDefault().Id;
            var coreResultId = pathwayResults.FirstOrDefault().TlLookupId;
            var splResultId = specialismResults.FirstOrDefault().TlLookupId;
            _overallGradeLookup.Add(new OverallGradeLookup { TlPathwayId = 3, TlLookupCoreGradeId = coreResultId, TlLookupSpecialismGradeId = splResultId, TlLookupOverallGradeId = 17 });
            OverallGradeLookupProvider.CreateOverallGradeLookupList(_bulkRegistrationTestFixture.DbContext, _overallGradeLookup);

            // Seed Overall results
            OverallResultDataProvider.CreateOverallResult(_bulkRegistrationTestFixture.DbContext, new List<OverallResult> { new OverallResult { TqRegistrationPathwayId = pathwayId,
                Details = "{\"TlevelTitle\":\"T Level in Design, Surveying and Planning for Construction\",\"PathwayName\":\"Design, Surveying and Planning\",\"PathwayLarId\":\"10123456\",\"PathwayResult\":\"A*\",\"SpecialismDetails\":[{\"SpecialismName\":\"Surveying and design for construction and the built environment\",\"SpecialismLarId\":\"10123456\",\"SpecialismResult\":\"Distinction\"}],\"IndustryPlacementStatus\":\"Completed\",\"OverallResult\":\"Distinction*\"}",
                ResultAwarded = "Distinction*", CalculationStatus = CalculationStatus.Completed, CertificateType = PrintCertificateType.Certificate, StartDate = DateTime.UtcNow.AddMonths(-1), IsOptedin = true, CreatedOn = DateTime.UtcNow } }, true);

            // Input param
            var registrationDataToProcess = _bulkRegistrationTestFixture.GetRegistrationsDataToProcess(_bulkRegistrationTestFixture.Uln, walsallCollegeTqProvider);
            registrationDataToProcess.Id = 0 - Constants.RegistrationProfileStartIndex;

            var pathwayIndex = 0;
            foreach (var pathway in registrationDataToProcess.TqRegistrationPathways)
            {
                pathway.Id = pathwayIndex - Constants.RegistrationPathwayStartIndex;
            }

            var specialismIndex = 0;
            foreach (var sp in registrationDataToProcess.TqRegistrationPathways.SelectMany(p => p.TqRegistrationSpecialisms))
            {
                sp.Id = specialismIndex - Constants.RegistrationSpecialismsStartIndex;
                specialismIndex++;
            }

            var overallResultIndex = 0;
            foreach (var ovrResult in registrationDataToProcess.TqRegistrationPathways.SelectMany(p => p.OverallResults))
            {
                ovrResult.Id = overallResultIndex - Constants.OverallResultStartIndex;
                overallResultIndex++;
            }
            _bulkRegistrationTestFixture.TqRegistrationProfilesData = new List<TqRegistrationProfile> { registrationDataToProcess };
        }

        [Fact]
        public async Task Then_Expected_Registrations_Are_Amended()
        {
            // when
            await _bulkRegistrationTestFixture.WhenAsync();

            // then
            _result = _bulkRegistrationTestFixture.Result;
            _result.Should().NotBeNull();
            _result.IsSuccess.Should().BeTrue();
            _result.BulkUploadStats.Should().NotBeNull();
            _result.BulkUploadStats.TotalRecordsCount.Should().Be(_bulkRegistrationTestFixture.TqRegistrationProfilesData.Count);
            _result.BulkUploadStats.NewRecordsCount.Should().Be(0);
            _result.BulkUploadStats.AmendedRecordsCount.Should().Be(1);
            _result.BulkUploadStats.UnchangedRecordsCount.Should().Be(0);
            _result.ValidationErrors.Should().BeNullOrEmpty();

            var expectedRegistrationProfile = _bulkRegistrationTestFixture.TqRegistrationProfilesData.FirstOrDefault(p => p.UniqueLearnerNumber == _bulkRegistrationTestFixture.Uln);

            var actualRegistrationProfile = _bulkRegistrationTestFixture.DbContext.TqRegistrationProfile.AsNoTracking().Where(x => x.UniqueLearnerNumber == _bulkRegistrationTestFixture.Uln)
                                                                                                                       .Include(x => x.TqRegistrationPathways)
                                                                                                                           .ThenInclude(x => x.TqRegistrationSpecialisms)
                                                                                                                                .ThenInclude(x => x.TqSpecialismAssessments)
                                                                                                                                    .ThenInclude(x => x.TqSpecialismResults)
                                                                                                                       .Include(x => x.TqRegistrationPathways)
                                                                                                                            .ThenInclude(x => x.TqPathwayAssessments)
                                                                                                                                .ThenInclude(x => x.TqPathwayResults)
                                                                                                                       .Include(x => x.TqRegistrationPathways)
                                                                                                                            .ThenInclude(x => x.IndustryPlacements)
                                                                                                                       .Include(x => x.TqRegistrationPathways)
                                                                                                                            .ThenInclude(x => x.OverallResults)
                                                                                                                       .FirstOrDefault();
            // Assert registration profile data
            actualRegistrationProfile.Should().NotBeNull();
            actualRegistrationProfile.UniqueLearnerNumber.Should().Be(expectedRegistrationProfile.UniqueLearnerNumber);
            actualRegistrationProfile.Firstname.Should().Be(expectedRegistrationProfile.Firstname);
            actualRegistrationProfile.Lastname.Should().Be(expectedRegistrationProfile.Lastname);
            actualRegistrationProfile.DateofBirth.Should().Be(expectedRegistrationProfile.DateofBirth);
            actualRegistrationProfile.UniqueLearnerNumber.Should().Be(expectedRegistrationProfile.UniqueLearnerNumber);

            // Assert registration pathway data
            actualRegistrationProfile.TqRegistrationPathways.Where(x => x.Status == Common.Enum.RegistrationPathwayStatus.Active).ToList().Count.Should().Be(1);
            actualRegistrationProfile.TqRegistrationPathways.Where(x => x.Status == Common.Enum.RegistrationPathwayStatus.Transferred).ToList().Count.Should().Be(1);

            // Assert Transferred Pathway
            var actualTransferredPathway = actualRegistrationProfile.TqRegistrationPathways.FirstOrDefault(x => _bulkRegistrationTestFixture.TqRegistrationProfileBeforeSeed.TqRegistrationPathways.Any(y => y.TqProviderId == x.TqProviderId));
            var expectedTransferredPathway = _bulkRegistrationTestFixture.TqRegistrationProfileBeforeSeed.TqRegistrationPathways.FirstOrDefault(x => actualRegistrationProfile.TqRegistrationPathways.Any(y => y.TqProviderId == x.TqProviderId));
            AssertRegistrationPathway(actualTransferredPathway, expectedTransferredPathway);

            // Assert Active Pathway
            var activePathway = actualRegistrationProfile.TqRegistrationPathways.FirstOrDefault(x => _bulkRegistrationTestFixture.TqRegistrationProfilesData.FirstOrDefault().TqRegistrationPathways.Any(y => y.TqProviderId == x.TqProviderId));
            var expectedActivePathway = _bulkRegistrationTestFixture.TqRegistrationProfilesData.FirstOrDefault().TqRegistrationPathways.FirstOrDefault(x => actualRegistrationProfile.TqRegistrationPathways.Any(y => y.TqProviderId == x.TqProviderId));
            AssertRegistrationPathway(activePathway, expectedActivePathway);

            // Assert Active PathwayAssessment
            var actualActiveAssessment = activePathway.TqPathwayAssessments.FirstOrDefault(x => x.EndDate == null);
            var expectedActiveAssessment = expectedActivePathway.TqPathwayAssessments.FirstOrDefault(x => x.EndDate == null);
            AssertPathwayAssessment(actualActiveAssessment, expectedActiveAssessment);

            // Assert Transferred PathwayAssessment
            var actualTransferredAssessment = actualTransferredPathway.TqPathwayAssessments.FirstOrDefault(x => x.EndDate != null);
            var expectedTransferredAssessment = expectedTransferredPathway.TqPathwayAssessments.FirstOrDefault(x => x.EndDate != null);
            AssertPathwayAssessment(actualTransferredAssessment, expectedTransferredAssessment);

            // Assert Active PathwayResult
            var actualActiveResult = actualActiveAssessment.TqPathwayResults.FirstOrDefault(x => x.EndDate == null);
            var expectedActiveResult = expectedActiveAssessment.TqPathwayResults.FirstOrDefault(x => x.EndDate == null);
            AssertPathwayResults(actualActiveResult, expectedActiveResult);

            // Assert Transferred PathwayResult
            var actualTransferredResult = actualTransferredAssessment.TqPathwayResults.FirstOrDefault(x => x.EndDate != null);
            var expectedTransferredResult = expectedTransferredAssessment.TqPathwayResults.FirstOrDefault(x => x.EndDate != null);
            AssertPathwayResults(actualTransferredResult, expectedTransferredResult);

            // Assert Active SpecialismAssessment
            var actualActiveSpecialismAssessments = activePathway.TqRegistrationSpecialisms.Where(s => s.EndDate == null).SelectMany(s => s.TqSpecialismAssessments.Where(sa => sa.EndDate == null));
            var expectedActiveSpecialismAssessments = expectedActivePathway.TqRegistrationSpecialisms.Where(s => s.EndDate == null).SelectMany(s => s.TqSpecialismAssessments.Where(sa => sa.EndDate == null));
            AssertSpecialismAssessments(actualActiveSpecialismAssessments, expectedActiveSpecialismAssessments);

            // Assert Transferred SpecialismAssessment
            var actualTransferredSpecialismAssessments = actualTransferredPathway.TqRegistrationSpecialisms.Where(s => s.EndDate != null).SelectMany(s => s.TqSpecialismAssessments.Where(sa => sa.EndDate != null));
            var expectedTransferredSpecialismAssessments = expectedTransferredPathway.TqRegistrationSpecialisms.Where(s => s.EndDate != null).SelectMany(s => s.TqSpecialismAssessments.Where(sa => sa.EndDate != null));
            AssertSpecialismAssessments(actualTransferredSpecialismAssessments, expectedTransferredSpecialismAssessments);


            // Assert Active SpecialismAssessment Results
            var actualActiveSpecialismResults = actualActiveSpecialismAssessments.SelectMany(s => s.TqSpecialismResults.Where(sa => sa.EndDate == null));
            var expectedActiveSpecialismResults = expectedActiveSpecialismAssessments.SelectMany(s => s.TqSpecialismResults.Where(sa => sa.EndDate == null));
            AssertSpecialismResults(actualActiveSpecialismResults, expectedActiveSpecialismResults);

            // Assert Transferred SpecialismAssessment Results
            var actualTransferredSpecialismResults = actualTransferredSpecialismAssessments.SelectMany(s => s.TqSpecialismResults.Where(sa => sa.EndDate != null));
            var expectedTransferredSpecialismResults = expectedTransferredSpecialismAssessments.SelectMany(s => s.TqSpecialismResults.Where(sa => sa.EndDate != null));
            AssertSpecialismResults(actualTransferredSpecialismResults, expectedTransferredSpecialismResults);

            // Assert IndustryPlacement Data
            var actualActiveIndustryPlacement = activePathway.IndustryPlacements.FirstOrDefault();
            var expectedPreviousIndustryPlacement = expectedTransferredPathway.IndustryPlacements.FirstOrDefault();

            actualActiveIndustryPlacement.Status.Should().Be(expectedPreviousIndustryPlacement.Status);
            actualActiveIndustryPlacement.Details.Should().Be(expectedPreviousIndustryPlacement.Details);

            // Assert Active Overall result
            var actualActiveOverallResult = activePathway.OverallResults.FirstOrDefault(ovr => ovr.EndDate == null);
            var expectedActiveOverallResult = expectedActivePathway.OverallResults.FirstOrDefault(ovr => ovr.EndDate == null);
            AssertOverallResult(actualActiveOverallResult, expectedActiveOverallResult);

            // Assert Transferred Overall result
            var actualTransferredOverallResult = actualTransferredPathway.OverallResults.FirstOrDefault(ovr => ovr.EndDate != null);
            var expectedTransferredOverallResult = expectedTransferredPathway.OverallResults.FirstOrDefault(ovr => ovr.EndDate != null);
            AssertOverallResult(actualTransferredOverallResult, expectedTransferredOverallResult);
        }

        private static void AssertRegistrationPathway(TqRegistrationPathway actualPathway, TqRegistrationPathway expectedPathway)
        {
            actualPathway.Should().NotBeNull();
            actualPathway.TqProviderId.Should().Be(expectedPathway.TqProviderId);
            actualPathway.AcademicYear.Should().Be(expectedPathway.AcademicYear);
            actualPathway.Status.Should().Be(expectedPathway.Status);
            actualPathway.IsBulkUpload.Should().Be(expectedPathway.IsBulkUpload);

            // Assert specialisms
            actualPathway.TqRegistrationSpecialisms.Count.Should().Be(expectedPathway.TqRegistrationSpecialisms.Count);

            foreach (var expectedSpecialism in expectedPathway.TqRegistrationSpecialisms)
            {
                var actualSpecialism = actualPathway.TqRegistrationSpecialisms.FirstOrDefault(s => s.TlSpecialismId == expectedSpecialism.TlSpecialismId);

                actualSpecialism.Should().NotBeNull();
                actualSpecialism.TlSpecialismId.Should().Be(expectedSpecialism.TlSpecialismId);
                actualSpecialism.IsOptedin.Should().Be(expectedSpecialism.IsOptedin);
                actualSpecialism.IsBulkUpload.Should().Be(expectedSpecialism.IsBulkUpload);
            }
        }

        private static void AssertPathwayAssessment(TqPathwayAssessment actualAssessment, TqPathwayAssessment expectedAssessment)
        {
            actualAssessment.Should().NotBeNull();
            actualAssessment.TqRegistrationPathwayId.Should().Be(expectedAssessment.TqRegistrationPathwayId);
            actualAssessment.AssessmentSeriesId.Should().Be(expectedAssessment.AssessmentSeriesId);
            actualAssessment.IsBulkUpload.Should().BeTrue();

            if (actualAssessment.TqRegistrationPathway.Status == RegistrationPathwayStatus.Active)
            {
                actualAssessment.IsOptedin.Should().BeTrue();
                actualAssessment.EndDate.Should().BeNull();
            }
            else
            {
                actualAssessment.IsOptedin.Should().BeFalse();
                actualAssessment.EndDate.Should().NotBeNull();
            }
        }

        private static void AssertPathwayResults(TqPathwayResult actualResult, TqPathwayResult expectedResult)
        {
            actualResult.Should().NotBeNull();
            actualResult.TqPathwayAssessmentId.Should().Be(expectedResult.TqPathwayAssessmentId);
            actualResult.TlLookupId.Should().Be(expectedResult.TlLookupId);
            actualResult.PrsStatus.Should().Be(expectedResult.PrsStatus);
            actualResult.IsBulkUpload.Should().BeTrue();

            if (actualResult.TqPathwayAssessment.TqRegistrationPathway.Status == RegistrationPathwayStatus.Active)
            {
                actualResult.IsOptedin.Should().BeTrue();
                actualResult.EndDate.Should().BeNull();
            }
            else
            {
                actualResult.IsOptedin.Should().BeFalse();
                actualResult.EndDate.Should().NotBeNull();
            }
        }

        private void AssertSpecialismAssessments(IEnumerable<TqSpecialismAssessment> actualAssessments, IEnumerable<TqSpecialismAssessment> expectedAssessments)
        {
            actualAssessments.Should().NotBeEmpty();
            actualAssessments.Should().HaveSameCount(expectedAssessments);

            foreach (var expectedAssessment in expectedAssessments)
            {
                var actualAssessment = actualAssessments.FirstOrDefault(x => x.TqRegistrationSpecialismId == expectedAssessment.TqRegistrationSpecialismId);
                actualAssessment.Should().NotBeNull();
                actualAssessment.TqRegistrationSpecialismId.Should().Be(expectedAssessment.TqRegistrationSpecialismId);
                actualAssessment.AssessmentSeriesId.Should().Be(expectedAssessment.AssessmentSeriesId);
                actualAssessment.IsBulkUpload.Should().BeTrue();

                if (actualAssessment.TqRegistrationSpecialism.TqRegistrationPathway.Status == RegistrationPathwayStatus.Active)
                {
                    actualAssessment.IsOptedin.Should().BeTrue();
                    actualAssessment.EndDate.Should().BeNull();
                }
                else
                {
                    actualAssessment.IsOptedin.Should().BeFalse();
                    actualAssessment.EndDate.Should().NotBeNull();
                }
            }
        }

        private void AssertSpecialismResults(IEnumerable<TqSpecialismResult> actualResults, IEnumerable<TqSpecialismResult> expectedResults)
        {
            actualResults.Should().NotBeEmpty();
            actualResults.Should().HaveSameCount(expectedResults);

            foreach (var expectedResult in expectedResults)
            {
                var actualResult = actualResults.FirstOrDefault(x => x.TqSpecialismAssessmentId == expectedResult.TqSpecialismAssessmentId);
                actualResult.Should().NotBeNull();
                actualResult.TqSpecialismAssessmentId.Should().Be(expectedResult.TqSpecialismAssessmentId);
                actualResult.TlLookupId.Should().Be(expectedResult.TlLookupId);
                actualResult.PrsStatus.Should().Be(expectedResult.PrsStatus);
                actualResult.IsBulkUpload.Should().BeTrue();

                if (actualResult.TqSpecialismAssessment.TqRegistrationSpecialism.TqRegistrationPathway.Status == RegistrationPathwayStatus.Active)
                {
                    actualResult.IsOptedin.Should().BeTrue();
                    actualResult.EndDate.Should().BeNull();
                }
                else
                {
                    actualResult.IsOptedin.Should().BeFalse();
                    actualResult.EndDate.Should().NotBeNull();
                }
            }
        }

        private void AssertOverallResult(OverallResult actualOverallResult, OverallResult expectedOverallResult)
        {
            actualOverallResult.TqRegistrationPathwayId.Should().Be(expectedOverallResult.TqRegistrationPathwayId);
            actualOverallResult.Details.Should().Be(expectedOverallResult.Details);
            actualOverallResult.ResultAwarded.Should().Be(expectedOverallResult.ResultAwarded);
            actualOverallResult.CalculationStatus.Should().Be(expectedOverallResult.CalculationStatus);
            actualOverallResult.PrintAvailableFrom.Should().Be(expectedOverallResult.PrintAvailableFrom);
            actualOverallResult.PublishDate.Should().Be(expectedOverallResult.PublishDate);
            actualOverallResult.CertificateType.Should().Be(expectedOverallResult.CertificateType);
            if (expectedOverallResult.EndDate == null)
                actualOverallResult.EndDate.Should().BeNull();
            else
                actualOverallResult.EndDate.Should().NotBeNull();
        }
    }
}
