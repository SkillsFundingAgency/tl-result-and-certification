﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sfa.Tl.ResultsAndCertification.Common.Constants;
using Sfa.Tl.ResultsAndCertification.Common.Extensions;
using Sfa.Tl.ResultsAndCertification.Common.Helpers;
using Sfa.Tl.ResultsAndCertification.Common.Services.Cache;
using Sfa.Tl.ResultsAndCertification.Web.Loader.Interfaces;
using Sfa.Tl.ResultsAndCertification.Web.ViewModel.AdminPostResults;
using System.Threading.Tasks;

namespace Sfa.Tl.ResultsAndCertification.Web.Controllers
{
    [Authorize(Policy = RolesExtensions.RequireAdminDashboardAccess)]
    public class AdminPostResultsController : Controller
    {
        private readonly IAdminPostResultsLoader _loader;
        private readonly ICacheService _cacheService;
        private readonly ILogger _logger;

        private string CacheKey
            => CacheKeyHelper.GetCacheKey(User.GetUserId(), CacheConstants.AdminPostResultsCacheKey);

        public AdminPostResultsController(
            IAdminPostResultsLoader loader,
            ICacheService cacheService,
            ILogger<AdminPostResultsController> logger)
        {
            _loader = loader;
            _cacheService = cacheService;
            _logger = logger;
        }

        [HttpGet]
        [Route("admin/open-romm-core-clear", Name = RouteConstants.AdminOpenPathwayRommClear)]
        public async Task<IActionResult> AdminOpenPathwayRommClearAsync(int registrationPathwayId, int assessmentId)
        {
            await _cacheService.RemoveAsync<AdminOpenPathwayRommViewModel>(CacheKey);
            return RedirectToRoute(RouteConstants.AdminOpenPathwayRomm, new { registrationPathwayId, assessmentId });
        }

        [HttpGet]
        [Route("admin/open-romm-core", Name = RouteConstants.AdminOpenPathwayRomm)]
        public async Task<IActionResult> AdminOpenPathwayRommAsync(int registrationPathwayId, int assessmentId)
        {
            var cachedModel = await _cacheService.GetAsync<AdminOpenPathwayRommViewModel>(CacheKey);
            if (cachedModel != null)
            {
                return View(cachedModel);
            }

            AdminOpenPathwayRommViewModel viewModel = await _loader.GetAdminOpenPathwayRommAsync(registrationPathwayId, assessmentId);
            if (viewModel == null)
            {
                _logger.LogWarning(LogEvent.NoDataFound, $"No core result details found. Method: AdminOpenPathwayRommAsync({registrationPathwayId}, {assessmentId}), User: {User.GetUserEmail()}");
                return RedirectToRoute(RouteConstants.PageNotFound);
            }

            return View(viewModel);
        }

        [HttpPost]
        [Route("admin/open-romm-core", Name = RouteConstants.SubmitAdminOpenPathwayRomm)]
        public async Task<IActionResult> AdminOpenPathwayRommAsync(AdminOpenPathwayRommViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await Task.CompletedTask;
            return View(model);
        }

        [HttpGet]
        [Route("admin/open-romm-specialism-clear", Name = RouteConstants.AdminOpenSpecialismRommClear)]
        public async Task<IActionResult> AdminOpenSpecialismRommClearAsync(int registrationPathwayId, int assessmentId)
        {
            await _cacheService.RemoveAsync<AdminOpenSpecialismRommViewModel>(CacheKey);
            return RedirectToRoute(RouteConstants.AdminOpenSpecialismRomm, new { registrationPathwayId, assessmentId });
        }

        [HttpGet]
        [Route("admin/open-romm-specialism", Name = RouteConstants.AdminOpenSpecialismRomm)]
        public async Task<IActionResult> AdminOpenSpecialismRommAsync(int registrationPathwayId, int assessmentId)
        {
            var cachedModel = await _cacheService.GetAsync<AdminOpenSpecialismRommViewModel>(CacheKey);
            if (cachedModel != null)
            {
                return View(cachedModel);
            }

            AdminOpenSpecialismRommViewModel viewModel = await _loader.GetAdminOpenSpecialismRommAsync(registrationPathwayId, assessmentId);
            if (viewModel == null)
            {
                _logger.LogWarning(LogEvent.NoDataFound, $"No core result details found. Method: AdminOpenSpecialismRommAsync({registrationPathwayId}, {assessmentId}), User: {User.GetUserEmail()}");
                return RedirectToRoute(RouteConstants.PageNotFound);
            }

            return View(viewModel);
        }

        [HttpPost]
        [Route("admin/open-romm-specialism", Name = RouteConstants.SubmitAdminOpenSpecialismRomm)]
        public async Task<IActionResult> AdminOpenSpecialismRommAsync(AdminOpenSpecialismRommViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await Task.CompletedTask;
            return View(model);
        }
    }
}