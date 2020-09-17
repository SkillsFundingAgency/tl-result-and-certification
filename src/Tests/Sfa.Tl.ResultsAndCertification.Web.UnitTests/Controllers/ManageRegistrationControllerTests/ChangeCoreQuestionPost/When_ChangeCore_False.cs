﻿using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Sfa.Tl.ResultsAndCertification.Common.Helpers;
using Sfa.Tl.ResultsAndCertification.Web.ViewModel.Registration.Manual;
using Xunit;

namespace Sfa.Tl.ResultsAndCertification.Web.UnitTests.Controllers.ManageRegistrationControllerTests.ChangeCoreQuestionPost
{
    public class When_ChangeCore_False : TestSetup
    {
        private ChangeProviderCoreNotSupportedViewModel cacheResult;

        public override void Given()
        {
            ViewModel.CanChangeCore = false;
            cacheResult = new ChangeProviderCoreNotSupportedViewModel
            {
                ProfileId = 1,
                ProviderDisplayName = "Test (12345678)",
                CoreDisplayName = "Test core (987654321)"
            };

            CacheService.GetAsync<ChangeProviderCoreNotSupportedViewModel>(CacheKey).Returns(cacheResult);
        }

        [Fact]
        public void Then_Expecected_Methods_Called()
        {
            CacheService.Received(1).SetAsync(CacheKey, Arg.Any<ChangeProviderCoreNotSupportedViewModel>());
        }

        [Fact]
        public void Then_Redirected_To_ChangeRegistrationProviderNotOfferingSameCore()
        {
            var routeName = (Result as RedirectToRouteResult).RouteName;
            routeName.Should().Be(RouteConstants.ChangeRegistrationProviderNotOfferingSameCore);
        }
    }
}
