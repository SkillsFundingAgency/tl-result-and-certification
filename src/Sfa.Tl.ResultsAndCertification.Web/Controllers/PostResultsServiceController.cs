﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sfa.Tl.ResultsAndCertification.Common.Extensions;
using Sfa.Tl.ResultsAndCertification.Common.Helpers;
using Sfa.Tl.ResultsAndCertification.Web.ViewModel.PostResultsService;
using System.Threading.Tasks;

namespace Sfa.Tl.ResultsAndCertification.Web.Controllers
{
    [Authorize(Policy = RolesExtensions.RequireReviewsAndAppealsEditorAccess)]
    public class PostResultsServiceController : Controller
    {

        [HttpGet]
        [Route("reviews-and-appeals", Name = RouteConstants.StartReviewAndAppeals)]
        public IActionResult StartReviewsAndAppealsAsync()
        {
            return View(new StartReviewsAndAppealsViewModel());
        }

        [HttpGet]
        [Route("reviews-and-appeals-search-learner", Name = RouteConstants.SearchPostResultsService)]
        public async Task<IActionResult> SearchPostResultsServiceAsync()
        {
            await Task.CompletedTask;
            return View(new SearchPostResultsServiceViewModel());
        }

        [HttpPost]
        [Route("reviews-and-appeals-search-learner", Name = RouteConstants.SubmitSearchPostResultsService)]
        public async Task<IActionResult> SearchPostResultsServiceAsync(SearchPostResultsServiceViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await Task.CompletedTask;
            return View(new SearchPostResultsServiceViewModel());
        }
    }
}
