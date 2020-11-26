﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using Sfa.Tl.ResultsAndCertification.Common.Constants;
using Sfa.Tl.ResultsAndCertification.Common.Enum;
using Sfa.Tl.ResultsAndCertification.Common.Extensions;
using Sfa.Tl.ResultsAndCertification.Common.Helpers;
using Sfa.Tl.ResultsAndCertification.Common.Services.Cache;
using Sfa.Tl.ResultsAndCertification.Web.Loader.Interfaces;
using Sfa.Tl.ResultsAndCertification.Web.ViewModel.Assessment;
using Sfa.Tl.ResultsAndCertification.Web.ViewModel.Assessment.Manual;
using System.Threading.Tasks;
using AssessmentContent = Sfa.Tl.ResultsAndCertification.Web.Content.Assessment;


namespace Sfa.Tl.ResultsAndCertification.Web.Controllers
{
    [Authorize(Policy = RolesExtensions.RequireRegistrationsEditorAccess)]
    public class AssessmentController : Controller
    {
        private readonly IAssessmentLoader _assessmentLoader;
        private readonly ICacheService _cacheService;
        private readonly ILogger _logger;

        private string CacheKey
        {
            get { return CacheKeyHelper.GetCacheKey(User.GetUserId(), CacheConstants.AssessmentCacheKey); }
        }

        public AssessmentController(IAssessmentLoader assessmentLoader, ICacheService cacheService, ILogger<AssessmentController> logger)
        {
            _assessmentLoader = assessmentLoader;
            _cacheService = cacheService;
            _logger = logger;
        }

        [HttpGet]
        [Route("assessment-entries", Name = RouteConstants.AssessmentDashboard)]
        public IActionResult Index()
        {
            var viewmodel = new DashboardViewModel();
            return View(viewmodel);
        }

        [HttpGet]
        [Route("upload-assessment-entries-file", Name = RouteConstants.UploadAssessmentsFile)]
        public IActionResult UploadAssessmentsFile()
        {
            return View(new UploadAssessmentsRequestViewModel());
        }

        [HttpPost]
        [Route("upload-assessment-entries-file", Name = RouteConstants.SubmitUploadAssessmentsFile)]
        public async Task<IActionResult> UploadAssessmentsFileAsync(UploadAssessmentsRequestViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            viewModel.AoUkprn = User.GetUkPrn();
            var response = await _assessmentLoader.ProcessBulkAssessmentsAsync(viewModel);

            if (response.IsSuccess)
            {
                var successfulViewModel = new UploadSuccessfulViewModel { Stats = response.Stats };
                await _cacheService.SetAsync(string.Concat(CacheKey, Constants.AssessmentsUploadSuccessfulViewModel), successfulViewModel, CacheExpiryTime.XSmall);
                return RedirectToRoute(RouteConstants.AssessmentsUploadSuccessful);
            }

            if (response.ShowProblemWithServicePage)
                return RedirectToRoute(RouteConstants.ProblemWithAssessmentsUpload);

            var unsuccessfulViewModel = new ViewModel.Registration.UploadUnsuccessfulViewModel 
            { 
                BlobUniqueReference = response.BlobUniqueReference, 
                FileSize = response.ErrorFileSize, 
                FileType = FileType.Csv.ToString().ToUpperInvariant() 
            };

            await _cacheService.SetAsync(string.Concat(CacheKey, Constants.UploadUnsuccessfulViewModel), unsuccessfulViewModel, CacheExpiryTime.XSmall);
            return RedirectToRoute(RouteConstants.AssessmentsUploadUnsuccessful);
        }

        [HttpGet]
        [Route("assessment-entries-upload-confirmation", Name = RouteConstants.AssessmentsUploadSuccessful)]
        public async Task<IActionResult> UploadSuccessful()
        {
            var viewModel = await _cacheService.GetAndRemoveAsync<UploadSuccessfulViewModel>(string.Concat(CacheKey, Constants.AssessmentsUploadSuccessfulViewModel));

            if (viewModel == null)
            {
                _logger.LogWarning(LogEvent.UploadSuccessfulPageFailed, $"Unable to read upload successful assessment response from redis cache. Ukprn: {User.GetUkPrn()}, User: {User.GetUserEmail()}");
                return RedirectToRoute(RouteConstants.PageNotFound);
            }
            return View(viewModel);
        }

        [HttpGet]
        [Route("assessment-entries-upload-unsuccessful", Name = RouteConstants.AssessmentsUploadUnsuccessful)]
        public async Task<IActionResult> UploadUnsuccessful()
        {
            var viewModel = await _cacheService.GetAndRemoveAsync<ViewModel.Registration.UploadUnsuccessfulViewModel>(string.Concat(CacheKey, Constants.UploadUnsuccessfulViewModel));
            if (viewModel == null)
            {
                _logger.LogWarning(LogEvent.UploadUnsuccessfulPageFailed,
                    $"Unable to read upload unsuccessful registration response from temp data. Ukprn: {User.GetUkPrn()}, User: {User.GetUserEmail()}");
                return RedirectToRoute(RouteConstants.PageNotFound);
            }
            return View(viewModel);
        }

        [HttpGet]
        [Route("assessment-entries-file-upload-service-problem", Name = RouteConstants.ProblemWithAssessmentsUpload)]
        public IActionResult ProblemWithAssessmentsUpload()
        {
            return View();
        }

        [HttpGet]
        [Route("download-assessment-errors", Name = RouteConstants.DownloadAssessmentErrors)]
        public async Task<IActionResult> DownloadAssessmentErrors(string id)
        {
            if (id.IsGuid())
            {
                var fileStream = await _assessmentLoader.GetAssessmentValidationErrorsFileAsync(User.GetUkPrn(), id.ToGuid());
                if (fileStream == null)
                {
                    _logger.LogWarning(LogEvent.FileStreamNotFound, $"No FileStream found to download assessment validation errors. Method: GetAssessmentValidationErrorsFileAsync(AoUkprn: {User.GetUkPrn()}, BlobUniqueReference = {id})");
                    return RedirectToRoute(RouteConstants.PageNotFound);
                }

                fileStream.Position = 0;
                return new FileStreamResult(fileStream, "text/csv")
                {
                    FileDownloadName = AssessmentContent.UploadUnsuccessful.Assessment_Error_Report_File_Name_Text
                };
            }
            else
            {
                _logger.LogWarning(LogEvent.DownloadAssesssmentErrorsFailed, $"Not a valid guid to read file.Method: DownloadAssessmentErrors(Id = { id}), Ukprn: { User.GetUkPrn()}, User: { User.GetUserEmail()}");
                return RedirectToRoute(RouteConstants.Error, new { StatusCode = 500 });
            }
        }

        [HttpGet]
        [Route("assessment-entries-learner-search", Name = RouteConstants.SearchAssessments)]
        public async Task<IActionResult> SearchAssessmentsAsync()
        {
            var defaultValue = await _cacheService.GetAndRemoveAsync<string>(Constants.AssessmentsSearchCriteria);
            var viewModel = new SearchAssessmentsViewModel { SearchUln = defaultValue };
            return View(viewModel);
        }

        [Route("assessment-entries-learner-search", Name = RouteConstants.SubmitSearchAssessments)]
        public async Task<IActionResult> SearchAssessmentsAsync(SearchAssessmentsViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var searchResult = await _assessmentLoader.FindUlnAssessmentsAsync(User.GetUkPrn(), model.SearchUln.ToLong());

            if (searchResult?.IsAllowed == true)
            {
                return RedirectToRoute(searchResult.IsWithdrawn ? RouteConstants.AssessmentWithdrawnDetails : RouteConstants.AssessmentDetails, new { profileId = searchResult.RegistrationProfileId });
            }
            else
            {
                await _cacheService.SetAsync(Constants.AssessmentsSearchCriteria, model.SearchUln);

                var ulnAssessmentsNotfoundModel = new UlnAssessmentsNotFoundViewModel { Uln = model.SearchUln.ToString() };
                await _cacheService.SetAsync(string.Concat(CacheKey, Constants.SearchAssessmentsUlnNotFound), ulnAssessmentsNotfoundModel, CacheExpiryTime.XSmall);

                return RedirectToRoute(RouteConstants.SearchAssessmentsNotFound);
            }
        }

        [HttpGet]
        [Route("search-for-learner-ULN-not-found", Name = RouteConstants.SearchAssessmentsNotFound)]
        public async Task<IActionResult> SearchAssessmentsNotFoundAsync()
        {
            var viewModel = await _cacheService.GetAndRemoveAsync<UlnAssessmentsNotFoundViewModel>(string.Concat(CacheKey, Constants.SearchAssessmentsUlnNotFound));

            if (viewModel == null)
            {
                _logger.LogWarning(LogEvent.NoDataFound, $"Unable to read SearchAssessmentsUlnNotFound from redis cache in search assessments not found page. Ukprn: {User.GetUkPrn()}, User: {User.GetUserEmail()}");
                return RedirectToRoute(RouteConstants.PageNotFound);
            }
            return View(viewModel);
        }

        [HttpGet]
        [Route("learners-assessment-entries-withdrawn-learner/{profileId}", Name = RouteConstants.AssessmentWithdrawnDetails)]
        public async Task<IActionResult> AssessmentWithdrawnDetailsAsync(int profileId)
        {
            var viewModel = await _assessmentLoader.GetAssessmentDetailsAsync(User.GetUkPrn(), profileId, RegistrationPathwayStatus.Withdrawn);

            if (viewModel == null)
            {
                _logger.LogWarning(LogEvent.NoDataFound, $"No assessment withdrawn details found. Method: GetAssessmentDetailsAsync({User.GetUkPrn()}, {profileId}, {RegistrationPathwayStatus.Withdrawn}), User: {User.GetUserEmail()}");
                return RedirectToRoute(RouteConstants.PageNotFound);
            }

            return View(viewModel);
        }

        [HttpGet]
        [Route("learners-assessment-entries/{profileId}", Name = RouteConstants.AssessmentDetails)]
        public async Task<IActionResult> AssessmentDetailsAsync(int profileId)
        {
            var viewModel = await _assessmentLoader.GetAssessmentDetailsAsync(User.GetUkPrn(), profileId, RegistrationPathwayStatus.Active);

            if (viewModel == null)
            {
                _logger.LogWarning(LogEvent.NoDataFound, $"No assessment details found. Method: GetAssessmentDetailsAsync({User.GetUkPrn()}, {profileId}, {RegistrationPathwayStatus.Active}), User: {User.GetUserEmail()}");
                return RedirectToRoute(RouteConstants.PageNotFound);
            }

            return View(viewModel);
        }

        [HttpGet]
        [Route("add-core-assessment-entry-next-available-series/{profileId}", Name = RouteConstants.AddCoreAssessmentSeries)]
        public async Task<IActionResult> AddCoreAssessmentSeriesAsync(int profileId)
        {
            var model = new AddAssessmentSeriesViewModel
            {
                ProfileId = profileId,
                AssessmentSeriesId = 1, 
                AssessmentSeriesName = "Summer 2021", /*TODO*/ 
            };

            return View(model);
        }

        [HttpPost]
        [Route("add-core-assessment-entry-next-available-series", Name = RouteConstants.SubmitAddCoreAssessmentSeries)]
        public async Task<IActionResult> AddCoreAssessmentSeriesAsync(AddAssessmentSeriesViewModel model)
        {
            if (!IsValidModelState(ModelState, model))
            {
                return View(model);
            }

            return View(model);
        }

        private bool IsValidModelState(ModelStateDictionary modelState, AddAssessmentSeriesViewModel model)
        {
            if (!model.IsOpted.HasValue)
                modelState.AddModelError("IsOpted", $"{AssessmentContent.AddCoreAssessmentSeries.Select_Option_To_Add_Validation_Text} {model.AssessmentSeriesName}");
            
            return modelState.IsValid;
        }
    }
}
