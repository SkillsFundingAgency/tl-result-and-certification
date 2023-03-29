﻿using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Sfa.Tl.ResultsAndCertification.Common.Enum;
using Sfa.Tl.ResultsAndCertification.Common.Helpers;
using Sfa.Tl.ResultsAndCertification.Models.Contracts.StatementOfAchievement;
using Sfa.Tl.ResultsAndCertification.Web.ViewModel.ProviderAddress;
using Sfa.Tl.ResultsAndCertification.Web.ViewModel.StatementOfAchievement;
using System;
using Xunit;

namespace Sfa.Tl.ResultsAndCertification.Web.UnitTests.Controllers.StatementOfAchievementControllerTests.RequestSoaCheckAndSubmitPost
{
    public class When_Success : TestSetup
    {
        private SoaLearnerRecordDetailsViewModel _mockLearnerDetails = null;
        private AddressViewModel _address;

        public override void Given()
        {
            ProfileId = 11;
            _address = new AddressViewModel { AddressId = 1, DepartmentName = "Operations", OrganisationName = "College Ltd", AddressLine1 = "10, House", AddressLine2 = "Street", Town = "Birmingham", Postcode = "B1 1AA" };
            _mockLearnerDetails = new SoaLearnerRecordDetailsViewModel
            {
                ProfileId = ProfileId,
                Uln = 1234567890,
                LearnerName = "John Smith",
                DateofBirth = DateTime.Now.AddYears(-20),
                ProviderDisplayName = "Barsley College (569874567)",
                ProviderName = "Barsley College",
                ProviderUkprn = 569874567,

                TlevelTitle = "Design, Surveying and Planning for Construction",
                RegistrationPathwayId = 1,
                PathwayDisplayName = "Design, Surveying and Planning for Construction (60358300)",
                PathwayName = "Design, Surveying and Planning for Construction",
                PathwayCode = "60358300",
                PathwayGrade = "A*",
                SpecialismDisplayName = "Building Services Design (ZTLOS003)",
                SpecialismName = "Building Services Design",
                SpecialismCode = "ZTLOS003",
                SpecialismGrade = "None",

                EnglishStatus = SubjectStatus.Achieved,
                MathsStatus = SubjectStatus.Achieved,
                IndustryPlacementStatus = IndustryPlacementStatus.Completed,

                HasPathwayResult = true,
                IsNotWithdrawn = false,
                IsLearnerRegistered = true,
                IsIndustryPlacementAdded = true,
                IsIndustryPlacementCompleted = true,

                ProviderAddress = _address,
            };

            SoaLearnerRecordDetailsViewModel = new SoaLearnerRecordDetailsViewModel { ProfileId = ProfileId };
            SoaPrintingResponse = new SoaPrintingResponse { Uln = _mockLearnerDetails.Uln, LearnerName = _mockLearnerDetails.LearnerName, IsSuccess = true };

            StatementOfAchievementLoader.GetSoaLearnerRecordDetailsAsync(ProviderUkprn, ProfileId).Returns(_mockLearnerDetails);
            StatementOfAchievementLoader.CreateSoaPrintingRequestAsync(ProviderUkprn, _mockLearnerDetails).Returns(SoaPrintingResponse);
        }

        [Fact]
        public void Then_Expected_Methods_Called()
        {
            StatementOfAchievementLoader.Received(1).CreateSoaPrintingRequestAsync(ProviderUkprn, _mockLearnerDetails);            
            CacheService.Received(1).SetAsync(string.Concat(CacheKey, Constants.RequestSoaConfirmation),
                Arg.Is<SoaConfirmationViewModel>
                (x => x.Name == _mockLearnerDetails.LearnerName &&
                      x.Uln == _mockLearnerDetails.Uln),
                 CacheExpiryTime.XSmall);
        }

        [Fact]
        public void Then_Redirected_To_RequestSoaConfirmation()
        {
            var routeName = (Result as RedirectToRouteResult).RouteName;
            routeName.Should().Be(RouteConstants.RequestSoaConfirmation);
        }
    }
}
