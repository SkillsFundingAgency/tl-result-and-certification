﻿using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Sfa.Tl.ResultsAndCertification.Common.Enum;
using Sfa.Tl.ResultsAndCertification.Common.Helpers;
using Sfa.Tl.ResultsAndCertification.Web.ViewModel.PostResultsService;
using Xunit;

namespace Sfa.Tl.ResultsAndCertification.Web.UnitTests.Controllers.PostResultsServiceControllerTests.PrsAddAppealOutcomePost
{
    public class When_PrsStatus_Is_Invalid_For_Specialism : TestSetup
    {
        private PrsAddAppealOutcomeViewModel _mockLoaderResponse = null;

        public override void Given()
        {
            ProfileId = 1;
            AssessmentId = 10;
            ComponentType = ComponentType.Specialism;

            _mockLoaderResponse = new PrsAddAppealOutcomeViewModel
            {
                ProfileId = 1,
                AssessmentId = 11,
                ResultId = 17,
                Firstname = "Test",
                Lastname = "John",
                ExamPeriod = "Summer 2022",
                CoreName = "Education",
                CoreLarId = "1234567",
                SpecialismName = "Plumbing Engineering",
                SpecialismLarId = "Z1234567",
                ComponentType = ComponentType,
                PrsStatus = null,
                AppealOutcome = AppealOutcomeType.GradeChanged
            };

            ViewModel = new PrsAddAppealOutcomeViewModel { ProfileId = 1, AssessmentId = 11, AppealOutcome = AppealOutcomeType.GradeChanged };
            Loader.GetPrsLearnerDetailsAsync<PrsAddAppealOutcomeViewModel>(AoUkprn, ViewModel.ProfileId, ViewModel.AssessmentId, ComponentType)
                  .Returns(_mockLoaderResponse);
        }

        [Fact]
        public void Then_Redirected_To_PageNotFound()
        {
            var routeName = (Result as RedirectToRouteResult).RouteName;
            routeName.Should().Be(RouteConstants.PageNotFound);
        }
    }
}