﻿using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Sfa.Tl.ResultsAndCertification.Common.Helpers;
using Sfa.Tl.ResultsAndCertification.Web.ViewModel.ProviderAddress;
using Xunit;

namespace Sfa.Tl.ResultsAndCertification.Web.UnitTests.Controllers.ProviderAddressTests.AddPostalAddressManualGet
{
    public class When_Cache_NotFound : TestSetup
    {
        private AddAddressViewModel _cacheResult;

        public override void Given()
        {
            IsFromSelectAddress = true;
            IsFromAddressMissing = true;

            _cacheResult = null;
            CacheService.GetAsync<AddAddressViewModel>(CacheKey).Returns(_cacheResult);
        }


        [Fact]
        public void Then_Returns_Expected_Results()
        {
            Result.Should().NotBeNull();
            Result.Should().BeOfType(typeof(ViewResult));

            var viewResult = Result as ViewResult;
            viewResult.Model.Should().BeOfType(typeof(AddAddressManualViewModel));

            var model = viewResult.Model as AddAddressManualViewModel;
            model.Should().NotBeNull();

            model.IsFromSelectAddress.Should().Be(IsFromSelectAddress);
            model.IsFromAddressMissing.Should().Be(IsFromAddressMissing);

            model.BackLink.Should().NotBeNull();
            model.BackLink.RouteAttributes.Count.Should().Be(1);
            model.BackLink.RouteAttributes[Constants.IsAddressMissing].Should().Be("true");
        }
    }
}
