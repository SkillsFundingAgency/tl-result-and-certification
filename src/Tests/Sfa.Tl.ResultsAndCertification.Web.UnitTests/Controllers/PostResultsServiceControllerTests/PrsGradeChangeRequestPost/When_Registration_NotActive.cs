﻿using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Sfa.Tl.ResultsAndCertification.Common.Enum;
using Sfa.Tl.ResultsAndCertification.Common.Helpers;
using Sfa.Tl.ResultsAndCertification.Web.ViewModel.PostResultsService;
using Xunit;

namespace Sfa.Tl.ResultsAndCertification.Web.UnitTests.Controllers.PostResultsServiceControllerTests.PrsGradeChangeRequestPost
{
    public class When_Registration_NotActive : TestSetup
    {
        private PrsGradeChangeRequestViewModel _mockGradeChangeRequestViewModel;

        public override void Given()
        {
            ViewModel = new PrsGradeChangeRequestViewModel
            {
                ProfileId = 1,
                AssessmentId = 2,
                ResultId = 3,
                ChangeRequestData = "Grade change"
            };

            _mockGradeChangeRequestViewModel = new PrsGradeChangeRequestViewModel
            {
                ProfileId = ViewModel.ProfileId,
                AssessmentId = ViewModel.AssessmentId,
                ResultId = 10,
                Status = RegistrationPathwayStatus.Withdrawn,
                PathwayPrsStatus = PrsStatus.Final
            };

            Loader.GetPrsLearnerDetailsAsync<PrsGradeChangeRequestViewModel>(AoUkprn, ViewModel.ProfileId, ViewModel.AssessmentId).Returns(_mockGradeChangeRequestViewModel);
        }

        [Fact]
        public void Then_Redirected_To_PageNotFound()
        {
            var routeName = (Result as RedirectToRouteResult).RouteName;
            routeName.Should().Be(RouteConstants.PageNotFound);
        }
    }
}
