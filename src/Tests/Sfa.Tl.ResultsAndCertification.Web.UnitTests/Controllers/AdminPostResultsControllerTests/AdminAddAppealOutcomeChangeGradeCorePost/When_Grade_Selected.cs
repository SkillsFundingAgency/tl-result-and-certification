﻿using NSubstitute;
using Sfa.Tl.ResultsAndCertification.Common.Helpers;
using Sfa.Tl.ResultsAndCertification.Web.UnitTests.Helpers;
using Sfa.Tl.ResultsAndCertification.Web.ViewModel.AdminPostResults;
using System.Threading.Tasks;
using Xunit;

namespace Sfa.Tl.ResultsAndCertification.Web.UnitTests.Controllers.AdminPostResultsControllerTests.AdminAddAppealOutcomeChangeGradeCorePost
{
    public class When_Grade_Selected : TestSetup
    {
        private AdminAddAppealOutcomeChangeGradeCoreViewModel _viewModel;

        public override void Given()
        {
            _viewModel = CreateViewModel(selectedGradeId: 1);
        }

        public async override Task When()
        {
            Result = await Controller.AdminAddAppealOutcomeChangeGradeCoreAsync(_viewModel);
        }

        [Fact]
        public void Then_Expected_Methods_AreCalled()
        {
            AdminPostResultsLoader.Received(1).LoadAdminAddAppealOutcomeChangeGradeCoreGrades(_viewModel);
            CacheService.Received(1).SetAsync(CacheKey, _viewModel);
        }

        [Fact]
        public void Then_Redirected_To_ReviewChangePathwayResult()
        {
            Result.ShouldBeRedirectToRouteResult(RouteConstants.AdminReviewChangesAppealOutcomeCore);
        }
    }
}