﻿using FluentAssertions;
using NSubstitute;
using Sfa.Tl.ResultsAndCertification.Common.Enum;
using Sfa.Tl.ResultsAndCertification.Web.ViewModel.PostResultsService;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Sfa.Tl.ResultsAndCertification.Web.UnitTests.Loader.PostResultsServiceLoaderTests.GetPrsLearnerDetailsTests
{
    public class When_Called_With_AppealOutcomePathwayGradeViewModel : TestSetup
    {
        private Models.Contracts.PostResultsService.PrsLearnerDetails _expectedApiResult;
        protected AppealOutcomePathwayGradeViewModel ActualResult { get; set; }
        public override void Given()
        {
            _expectedApiResult = new Models.Contracts.PostResultsService.PrsLearnerDetails
            {
                ProfileId = 1,
                Uln = 123456789,
                Firstname = "John",
                Lastname = "Smith",
                DateofBirth = DateTime.UtcNow.AddYears(-20),
                ProviderName = "Barsley College",
                ProviderUkprn = 54678945,
                TlevelTitle = "Title",
                Status = RegistrationPathwayStatus.Active,

                PathwayAssessmentId = 99,
                PathwayAssessmentSeries = "Summer 2021",
                PathwayCode = "12345678",
                PathwayName = "Childcare Education",
                PathwayGrade = "A*",
                PathwayResultId = 77,
                PathwayPrsStatus = PrsStatus.BeingAppealed,
                PathwayGradeLastUpdatedBy = "Barsley User",
                PathwayGradeLastUpdatedOn = DateTime.Today
            };

            InternalApiClient.GetPrsLearnerDetailsAsync(AoUkprn, ProfileId, AssessmentId).Returns(_expectedApiResult);
        }

        public async override Task When()
        {
            ActualResult = await Loader.GetPrsLearnerDetailsAsync<AppealOutcomePathwayGradeViewModel>(AoUkprn, ProfileId, AssessmentId);
        }

        [Fact]
        public void Then_Returns_Expected_Results()
        {
            ActualResult.Should().NotBeNull();
            ActualResult.ProfileId.Should().Be(_expectedApiResult.ProfileId);
            ActualResult.PathwayAssessmentId.Should().Be(_expectedApiResult.PathwayAssessmentId);
            ActualResult.PathwayResultId.Should().Be(_expectedApiResult.PathwayResultId);
            ActualResult.Uln.Should().Be(_expectedApiResult.Uln);
            ActualResult.LearnerName.Should().Be($"{_expectedApiResult.Firstname} {_expectedApiResult.Lastname}");
            ActualResult.DateofBirth.Should().Be(_expectedApiResult.DateofBirth);
            ActualResult.PathwayName.Should().Be(_expectedApiResult.PathwayName);
            ActualResult.PathwayCode.Should().Be(_expectedApiResult.PathwayCode);
            ActualResult.PathwayDisplayName.Should().Be($"{_expectedApiResult.PathwayName}<br/>({_expectedApiResult.PathwayCode})");
            ActualResult.PathwayAssessmentSeries.Should().Be(_expectedApiResult.PathwayAssessmentSeries);
            ActualResult.PathwayGrade.Should().Be(_expectedApiResult.PathwayGrade);
            ActualResult.PathwayPrsStatus.Should().Be(_expectedApiResult.PathwayPrsStatus);
        }
    }
}
