﻿using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Sfa.Tl.ResultsAndCertification.Common.Extensions;
using Sfa.Tl.ResultsAndCertification.Web.Controllers;
using System.Collections.Generic;
using System.Security.Claims;
using Xunit;

namespace Sfa.Tl.ResultsAndCertification.Web.UnitTests.Controllers.AccountControllerTests.PostSignIn
{
    public class When_User_WithNoServiceAccess : TestSetup
    {
        public override void Given()
        {
            HttpContext.User.Identity.IsAuthenticated.Returns(true);
            HttpContext.User.Claims.Returns(new List<Claim> { new Claim(CustomClaimTypes.HasAccessToService, "false") });

            Controller.ControllerContext = new ControllerContext
            {
                HttpContext = HttpContext,
                RouteData = new Microsoft.AspNetCore.Routing.RouteData()
            };
        }

        [Fact]
        public void Then_Redirected_To_ServiceAccessDenied()
        {
            Result.Should().NotBeNull();

            var actualControlName = (Result as RedirectToActionResult).ControllerName;
            var actualActionName = (Result as RedirectToActionResult).ActionName;

            actualControlName.Should().Be(Common.Helpers.Constants.ErrorController);
            actualActionName.Should().Be(nameof(ErrorController.ServiceAccessDenied));
        }
    }
}
