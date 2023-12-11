﻿using Lrs.LearnerService.Api.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sfa.Tl.ResultsAndCertification.Common.Constants;
using Sfa.Tl.ResultsAndCertification.Common.Extensions;
using Sfa.Tl.ResultsAndCertification.Common.Helpers;
using Sfa.Tl.ResultsAndCertification.Common.Services.Cache;
using Sfa.Tl.ResultsAndCertification.Web.Loader.Interfaces;
using Sfa.Tl.ResultsAndCertification.Web.ViewComponents.InformationBanner;
using Sfa.Tl.ResultsAndCertification.Web.ViewComponents.NotificationBanner;
using Sfa.Tl.ResultsAndCertification.Web.ViewModel.AdminDashboard;
using Sfa.Tl.ResultsAndCertification.Web.ViewModel.AdminDashboard.LearnerRecord;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Sfa.Tl.ResultsAndCertification.Web.Controllers
{
    [Authorize(Policy = RolesExtensions.RequireAdminDashboardAccess)]
    public class AdminDashboardController : Controller
    {
        private readonly ITrainingProviderLoader _trainingProviderLoader;
        private readonly ICacheService _cacheService;
        private readonly ILogger _logger;
        private readonly IAdminDashboardLoader _loader;
        private string CacheKey { get { return CacheKeyHelper.GetCacheKey(User.GetUserId(), CacheConstants.AdminDashboardCacheKey); } }
        private string InformationCacheKey { get { return CacheKeyHelper.GetCacheKey(User.GetUserId(), CacheConstants.AdminDashboardInformationCacheKey); } }

        public AdminDashboardController(IAdminDashboardLoader loader, ICacheService cacheService, ILogger<AdminDashboardController> logger)
        {
            _loader = loader;
            _cacheService = cacheService;
            _logger = logger;
        }

        [HttpGet]
        [Route("admin/home", Name = RouteConstants.AdminHome)]
        public async Task<IActionResult> AdminHome()
        {
            await _cacheService.RemoveAsync<AdminSearchLearnerViewModel>(CacheKey);
            return RedirectToRoute(RouteConstants.Home);
        }

        [HttpGet]
        [Route("admin/learner-record/{pathwayid}", Name = RouteConstants.AdminLearnerRecord)]
        public async Task<IActionResult> AdminLearnerRecordAsync(int pathwayId)
        {
            var viewModel = await _loader.GetAdminLearnerRecordAsync<AdminLearnerRecordViewModel>(pathwayId);
            if (viewModel == null || !viewModel.IsLearnerRegistered)
            {
                _logger.LogWarning(LogEvent.NoDataFound, $"No learner record details found or learner is not registerd or learner record not added. Method: LearnerRecordDetailsAsync({pathwayId}), User: {User.GetUserEmail()}");
                return RedirectToRoute(RouteConstants.PageNotFound);
            }
            viewModel.InformationBanner = await _cacheService.GetAndRemoveAsync<InformationBannerModel>(InformationCacheKey);
            viewModel.SuccessBanner = await _cacheService.GetAndRemoveAsync<NotificationBannerModel>(CacheKey);

            return View(viewModel);
        }

        [HttpGet]
        [Route("admin/search-learner-records-clear", Name = RouteConstants.AdminSearchLearnersRecordsClear)]
        public async Task<IActionResult> AdminSearchLearnersRecordsClearAsync()
        {
            await _cacheService.RemoveAsync<AdminSearchLearnerViewModel>(CacheKey);
            return RedirectToRoute(RouteConstants.AdminSearchLearnersRecords);
        }

        [HttpGet]
        [Route("admin/search-learner-records/{pageNumber:int?}", Name = RouteConstants.AdminSearchLearnersRecords)]
        public async Task<IActionResult> AdminSearchLearnersAsync(int? pageNumber = default)
        {
            var viewModel = await _cacheService.GetAsync<AdminSearchLearnerViewModel>(CacheKey);
            if (viewModel == null)
            {
                AdminSearchLearnerFiltersViewModel filters = await _loader.GetAdminSearchLearnerFiltersAsync();
                viewModel = new AdminSearchLearnerViewModel(filters);

                await _cacheService.SetAsync(CacheKey, viewModel);
                return View(viewModel);
            }

            var searchCriteria = viewModel.SearchLearnerCriteria;

            if (!searchCriteria.IsSearchKeyApplied)
            {
                viewModel.ClearLearnerDetails();
                return View(viewModel);
            }

            searchCriteria.PageNumber = pageNumber;

            AdminSearchLearnerDetailsListViewModel learnerDetailsListViewModel = await _loader.GetAdminSearchLearnerDetailsListAsync(searchCriteria);
            viewModel.SetLearnerDetails(learnerDetailsListViewModel);

            viewModel.SearchLearnerDetailsList = learnerDetailsListViewModel;

            await _cacheService.SetAsync(CacheKey, viewModel);
            return View(viewModel);
        }

        [HttpPost]
        [Route("admin/search-learner-records-search-key", Name = RouteConstants.SubmitAdminSearchLearnerRecordsApplySearchKey)]
        public async Task<IActionResult> SubmitAdminSearchLearnersRecordsApplySearchKeyAsync(AdminSearchLearnerCriteriaViewModel searchCriteriaViewModel)
        {
            var viewModel = await _cacheService.GetAsync<AdminSearchLearnerViewModel>(CacheKey);
            viewModel.SetSearchKey(searchCriteriaViewModel.SearchKey);

            await _cacheService.SetAsync(CacheKey, viewModel);
            return RedirectToRoute(RouteConstants.AdminSearchLearnersRecords, new { pageNumber = searchCriteriaViewModel.PageNumber });
        }

        [HttpPost]
        [Route("admin/search-learner-records-clear-key", Name = RouteConstants.SubmitAdminSearchLearnerClearKey)]
        public async Task<IActionResult> AdminSearchLearnerClearKeyAsync()
        {
            await _cacheService.RemoveAsync<AdminSearchLearnerViewModel>(CacheKey);
            return RedirectToRoute(RouteConstants.AdminSearchLearnersRecords);
        }

        [HttpGet]
        [Route("admin/change-start-year/{pathwayId}", Name = RouteConstants.AdminChangeStartYear)]
        public async Task<IActionResult> AdminChangeStartYearAsync(int pathwayId)
        {
                      var viewModel = await _loader.GetAdminLearnerRecordAsync<AdminChangeStartYearViewModel>(pathwayId);

            if (viewModel == null)
                return RedirectToRoute(RouteConstants.PageNotFound);
            await _cacheService.SetAsync<AdminChangeStartYearViewModel>(CacheKey, viewModel);

            return View(viewModel);
        }

        [HttpPost]
        [Route("admin/submit-change-start-year", Name = RouteConstants.SubmitAdminChangeStartYear)]
        public async Task<IActionResult> AdminChangeStartYearAsync(AdminChangeStartYearViewModel model)
        {
             var _academicStartYearNew = model.AcademicStartYearNew;

             var viewModel = await _cacheService.GetAsync<AdminChangeStartYearViewModel>(CacheKey);

             if (viewModel.AcademicStartYearsToBe.Count > 0 &&
                 string.IsNullOrEmpty(model.AcademicStartYearNew))
             {
                 model.AcademicStartYearsToBe = viewModel.AcademicStartYearsToBe;
                 return View(viewModel);
             }

             model.AcademicStartYearNew = _academicStartYearNew;

             return View(model); // This should be re-direct to next page.
        }
    }
}