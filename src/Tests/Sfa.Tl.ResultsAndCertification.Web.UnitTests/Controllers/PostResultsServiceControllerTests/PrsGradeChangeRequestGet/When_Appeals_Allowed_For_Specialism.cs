﻿using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Sfa.Tl.ResultsAndCertification.Common.Enum;
using Sfa.Tl.ResultsAndCertification.Common.Helpers;
using Sfa.Tl.ResultsAndCertification.Web.ViewModel.PostResultsService;
using System;
using Xunit;

namespace Sfa.Tl.ResultsAndCertification.Web.UnitTests.Controllers.PostResultsServiceControllerTests.PrsGradeChangeRequestGet
{
    public class When_Appeals_Allowed_For_Specialism : TestSetup
    {
        private PrsGradeChangeRequestViewModel _mockGradeChangeRequestViewModel;

        public override void Given()
        {
            ProfileId = 11;
            AssessmentId = 1;
            ComponentType = (int)Common.Enum.ComponentType.Specialism;
            ResultId = 1;

            _mockGradeChangeRequestViewModel = new PrsGradeChangeRequestViewModel
            {
                ProfileId = ProfileId,
                AssessmentId = AssessmentId,
                ResultId = ResultId,
                Status = RegistrationPathwayStatus.Active,
                PrsStatus = PrsStatus.Reviewed,
                RommEndDate = DateTime.Now.AddDays(-1),
                AppealEndDate = DateTime.Now.AddDays(10)
            };

            Loader.GetPrsLearnerDetailsAsync<PrsGradeChangeRequestViewModel>(AoUkprn, ProfileId, AssessmentId, (ComponentType)ComponentType).Returns(_mockGradeChangeRequestViewModel);
        }

        [Fact]
        public void Then_Redirected_To_PageNotFound()
        {
            var routeName = (Result as RedirectToRouteResult).RouteName;
            routeName.Should().Be(RouteConstants.PageNotFound);
        }
    }
}