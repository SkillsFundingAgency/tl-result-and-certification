﻿using FluentAssertions;
using Sfa.Tl.ResultsAndCertification.Common.Helpers;
using Sfa.Tl.ResultsAndCertification.Web.UnitTests.Helpers;
using Sfa.Tl.ResultsAndCertification.Web.ViewModel.PostResultsService;
using Xunit;
using BreadcrumbContent = Sfa.Tl.ResultsAndCertification.Web.Content.ViewComponents.Breadcrumb;

namespace Sfa.Tl.ResultsAndCertification.Web.UnitTests.Controllers.PostResultsServiceControllerTests.StartReviewsAndAppeals
{
    public class When_Action_IsCalled : TestSetup
    {
        public override void Given() { }

        [Fact]
        public void Then_Returns_Expected_Results()
        {
            var model = Result.ShouldBeViewResult<StartReviewsAndAppealsViewModel>();
            model.Should().NotBeNull();

            model.Breadcrumb.Should().NotBeNull();
            model.Breadcrumb.BreadcrumbItems.Should().NotBeNull();
            model.Breadcrumb.BreadcrumbItems.Count.Should().Be(2);

            model.Breadcrumb.BreadcrumbItems[0].DisplayName.Should().Be(BreadcrumbContent.Home);
            model.Breadcrumb.BreadcrumbItems[0].RouteName.Should().Be(RouteConstants.Home);

            model.Breadcrumb.BreadcrumbItems[1].DisplayName.Should().Be(BreadcrumbContent.StartPostResultsService);
            model.Breadcrumb.BreadcrumbItems[1].RouteName.Should().BeNull();
        }
    }
}
