﻿using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Sfa.Tl.ResultsAndCertification.Common.Enum;
using Sfa.Tl.ResultsAndCertification.Common.Extensions;
using Sfa.Tl.ResultsAndCertification.Common.Helpers;
using Sfa.Tl.ResultsAndCertification.Web.ViewModel.PostResultsService;
using System;
using Xunit;
using LearnerDetailsContent = Sfa.Tl.ResultsAndCertification.Web.Content.PostResultsService.PrsPathwayGradeCheckAndSubmit;

namespace Sfa.Tl.ResultsAndCertification.Web.UnitTests.Controllers.PostResultsServiceControllerTests.PrsPathwayGradeCheckAndSubmitGet
{
    public class When_Grades_AreSame : TestSetup
    {
        private PrsPathwayGradeCheckAndSubmitViewModel _mockCache = null;

        public override void Given()
        {
            var previousGrade = "A";
            var newGrade = "A";

            _mockCache = new PrsPathwayGradeCheckAndSubmitViewModel
            {
                Uln = 1234567890,
                Firstname = "John",
                Lastname = "Smith",
                DateofBirth = DateTime.Today.AddYears(-20),
                TlevelTitle = "Tlevel in Education",
                PathwayTitle = "Educateion (1234455)",
                ProviderName = "Barsley College",
                ProviderUkprn = 87654321,
                NewGrade = newGrade,
                OldGrade = previousGrade,
                IsGradeChanged = false,

                ProfileId = 1,
                AssessmentId = 2,
                ResultId= 3,
            };
            CacheService.GetAsync<PrsPathwayGradeCheckAndSubmitViewModel>(CacheKey).Returns(_mockCache);
        }

        [Fact]
        public void Then_Returns_Expected_Results()
        {
            var viewResult = Result as ViewResult;
            var model = viewResult.Model as PrsPathwayGradeCheckAndSubmitViewModel;

            model.Should().NotBeNull();
            model.Firstname.Should().Be(_mockCache.Firstname);
            model.Lastname.Should().Be(_mockCache.Lastname);
            model.LearnerName.Should().Be($"{_mockCache.Firstname} {_mockCache.Lastname}");
            model.ProviderName.Should().Be(_mockCache.ProviderName);
            model.ProviderUkprn.Should().Be(_mockCache.ProviderUkprn);
            model.ProviderDisplayName.Should().Be($"{_mockCache.ProviderName}<br/>({_mockCache.ProviderUkprn})");
            model.TlevelTitle.Should().Be(_mockCache.TlevelTitle);
            model.NewGrade.Should().Be(_mockCache.NewGrade);
            model.OldGrade.Should().Be(_mockCache.OldGrade);

            model.ProfileId.Should().Be(_mockCache.ProfileId);
            model.AssessmentId.Should().Be(_mockCache.AssessmentId);
            model.ResultId.Should().Be(_mockCache.ResultId);
            model.IsGradeChanged.Should().BeFalse();

            // Uln
            model.SummaryUln.Title.Should().Be(LearnerDetailsContent.Title_Uln_Text);
            model.SummaryUln.Value.Should().Be(_mockCache.Uln.ToString());

            // LearnerName
            model.SummaryLearnerName.Title.Should().Be(LearnerDetailsContent.Title_Name_Text);
            model.SummaryLearnerName.Value.Should().Be(_mockCache.LearnerName);

            // DateofBirth
            model.SummaryDateofBirth.Title.Should().Be(LearnerDetailsContent.Title_DateofBirth_Text);
            model.SummaryDateofBirth.Value.Should().Be(_mockCache.DateofBirth.ToDobFormat());

            // ProviderName
            model.SummaryProvider.Title.Should().Be(LearnerDetailsContent.Title_Provider_Text);
            model.SummaryProvider.Value.Should().Be(_mockCache.ProviderDisplayName);

            // TLevelTitle
            model.SummaryTlevelTitle.Title.Should().Be(LearnerDetailsContent.Title_TLevel_Text);
            model.SummaryTlevelTitle.Value.Should().Be(_mockCache.TlevelTitle);

            // Old Grade
            model.SummaryOldGrade.Title.Should().Be(LearnerDetailsContent.Title_Old_Grade);
            model.SummaryOldGrade.Value.Should().Be(_mockCache.OldGrade);

            // New Grade
            model.SummaryNewGrade.Title.Should().Be(LearnerDetailsContent.Title_New_Grade);
            model.SummaryNewGrade.Value.Should().Be(_mockCache.NewGrade);

            // Backlink
            model.BackLink.Should().NotBeNull();
            model.BackLink.RouteName.Should().Be(RouteConstants.PrsAppealOutcomePathwayGrade);
            model.BackLink.RouteAttributes.Count.Should().Be(4);
            model.BackLink.RouteAttributes.TryGetValue(Constants.ProfileId, out string routeProfileId);
            routeProfileId.Should().Be(_mockCache.ProfileId.ToString());
            model.BackLink.RouteAttributes.TryGetValue(Constants.AssessmentId, out string routeAssessmentId);
            routeAssessmentId.Should().Be(_mockCache.AssessmentId.ToString());
            model.BackLink.RouteAttributes.TryGetValue(Constants.ResultId, out string routeResultId);
            routeResultId.Should().Be(_mockCache.ResultId.ToString());
            model.BackLink.RouteAttributes.TryGetValue(Constants.AppealOutcomeTypeId, out string routeAppealOutcomeTypeId);
            routeAppealOutcomeTypeId.Should().Be(((int)AppealOutcomeType.SameGrade).ToString());
        }

        [Fact]
        public void Then_Expected_Methods_AreCalled()
        {
            CacheService.Received(1).GetAsync<PrsPathwayGradeCheckAndSubmitViewModel>(CacheKey);
        }
    }
}
