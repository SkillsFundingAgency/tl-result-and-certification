﻿using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Sfa.Tl.ResultsAndCertification.Application.Interfaces;
using Sfa.Tl.ResultsAndCertification.Application.Services;
using Sfa.Tl.ResultsAndCertification.Common.Enum;
using Sfa.Tl.ResultsAndCertification.Data.Interfaces;
using Sfa.Tl.ResultsAndCertification.Data.Repositories;
using Sfa.Tl.ResultsAndCertification.Domain.Models;
using Sfa.Tl.ResultsAndCertification.Models.Configuration;
using Sfa.Tl.ResultsAndCertification.Models.Functions;
using Sfa.Tl.ResultsAndCertification.Tests.Common.DataBuilders;
using Sfa.Tl.ResultsAndCertification.Tests.Common.DataProvider;
using Sfa.Tl.ResultsAndCertification.Tests.Common.Enum;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Sfa.Tl.ResultsAndCertification.IntegrationTests.Services.UcasDataServiceTests
{
    public abstract class UcasDataServiceBaseTest : BaseTest<TqRegistrationPathway>
    {
        protected long AoUkprn = 10011881;
        protected TlRoute Route;
        protected TlPathway Pathway;
        protected TlSpecialism Specialism;
        protected IList<TlSpecialism> Specialisms;
        protected TlProvider TlProvider;
        protected TlAwardingOrganisation TlAwardingOrganisation;
        protected TqAwardingOrganisation TqAwardingOrganisation;
        protected TqProvider TqProvider;
        protected IList<TlProvider> TlProviders;
        protected IList<AssessmentSeries> AssessmentSeries;
        protected IList<TlLookup> TlLookup;
        protected IList<TlLookup> PathwayComponentGrades;
        protected IList<TlLookup> SpecialismComponentGrades;
        protected IList<AcademicYear> AcademicYears;

        protected IUcasRepository UcasRepository;
        protected ICommonRepository CommonRepository;
        protected IUcasRecordSegment<UcasRecordEntriesSegment> UcasRecordEntrySegment;
        protected IUcasRecordSegment<UcasRecordResultsSegment> UcasRecordResultsSegment;
        protected ResultsAndCertificationConfiguration ResultsAndCertificationConfiguration;
        protected IUcasDataService UcasDataService;
        protected UcasData ActualResult;

        protected void CreateService()
        {
            ResultsAndCertificationConfiguration = new ResultsAndCertificationConfiguration
            {
                UcasDataSettings = new UcasDataSettings
                {
                    CentreNumber = "1111111",
                    ExamMonth = "06",
                    OverallSubjectCode = "TLEVEL",
                    ReceivingOrganisation = "90",
                    SendingOrganisation = "30",
                    IndustryPlacementCode ="INDUSTRYP"
                }
            };

            CommonRepository = new CommonRepository(DbContext);
            UcasRepository = new UcasRepository(DbContext, CommonRepository);
            UcasRecordEntrySegment = new UcasRecordEntriesSegment();
            UcasRecordResultsSegment = new UcasRecordResultsSegment();

            UcasDataService = new UcasDataService(UcasRepository, UcasRecordEntrySegment, UcasRecordResultsSegment, ResultsAndCertificationConfiguration);
        }

        protected virtual void SeedTestData(EnumAwardingOrganisation awardingOrganisation = EnumAwardingOrganisation.Pearson, bool seedMultipleProviders = false)
        {
            TlAwardingOrganisation = TlevelDataProvider.CreateTlAwardingOrganisation(DbContext, awardingOrganisation);
            Route = TlevelDataProvider.CreateTlRoute(DbContext, awardingOrganisation);
            Pathway = TlevelDataProvider.CreateTlPathway(DbContext, awardingOrganisation, Route);
            Specialism = TlevelDataProvider.CreateTlSpecialisms(DbContext, awardingOrganisation, Pathway).First();
            TqAwardingOrganisation = TlevelDataProvider.CreateTqAwardingOrganisation(DbContext, Pathway, TlAwardingOrganisation);
            TlProvider = ProviderDataProvider.CreateTlProvider(DbContext);
            TqProvider = ProviderDataProvider.CreateTqProvider(DbContext, TqAwardingOrganisation, TlProvider);
            AssessmentSeries = AssessmentSeriesDataProvider.CreateAssessmentSeriesList(DbContext, null, true);
            TlLookup = TlLookupDataProvider.CreateTlLookupList(DbContext, null, true);
            PathwayComponentGrades = TlLookup.Where(x => x.Category.Equals(LookupCategory.PathwayComponentGrade.ToString(), StringComparison.InvariantCultureIgnoreCase)).ToList();
            SpecialismComponentGrades = TlLookupDataProvider.CreateSpecialismGradeTlLookupList(DbContext, null, true);
            AcademicYears = AcademicYearDataProvider.CreateAcademicYearList(DbContext, null);

            DbContext.SaveChanges();
        }

        public List<TqRegistrationProfile> SeedRegistrationsDataByStatus(Dictionary<long, RegistrationPathwayStatus> ulns, TqProvider tqProvider = null)
        {
            var profiles = new List<TqRegistrationProfile>();

            foreach (var uln in ulns)
            {
                profiles.Add(SeedRegistrationDataByStatus(uln.Key, uln.Value, tqProvider));
            }
            return profiles;
        }

        public TqRegistrationProfile SeedRegistrationDataByStatus(long uln, RegistrationPathwayStatus status = RegistrationPathwayStatus.Active, TqProvider tqProvider = null)
        {
            var profile = new TqRegistrationProfileBuilder().BuildList().FirstOrDefault(p => p.UniqueLearnerNumber == uln);
            var tqRegistrationProfile = RegistrationsDataProvider.CreateTqRegistrationProfile(DbContext, profile);
            var tqRegistrationPathway = RegistrationsDataProvider.CreateTqRegistrationPathway(DbContext, tqRegistrationProfile, tqProvider ?? TqProvider);
            tqRegistrationPathway.Status = status;

            var tqRegistrationSpecialism = RegistrationsDataProvider.CreateTqRegistrationSpecialism(DbContext, tqRegistrationPathway, Specialism);

            if (status == RegistrationPathwayStatus.Withdrawn)
            {
                tqRegistrationPathway.EndDate = DateTime.UtcNow.AddDays(-1);
                tqRegistrationSpecialism.IsOptedin = true;
                tqRegistrationSpecialism.EndDate = DateTime.UtcNow.AddDays(-1);
            }

            DbContext.SaveChanges();
            return profile;
        }

        public List<TqPathwayAssessment> GetPathwayAssessmentsDataToProcess(List<TqRegistrationPathway> pathwayRegistrations, string assessmentSeriesName = "Summer 2021", bool seedPathwayAssessmentsAsActive = true, bool isHistorical = false)
        {
            var tqPathwayAssessments = new List<TqPathwayAssessment>();

            foreach (var (pathwayRegistration, index) in pathwayRegistrations.Select((value, i) => (value, i)))
            {
                if (isHistorical)
                {
                    // Historical record
                    var pathwayAssessment = new TqPathwayAssessmentBuilder().Build(pathwayRegistration, AssessmentSeries[index]);
                    pathwayAssessment.IsOptedin = false;
                    pathwayAssessment.EndDate = DateTime.UtcNow.AddDays(-1);

                    var tqPathwayAssessmentHistorical = PathwayAssessmentDataProvider.CreateTqPathwayAssessment(DbContext, pathwayAssessment);
                    tqPathwayAssessments.Add(tqPathwayAssessmentHistorical);
                }

                var assessmentSeries = AssessmentSeries.FirstOrDefault(x => x.Name.Equals(assessmentSeriesName, StringComparison.InvariantCultureIgnoreCase));
                var activePathwayAssessment = new TqPathwayAssessmentBuilder().Build(pathwayRegistration, assessmentSeries);
                var tqPathwayAssessment = PathwayAssessmentDataProvider.CreateTqPathwayAssessment(DbContext, activePathwayAssessment);
                if (!seedPathwayAssessmentsAsActive)
                {
                    tqPathwayAssessment.IsOptedin = pathwayRegistration.Status == RegistrationPathwayStatus.Withdrawn ? true : false;
                    tqPathwayAssessment.EndDate = DateTime.UtcNow;
                }

                tqPathwayAssessments.Add(tqPathwayAssessment);
            }
            return tqPathwayAssessments;
        }

        public List<TqPathwayResult> GetPathwayResultsDataToProcess(List<TqPathwayAssessment> pathwayAssessments, bool seedPathwayResultsAsActive = true, bool isHistorical = false)
        {
            var tqPathwayResults = new List<TqPathwayResult>();

            foreach (var (pathwayAssessment, index) in pathwayAssessments.Select((value, i) => (value, i)))
            {
                var pathwayGrade = TlLookup.FirstOrDefault();
                var tqresults = GetPathwayResultDataToProcess(pathwayAssessment, pathwayGrade, seedPathwayResultsAsActive, isHistorical, null, true);
                tqPathwayResults.AddRange(tqresults);
            }
            return tqPathwayResults;
        }

        public List<TqPathwayResult> GetPathwayResultDataToProcess(TqPathwayAssessment pathwayAssessment, TlLookup tlLookupComponentGrade, bool seedPathwayResultsAsActive = true, bool isHistorical = false, PrsStatus? prsStatus = null, bool isBulkUpload = true)
        {
            var tqPathwayResults = new List<TqPathwayResult>();

            if (isHistorical)
            {
                // Historical record
                var pathwayResult = new TqPathwayResultBuilder().Build(pathwayAssessment, tlLookupComponentGrade, isBulkUpload: isBulkUpload);
                pathwayResult.IsOptedin = false;
                pathwayResult.EndDate = DateTime.UtcNow.AddDays(-1);

                var tqPathwayResultHistorical = TqPathwayResultDataProvider.CreateTqPathwayResult(DbContext, pathwayResult);
                tqPathwayResults.Add(tqPathwayResultHistorical);
            }

            var activePathwayResult = new TqPathwayResultBuilder().Build(pathwayAssessment, tlLookupComponentGrade, isBulkUpload: isBulkUpload);
            var tqPathwayResult = TqPathwayResultDataProvider.CreateTqPathwayResult(DbContext, activePathwayResult);
            if (!seedPathwayResultsAsActive)
            {
                tqPathwayResult.IsOptedin = pathwayAssessment.TqRegistrationPathway.Status == RegistrationPathwayStatus.Withdrawn;
                tqPathwayResult.EndDate = DateTime.UtcNow;
            }
            else
            {
                tqPathwayResult.PrsStatus = prsStatus;
            }

            tqPathwayResults.Add(tqPathwayResult);
            return tqPathwayResults;
        }

        public List<TqSpecialismAssessment> GetSpecialismAssessmentsDataToProcess(List<TqRegistrationSpecialism> specialismRegistrations, string assessmentSeriesName = "Summer 2021", bool seedSpecialismAssessmentsAsActive = true, bool isHistorical = false)
        {
            var tqSpecialismAssessments = new List<TqSpecialismAssessment>();

            foreach (var (specialismRegistration, index) in specialismRegistrations.Select((value, i) => (value, i)))
            {
                if (isHistorical)
                {
                    // Historical record
                    var specialismAssessment = new TqSpecialismAssessmentBuilder().Build(specialismRegistration, AssessmentSeries[index]);
                    specialismAssessment.IsOptedin = false;
                    specialismAssessment.EndDate = DateTime.UtcNow.AddDays(-1);

                    var tqSpecialismAssessmentHistorical = SpecialismAssessmentDataProvider.CreateTqSpecialismAssessment(DbContext, specialismAssessment);
                    tqSpecialismAssessments.Add(tqSpecialismAssessmentHistorical);
                }

                var assessmentSeries = AssessmentSeries.FirstOrDefault(x => x.Name.Equals(assessmentSeriesName, StringComparison.InvariantCultureIgnoreCase));
                var activeSpecialismAssessment = new TqSpecialismAssessmentBuilder().Build(specialismRegistration, assessmentSeries);
                var tqSpecialismAssessment = SpecialismAssessmentDataProvider.CreateTqSpecialismAssessment(DbContext, activeSpecialismAssessment);
                if (!seedSpecialismAssessmentsAsActive)
                {
                    tqSpecialismAssessment.IsOptedin = specialismRegistration.TqRegistrationPathway.Status == Common.Enum.RegistrationPathwayStatus.Withdrawn ? true : false;
                    tqSpecialismAssessment.EndDate = DateTime.UtcNow;
                }
                tqSpecialismAssessments.Add(tqSpecialismAssessment);

            }
            return tqSpecialismAssessments;
        }

        public List<TqSpecialismResult> GetSpecialismResultsDataToProcess(List<TqSpecialismAssessment> specialismAssessments, bool seedSpecialismResultsAsActive = true, bool isHistorical = false)
        {
            var tqSpecialismResults = new List<TqSpecialismResult>();

            foreach (var (specialismAssessment, index) in specialismAssessments.Select((value, i) => (value, i)))
            {
                if (isHistorical)
                {
                    // Historical record
                    var specialismResult = new TqSpecialismResultBuilder().Build(specialismAssessment, SpecialismComponentGrades[index]);
                    specialismResult.IsOptedin = false;
                    specialismResult.EndDate = DateTime.UtcNow.AddDays(-1);

                    var tqSpecialismResultHistorical = TqSpecialismResultDataProvider.CreateTqSpecialismResult(DbContext, specialismResult);
                    tqSpecialismResults.Add(tqSpecialismResultHistorical);
                }

                var activeSpecialismResult = new TqSpecialismResultBuilder().Build(specialismAssessment, SpecialismComponentGrades[index]);
                var tqSpecialismResult = TqSpecialismResultDataProvider.CreateTqSpecialismResult(DbContext, activeSpecialismResult);
                if (!seedSpecialismResultsAsActive)
                {
                    tqSpecialismResult.IsOptedin = specialismAssessment.TqRegistrationSpecialism.TqRegistrationPathway.Status == RegistrationPathwayStatus.Withdrawn ? true : false;
                    tqSpecialismResult.EndDate = DateTime.UtcNow;
                }

                tqSpecialismResults.Add(tqSpecialismResult);
            }
            return tqSpecialismResults;
        }

        public void SeedAssessmentsAndResults(List<TqRegistrationProfile> registrations, List<long> componentWithAssessments, List<long> componentWithResults, string assessmentSeriesName)
        {
            // Pathway Assessments
            var tqPathwayAssessmentsSeedData = new List<TqPathwayAssessment>();
            var tqPathwayResultsSeedData = new List<TqPathwayResult>();

            foreach (var registration in registrations.Where(x => componentWithAssessments.Contains(x.UniqueLearnerNumber)))
            {
                var pathwayAssessments = GetPathwayAssessmentsDataToProcess(registration.TqRegistrationPathways.ToList(), assessmentSeriesName);
                tqPathwayAssessmentsSeedData.AddRange(pathwayAssessments);

                var pathwayAssessmentsWithResults = pathwayAssessments.Where(x => componentWithResults.Contains(x.TqRegistrationPathway.TqRegistrationProfile.UniqueLearnerNumber)).ToList();
                tqPathwayResultsSeedData.AddRange(GetPathwayResultsDataToProcess(pathwayAssessmentsWithResults));
            }

            // Specialism Assessments
            var tqSpecialismAssessmentsSeedData = new List<TqSpecialismAssessment>();
            var tqSpecialismResultsSeedData = new List<TqSpecialismResult>();

            foreach (var registration in registrations.Where(x => componentWithAssessments.Contains(x.UniqueLearnerNumber)))
            {
                var specialismAssessments = GetSpecialismAssessmentsDataToProcess(registration.TqRegistrationPathways.FirstOrDefault().TqRegistrationSpecialisms.ToList(), assessmentSeriesName);
                tqSpecialismAssessmentsSeedData.AddRange(specialismAssessments);

                var specialismAssessmentsWithResults = specialismAssessments.Where(x => componentWithResults.Contains(x.TqRegistrationSpecialism.TqRegistrationPathway.TqRegistrationProfile.UniqueLearnerNumber)).ToList();
                tqSpecialismResultsSeedData.AddRange(GetSpecialismResultsDataToProcess(specialismAssessmentsWithResults));
            }

            DbContext.SaveChanges();
        }

        public List<OverallResult> SeedOverallResultData(List<TqRegistrationProfile> registrations, List<long> ulnsWithOverallResult, bool saveChanges = true)
        {
            var overallResults = new List<OverallResult>();
            foreach (var ulnOverResult in ulnsWithOverallResult)
            {
                var registration = registrations.FirstOrDefault(reg => reg.UniqueLearnerNumber == ulnOverResult);
                overallResults.Add(OverallResultDataProvider.CreateOverallResult(DbContext, registration.TqRegistrationPathways.FirstOrDefault()));
            }

            if (saveChanges)
                DbContext.SaveChanges();
            return overallResults;
        }

        public FunctionLog SeedFunctionLog(FunctionType functionType, bool saveChanges = true)
        {
            var functionLogData = new FunctionLog
            {
                FunctionType = functionType,
                Name = "Function",
                StartDate = DateTime.UtcNow.AddMonths(-1),
                EndDate = DateTime.UtcNow.AddMonths(-1).AddHours(1),
                Message = "Completed successfully",
                Status = FunctionStatus.Processed,
                CreatedOn = DateTime.UtcNow.AddHours(-1),
            };

            var functionLog = FunctionLogDataProvider.CreateFunctionLog(DbContext, functionLogData);

            if (saveChanges)
                DbContext.SaveChanges();

            return functionLog;

        }

        public void SetAcademicYear(List<TqRegistrationProfile> _registrations, List<long> ulns, int offset)
        {
            var currentAcademicYear = AcademicYears.FirstOrDefault(x => DateTime.Today >= x.StartDate && DateTime.Today <= x.EndDate).Year + offset;
            _registrations.Where(x => ulns.Contains(x.UniqueLearnerNumber)).ToList()
                .ForEach(x => { x.TqRegistrationPathways.FirstOrDefault().AcademicYear = currentAcademicYear; });
        }

        public void AssertHeaderRecord(UcasDataType ucasDataType)
        {
            ActualResult.Header.Should().NotBeNull();
            ActualResult.Header.UcasRecordType.Should().Be((char)UcasRecordType.Header);
            ActualResult.Header.SendingOrganisation.Should().Be(ResultsAndCertificationConfiguration.UcasDataSettings.SendingOrganisation);
            ActualResult.Header.ReceivingOrganisation.Should().Be(ResultsAndCertificationConfiguration.UcasDataSettings.ReceivingOrganisation);
            ActualResult.Header.UcasDataType.Should().Be((char)ucasDataType);
            ActualResult.Header.ExamMonth.Should().Be(ResultsAndCertificationConfiguration.UcasDataSettings.ExamMonth);
            ActualResult.Header.ExamYear.Should().Be(DateTime.UtcNow.Year.ToString());
            ActualResult.Header.DateCreated.Should().Be(DateTime.Today.ToString("ddMMyyyy", CultureInfo.InvariantCulture));
        }

        public void AssertTrailerRecord()
        {
            ActualResult.Trailer.Should().NotBeNull();
            ActualResult.Trailer.UcasRecordType.Should().Be((char)UcasRecordType.Trailer);
            ActualResult.Trailer.SendingOrganisation.Should().Be(ResultsAndCertificationConfiguration.UcasDataSettings.SendingOrganisation);
            ActualResult.Trailer.ReceivingOrganisation.Should().Be(ResultsAndCertificationConfiguration.UcasDataSettings.ReceivingOrganisation);
            ActualResult.Trailer.Count.Should().Be(ActualResult.UcasDataRecords.Count() + 2);
            ActualResult.Trailer.ExamDate.Should().Be($"{ResultsAndCertificationConfiguration.UcasDataSettings.ExamMonth}{DateTime.UtcNow.Year}");
            ActualResult.Trailer.RecordTerminator.Should().BeEmpty();
        }

        public int GetAcademicYear()
        {
            return AcademicYears.FirstOrDefault(x => DateTime.Today >= x.StartDate && DateTime.Today <= x.EndDate).Year;
        }
    }

    public class ExepectedUcasDataRecord
    {
        public long Uln { get; set; }
        public string Name { get; set; }
        public string Sex { get; set; }
        public string DateOfBirth { get; set; }
        public string ComponentRecord { get; set; }
    }
}
