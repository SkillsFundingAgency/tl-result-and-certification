﻿using FluentAssertions;
using NSubstitute;
using Sfa.Tl.ResultsAndCertification.Common.Enum;
using Sfa.Tl.ResultsAndCertification.Common.Extensions;
using Sfa.Tl.ResultsAndCertification.Models.Contracts;
using Sfa.Tl.ResultsAndCertification.Web.Loader;
using Sfa.Tl.ResultsAndCertification.Web.ViewModel.Registration.Manual;
using Xunit;

namespace Sfa.Tl.ResultsAndCertification.Web.UnitTests.Loader.RegistrationLoaderTests.Reregistration
{
    public class When_Success : TestSetup
    {
        private RegistrationDetails mockApiClientResponse;
        private long _providerUkprn;
        private string _coreCode;

        public override void Given()
        {
            _coreCode = "10000111";
            _providerUkprn = 12345678;
            ApiClientResponse = true;

            ViewModel = new ReregisterViewModel
            {
                ReregisterProvider = new ReregisterProviderViewModel { ProfileId = ProfileId, SelectedProviderUkprn = _providerUkprn.ToString() },
                ReregisterCore = new ReregisterCoreViewModel { ProfileId = ProfileId, SelectedCoreCode = _coreCode },
                SpecialismQuestion = new ReregisterSpecialismQuestionViewModel { HasLearnerDecidedSpecialism = false },
                ReregisterAcademicYear = new ReregisterAcademicYearViewModel { SelectedAcademicYear = "2020" }
            };

            mockApiClientResponse = new RegistrationDetails
            {
                ProfileId = 1,
                Uln = Uln,
                ProviderUkprn = _providerUkprn,
                PathwayLarId = "34567111",
            };

            Loader = new RegistrationLoader(Mapper, Logger, InternalApiClient, BlobStorageService);

            InternalApiClient.GetRegistrationDetailsAsync(AoUkprn, ProfileId, RegistrationPathwayStatus.Withdrawn).Returns(mockApiClientResponse);
            InternalApiClient.ReregistrationAsync(Arg.Any<ReregistrationRequest>()).Returns(ApiClientResponse);
        }

        [Fact]
        public void Then_Recieved_Call_To_GetRegistrations()
        {
            InternalApiClient.Received(1).GetRegistrationDetailsAsync(AoUkprn, ProfileId, RegistrationPathwayStatus.Withdrawn);
            InternalApiClient.Received(1).ReregistrationAsync(Arg.Any<ReregistrationRequest>());
        }

        [Fact]
        public void Then_Mapper_Has_Expected_Results()
        {
            var result = Mapper.Map<ReregistrationRequest>(ViewModel, opt => opt.Items["aoUkprn"] = AoUkprn);

            result.Should().NotBeNull();
            result.AoUkprn.Should().Be(AoUkprn);
            result.ProfileId.Should().Be(ProfileId);
            result.ProviderUkprn.Should().Be(ViewModel.ReregisterProvider.SelectedProviderUkprn.ToLong());
            result.CoreCode.Should().Be(ViewModel.ReregisterCore.SelectedCoreCode);
            result.SpecialismCodes.Should().BeNullOrEmpty();
            result.AcademicYear.Should().Be(ViewModel.ReregisterAcademicYear.SelectedAcademicYear.ToInt());
            result.PerformedBy.Should().Be($"{Givenname} {Surname}");
        }

        [Fact]
        public void Then_Expected_Result_Is_Returned()
        {
            ActualResult.Should().NotBeNull();

            ActualResult.IsSelectedCoreSameAsWithdrawn.Should().BeFalse();
            ActualResult.IsSuccess.Should().BeTrue();
            ActualResult.ProfileId.Should().Be(ProfileId);
            ActualResult.Uln.Should().Be(Uln);
        }
    }
}
