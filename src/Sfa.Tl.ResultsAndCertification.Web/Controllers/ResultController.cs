﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sfa.Tl.ResultsAndCertification.Common.Constants;
using Sfa.Tl.ResultsAndCertification.Common.Enum;
using Sfa.Tl.ResultsAndCertification.Common.Extensions;
using Sfa.Tl.ResultsAndCertification.Common.Helpers;
using Sfa.Tl.ResultsAndCertification.Common.Services.Cache;
using Sfa.Tl.ResultsAndCertification.Web.Loader.Interfaces;
using Sfa.Tl.ResultsAndCertification.Web.ViewModel.Result;
using Sfa.Tl.ResultsAndCertification.Web.ViewModel.Result.Manual;
using System.Threading.Tasks;

namespace Sfa.Tl.ResultsAndCertification.Web.Controllers
{
    [Authorize(Policy = RolesExtensions.RequireResultsEditorAccess)]
    public class ResultController : Controller
    {
        private readonly IResultLoader _resultLoader;
        private readonly ICacheService _cacheService;
        private readonly ILogger _logger;

        private string CacheKey
        {
            get { return CacheKeyHelper.GetCacheKey(User.GetUserId(), CacheConstants.ResultCacheKey); }
        }

        public ResultController(IResultLoader resultLoader, ICacheService cacheService, ILogger<ResultController> logger)
        {
            _resultLoader = resultLoader;
            _cacheService = cacheService;
            _logger = logger;
        }

        [HttpGet]
        [Route("results", Name = RouteConstants.ResultsDashboard)]
        public IActionResult Index()
        {
            var viewmodel = new DashboardViewModel();
            return View(viewmodel);
        }

        [HttpGet]
        [Route("upload-results-file/{requestErrorTypeId:int?}", Name = RouteConstants.UploadResultsFile)]
        public IActionResult UploadResultsFile(int? requestErrorTypeId)
        {
            var model = new UploadResultsRequestViewModel { RequestErrorTypeId = requestErrorTypeId };
            model.SetAnyModelErrors(ModelState);
            return View(model);
        }

        [HttpPost]
        [Route("upload-results-file", Name = RouteConstants.SubmitUploadResultsFile)]
        public async Task<IActionResult> UploadResultsFileAsync(UploadResultsRequestViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            viewModel.AoUkprn = User.GetUkPrn();
            var response = await _resultLoader.ProcessBulkResultsAsync(viewModel);

            // TODO: refine in upcoming stories
            if (response.IsSuccess)
                return RedirectToRoute(RouteConstants.ResultsUploadSuccessful);
            else
            {
                ViewBag.BlobId = response.BlobUniqueReference;
                return View("UploadUnsuccessful");
                //return RedirectToRoute(RouteConstants.ResultsUploadUnsuccessful);
            }
        }

        [HttpGet]
        [Route("results-upload-successful", Name = RouteConstants.ResultsUploadSuccessful)]
        public async Task<IActionResult> UploadSuccessful()
        {
            return View();
        }

        [HttpGet]
        [Route("results-upload-unsuccessful", Name = RouteConstants.ResultsUploadUnsuccessful)]
        public async Task<IActionResult> UploadUnsuccessful()
        {
            return View();
        }

        [HttpGet]
        [Route("download-result-errors", Name = RouteConstants.DownloadResultErrors)]
        public async Task<IActionResult> DownloadAssessmentErrors(string id)
        {
            var fileStream = await _resultLoader.GetResultValidationErrorsFileAsync(User.GetUkPrn(), id.ToGuid());
            fileStream.Position = 0;
            return new FileStreamResult(fileStream, "text/csv")
            {
                FileDownloadName = "ValidationErrors.csv"
            };
        }

        [HttpGet]
        [Route("results-learner-search", Name = RouteConstants.SearchResults)]
        public async Task<IActionResult> SearchResultsAsync()
        {
            var defaultValue = await _cacheService.GetAndRemoveAsync<string>(Constants.ResultsSearchCriteria);
            var viewModel = new SearchResultsViewModel { SearchUln = defaultValue };
            return View(viewModel);
        }

        [HttpPost]
        [Route("results-learner-search", Name = RouteConstants.SubmitSearchResults)]
        public async Task<IActionResult> SearchResultsAsync(SearchResultsViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var searchResult = await _resultLoader.FindUlnResultsAsync(User.GetUkPrn(), model.SearchUln.ToLong());

            return View(model);
        }
    }
}