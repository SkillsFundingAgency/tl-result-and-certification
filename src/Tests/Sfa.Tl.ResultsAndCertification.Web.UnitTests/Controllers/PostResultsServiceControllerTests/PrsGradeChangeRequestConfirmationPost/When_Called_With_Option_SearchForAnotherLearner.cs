﻿using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Sfa.Tl.ResultsAndCertification.Common.Enum;
using Sfa.Tl.ResultsAndCertification.Common.Helpers;
using Sfa.Tl.ResultsAndCertification.Web.ViewModel.PostResultsService;
using Sfa.Tl.ResultsAndCertification.Web.ViewModel.SearchRegistration.Enum;
using Xunit;

namespace Sfa.Tl.ResultsAndCertification.Web.UnitTests.Controllers.PostResultsServiceControllerTests.PrsGradeChangeRequestConfirmationPost
{
    public class When_Called_With_Option_SearchForAnotherLearner : TestSetup
    {
        public override void Given()
        {
            ViewModel = new PrsGradeChangeRequestConfirmationViewModel { ProfileId = 1, NavigationOption = PrsGradeChangeConfirmationNavigationOptions.SearchForAnotherLearner };
        }

        [Fact]
        public void Then_Redirected_To_PrsSearchLearner()
        {
            var route = Result as RedirectToRouteResult;
            route.RouteName.Should().Be(RouteConstants.SearchRegistration);

            route.RouteValues.Should().HaveCount(1);
            route.RouteValues.Should().ContainKey(Constants.Type);
            route.RouteValues[Constants.Type].Should().Be(SearchRegistrationType.PostResult.ToString());
        }
    }
}
