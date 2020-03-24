﻿using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Sfa.Tl.ResultsAndCertification.Web.ViewModel.Provider;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Sfa.Tl.ResultsAndCertification.Web.UnitTests.Controllers.ProviderControllerTests.FindProviderPost
{
    public class Then_On_SelectedProviderId_Zero_More_Providers_Returns_Validation_Error : When_FindProviderAsync_Post_Action_Is_Called
    {
        private IEnumerable<ProviderLookupData> expectedMockProviders;

        public override void Given()
        {
            ViewModel.SelectedProviderId = 0;

            expectedMockProviders = new List<ProviderLookupData>
            {
                new ProviderLookupData(),
                new ProviderLookupData(),
            };
            ProviderLoader.GetProviderLookupDataAsync(ViewModel.Search, true)
                .Returns(expectedMockProviders);
        }

        [Fact]
        public void Then_GetProviderLookupDataAsync_Method_Is_Called()
        {
            ProviderLoader.Received(1).GetProviderLookupDataAsync(ViewModel.Search, true);
        }

        [Fact]
        public void Then_Expected_Validation_Error_Returned()
        {
            Result.Result.Should().BeOfType(typeof(ViewResult));

            var viewResult = Result.Result as ViewResult;
            viewResult.Model.Should().BeOfType(typeof(FindProviderViewModel));

            var model = viewResult.Model as FindProviderViewModel;
            model.Search.Should().Be(ProviderName);
            model.SelectedProviderId.Should().Be(0);

            // Assert Error
            Controller.ModelState.IsValid.Should().Be(false);
            Controller.ModelState.ErrorCount.Should().Be(1);

            var expectedErrorMessage = Controller.ModelState.Values.FirstOrDefault().Errors[0].ErrorMessage;
            expectedErrorMessage.Should().Be(Content.Provider.FindProvider.ProviderName_NotValid_Validation_Message);
        }
    }
}
