﻿using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Sfa.Tl.ResultsAndCertification.Web.ViewModel.IndustryPlacement;
using Xunit;
using UploadContent = Sfa.Tl.ResultsAndCertification.Web.Content.IndustryPlacement.Upload;

namespace Sfa.Tl.ResultsAndCertification.Web.UnitTests.Controllers.IndustryPlacementImportControllerTests.UploadIndustryPlacementsFilePost
{
    public class When_ModelState_MaxRecord_Invalid : TestSetup
    {
        public override void Given()
        {
            FormFile = Substitute.For<IFormFile>();
            Controller.ModelState.AddModelError("File", string.Format(UploadContent.File_Max_Record_Count_Validation_Message, 10000));
        }

        [Fact]
        public void Then_Returns_Expected_Results()
        {
            Result.Should().BeOfType(typeof(ViewResult));

            var viewResult = Result as ViewResult;
            viewResult.Model.Should().BeOfType(typeof(UploadIndustryPlacementsRequestViewModel));

            Controller.ViewData.ModelState.Should().ContainSingle();
            Controller.ViewData.ModelState.ContainsKey(nameof(UploadIndustryPlacementsRequestViewModel.File)).Should().BeTrue();

            var modelState = Controller.ViewData.ModelState[nameof(UploadIndustryPlacementsRequestViewModel.File)];
            modelState.Errors[0].ErrorMessage.Should().Be(string.Format(UploadContent.File_Max_Record_Count_Validation_Message, 10000));
        }
    }
}
