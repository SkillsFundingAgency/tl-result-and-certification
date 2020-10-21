﻿using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Sfa.Tl.ResultsAndCertification.Common.Helpers;
using Xunit;

namespace Sfa.Tl.ResultsAndCertification.Web.UnitTests.Controllers.ManageRegistrationControllerTests.ChangeDateofBirthPost
{
    public class When_DateofBirth_Change_Failed : TestSetup
    {
        public override void Given()
        {
            MockResult.IsSuccess = false;
            MockResult.IsModified = true;

            RegistrationLoader.ProcessDateofBirthChangeAsync(AoUkprn, ViewModel)
                .Returns(MockResult);
        }

        [Fact]
        public void Then_Redirected_To_ProblemWithService()
        {
            var routeName = (Result as RedirectToRouteResult).RouteName;
            routeName.Should().Be(RouteConstants.ProblemWithService);
        }
    }
}
