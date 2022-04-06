﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sfa.Tl.ResultsAndCertification.Common.Constants;
using Sfa.Tl.ResultsAndCertification.Common.Enum;
using Sfa.Tl.ResultsAndCertification.Common.Extensions;
using Sfa.Tl.ResultsAndCertification.Common.Helpers;
using Sfa.Tl.ResultsAndCertification.Common.Services.Cache;
using Sfa.Tl.ResultsAndCertification.Web.Loader.Interfaces;
using Sfa.Tl.ResultsAndCertification.Web.ViewComponents.NotificationBanner;
using Sfa.Tl.ResultsAndCertification.Web.ViewModel.PostResultsService;
using System.Linq;
using System.Threading.Tasks;

namespace Sfa.Tl.ResultsAndCertification.Web.Controllers
{
    [Authorize(Policy = RolesExtensions.RequireReviewsAndAppealsEditorAccess)]
    public class PostResultsServiceController : Controller
    {
        private readonly IPostResultsServiceLoader _postResultsServiceLoader;
        private readonly ICacheService _cacheService;
        private readonly ILogger _logger;

        private string CacheKey { get { return CacheKeyHelper.GetCacheKey(User.GetUserId(), CacheConstants.PrsCacheKey); } }

        public PostResultsServiceController(IPostResultsServiceLoader postResultsServiceLoader, ICacheService cacheService, ILogger<PostResultsServiceController> logger)
        {
            _postResultsServiceLoader = postResultsServiceLoader;
            _cacheService = cacheService;
            _logger = logger;
        }

        [HttpGet]
        [Route("post-results-reviews-appeals-and-grade-changes", Name = RouteConstants.StartReviewsAndAppeals)]
        public async Task<IActionResult> StartReviewsAndAppealsAsync()
        {
            await _cacheService.RemoveAsync<PrsSearchLearnerViewModel>(CacheKey);
            return View(new StartReviewsAndAppealsViewModel());
        }

        [HttpGet]
        [Route("post-results-search-uln/{populateUln:bool?}", Name = RouteConstants.PrsSearchLearner)]
        public async Task<IActionResult> PrsSearchLearnerAsync(bool populateUln)
        {
            var cacheModel = await _cacheService.GetAsync<PrsSearchLearnerViewModel>(CacheKey);
            var viewModel = cacheModel != null && populateUln ? cacheModel : new PrsSearchLearnerViewModel();

            return View(viewModel);
        }

        [HttpPost]
        [Route("post-results-search-uln/{populateUln:bool?}", Name = RouteConstants.SubmitPrsSearchLearner)]
        public async Task<IActionResult> PrsSearchLearnerAsync(PrsSearchLearnerViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var prsLearnerRecord = await _postResultsServiceLoader.FindPrsLearnerRecordAsync(User.GetUkPrn(), model.SearchUln.ToLong());
            await _cacheService.SetAsync(CacheKey, model);

            if (prsLearnerRecord == null)
            {
                await _cacheService.SetAsync(CacheKey, new PrsUlnNotFoundViewModel { Uln = model.SearchUln }, CacheExpiryTime.XSmall);
                return RedirectToRoute(RouteConstants.PrsUlnNotFound);
            }
            else if (prsLearnerRecord.IsWithdrawn)
            {
                var prsUlnWithdrawnViewModel = _postResultsServiceLoader.TransformLearnerDetailsTo<PrsUlnWithdrawnViewModel>(prsLearnerRecord);
                await _cacheService.SetAsync(CacheKey, prsUlnWithdrawnViewModel, CacheExpiryTime.XSmall);
                return RedirectToRoute(RouteConstants.PrsUlnWithdrawn);
            }
            else if (!prsLearnerRecord.HasResults)
            {
                var prsNoResultsViewModel = _postResultsServiceLoader.TransformLearnerDetailsTo<PrsNoResultsViewModel>(prsLearnerRecord);
                await _cacheService.SetAsync(CacheKey, prsNoResultsViewModel, CacheExpiryTime.XSmall);
                return RedirectToRoute(RouteConstants.PrsNoResults);
            }
            else
            {
                return RedirectToRoute(RouteConstants.PrsLearnerDetails, new { profileId = prsLearnerRecord.ProfileId });
            }
        }

        [HttpGet]
        [Route("post-results-uln-not-found", Name = RouteConstants.PrsUlnNotFound)]
        public async Task<IActionResult> PrsUlnNotFoundAsync()
        {
            var cacheModel = await _cacheService.GetAndRemoveAsync<PrsUlnNotFoundViewModel>(CacheKey);
            if (cacheModel == null)
            {
                _logger.LogWarning(LogEvent.NoDataFound, $"Unable to read PrsUlnNotFoundViewModel from redis cache in request Prs Uln not found page. Ukprn: {User.GetUkPrn()}, User: {User.GetUserEmail()}");
                return RedirectToRoute(RouteConstants.PageNotFound);
            }

            return View(cacheModel);
        }

        [HttpGet]
        [Route("post-results-learner-withdrawn", Name = RouteConstants.PrsUlnWithdrawn)]
        public async Task<IActionResult> PrsUlnWithdrawnAsync()
        {
            var cacheModel = await _cacheService.GetAndRemoveAsync<PrsUlnWithdrawnViewModel>(CacheKey);
            if (cacheModel == null)
            {
                _logger.LogWarning(LogEvent.NoDataFound, $"Unable to read PrsUlnWithdrawnViewModel from redis cache in post results service uln withdrawn page. Ukprn: {User.GetUkPrn()}, User: {User.GetUserEmail()}");
                return RedirectToRoute(RouteConstants.PageNotFound);
            }

            return View(cacheModel);
        }        

        [HttpGet]
        [Route("post-results-no-results", Name = RouteConstants.PrsNoResults)]
        public async Task<IActionResult> PrsNoResultsAsync()
        {
            var cacheModel = await _cacheService.GetAndRemoveAsync<PrsNoResultsViewModel>(CacheKey);
            if (cacheModel == null)
            {
                _logger.LogWarning(LogEvent.NoDataFound, $"Unable to read PrsNoResultsViewModel from redis cache in post results service no results page. Ukprn: {User.GetUkPrn()}, User: {User.GetUserEmail()}");
                return RedirectToRoute(RouteConstants.PageNotFound);
            }

            return View(cacheModel);
        }

        [HttpGet]
        [Route("post-results-learners-grades/{profileId}", Name = RouteConstants.PrsLearnerDetails)]
        public async Task<IActionResult> PrsLearnerDetailsAsync(int profileId)
        {
            var viewModel = await _postResultsServiceLoader.GetPrsLearnerDetailsAsync<PrsLearnerDetailsViewModel1>(User.GetUkPrn(), profileId);
            if (viewModel == null)
                return RedirectToRoute(RouteConstants.PageNotFound);

            viewModel.SuccessBanner = await _cacheService.GetAndRemoveAsync<NotificationBannerModel>(CacheKey);
            return View(viewModel);
        }

        [HttpGet]
        [Route("post-results-add-romm/{profileId}/{assessmentId}/{componentType}/{isBack:bool?}", Name = RouteConstants.PrsAddRomm)]
        public async Task<IActionResult> PrsAddRommAsync(int profileId, int assessmentId, ComponentType componentType, bool? isBack)
        {
            var viewModel = await _postResultsServiceLoader.GetPrsLearnerDetailsAsync<PrsAddRommViewModel>(User.GetUkPrn(), profileId, assessmentId, componentType);

            if (viewModel == null || !viewModel.IsValid)
                return RedirectToRoute(RouteConstants.PageNotFound);

            viewModel.IsRommRequested = isBack;

            return View(viewModel);
        }

        [HttpPost]
        [Route("post-results-add-romm/{profileId}/{assessmentId}/{componentType}/{isBack:bool?}", Name = RouteConstants.SubmitPrsAddRomm)]
        public async Task<IActionResult> PrsAddRommAsync(PrsAddRommViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var prsDetails = await _postResultsServiceLoader.GetPrsLearnerDetailsAsync<PrsAddRommViewModel>(User.GetUkPrn(), model.ProfileId, model.AssessmentId, model.ComponentType);
                return View(prsDetails);
            }

            if (model.IsRommRequested == true)
                return RedirectToRoute(RouteConstants.PrsAddRommOutcomeKnown, new { profileId = model.ProfileId, assessmentId = model.AssessmentId, componentType = (int)model.ComponentType });
            else
                return RedirectToRoute(RouteConstants.PrsLearnerDetails, new { profileId = model.ProfileId });
        }

        [HttpGet]
        [Route("post-results-add-romm-outcome/{profileId}/{assessmentId}/{componentType}/{outcomeTypeId:int?}", Name = RouteConstants.PrsAddRommOutcome)]
        public async Task<IActionResult> PrsAddRommOutcomeAsync(int profileId, int assessmentId, ComponentType componentType, int? outcomeTypeId)
        {
            var viewModel = await _postResultsServiceLoader.GetPrsLearnerDetailsAsync<PrsAddRommOutcomeViewModel>(User.GetUkPrn(), profileId, assessmentId, componentType);

            if (viewModel == null || !viewModel.IsValid)
                return RedirectToRoute(RouteConstants.PageNotFound);

            viewModel.SetOutcomeType(outcomeTypeId);
            return View(viewModel);
        }

        [HttpPost]
        [Route("post-results-add-romm-outcome/{profileId}/{assessmentId}/{componentType}/{outcomeTypeId:int?}", Name = RouteConstants.SubmitPrsAddRommOutcome)]
        public async Task<IActionResult> PrsAddRommOutcomeAsync(PrsAddRommOutcomeViewModel model)
        {
            var prsDetails = await _postResultsServiceLoader.GetPrsLearnerDetailsAsync<PrsAddRommOutcomeViewModel>(User.GetUkPrn(), model.ProfileId, model.AssessmentId, model.ComponentType);

            if (!ModelState.IsValid)
                return View(prsDetails);

            if (prsDetails == null || !prsDetails.IsValid)
                return RedirectToRoute(RouteConstants.PageNotFound);

            if (model.RommOutcome == RommOutcomeType.GradeChanged)
            {
                await _cacheService.RemoveAsync<PrsRommCheckAndSubmitViewModel>(CacheKey);
                return RedirectToRoute(RouteConstants.PrsRommGradeChange, new { profileId = model.ProfileId, assessmentId = model.AssessmentId, componentType = (int)model.ComponentType, isRommOutcomeJourney = "true" });
            }
            else if (model.RommOutcome == RommOutcomeType.GradeNotChanged)
            {
                var checkAndSubmitViewModel = await _postResultsServiceLoader.GetPrsLearnerDetailsAsync<PrsRommCheckAndSubmitViewModel>(User.GetUkPrn(), model.ProfileId, model.AssessmentId, model.ComponentType);
                checkAndSubmitViewModel.NewGrade = checkAndSubmitViewModel.OldGrade;
                checkAndSubmitViewModel.IsGradeChanged = false;
                await _cacheService.SetAsync(CacheKey, checkAndSubmitViewModel);

                return RedirectToRoute(RouteConstants.PrsRommCheckAndSubmit);
            }
            else if (model.RommOutcome == RommOutcomeType.Withdraw)
            {
                bool isSuccess = await _postResultsServiceLoader.PrsRommActivityAsync(User.GetUkPrn(), model);
                if (!isSuccess)
                    return RedirectToRoute(RouteConstants.ProblemWithService);

                var notificationBanner = new NotificationBannerModel { IsPrsJourney = true, HeaderMessage = prsDetails.Banner_HeaderMesage, Message = prsDetails.SuccessBannerMessage };
                await _cacheService.SetAsync(CacheKey, notificationBanner, CacheExpiryTime.XSmall);

                return RedirectToRoute(RouteConstants.PrsLearnerDetails, new { profileId = model.ProfileId });
            }
            else
            {
                return RedirectToRoute(RouteConstants.PrsLearnerDetails, new { profileId = model.ProfileId });
            }
        }

        [HttpGet]
        [Route("post-results-romm-outcome-known/{profileId}/{assessmentId}/{componentType}/{outcomeKnownTypeId:int?}", Name = RouteConstants.PrsAddRommOutcomeKnown)]
        public async Task<IActionResult> PrsAddRommOutcomeKnownAsync(int profileId, int assessmentId, ComponentType componentType, int? outcomeKnownTypeId)
        {
            var viewModel = await _postResultsServiceLoader.GetPrsLearnerDetailsAsync<PrsAddRommOutcomeKnownViewModel>(User.GetUkPrn(), profileId, assessmentId, componentType);

            if (viewModel == null || !viewModel.IsValid)
                return RedirectToRoute(RouteConstants.PageNotFound);

            viewModel.SetOutcomeType(outcomeKnownTypeId);
            return View(viewModel);
        }

        [HttpPost]
        [Route("post-results-romm-outcome-known/{profileId}/{assessmentId}/{componentType}/{outcomeKnownTypeId:int?}", Name = RouteConstants.SubmitPrsAddRommOutcomeKnown)]
        public async Task<IActionResult> PrsAddRommOutcomeKnownAsync(PrsAddRommOutcomeKnownViewModel model)
        {
            var prsDetails = await _postResultsServiceLoader.GetPrsLearnerDetailsAsync<PrsAddRommOutcomeKnownViewModel>(User.GetUkPrn(), model.ProfileId, model.AssessmentId, model.ComponentType);

            if (!ModelState.IsValid)
                return View(prsDetails);

            if (prsDetails == null || !prsDetails.IsValid)
                return RedirectToRoute(RouteConstants.PageNotFound);

            if (model.RommOutcome == RommOutcomeKnownType.GradeChanged)
            {
                await _cacheService.RemoveAsync<PrsRommCheckAndSubmitViewModel>(CacheKey);
                return RedirectToRoute(RouteConstants.PrsRommGradeChange, new { profileId = model.ProfileId, assessmentId = model.AssessmentId, componentType = (int)model.ComponentType });
            }
            else if (model.RommOutcome == RommOutcomeKnownType.GradeNotChanged)
            {
                var checkAndSubmitViewModel = await _postResultsServiceLoader.GetPrsLearnerDetailsAsync<PrsRommCheckAndSubmitViewModel>(User.GetUkPrn(), model.ProfileId, model.AssessmentId, model.ComponentType);
                checkAndSubmitViewModel.NewGrade = checkAndSubmitViewModel.OldGrade;
                checkAndSubmitViewModel.IsGradeChanged = false;
                await _cacheService.SetAsync(CacheKey, checkAndSubmitViewModel);

                return RedirectToRoute(RouteConstants.PrsRommCheckAndSubmit);
            }
            else if (model.RommOutcome == RommOutcomeKnownType.No)
            {
                bool isSuccess = await _postResultsServiceLoader.PrsRommActivityAsync(User.GetUkPrn(), model);
                if (!isSuccess)
                    return RedirectToRoute(RouteConstants.ProblemWithService);

                var notificationBanner = new NotificationBannerModel { IsPrsJourney = true, HeaderMessage = prsDetails.Banner_HeaderMesage, Message = prsDetails.SuccessBannerMessage };
                await _cacheService.SetAsync(CacheKey, notificationBanner, CacheExpiryTime.XSmall);

                return RedirectToRoute(RouteConstants.PrsLearnerDetails, new { profileId = model.ProfileId });
            }
            else
            {
                return RedirectToRoute(RouteConstants.PrsLearnerDetails, new { profileId = model.ProfileId });
            }
        }

        [HttpGet]
        [Route("post-results-romm-change-grade/{profileId}/{assessmentId}/{componentType}/{isRommOutcomeJourney:bool?}/{isChangeMode:bool?}", Name = RouteConstants.PrsRommGradeChange)]
        public async Task<IActionResult> PrsRommGradeChangeAsync(int profileId, int assessmentId, ComponentType componentType, bool? isRommOutcomeJourney, bool? isChangeMode)
        {
            var viewModel = await _postResultsServiceLoader.GetPrsLearnerDetailsAsync<PrsRommGradeChangeViewModel>(User.GetUkPrn(), profileId, assessmentId, componentType);

            if (viewModel == null || !viewModel.IsValid)
                return RedirectToRoute(RouteConstants.PageNotFound);

            var checkAndSubmitDetails = await _cacheService.GetAsync<PrsRommCheckAndSubmitViewModel>(CacheKey);
            if (checkAndSubmitDetails != null && (isChangeMode == null || isChangeMode.Value == false))
                viewModel.SelectedGradeCode = viewModel.Grades?.FirstOrDefault(g => g.Value == checkAndSubmitDetails?.NewGrade)?.Code;

            viewModel.IsRommOutcomeJourney = isRommOutcomeJourney ?? false;
            viewModel.IsChangeMode = isChangeMode ?? false;
            return View(viewModel);
        }

        [HttpPost]
        [Route("post-results-romm-change-grade/{profileId}/{assessmentId}/{componentType}/{isRommOutcomeJourney:bool?}/{isChangeMode:bool?}", Name = RouteConstants.SubmitPrsRommGradeChange)]
        public async Task<IActionResult> PrsRommGradeChangeAsync(PrsRommGradeChangeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var prsDetails = await _postResultsServiceLoader.GetPrsLearnerDetailsAsync<PrsRommGradeChangeViewModel>(User.GetUkPrn(), model.ProfileId, model.AssessmentId, model.ComponentType);
                prsDetails.IsRommOutcomeJourney = model.IsRommOutcomeJourney;
                prsDetails.IsChangeMode = model.IsChangeMode;
                return View(prsDetails);
            }

            var checkAndSubmitViewModel = await _postResultsServiceLoader.GetPrsLearnerDetailsAsync<PrsRommCheckAndSubmitViewModel>(User.GetUkPrn(), model.ProfileId, model.AssessmentId, model.ComponentType);
            checkAndSubmitViewModel.NewGrade = model.Grades?.FirstOrDefault(x => x.Code == model.SelectedGradeCode)?.Value;

            if (string.IsNullOrWhiteSpace(checkAndSubmitViewModel.NewGrade))
                return RedirectToRoute(RouteConstants.PageNotFound);

            checkAndSubmitViewModel.IsGradeChanged = true;
            await _cacheService.SetAsync(CacheKey, checkAndSubmitViewModel);

            return RedirectToRoute(RouteConstants.PrsRommCheckAndSubmit);
        }

        [HttpGet]
        [Route("post-results-romm-check", Name = RouteConstants.PrsRommCheckAndSubmit)]
        public async Task<IActionResult> PrsRommCheckAndSubmitAsync()
        {
            var viewModel = await _cacheService.GetAsync<PrsRommCheckAndSubmitViewModel>(CacheKey);
            if (viewModel == null)
            {
                _logger.LogWarning(LogEvent.NoDataFound, $"Unable to read PrsRommCheckAndSubmitViewModel from redis cache in Prs romm outcome check and submit page. Ukprn: {User.GetUkPrn()}, User: {User.GetUserEmail()}");
                return RedirectToRoute(RouteConstants.PageNotFound);
            }

            return View(viewModel);
        }

        [HttpPost]
        [Route("post-results-romm-check", Name = RouteConstants.SubmitPrsRommCheckAndSubmit)]
        public async Task<IActionResult> PrsRommCheckAndSubmitAsync(PrsRommCheckAndSubmitViewModel model)
        {
            bool isSuccess = await _postResultsServiceLoader.PrsRommActivityAsync(User.GetUkPrn(), model);
            if (!isSuccess)
                return RedirectToRoute(RouteConstants.ProblemWithService);

            await _cacheService.RemoveAsync<PrsRommCheckAndSubmitViewModel>(CacheKey);
            var notificationBanner = new NotificationBannerModel { IsPrsJourney = true, HeaderMessage = model.Banner_HeaderMesage, Message = model.SuccessBannerMessage };
            await _cacheService.SetAsync(CacheKey, notificationBanner, CacheExpiryTime.XSmall);

            return RedirectToRoute(RouteConstants.PrsLearnerDetails, new { profileId = model.ProfileId });
        }

        [HttpGet]
        [Route("post-results-cancel-ROMM-update", Name = RouteConstants.PrsCancelRommUpdate)]
        public async Task<IActionResult> PrsCancelRommUpdateAsync()
        {
            var cacheModel = await _cacheService.GetAsync<PrsRommCheckAndSubmitViewModel>(CacheKey);

            if (cacheModel == null)
                return RedirectToRoute(RouteConstants.PageNotFound);

            var viewModel = new PrsCancelRommUpdateViewModel { ProfileId = cacheModel.ProfileId };
            return View(viewModel);
        }

        [HttpPost]
        [Route("post-results-cancel-ROMM-update", Name = RouteConstants.SubmitPrsCancelRommUpdate)]
        public async Task<IActionResult> PrsCancelRommUpdateAsync(PrsCancelRommUpdateViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            
            if (model.AreYouSureToCancel.Value)
            {
                await _cacheService.RemoveAsync<PrsRommCheckAndSubmitViewModel>(CacheKey);
                return RedirectToRoute(RouteConstants.PrsLearnerDetails, new { profileId = model.ProfileId });
            }
            else
            {
                return RedirectToRoute(RouteConstants.PrsRommCheckAndSubmit);
            }
        }

        // Appeals Process

        [HttpGet]
        [Route("post-results-add-appeal/{profileId}/{assessmentId}/{componentType}/{isBack:bool?}", Name = RouteConstants.PrsAddAppeal)]
        public async Task<IActionResult> PrsAddAppealAsync(int profileId, int assessmentId, ComponentType componentType, bool? isBack)
        {
            var viewModel = await _postResultsServiceLoader.GetPrsLearnerDetailsAsync<PrsAddAppealViewModel>(User.GetUkPrn(), profileId, assessmentId, componentType);

            if (viewModel == null || !viewModel.IsValid)
                return RedirectToRoute(RouteConstants.PageNotFound);

            viewModel.IsAppealRequested = isBack;
            return View(viewModel);
        }

        [HttpPost]
        [Route("post-results-add-appeal/{profileId}/{assessmentId}/{componentType}/{isBack:bool?}", Name = RouteConstants.SubmitPrsAddAppeal)]
        public async Task<IActionResult> PrsAddAppealAsync(PrsAddAppealViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var prsDetails = await _postResultsServiceLoader.GetPrsLearnerDetailsAsync<PrsAddAppealViewModel>(User.GetUkPrn(), model.ProfileId, model.AssessmentId, model.ComponentType);
                return View(prsDetails);
            }

            if (model.IsAppealRequested == true)
                return RedirectToRoute(RouteConstants.PrsAddAppealOutcomeKnown, new { profileId = model.ProfileId, assessmentId = model.AssessmentId, componentType = (int)model.ComponentType });
            else
                return RedirectToRoute(RouteConstants.PrsLearnerDetails, new { profileId = model.ProfileId });
        }

        [HttpGet]
        [Route("post-results-add-appeal-outcome/{profileId}/{assessmentId}/{componentType}/{outcomeTypeId:int?}", Name = RouteConstants.PrsAddAppealOutcome)]
        public async Task<IActionResult> PrsAddAppealOutcomeAsync(int profileId, int assessmentId, ComponentType componentType, int? outcomeTypeId)
        {
            var viewModel = await _postResultsServiceLoader.GetPrsLearnerDetailsAsync<PrsAddAppealOutcomeViewModel>(User.GetUkPrn(), profileId, assessmentId, componentType);

            if (viewModel == null || !viewModel.IsValid)
                return RedirectToRoute(RouteConstants.PageNotFound);

            viewModel.SetOutcomeType(outcomeTypeId);
            return View(viewModel);
        }

        [HttpPost]
        [Route("post-results-add-appeal-outcome/{profileId}/{assessmentId}/{componentType}/{outcomeTypeId:int?}", Name = RouteConstants.SubmitPrsAddAppealOutcome)]
        public async Task<IActionResult> PrsAddAppealOutcomeAsync(PrsAddAppealOutcomeViewModel model)
        {
            var prsDetails = await _postResultsServiceLoader.GetPrsLearnerDetailsAsync<PrsAddAppealOutcomeViewModel>(User.GetUkPrn(), model.ProfileId, model.AssessmentId, model.ComponentType);

            if (!ModelState.IsValid)
                return View(prsDetails);

            if (prsDetails == null || !prsDetails.IsValid)
                return RedirectToRoute(RouteConstants.PageNotFound);

            if (model.AppealOutcome == AppealOutcomeType.GradeChanged)
            {
                await _cacheService.RemoveAsync<PrsAppealCheckAndSubmitViewModel>(CacheKey);
                return RedirectToRoute(RouteConstants.PrsAppealGradeChange, new { profileId = model.ProfileId, assessmentId = model.AssessmentId, componentType = (int)model.ComponentType, isAppealOutcomeJourney = "true" });
            }
            else if (model.AppealOutcome == AppealOutcomeType.GradeNotChanged)
            {
                var checkAndSubmitViewModel = await _postResultsServiceLoader.GetPrsLearnerDetailsAsync<PrsAppealCheckAndSubmitViewModel>(User.GetUkPrn(), model.ProfileId, model.AssessmentId, model.ComponentType);
                checkAndSubmitViewModel.NewGrade = checkAndSubmitViewModel.OldGrade;
                checkAndSubmitViewModel.IsGradeChanged = false;
                await _cacheService.SetAsync(CacheKey, checkAndSubmitViewModel);

                return RedirectToRoute(RouteConstants.PrsAppealCheckAndSubmit);
            }
            else if (model.AppealOutcome == AppealOutcomeType.Withdraw)
            {
                bool isSuccess = await _postResultsServiceLoader.PrsAppealActivityAsync(User.GetUkPrn(), model);
                if (!isSuccess)
                    return RedirectToRoute(RouteConstants.ProblemWithService);

                var notificationBanner = new NotificationBannerModel { IsPrsJourney = true, HeaderMessage = prsDetails.Banner_HeaderMesage, Message = prsDetails.SuccessBannerMessage };
                await _cacheService.SetAsync(CacheKey, notificationBanner, CacheExpiryTime.XSmall);

                return RedirectToRoute(RouteConstants.PrsLearnerDetails, new { profileId = model.ProfileId });
            }
            else
            {
                return RedirectToRoute(RouteConstants.PrsLearnerDetails, new { profileId = model.ProfileId });
            }
        }

        [HttpGet]
        [Route("post-results-appeal-outcome-known/{profileId}/{assessmentId}/{componentType}/{outcomeKnownTypeId:int?}", Name = RouteConstants.PrsAddAppealOutcomeKnown)]
        public async Task<IActionResult> PrsAddAppealOutcomeKnownAsync(int profileId, int assessmentId, ComponentType componentType, int? outcomeKnownTypeId)
        {
            var viewModel = await _postResultsServiceLoader.GetPrsLearnerDetailsAsync<PrsAddAppealOutcomeKnownViewModel>(User.GetUkPrn(), profileId, assessmentId, componentType);

            if (viewModel == null || !viewModel.IsValid)
                return RedirectToRoute(RouteConstants.PageNotFound);

            viewModel.SetOutcomeType(outcomeKnownTypeId);
            return View(viewModel);
        }

        [HttpPost]
        [Route("post-results-appeal-outcome-known/{profileId}/{assessmentId}/{componentType}/{outcomeKnownTypeId:int?}", Name = RouteConstants.SubmitPrsAddAppealOutcomeKnown)]
        public async Task<IActionResult> PrsAddAppealOutcomeKnownAsync(PrsAddAppealOutcomeKnownViewModel model)
        {
            var prsDetails = await _postResultsServiceLoader.GetPrsLearnerDetailsAsync<PrsAddAppealOutcomeKnownViewModel>(User.GetUkPrn(), model.ProfileId, model.AssessmentId, model.ComponentType);

            if (!ModelState.IsValid)
                return View(prsDetails);

            if (prsDetails == null || !prsDetails.IsValid)
                return RedirectToRoute(RouteConstants.PageNotFound);

            if (model.AppealOutcome == AppealOutcomeKnownType.GradeChanged)
            {
                await _cacheService.RemoveAsync<PrsAppealCheckAndSubmitViewModel>(CacheKey);
                return RedirectToRoute(RouteConstants.PrsAppealGradeChange, new { profileId = model.ProfileId, assessmentId = model.AssessmentId, componentType = (int)model.ComponentType });
            }
            else if (model.AppealOutcome == AppealOutcomeKnownType.GradeNotChanged)
            {
                var checkAndSubmitViewModel = await _postResultsServiceLoader.GetPrsLearnerDetailsAsync<PrsAppealCheckAndSubmitViewModel>(User.GetUkPrn(), model.ProfileId, model.AssessmentId, model.ComponentType);
                checkAndSubmitViewModel.NewGrade = checkAndSubmitViewModel.OldGrade;
                checkAndSubmitViewModel.IsGradeChanged = false;
                await _cacheService.SetAsync(CacheKey, checkAndSubmitViewModel);

                return RedirectToRoute(RouteConstants.PrsAppealCheckAndSubmit);
            }
            else if (model.AppealOutcome == AppealOutcomeKnownType.No)
            {
                bool isSuccess = await _postResultsServiceLoader.PrsAppealActivityAsync(User.GetUkPrn(), model);
                if (!isSuccess)
                    return RedirectToRoute(RouteConstants.ProblemWithService);

                var notificationBanner = new NotificationBannerModel { IsPrsJourney = true, HeaderMessage = prsDetails.Banner_HeaderMesage, Message = prsDetails.SuccessBannerMessage };
                await _cacheService.SetAsync(CacheKey, notificationBanner, CacheExpiryTime.XSmall);

                return RedirectToRoute(RouteConstants.PrsLearnerDetails, new { profileId = model.ProfileId });
            }
            else
            {
                return RedirectToRoute(RouteConstants.PrsLearnerDetails, new { profileId = model.ProfileId });
            }
        }

        [HttpGet]
        [Route("post-results-appeal-change-grade/{profileId}/{assessmentId}/{componentType}/{isAppealOutcomeJourney:bool?}/{isChangeMode:bool?}", Name = RouteConstants.PrsAppealGradeChange)]
        public async Task<IActionResult> PrsAppealGradeChangeAsync(int profileId, int assessmentId, ComponentType componentType, bool? isAppealOutcomeJourney, bool? isChangeMode)
        {
            var viewModel = await _postResultsServiceLoader.GetPrsLearnerDetailsAsync<PrsAppealGradeChangeViewModel>(User.GetUkPrn(), profileId, assessmentId, componentType);

            if (viewModel == null || !viewModel.IsValid)
                return RedirectToRoute(RouteConstants.PageNotFound);

            var checkAndSubmitDetails = await _cacheService.GetAsync<PrsAppealCheckAndSubmitViewModel>(CacheKey);
            if (checkAndSubmitDetails != null && (isChangeMode == null || isChangeMode.Value == false))
                viewModel.SelectedGradeCode = viewModel.Grades?.FirstOrDefault(g => g.Value == checkAndSubmitDetails?.NewGrade)?.Code;

            viewModel.IsAppealOutcomeJourney = isAppealOutcomeJourney ?? false;
            viewModel.IsChangeMode = isChangeMode ?? false;
            return View(viewModel);
        }

        [HttpPost]
        [Route("post-results-appeal-change-grade/{profileId}/{assessmentId}/{componentType}/{isAppealOutcomeJourney:bool?}/{isChangeMode:bool?}", Name = RouteConstants.SubmitPrsAppealGradeChange)]
        public async Task<IActionResult> PrsAppealGradeChangeAsync(PrsAppealGradeChangeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var prsDetails = await _postResultsServiceLoader.GetPrsLearnerDetailsAsync<PrsAppealGradeChangeViewModel>(User.GetUkPrn(), model.ProfileId, model.AssessmentId, model.ComponentType);
                prsDetails.IsAppealOutcomeJourney = model.IsAppealOutcomeJourney;
                prsDetails.IsChangeMode = model.IsChangeMode;
                return View(prsDetails);
            }

            var checkAndSubmitViewModel = await _postResultsServiceLoader.GetPrsLearnerDetailsAsync<PrsAppealCheckAndSubmitViewModel>(User.GetUkPrn(), model.ProfileId, model.AssessmentId, model.ComponentType);
            checkAndSubmitViewModel.NewGrade = model.Grades?.FirstOrDefault(x => x.Code == model.SelectedGradeCode)?.Value;

            if (string.IsNullOrWhiteSpace(checkAndSubmitViewModel.NewGrade))
                return RedirectToRoute(RouteConstants.PageNotFound);

            checkAndSubmitViewModel.IsGradeChanged = true;
            await _cacheService.SetAsync(CacheKey, checkAndSubmitViewModel);

            return RedirectToRoute(RouteConstants.PrsAppealCheckAndSubmit);
        }

        [HttpGet]
        [Route("post-results-appeal-check", Name = RouteConstants.PrsAppealCheckAndSubmit)]
        public async Task<IActionResult> PrsAppealCheckAndSubmitAsync()
        {
            var viewModel = await _cacheService.GetAsync<PrsAppealCheckAndSubmitViewModel>(CacheKey);
            if (viewModel == null)
            {
                _logger.LogWarning(LogEvent.NoDataFound, $"Unable to read PrsAppealCheckAndSubmitViewModel from redis cache in Prs appeal outcome check and submit page. Ukprn: {User.GetUkPrn()}, User: {User.GetUserEmail()}");
                return RedirectToRoute(RouteConstants.PageNotFound);
            }

            return View(viewModel);
        }

        [HttpPost]
        [Route("post-results-appeal-check", Name = RouteConstants.SubmitPrsAppealCheckAndSubmit)]
        public async Task<IActionResult> PrsAppealCheckAndSubmitAsync(PrsAppealCheckAndSubmitViewModel model)
        {
            bool isSuccess = await _postResultsServiceLoader.PrsAppealActivityAsync(User.GetUkPrn(), model);
            if (!isSuccess)
                return RedirectToRoute(RouteConstants.ProblemWithService);

            await _cacheService.RemoveAsync<PrsAppealCheckAndSubmitViewModel>(CacheKey);
            var notificationBanner = new NotificationBannerModel { IsPrsJourney = true, HeaderMessage = model.Banner_HeaderMesage, Message = model.SuccessBannerMessage };
            await _cacheService.SetAsync(CacheKey, notificationBanner, CacheExpiryTime.XSmall);

            return RedirectToRoute(RouteConstants.PrsLearnerDetails, new { profileId = model.ProfileId });
        }

        [HttpGet]
        [Route("post-results-cancel-appeal-update", Name = RouteConstants.PrsCancelAppealUpdate)]
        public async Task<IActionResult> PrsCancelAppealUpdateAsync()
        {
            var cacheModel = await _cacheService.GetAsync<PrsAppealCheckAndSubmitViewModel>(CacheKey);

            if (cacheModel == null)
                return RedirectToRoute(RouteConstants.PageNotFound);

            var viewModel = new PrsCancelAppealUpdateViewModel { ProfileId = cacheModel.ProfileId };
            return View(viewModel);
        }

        [HttpPost]
        [Route("post-results-cancel-appeal-update", Name = RouteConstants.SubmitPrsCancelAppealUpdate)]
        public async Task<IActionResult> PrsCancelAppealUpdateAsync(PrsCancelAppealUpdateViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (model.AreYouSureToCancel.Value)
            {
                await _cacheService.RemoveAsync<PrsAppealCheckAndSubmitViewModel>(CacheKey);
                return RedirectToRoute(RouteConstants.PrsLearnerDetails, new { profileId = model.ProfileId });
            }
            else
            {
                return RedirectToRoute(RouteConstants.PrsAppealCheckAndSubmit);
            }
        }

        //[NonAction]
        //public async Task<IActionResult> PrsWithdrawAppealAsync(AppealOutcomePathwayGradeViewModel model)
        //{
        //    bool isSuccess = await _postResultsServiceLoader.WithdrawAppealCoreGradeAsync(User.GetUkPrn(), model);
        //    if (!isSuccess)
        //        return RedirectToRoute(RouteConstants.ProblemWithService);

        //    var successMessage = string.Format(WithdrawAppealContent.Success_Banner_Message, model.PathwayName, model.PathwayCode);
        //    var notificationBanner = new NotificationBannerModel { Message = successMessage };
        //    await _cacheService.SetAsync(CacheKey, notificationBanner, CacheExpiryTime.XSmall);
        //    return RedirectToRoute(RouteConstants.PrsLearnerDetails, new { profileId = model.ProfileId, assessmentId = model.PathwayAssessmentId });
        //}

        [HttpGet]
        [Route("post-results-final-grade-change-request/{profileId}/{assessmentId}/{componentType}/{isResultJourney:bool?}", Name = RouteConstants.PrsGradeChangeRequest)]
        public async Task<IActionResult> PrsGradeChangeRequestAsync(int profileId, int assessmentId, ComponentType componentType, bool? isResultJourney)
        {
            var viewModel = await _postResultsServiceLoader.GetPrsLearnerDetailsAsync<PrsGradeChangeRequestViewModel>(User.GetUkPrn(), profileId, assessmentId, componentType);
            if (viewModel == null || !viewModel.CanRequestFinalGradeChange)
                return RedirectToRoute(RouteConstants.PageNotFound);

            viewModel.IsResultJourney = isResultJourney;
            return View(viewModel);
        }

        [HttpPost]
        [Route("post-results-final-grade-change-request/{profileId}/{assessmentId}/{componentType}/{isResultJourney:bool?}", Name = RouteConstants.SubmitPrsGradeChangeRequest)]
        public async Task<IActionResult> PrsGradeChangeRequestAsync(PrsGradeChangeRequestViewModel viewModel)
        {
            var learnerDetails = await _postResultsServiceLoader.GetPrsLearnerDetailsAsync<PrsGradeChangeRequestViewModel>(User.GetUkPrn(), viewModel.ProfileId, viewModel.AssessmentId, viewModel.ComponentType);

            if (learnerDetails == null || !learnerDetails.CanRequestFinalGradeChange)
                return RedirectToRoute(RouteConstants.PageNotFound);

            if (!ModelState.IsValid)
            {
                learnerDetails.IsResultJourney = viewModel.IsResultJourney;
                return View(learnerDetails);
            }

            var isSuccess = await _postResultsServiceLoader.PrsGradeChangeRequestAsync(viewModel);

            if (!isSuccess)
                return RedirectToRoute(RouteConstants.ProblemWithService);

            var confirmationViewModel = new PrsGradeChangeRequestConfirmationViewModel { ProfileId = viewModel.ProfileId };
            await _cacheService.SetAsync(CacheKey, confirmationViewModel, CacheExpiryTime.XSmall);

            return RedirectToRoute(RouteConstants.PrsGradeChangeRequestConfirmation);
        }

        [HttpGet]
        [Route("post-results-cancel-final-grade-change-request/{profileId}/{assessmentId}/{componentType}/{isResultJourney:bool?}", Name = RouteConstants.PrsCancelGradeChangeRequest)]
        public async Task<IActionResult> PrsCancelGradeChangeRequestAsync(int profileId, int assessmentId, ComponentType componentType, bool isResultJourney)
        {
            var viewModel = await _postResultsServiceLoader.GetPrsLearnerDetailsAsync<PrsCancelGradeChangeRequestViewModel>(User.GetUkPrn(), profileId, assessmentId, componentType);
            if (viewModel == null || !viewModel.IsValid)
                return RedirectToRoute(RouteConstants.PageNotFound);

            viewModel.IsResultJourney = isResultJourney;

            return View(viewModel);
        }

        [HttpPost]
        [Route("post-results-cancel-final-grade-change-request/{profileId}/{assessmentId}/{componentType}/{isResultJourney:bool?}", Name = RouteConstants.SubmitPrsCancelGradeChangeRequest)]
        public IActionResult PrsCancelGradeChangeRequest(PrsCancelGradeChangeRequestViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            if (viewModel.IsResultJourney)
            {
                if (!viewModel.AreYouSureToCancel.Value)
                    return RedirectToRoute(RouteConstants.PrsGradeChangeRequest, new { profileId = viewModel.ProfileId, assessmentId = viewModel.AssessmentId, isResultJourney = true.ToString() });

                return RedirectToRoute(RouteConstants.ResultDetails, new { profileId = viewModel.ProfileId });
            }

            if (!viewModel.AreYouSureToCancel.Value)
                return RedirectToRoute(RouteConstants.PrsGradeChangeRequest, new { profileId = viewModel.ProfileId, assessmentId = viewModel.AssessmentId, componentType = (int)viewModel.ComponentType });

            return RedirectToRoute(RouteConstants.PrsLearnerDetails, new { profileId = viewModel.ProfileId });
        }

        [HttpGet]
        [Route("post-results-final-grade-change-request-sent", Name = RouteConstants.PrsGradeChangeRequestConfirmation)]
        public async Task<IActionResult> PrsGradeChangeRequestConfirmationAsync()
        {
            var viewModel = await _cacheService.GetAndRemoveAsync<PrsGradeChangeRequestConfirmationViewModel>(CacheKey);

            if (viewModel == null)
            {
                _logger.LogWarning(LogEvent.ConfirmationPageFailed, $"Unable to read PrsGradeChangeRequestConfirmationViewModel from redis cache in request Prs grade change confirmation page. Ukprn: {User.GetUkPrn()}, User: {User.GetUserEmail()}");
                return RedirectToRoute(RouteConstants.PageNotFound);
            }

            return View(viewModel);
        }

        [HttpPost]
        [Route("post-results-final-grade-change-request-sent", Name = RouteConstants.SubmitPrsGradeChangeRequestConfirmation)]
        public IActionResult PrsGradeChangeRequestConfirmation(PrsGradeChangeRequestConfirmationViewModel viewModel)
        {
            if (viewModel == null)
                return RedirectToRoute(RouteConstants.PageNotFound);

            return viewModel.NavigationOption switch
            {
                PrsGradeChangeConfirmationNavigationOptions.BackToLearnersPage => RedirectToRoute(RouteConstants.PrsLearnerDetails, new { profileId = viewModel.ProfileId }),
                PrsGradeChangeConfirmationNavigationOptions.SearchForAnotherLearner => RedirectToRoute(RouteConstants.PrsSearchLearner),
                PrsGradeChangeConfirmationNavigationOptions.BackToHome => RedirectToRoute(RouteConstants.Home),
                _ => RedirectToRoute(RouteConstants.Home)
            };
        }

        [HttpGet]
        [Route("confirm-appeal-after-deadline/{profileId}/{assessmentId}", Name = RouteConstants.PrsAppealAfterDeadlineConfirm)]
        // TODO: Not in user delete
        public async Task<IActionResult> PrsAppealGradeAfterDeadlineConfirmAsync(int profileId, int assessmentId)
        {
            var viewModel = await _postResultsServiceLoader.GetPrsLearnerDetailsAsync<AppealGradeAfterDeadlineConfirmViewModel>(User.GetUkPrn(), profileId, assessmentId);
            if (viewModel == null || !viewModel.IsValid)
                return RedirectToRoute(RouteConstants.PageNotFound);

            return View(viewModel);
        }

        [HttpPost]
        [Route("confirm-appeal-after-deadline/{profileId}/{assessmentId}", Name = RouteConstants.SubmitPrsAppealAfterDeadlineConfirm)]
        // TODO: Not in user delete
        public async Task<IActionResult> PrsAppealGradeAfterDeadlineConfirmAsync(AppealGradeAfterDeadlineConfirmViewModel viewModel)
        {
            var prsLearner = await _postResultsServiceLoader.GetPrsLearnerDetailsAsync<AppealGradeAfterDeadlineConfirmViewModel>(User.GetUkPrn(), viewModel.ProfileId, viewModel.PathwayAssessmentId);

            if (prsLearner == null || !prsLearner.IsValid)
                return RedirectToRoute(RouteConstants.PageNotFound);

            if (!ModelState.IsValid)
                return View(prsLearner);

            if (viewModel.IsThisGradeBeingAppealed == false)
                return RedirectToRoute(RouteConstants.PrsLearnerDetails, new { profileId = viewModel.ProfileId, assessmentId = viewModel.PathwayAssessmentId });

            var isSuccess = await _postResultsServiceLoader.AppealGradeAfterDeadlineRequestAsync(viewModel);

            if (!isSuccess)
                return RedirectToRoute(RouteConstants.ProblemWithService);

            var confirmationViewModel = new PrsAppealGradeAfterDeadlineRequestConfirmationViewModel { ProfileId = viewModel.ProfileId, AssessmentId = viewModel.PathwayAssessmentId };
            await _cacheService.SetAsync(CacheKey, confirmationViewModel, CacheExpiryTime.XSmall);

            return RedirectToRoute(RouteConstants.PrsAppealGradeAfterDeadlineRequestConfirmation);
        }

        [HttpGet]
        [Route("appeal-after-deadline-request-sent", Name = RouteConstants.PrsAppealGradeAfterDeadlineRequestConfirmation)]
        // TODO: Not in user delete
        public async Task<IActionResult> PrsAppealGradeAfterDeadlineRequestConfirmationAsync()
        {
            var viewModel = await _cacheService.GetAndRemoveAsync<PrsAppealGradeAfterDeadlineRequestConfirmationViewModel>(CacheKey);

            if (viewModel == null)
            {
                _logger.LogWarning(LogEvent.ConfirmationPageFailed, $"Unable to read PrsAppealGradeAfterDeadlineRequestConfirmationViewModel from redis cache in request Prs appeal grade after deadline request confirmation page. Ukprn: {User.GetUkPrn()}, User: {User.GetUserEmail()}");
                return RedirectToRoute(RouteConstants.PageNotFound);
            }

            return View(viewModel);
        }        
    }
}