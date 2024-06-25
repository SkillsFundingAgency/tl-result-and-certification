﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sfa.Tl.ResultsAndCertification.Common.Constants;
using Sfa.Tl.ResultsAndCertification.Common.Enum;
using Sfa.Tl.ResultsAndCertification.Common.Extensions;
using Sfa.Tl.ResultsAndCertification.Common.Helpers;
using Sfa.Tl.ResultsAndCertification.Common.Services.Cache;
using Sfa.Tl.ResultsAndCertification.Models.Contracts.DataExport;
using Sfa.Tl.ResultsAndCertification.Web.Helpers;
using Sfa.Tl.ResultsAndCertification.Web.Loader.Interfaces;
using Sfa.Tl.ResultsAndCertification.Web.ViewModel;
using Sfa.Tl.ResultsAndCertification.Web.ViewModel.Common;
using Sfa.Tl.ResultsAndCertification.Web.ViewModel.Registration;
using Sfa.Tl.ResultsAndCertification.Web.ViewModel.Registration.Manual;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using RegistrationContent = Sfa.Tl.ResultsAndCertification.Web.Content.Registration;

namespace Sfa.Tl.ResultsAndCertification.Web.Controllers
{
    [Authorize(Policy = RolesExtensions.RequireRegistrationsEditorAccess)]
    public class RegistrationController : Controller
    {
        private readonly IRegistrationLoader _registrationLoader;
        private readonly ICacheService _cacheService;
        private readonly ILogger _logger;

        private string CacheKey
        {
            get { return CacheKeyHelper.GetCacheKey(User.GetUserId(), CacheConstants.RegistrationCacheKey); }
        }

        public RegistrationController(IRegistrationLoader registrationLoader, ICacheService cacheService, ILogger<RegistrationController> logger)
        {
            _registrationLoader = registrationLoader;
            _cacheService = cacheService;
            _logger = logger;
        }

        [HttpGet]
        [Route("registrations", Name = RouteConstants.RegistrationDashboard)]
        public IActionResult Index()
        {
            var viewmodel = new DashboardViewModel();
            return View(viewmodel);
        }

        [HttpGet]
        [Route("upload-registrations-file/{requestErrorTypeId:int?}", Name = RouteConstants.UploadRegistrationsFile)]
        public IActionResult UploadRegistrationsFile(int? requestErrorTypeId)
        {
            var model = new UploadRegistrationsRequestViewModel { RequestErrorTypeId = requestErrorTypeId };
            model.SetAnyModelErrors(ModelState);
            return View(model);
        }

        [HttpPost]
        [Route("upload-registrations-file", Name = RouteConstants.SubmitUploadRegistrationsFile)]
        public async Task<IActionResult> UploadRegistrationsFileAsync(UploadRegistrationsRequestViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            viewModel.AoUkprn = User.GetUkPrn();
            var response = await _registrationLoader.ProcessBulkRegistrationsAsync(viewModel);

            if (response.IsSuccess)
            {
                var successfulViewModel = new UploadSuccessfulViewModel { Stats = response.Stats };
                await _cacheService.SetAsync(string.Concat(CacheKey, Constants.UploadSuccessfulViewModel), successfulViewModel, CacheExpiryTime.XSmall);

                return RedirectToRoute(RouteConstants.RegistrationsUploadSuccessful);
            }
            else
            {
                if (response.ShowProblemWithServicePage)
                {
                    return RedirectToRoute(RouteConstants.ProblemWithRegistrationsUpload);
                }
                else
                {
                    var unsuccessfulViewModel = new UploadUnsuccessfulViewModel { BlobUniqueReference = response.BlobUniqueReference, FileSize = response.ErrorFileSize, FileType = FileType.Csv.ToString().ToUpperInvariant() };
                    await _cacheService.SetAsync(string.Concat(CacheKey, Constants.UploadUnsuccessfulViewModel), unsuccessfulViewModel, CacheExpiryTime.XSmall);
                    return RedirectToRoute(RouteConstants.RegistrationsUploadUnsuccessful);
                }
            }
        }

        [HttpGet]
        [Route("upload-registrations-file-success", Name = RouteConstants.RegistrationsUploadSuccessful)]
        public async Task<IActionResult> UploadSuccessful()
        {
            var viewModel = await _cacheService.GetAndRemoveAsync<UploadSuccessfulViewModel>(string.Concat(CacheKey, Constants.UploadSuccessfulViewModel));

            if (viewModel == null)
            {
                _logger.LogWarning(LogEvent.UploadSuccessfulPageFailed,
                    $"Unable to read upload successful registration response from temp data. Ukprn: {User.GetUkPrn()}, User: {User.GetUserEmail()}");
                return RedirectToRoute(RouteConstants.PageNotFound);
            }
            return View(viewModel);
        }

        [HttpGet]
        [Route("registrations-upload-unsuccessful", Name = RouteConstants.RegistrationsUploadUnsuccessful)]
        public async Task<IActionResult> UploadUnsuccessful()
        {
            var viewModel = await _cacheService.GetAndRemoveAsync<UploadUnsuccessfulViewModel>(string.Concat(CacheKey, Constants.UploadUnsuccessfulViewModel));
            if (viewModel == null)
            {
                _logger.LogWarning(LogEvent.UploadUnsuccessfulPageFailed,
                    $"Unable to read upload unsuccessful registration response from temp data. Ukprn: {User.GetUkPrn()}, User: {User.GetUserEmail()}");
                return RedirectToRoute(RouteConstants.PageNotFound);
            }
            return View(viewModel);
        }

        [HttpGet]
        [Route("problem-with-registrations-upload", Name = RouteConstants.ProblemWithRegistrationsUpload)]
        public IActionResult ProblemWithRegistrationsUpload()
        {
            return View();
        }

        [HttpGet]
        [Route("download-registration-errors", Name = RouteConstants.DownloadRegistrationErrors)]
        public async Task<IActionResult> DownloadRegistrationErrors(string id)
        {
            if (id.IsGuid())
            {
                var fileStream = await _registrationLoader.GetRegistrationValidationErrorsFileAsync(User.GetUkPrn(), id.ToGuid());
                if (fileStream == null)
                {
                    _logger.LogWarning(LogEvent.FileStreamNotFound, $"No FileStream found to download registration validation errors. Method: GetRegistrationValidationErrorsFileAsync(AoUkprn: {User.GetUkPrn()}, BlobUniqueReference = {id})");
                    return RedirectToRoute(RouteConstants.PageNotFound);
                }

                fileStream.Position = 0;
                return new FileStreamResult(fileStream, "text/csv")
                {
                    FileDownloadName = RegistrationContent.UploadUnsuccessful.Registrations_Error_Report_File_Name_Text
                };
            }
            else
            {
                _logger.LogWarning(LogEvent.DownloadRegistrationErrorsFailed, $"Not a valid guid to read file.Method: DownloadRegistrationErrors(Id = {id}), Ukprn: {User.GetUkPrn()}, User: {User.GetUserEmail()}");
                return RedirectToRoute(RouteConstants.Error, new { StatusCode = 500 });
            }
        }

        [HttpGet]
        [Route("add-registration-unique-learner", Name = RouteConstants.AddRegistration)]
        public async Task<IActionResult> AddRegistrationAsync()
        {
            await _cacheService.RemoveAsync<RegistrationViewModel>(CacheKey);
            return RedirectToRoute(RouteConstants.AddRegistrationUln);
        }

        [HttpGet]
        [Route("add-registration-unique-learner-number/{isChangeMode:bool?}", Name = RouteConstants.AddRegistrationUln)]
        public async Task<IActionResult> AddRegistrationUlnAsync(bool isChangeMode)
        {
            var cacheModel = await _cacheService.GetAsync<RegistrationViewModel>(CacheKey);
            var viewModel = cacheModel?.Uln != null ? cacheModel.Uln : new UlnViewModel();

            viewModel.IsChangeMode = isChangeMode && (cacheModel?.IsChangeModeAllowed == true);
            return View(viewModel);
        }

        [HttpPost]
        [Route("add-registration-unique-learner-number", Name = RouteConstants.SubmitRegistrationUln)]
        public async Task<IActionResult> AddRegistrationUlnAsync(UlnViewModel model)
        {
            if (model == null)
                return RedirectToRoute(RouteConstants.PageNotFound);

            if (!ModelState.IsValid)
                return View(model);

            await SyncCacheUln(model);

            var findUln = await _registrationLoader.FindUlnAsync(User.GetUkPrn(), model.Uln.ToLong());
            if (findUln != null && findUln.IsUlnRegisteredAlready)
            {
                findUln.IsChangeMode = model.IsChangeMode;
                await _cacheService.SetAsync(string.Concat(CacheKey, Constants.UlnRegistrationNotFoundViewModel), findUln, CacheExpiryTime.XSmall);
                return RedirectToRoute(RouteConstants.UlnCannotBeRegistered);
            }

            return model.IsChangeMode ? RedirectToRoute(RouteConstants.AddRegistrationCheckAndSubmit) : RedirectToRoute(RouteConstants.AddRegistrationLearnersName);
        }

        [HttpGet]
        [Route("ULN-cannot-be-registered", Name = RouteConstants.UlnCannotBeRegistered)]
        public async Task<IActionResult> UlnCannotBeRegistered()
        {
            var viewModel = await _cacheService.GetAndRemoveAsync<UlnRegistrationNotFoundViewModel>(string.Concat(CacheKey, Constants.UlnRegistrationNotFoundViewModel));
            return viewModel == null ? RedirectToRoute(RouteConstants.PageNotFound) : (IActionResult)View(viewModel);
        }

        [HttpGet]
        [Route("add-registration-learners-name/{isChangeMode:bool?}", Name = RouteConstants.AddRegistrationLearnersName)]
        public async Task<IActionResult> AddRegistrationLearnersNameAsync(bool isChangeMode)
        {
            var cacheModel = await _cacheService.GetAsync<RegistrationViewModel>(CacheKey);

            if (cacheModel?.Uln == null)
                return RedirectToRoute(RouteConstants.PageNotFound);

            var viewModel = cacheModel?.LearnersName == null ? new LearnersNameViewModel() : cacheModel.LearnersName;
            viewModel.IsChangeMode = isChangeMode && cacheModel.IsChangeModeAllowed;
            return View(viewModel);
        }

        [HttpPost]
        [Route("add-registration-learners-name", Name = RouteConstants.SubmitRegistrationLearnersName)]
        public async Task<IActionResult> AddRegistrationLearnersNameAsync(LearnersNameViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var cacheModel = await _cacheService.GetAsync<RegistrationViewModel>(CacheKey);
            if (model == null || cacheModel?.Uln == null)
                return RedirectToRoute(RouteConstants.PageNotFound);

            cacheModel.LearnersName = model;
            await _cacheService.SetAsync(CacheKey, cacheModel);

            return model.IsChangeMode ? RedirectToRoute(RouteConstants.AddRegistrationCheckAndSubmit) : RedirectToRoute(RouteConstants.AddRegistrationDateofBirth);
        }

        [HttpGet]
        [Route("add-registration-date-of-birth/{isChangeMode:bool?}", Name = RouteConstants.AddRegistrationDateofBirth)]
        public async Task<IActionResult> AddRegistrationDateofBirthAsync(bool isChangeMode)
        {
            var cacheModel = await _cacheService.GetAsync<RegistrationViewModel>(CacheKey);

            if (cacheModel?.LearnersName == null)
                return RedirectToRoute(RouteConstants.PageNotFound);

            var viewModel = cacheModel?.DateofBirth == null ? new DateofBirthViewModel() : cacheModel.DateofBirth;
            viewModel.IsChangeMode = isChangeMode && cacheModel.IsChangeModeAllowed;
            return View(viewModel);
        }

        [HttpPost]
        [Route("add-registration-date-of-birth", Name = RouteConstants.SubmitRegistrationDateofBirth)]
        public async Task<IActionResult> AddRegistrationDateofBirthAsync(DateofBirthViewModel model)
        {
            if (!IsValidDateofBirth(model))
                return View(model);

            var cacheModel = await _cacheService.GetAsync<RegistrationViewModel>(CacheKey);
            if (model == null || cacheModel?.LearnersName == null)
                return RedirectToRoute(RouteConstants.PageNotFound);

            cacheModel.DateofBirth = model;
            await _cacheService.SetAsync(CacheKey, cacheModel);

            return model.IsChangeMode ? RedirectToRoute(RouteConstants.AddRegistrationCheckAndSubmit) : RedirectToRoute(RouteConstants.AddRegistrationProvider);
        }

        [HttpGet]
        [Route("add-registration-provider/{isChangeMode:bool?}", Name = RouteConstants.AddRegistrationProvider)]
        public async Task<IActionResult> AddRegistrationProviderAsync(bool isChangeMode)
        {
            var cacheModel = await _cacheService.GetAsync<RegistrationViewModel>(CacheKey);

            if (cacheModel?.DateofBirth == null)
                return RedirectToRoute(RouteConstants.PageNotFound);

            var registeredProviders = await GetAoRegisteredProviders();
            var viewModel = cacheModel?.SelectProvider == null ? new SelectProviderViewModel() : cacheModel.SelectProvider;
            viewModel.ProvidersSelectList = registeredProviders.ProvidersSelectList;
            viewModel.IsChangeMode = isChangeMode && cacheModel.IsChangeModeAllowedForProvider;
            return View(viewModel);
        }

        [HttpPost]
        [Route("add-registration-provider", Name = RouteConstants.SubmitRegistrationProvider)]
        public async Task<IActionResult> AddRegistrationProviderAsync(SelectProviderViewModel model)
        {
            var cacheModel = await _cacheService.GetAsync<RegistrationViewModel>(CacheKey);
            if (model == null || cacheModel?.DateofBirth == null)
                return RedirectToRoute(RouteConstants.PageNotFound);

            var registeredProviderViewModel = await GetAoRegisteredProviders();

            if (!ModelState.IsValid)
            {
                model.ProvidersSelectList = registeredProviderViewModel.ProvidersSelectList;
                return View(model);
            }

            if (cacheModel?.SelectProvider?.SelectedProviderUkprn != model.SelectedProviderUkprn)
            {
                cacheModel.SelectCore = null;
                cacheModel.SpecialismQuestion = null;
                cacheModel.SelectSpecialisms = null;
            }

            model.SelectedProviderDisplayName = registeredProviderViewModel?.ProvidersSelectList?.FirstOrDefault(p => p.Value == model.SelectedProviderUkprn)?.Text;
            cacheModel.SelectProvider = model;
            await _cacheService.SetAsync(CacheKey, cacheModel);
            return model.IsChangeMode ? RedirectToRoute(RouteConstants.AddRegistrationCore, new { isChangeMode = "true" }) : RedirectToRoute(RouteConstants.AddRegistrationCore);
        }

        [HttpGet]
        [Route("add-registration-core/{isChangeMode:bool?}", Name = RouteConstants.AddRegistrationCore)]
        public async Task<IActionResult> AddRegistrationCoreAsync(bool isChangeMode)
        {
            var cacheModel = await _cacheService.GetAsync<RegistrationViewModel>(CacheKey);

            if (cacheModel?.SelectProvider == null)
                return RedirectToRoute(RouteConstants.PageNotFound);

            var providerCores = await GetRegisteredProviderCores(cacheModel.SelectProvider.SelectedProviderUkprn.ToLong());
            var viewModel = cacheModel?.SelectCore == null ? new SelectCoreViewModel() : cacheModel.SelectCore;
            viewModel.CoreSelectList = providerCores.CoreSelectList;
            viewModel.IsChangeMode = isChangeMode && cacheModel.IsChangeModeAllowedForCore;
            viewModel.IsChangeModeFromProvider = cacheModel.SelectProvider.IsChangeMode;
            return View(viewModel);
        }

        [HttpPost]
        [Route("add-registration-core", Name = RouteConstants.SubmitRegistrationCore)]
        public async Task<IActionResult> AddRegistrationCoreAsync(SelectCoreViewModel model)
        {
            var cacheModel = await _cacheService.GetAsync<RegistrationViewModel>(CacheKey);
            if (model == null || cacheModel?.SelectProvider == null)
                return RedirectToRoute(RouteConstants.PageNotFound);

            var coreViewModel = await GetRegisteredProviderCores(cacheModel.SelectProvider.SelectedProviderUkprn.ToLong());
            if (!ModelState.IsValid)
            {
                model.CoreSelectList = coreViewModel.CoreSelectList;
                return View(model);
            }

            if (cacheModel?.SelectCore?.SelectedCoreCode != model.SelectedCoreCode)
            {
                cacheModel.SpecialismQuestion = null;
                cacheModel.SelectSpecialisms = null;
            }

            model.SelectedCoreDisplayName = coreViewModel?.CoreSelectList?.FirstOrDefault(p => p.Value == model.SelectedCoreCode)?.Text;
            cacheModel.SelectCore = model;
            await _cacheService.SetAsync(CacheKey, cacheModel);
            return model.IsChangeMode ? RedirectToRoute(RouteConstants.AddRegistrationSpecialismQuestion, new { isChangeMode = "true" }) : RedirectToRoute(RouteConstants.AddRegistrationSpecialismQuestion);
        }

        [HttpGet]
        [Route("add-registration-learner-decided-specialism-question/{isChangeMode:bool?}", Name = RouteConstants.AddRegistrationSpecialismQuestion)]
        public async Task<IActionResult> AddRegistrationSpecialismQuestionAsync(bool isChangeMode)
        {
            var cacheModel = await _cacheService.GetAsync<RegistrationViewModel>(CacheKey);

            if (cacheModel?.SelectCore == null)
                return RedirectToRoute(RouteConstants.PageNotFound);

            var viewModel = cacheModel?.SpecialismQuestion == null ? new SpecialismQuestionViewModel() : cacheModel.SpecialismQuestion;
            viewModel.IsChangeMode = isChangeMode && cacheModel.IsChangeModeAllowedForSpecialismQuestion;
            viewModel.IsChangeModeFromCore = cacheModel.SelectCore.IsChangeMode;
            return View(viewModel);
        }

        [HttpPost]
        [Route("add-registration-learner-decided-specialism-question", Name = RouteConstants.SubmitRegistrationSpecialismQuestion)]
        public async Task<IActionResult> AddRegistrationSpecialismQuestionAsync(SpecialismQuestionViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var cacheModel = await _cacheService.GetAsync<RegistrationViewModel>(CacheKey);
            if (model == null || cacheModel?.SelectCore == null)
                return RedirectToRoute(RouteConstants.PageNotFound);

            if (!model.HasLearnerDecidedSpecialism.Value)
            {
                cacheModel.SelectSpecialisms = null;
            }

            cacheModel.SpecialismQuestion = model;
            await _cacheService.SetAsync(CacheKey, cacheModel);

            if (model.IsChangeMode)
            {
                return model.HasLearnerDecidedSpecialism.Value ? RedirectToRoute(RouteConstants.AddRegistrationSpecialisms, new { isChangeMode = "true" }) : RedirectToRoute(RouteConstants.AddRegistrationCheckAndSubmit);
            }
            else
            {
                return RedirectToRoute(model.HasLearnerDecidedSpecialism.Value ? RouteConstants.AddRegistrationSpecialisms : RouteConstants.AddRegistrationAcademicYear);
            }
        }

        [HttpGet]
        [Route("add-registration-specialism/{isChangeMode:bool?}", Name = RouteConstants.AddRegistrationSpecialisms)]
        public async Task<IActionResult> AddRegistrationSpecialismsAsync(bool isChangeMode)
        {
            var cacheModel = await _cacheService.GetAsync<RegistrationViewModel>(CacheKey);

            if (cacheModel?.SelectCore == null || cacheModel?.SpecialismQuestion == null || (!isChangeMode && cacheModel?.SpecialismQuestion?.HasLearnerDecidedSpecialism == false))
                return RedirectToRoute(RouteConstants.PageNotFound);

            var viewModel = cacheModel?.SelectSpecialisms == null ? new SelectSpecialismViewModel { PathwaySpecialisms = await GetPathwaySpecialismsByCoreCode(cacheModel.SelectCore.SelectedCoreCode) } : cacheModel.SelectSpecialisms;
            viewModel.IsChangeMode = isChangeMode && cacheModel.IsChangeModeAllowedForSelectSpecialism;
            viewModel.IsChangeModeFromSpecialismQuestion = cacheModel.SpecialismQuestion.IsChangeMode;
            return View(viewModel);
        }

        [HttpPost]
        [Route("add-registration-specialism", Name = RouteConstants.SubmitRegistrationSpecialisms)]
        public async Task<IActionResult> AddRegistrationSpecialismsAsync(SelectSpecialismViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var cacheModel = await _cacheService.GetAsync<RegistrationViewModel>(CacheKey);
            if (model == null || cacheModel?.SelectCore == null || cacheModel?.SpecialismQuestion == null || (!model.IsChangeMode && cacheModel?.SpecialismQuestion?.HasLearnerDecidedSpecialism == false))
                return RedirectToRoute(RouteConstants.PageNotFound);

            if (model.IsChangeMode && cacheModel.SpecialismQuestion.HasLearnerDecidedSpecialism.Value == false)
            {
                cacheModel.SpecialismQuestion.HasLearnerDecidedSpecialism = true;
            }

            model.PathwaySpecialisms?.Specialisms?.ToList().ForEach(x => { x.IsSelected = (x.Code == model.SelectedSpecialismCode); });
            var pathwaySpecialisms = await GetPathwaySpecialismsByCoreCode(cacheModel.SelectCore.SelectedCoreCode);
            model.PathwaySpecialisms.SpecialismsLookup = pathwaySpecialisms?.SpecialismsLookup;

            cacheModel.SelectSpecialisms = model;
            await _cacheService.SetAsync(CacheKey, cacheModel);
            return RedirectToRoute(model.IsChangeMode ? RouteConstants.AddRegistrationCheckAndSubmit : RouteConstants.AddRegistrationAcademicYear);
        }

        [HttpGet]
        [Route("add-registration-academic-year/{isChangeMode:bool?}", Name = RouteConstants.AddRegistrationAcademicYear)]
        public async Task<IActionResult> AddRegistrationAcademicYearAsync(bool isChangeMode)
        {
            var cacheModel = await _cacheService.GetAsync<RegistrationViewModel>(CacheKey);

            if (cacheModel?.SpecialismQuestion == null ||
                (cacheModel?.SpecialismQuestion?.HasLearnerDecidedSpecialism == true && cacheModel?.SelectSpecialisms == null))
                return RedirectToRoute(RouteConstants.PageNotFound);

            var hasSpecialismsSelected = cacheModel?.SelectSpecialisms != null;

            SelectAcademicYearViewModel viewModel;

            if (cacheModel?.SelectAcademicYear == null)
            {
                viewModel = new SelectAcademicYearViewModel { HasSpecialismsSelected = hasSpecialismsSelected, AcademicYears = await _registrationLoader.GetCurrentAcademicYearsAsync() };
            }
            else
            {
                cacheModel.SelectAcademicYear.HasSpecialismsSelected = hasSpecialismsSelected;
                viewModel = cacheModel?.SelectAcademicYear;
            }
            viewModel.IsChangeMode = isChangeMode && cacheModel.IsChangeModeAllowed;
            return View(viewModel);
        }

        [HttpPost]
        [Route("add-registration-academic-year", Name = RouteConstants.SubmitRegistrationAcademicYear)]
        public async Task<IActionResult> AddRegistrationAcademicYearAsync(SelectAcademicYearViewModel model)
        {
            var cacheModel = await _cacheService.GetAsync<RegistrationViewModel>(CacheKey);
            model.AcademicYears = await _registrationLoader.GetCurrentAcademicYearsAsync();

            if (!model.IsValidAcademicYear || cacheModel?.SpecialismQuestion == null || (cacheModel?.SpecialismQuestion?.HasLearnerDecidedSpecialism == true && cacheModel?.SelectSpecialisms == null))
                return RedirectToRoute(RouteConstants.PageNotFound);

            model.HasSpecialismsSelected = cacheModel?.SelectSpecialisms != null;
            cacheModel.SelectAcademicYear = model;
            await _cacheService.SetAsync(CacheKey, cacheModel);
            return RedirectToRoute(RouteConstants.AddRegistrationCheckAndSubmit);
        }

        [HttpGet]
        [Route("add-registration-check-and-submit", Name = RouteConstants.AddRegistrationCheckAndSubmit)]
        public async Task<IActionResult> AddRegistrationCheckAndSubmitAsync()
        {
            var cacheModel = await _cacheService.GetAsync<RegistrationViewModel>(CacheKey);

            var viewModel = new CheckAndSubmitViewModel { RegistrationModel = cacheModel };

            if (!viewModel.IsCheckAndSubmitPageValid)
                return RedirectToRoute(RouteConstants.PageNotFound);

            await _cacheService.SetAsync(CacheKey, viewModel.ResetChangeMode());
            return View(viewModel);
        }

        [HttpPost]
        [Route("add-registration-check-and-submit", Name = RouteConstants.SubmitRegistrationCheckAndSubmit)]
        public async Task<IActionResult> SubmitRegistrationCheckAndSubmitAsync()
        {
            var cacheModel = await _cacheService.GetAsync<RegistrationViewModel>(CacheKey);

            if (cacheModel == null)
                return RedirectToRoute(RouteConstants.PageNotFound);

            var isSuccess = await _registrationLoader.AddRegistrationAsync(User.GetUkPrn(), cacheModel);

            if (isSuccess)
            {
                await _cacheService.RemoveAsync<RegistrationViewModel>(CacheKey);
                await _cacheService.SetAsync(string.Concat(CacheKey, Constants.RegistrationConfirmationViewModel), new RegistrationConfirmationViewModel { UniqueLearnerNumber = cacheModel.Uln.Uln }, CacheExpiryTime.XSmall);
                return RedirectToRoute(RouteConstants.AddRegistrationConfirmation);
            }
            else
            {
                _logger.LogWarning(LogEvent.ManualRegistrationProcessFailed, $"Unable to add registration for UniqueLearnerNumber = {cacheModel.Uln}. Method: SubmitRegistrationCheckAndSubmitAsync, Ukprn: {User.GetUkPrn()}, User: {User.GetUserEmail()}");
                return RedirectToRoute(RouteConstants.Error, new { StatusCode = 500 });
            }
        }

        [HttpGet]
        [Route("add-registration-confirmation", Name = RouteConstants.AddRegistrationConfirmation)]
        public async Task<IActionResult> AddRegistrationConfirmationAsync()
        {
            var viewModel = await _cacheService.GetAndRemoveAsync<RegistrationConfirmationViewModel>(string.Concat(CacheKey, Constants.RegistrationConfirmationViewModel));

            if (viewModel == null)
            {
                _logger.LogWarning(LogEvent.ConfirmationPageFailed, $"Unable to read RegistrationConfirmationViewModel from temp data in add registration confirmation page. Ukprn: {User.GetUkPrn()}, User: {User.GetUserEmail()}");
                return RedirectToRoute(RouteConstants.PageNotFound);
            }
            return View(viewModel);
        }

        [HttpGet]
        [Route("search-for-registration-registration-details/{profileId}", Name = RouteConstants.RegistrationDetails)]
        public async Task<IActionResult> RegistrationDetailsAsync(int profileId)
        {
            var viewModel = await _registrationLoader.GetRegistrationDetailsAsync(User.GetUkPrn(), profileId);

            if (viewModel == null)
            {
                _logger.LogWarning(LogEvent.NoDataFound, $"No registration details found. Method: GetRegistrationDetailsAsync({User.GetUkPrn()}, {profileId}), User: {User.GetUserEmail()}");
                return RedirectToRoute(RouteConstants.PageNotFound);
            }

            viewModel.AcademicYears = await _registrationLoader.GetAcademicYearsAsync();
            return View(viewModel);
        }

        [HttpGet]
        [Route("delete-registration/{profileId}", Name = RouteConstants.DeleteRegistration)]
        public async Task<IActionResult> DeleteRegistrationAsync(int profileId)
        {
            var registrationDetails = await _registrationLoader.GetRegistrationAssessmentAsync(User.GetUkPrn(), profileId, RegistrationPathwayStatus.Active);
            if (registrationDetails == null || registrationDetails.AnyComponentResultExist || registrationDetails.IsIndustryPlacementExist)
                return RedirectToRoute(RouteConstants.PageNotFound);

            var viewModel = new DeleteRegistrationViewModel { ProfileId = registrationDetails.ProfileId, Uln = registrationDetails.Uln };
            return View(viewModel);
        }

        [HttpPost]
        [Route("delete-registration", Name = RouteConstants.SubmitDeleteRegistration)]
        public async Task<IActionResult> DeleteRegistrationAsync(DeleteRegistrationViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            if (!viewModel.DeleteRegistration.Value)
                return RedirectToRoute(RouteConstants.RegistrationDetails, new { profileId = viewModel.ProfileId });

            var isSuccess = await _registrationLoader.DeleteRegistrationAsync(User.GetUkPrn(), viewModel.ProfileId);

            if (isSuccess)
            {
                await _cacheService.SetAsync(CacheKey, new RegistrationCancelledConfirmationViewModel { Uln = viewModel.Uln }, CacheExpiryTime.XSmall);
                return RedirectToRoute(RouteConstants.RegistrationCancelledConfirmation);
            }
            else
            {
                _logger.LogWarning(LogEvent.RegistrationNotDeleted, $"Unable to delete registration. Method: DeleteRegistrationAsync(Ukprn: {User.GetUkPrn()}, id: {viewModel.ProfileId}), User: {User.GetUserEmail()}");
                return RedirectToRoute(RouteConstants.Error, new { StatusCode = 500 });
            }
        }

        [HttpGet]
        [Route("registration-deleted-confirmation", Name = RouteConstants.RegistrationCancelledConfirmation)]
        public async Task<IActionResult> RegistrationCancelledConfirmationAsync()
        {
            var viewModel = await _cacheService.GetAndRemoveAsync<RegistrationCancelledConfirmationViewModel>(CacheKey);

            if (viewModel == null)
            {
                _logger.LogWarning(LogEvent.ConfirmationPageFailed,
                    $"Unable to read cancel registration confirmation viewmodel from cache. Ukprn: {User.GetUkPrn()}, User: {User.GetUserEmail()}");
                return RedirectToRoute(RouteConstants.PageNotFound);
            }
            return View(viewModel);
        }

        [HttpGet]
        [Route("registrations-generating-download", Name = RouteConstants.RegistrationsGeneratingDownload)]
        public IActionResult RegistrationsGeneratingDownload()
        {
            return View();
        }

        [HttpPost]
        [Route("registrations-generating-download", Name = RouteConstants.SubmitRegistrationsGeneratingDownload)]
        public async Task<IActionResult> SubmitRegistrationsGeneratingDownloadAsync()
        {
            long ukprn = User.GetUkPrn();
            string email = User.GetUserEmail();

            Task<IList<DataExportResponse>> registrationsResponseTask = _registrationLoader.GenerateRegistrationsExportAsync(ukprn, email);
            Task<IList<DataExportResponse>> pendingWithdrawalsResponseTask = _registrationLoader.GeneratePendingWithdrawalsExportAsync(ukprn, email);

            await Task.WhenAll(registrationsResponseTask, pendingWithdrawalsResponseTask);

            IList<DataExportResponse> registrationsResponse = registrationsResponseTask.Result;
            IList<DataExportResponse> pendingWithdrawalsResponse = pendingWithdrawalsResponseTask.Result;

            if (!registrationsResponse.ContainsSingle() || !pendingWithdrawalsResponse.ContainsSingle())
                return RedirectToRoute(RouteConstants.ProblemWithService);

            if (!registrationsResponse.Any(x => x.IsDataFound))
            {
                _logger.LogWarning(LogEvent.NoDataFound,
                    $"There are no registrations found for the Data export. Method: GenerateRegistrationsExportAsync({ukprn}, {email})");

                return RedirectToRoute(RouteConstants.RegistrationsNoRecordsFound);
            }

            var registrationsDownloadViewModel = new RegistrationsDownloadViewModel
            {
                RegistrationsDownloadLinkViewModel = CreateDownloadLink(registrationsResponse.First()),
                PendingWithdrawalsDownloadLinkViewModel = CreateDownloadLink(pendingWithdrawalsResponse.First())
            };

            await _cacheService.SetAsync(CacheKey, registrationsDownloadViewModel, CacheExpiryTime.XSmall);
            return RedirectToRoute(RouteConstants.RegistrationsDownloadData);

            static DownloadLinkViewModel CreateDownloadLink(DataExportResponse response)
                => new()
                {
                    BlobUniqueReference = response.BlobUniqueReference,
                    FileSize = response.FileSize,
                    FileType = FileType.Csv.ToString().ToUpperInvariant()
                };
        }

        [HttpGet]
        [Route("registrations-no-records-found", Name = RouteConstants.RegistrationsNoRecordsFound)]
        public IActionResult RegistrationsNoRecordsFound()
        {
            return View(new RegistrationsNoRecordsFoundViewModel());
        }

        [HttpGet]
        [Route("registrations-download-data", Name = RouteConstants.RegistrationsDownloadData)]
        public async Task<IActionResult> RegistrationsDownloadDataAsync()
        {
            var cacheModel = await _cacheService.GetAndRemoveAsync<RegistrationsDownloadViewModel>(CacheKey);
            if (cacheModel == null)
            {
                _logger.LogWarning(LogEvent.NoDataFound, $"Unable to read DataExportResponse from redis cache in registrations download page. Ukprn: {User.GetUkPrn()}, User: {User.GetUserEmail()}");
                return RedirectToRoute(RouteConstants.PageNotFound);
            }

            return View(cacheModel);
        }

        [HttpGet]
        [Route("download-registrations-data/{id}", Name = RouteConstants.RegistrationsDownloadDataLink)]
        public Task<IActionResult> RegistrationsDownloadDataLinkAsync(string id)
        {
            return DownloadDataLinkAsync(
                id,
                () => _registrationLoader.GetRegistrationsDataFileAsync(User.GetUkPrn(), id.ToGuid()),
                RegistrationContent.RegistrationsDownloadData.Registrations_Data_Report_File_Name_Text,
                nameof(RegistrationsDownloadDataLinkAsync));
        }

        [HttpGet]
        [Route("download-pending-withdrawals-data/{id}", Name = RouteConstants.PendingWithdrawalsDownloadDataLink)]
        public Task<IActionResult> PendingWithdrawalsDownloadDataLinkAsync(string id)
        {
            return DownloadDataLinkAsync(
                id,
                () => _registrationLoader.GetPendingWithdrawalsDataFileAsync(User.GetUkPrn(), id.ToGuid()),
                RegistrationContent.RegistrationsDownloadData.Pending_Withdrawals_Data_Report_File_Name,
                nameof(PendingWithdrawalsDownloadDataLinkAsync));
        }

        [HttpGet]
        [Route("upload-withdrawals-file/{requestErrorTypeId:int?}", Name = RouteConstants.UploadWithdrawalsFile)]
        public IActionResult UploadWithdrawalsFile(int? requestErrorTypeId)
        {
            var model = new UploadWithdrawalsRequestViewModel { RequestErrorTypeId = requestErrorTypeId };
            model.SetAnyModelErrors(ModelState);
            return View(model);
        }

        [HttpPost]
        [Route("upload-withdrawls-file", Name = RouteConstants.SubmitUploadWithdrawalsFile)]
        public async Task<IActionResult> UploadWithdrawalsFileAsync(UploadWithdrawalsRequestViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            viewModel.AoUkprn = User.GetUkPrn();
            var response = await _registrationLoader.ProcessBulkWithdrawalsAsync(viewModel);

            if (response.IsSuccess)
            {
                var successfulViewModel = new UploadSuccessfulViewModel { Stats = response.Stats };
                await _cacheService.SetAsync(string.Concat(CacheKey, Constants.UploadSuccessfulViewModel), successfulViewModel, CacheExpiryTime.XSmall);

                return RedirectToRoute(RouteConstants.WithdrawalsUploadSuccessful);
            }
            else
            {
                if (response.ShowProblemWithServicePage)
                {
                    return RedirectToRoute(RouteConstants.ProblemWithWithdrawalsUpload);
                }
                else
                {
                    var unsuccessfulViewModel = new UploadUnsuccessfulViewModel { BlobUniqueReference = response.BlobUniqueReference, FileSize = response.ErrorFileSize, FileType = FileType.Csv.ToString().ToUpperInvariant() };
                    await _cacheService.SetAsync(string.Concat(CacheKey, Constants.UploadUnsuccessfulViewModel), unsuccessfulViewModel, CacheExpiryTime.XSmall);
                    return RedirectToRoute(RouteConstants.WithdrawalsUploadUnsuccessful);
                }
            }
        }

        [HttpGet]
        [Route("upload-withdrawals-file-success", Name = RouteConstants.WithdrawalsUploadSuccessful)]
        public async Task<IActionResult> UploadWithdrawalsSuccessful()
        {
            var viewModel = await _cacheService.GetAndRemoveAsync<UploadSuccessfulViewModel>(string.Concat(CacheKey, Constants.UploadSuccessfulViewModel));

            if (viewModel == null)
            {
                _logger.LogWarning(LogEvent.UploadSuccessfulPageFailed,
                    $"Unable to read upload successful withdrawal response from temp data. Ukprn: {User.GetUkPrn()}, User: {User.GetUserEmail()}");
                return RedirectToRoute(RouteConstants.PageNotFound);
            }
            return View(viewModel);
        }

        [HttpGet]
        [Route("withdrawals-upload-unsuccessful", Name = RouteConstants.WithdrawalsUploadUnsuccessful)]
        public async Task<IActionResult> UploadWithdrawalsUnsuccessful()
        {
            var viewModel = await _cacheService.GetAndRemoveAsync<UploadUnsuccessfulViewModel>(string.Concat(CacheKey, Constants.UploadUnsuccessfulViewModel));
            if (viewModel == null)
            {
                _logger.LogWarning(LogEvent.UploadUnsuccessfulPageFailed,
                    $"Unable to read upload unsuccessful withdrawal response from temp data. Ukprn: {User.GetUkPrn()}, User: {User.GetUserEmail()}");
                return RedirectToRoute(RouteConstants.PageNotFound);
            }
            return View(viewModel);
        }

        [HttpGet]
        [Route("download-withdrawl-errors", Name = RouteConstants.DownloadWithdrawalErrors)]
        public async Task<IActionResult> DownloadWithdrawlErrors(string id)
        {
            if (id.IsGuid())
            {
                var fileStream = await _registrationLoader.GetWithdrawalValidationErrorsFileAsync(User.GetUkPrn(), id.ToGuid());
                if (fileStream == null)
                {
                    _logger.LogWarning(LogEvent.FileStreamNotFound, $"No FileStream found to download withdrawl validation errors. Method: GetWithdrawalValidationErrorsFileAsync(AoUkprn: {User.GetUkPrn()}, BlobUniqueReference = {id})");
                    return RedirectToRoute(RouteConstants.PageNotFound);
                }

                fileStream.Position = 0;
                return new FileStreamResult(fileStream, "text/csv")
                {
                    FileDownloadName = RegistrationContent.UploadWithdrawalsUnsuccessful.Withdrawals_Error_Report_File_Name_Text
                };
            }
            else
            {
                _logger.LogWarning(LogEvent.DownloadRegistrationErrorsFailed, $"Not a valid guid to read file.Method: DownloadRegistrationErrors(Id = {id}), Ukprn: {User.GetUkPrn()}, User: {User.GetUserEmail()}");
                return RedirectToRoute(RouteConstants.Error, new { StatusCode = 500 });
            }
        }

        private async Task<IActionResult> DownloadDataLinkAsync(string id, Func<Task<Stream>> getDataFile, string fileDownloadName, string methodName)
        {
            if (!id.IsGuid())
            {
                _logger.LogWarning(LogEvent.DocumentDownloadFailed, $"Not a valid guid to read file.Method: {methodName}(Id = {id}), Ukprn: {User.GetUkPrn()}, User: {User.GetUserEmail()}");
                return RedirectToRoute(RouteConstants.Error, new { StatusCode = 500 });
            }

            var fileStream = await getDataFile();
            if (fileStream == null)
            {
                _logger.LogWarning(LogEvent.FileStreamNotFound, $"No FileStream found to download registration data. Method: {methodName}(AoUkprn: {User.GetUkPrn()}, BlobUniqueReference = {id})");
                return RedirectToRoute(RouteConstants.PageNotFound);
            }

            fileStream.Position = 0;
            return new FileStreamResult(fileStream, "text/csv")
            {
                FileDownloadName = fileDownloadName
            };
        }

        private async Task<SelectProviderViewModel> GetAoRegisteredProviders()
        {
            return await _registrationLoader.GetRegisteredTqAoProviderDetailsAsync(User.GetUkPrn());
        }

        private async Task<SelectCoreViewModel> GetRegisteredProviderCores(long providerUkprn)
        {
            return await _registrationLoader.GetRegisteredProviderPathwayDetailsAsync(User.GetUkPrn(), providerUkprn);
        }

        private async Task<PathwaySpecialismsViewModel> GetPathwaySpecialismsByCoreCode(string coreCode)
        {
            return await _registrationLoader.GetPathwaySpecialismsByPathwayLarIdAsync(User.GetUkPrn(), coreCode);
        }

        private async Task SyncCacheUln(UlnViewModel model)
        {
            var cacheModel = await _cacheService.GetAsync<RegistrationViewModel>(CacheKey);
            if (cacheModel?.Uln != null)
                cacheModel.Uln = model;
            else
                cacheModel = new RegistrationViewModel { Uln = model };

            await _cacheService.SetAsync(CacheKey, cacheModel);
        }

        private bool IsValidDateofBirth(DateofBirthViewModel model)
        {
            var dateofBirth = string.Concat(model.Day, "/", model.Month, "/", model.Year);
            var validationerrors = dateofBirth.ValidateDate("Date of birth");

            if (validationerrors?.Count == 0)
                return true;

            foreach (var error in validationerrors)
                ModelState.AddModelError(error.Key, error.Value);

            return false;
        }
    }
}