﻿using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Sfa.Tl.ResultsAndCertification.Common.Enum;
using Sfa.Tl.ResultsAndCertification.Common.Extensions;
using Sfa.Tl.ResultsAndCertification.Common.Helpers;
using Sfa.Tl.ResultsAndCertification.Tests.Common.Helpers;
using Sfa.Tl.ResultsAndCertification.Web.Controllers;
using Sfa.Tl.ResultsAndCertification.Web.ViewModel.Dashboard;
using System.Security.Claims;
using Xunit;

namespace Sfa.Tl.ResultsAndCertification.Web.UnitTests.Controllers.DashboardControllerTests.Index
{
    public class When_Invalid_Admin_UserType : TestSetup
    {
        public override void Given()
        {
            var httpContext = new ClaimsIdentityBuilder<DashboardController>(Controller)
                .Add(CustomClaimTypes.LoginUserType, ((int)LoginUserType.Admin).ToString())
                .Add(ClaimTypes.Role, string.Empty)
                .Build().HttpContext;

            HttpContextAccessor.HttpContext.Returns(httpContext);
            Loader.GetDashboardViewModel(httpContext.User).Returns(new DashboardViewModel { HasAccessToService = false });
        }

        [Fact]
        public void Then_Expected_Result()
        {
            var routeName = (Result as RedirectToRouteResult).RouteName;
            routeName.Should().Be(RouteConstants.ServiceAccessDenied);
        }
    }
}
