﻿using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Sfa.Tl.ResultsAndCertification.Common.Enum;
using Sfa.Tl.ResultsAndCertification.Common.Helpers;
using Sfa.Tl.ResultsAndCertification.Web.ViewModel.PostResultsService;
using System;
using Xunit;

namespace Sfa.Tl.ResultsAndCertification.Web.UnitTests.Controllers.PostResultsServiceControllerTests.PrsRommGradeChangeGet
{
    public class When_Called_With_InValid_GradeCode_Specialism : TestSetup
    {
        private PrsRommGradeChangeViewModel _rommGradeChangeViewModel;

        public override void Given()
        {
            ProfileId = 0;
            AssessmentId = 7;
            ComponentType = ComponentType.Specialism;

            _rommGradeChangeViewModel = new PrsRommGradeChangeViewModel
            {
                ProfileId = ProfileId,
                AssessmentId = AssessmentId,
                Uln = 1234567890,
                Firstname = "John",
                Lastname = " Smith",
                DateofBirth = DateTime.Today.AddYears(-20),
                TlevelTitle = "TLevel in Childcare",
                SpecialismName = "Childcare",
                SpecialismLarId = "12121212",
                ExamPeriod = "Summer 2021",
                Grade = "Q pending result",
                GradeCode = "SCG5",
                PrsStatus = null,
                RommEndDate = DateTime.UtcNow.AddDays(7),
                ComponentType = ComponentType
            };

            Loader.GetPrsLearnerDetailsAsync<PrsRommGradeChangeViewModel>(AoUkprn, ProfileId, AssessmentId, ComponentType).Returns(_rommGradeChangeViewModel);
        }

        [Fact]
        public void Then_Expected_Methods_AreCalled()
        {
            Loader.Received(1).GetPrsLearnerDetailsAsync<PrsRommGradeChangeViewModel>(AoUkprn, ProfileId, AssessmentId, ComponentType);
        }

        [Fact]
        public void Then_Redirected_To_PageNotFound()
        {
            var routeName = (Result as RedirectToRouteResult).RouteName;
            routeName.Should().Be(RouteConstants.PageNotFound);
        }
    }
}
