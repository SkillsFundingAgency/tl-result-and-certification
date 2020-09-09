﻿using AutoMapper;
using Microsoft.Extensions.Logging;
using Sfa.Tl.ResultsAndCertification.Api.Client.Interfaces;
using Sfa.Tl.ResultsAndCertification.Common.Enum;
using Sfa.Tl.ResultsAndCertification.Common.Extensions;
using Sfa.Tl.ResultsAndCertification.Common.Helpers;
using Sfa.Tl.ResultsAndCertification.Common.Services.BlobStorage.Interface;
using Sfa.Tl.ResultsAndCertification.Models.BlobStorage;
using Sfa.Tl.ResultsAndCertification.Models.Contracts;
using Sfa.Tl.ResultsAndCertification.Web.Loader.Interfaces;
using Sfa.Tl.ResultsAndCertification.Web.ViewModel;
using Sfa.Tl.ResultsAndCertification.Web.ViewModel.Registration;
using Sfa.Tl.ResultsAndCertification.Web.ViewModel.Registration.Manual;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Sfa.Tl.ResultsAndCertification.Web.Loader
{
    public class RegistrationLoader : IRegistrationLoader
    {
        private readonly IResultsAndCertificationInternalApiClient _internalApiClient;
        private readonly IBlobStorageService _blobStorageService;
        private readonly IMapper _mapper;
        private readonly ILogger<RegistrationLoader> _logger;

        public RegistrationLoader(IMapper mapper, ILogger<RegistrationLoader> logger, IResultsAndCertificationInternalApiClient internalApiClient, IBlobStorageService blobStorageService)
        {
            _mapper = mapper;
            _logger = logger;
            _internalApiClient = internalApiClient;
            _blobStorageService = blobStorageService;
        }

        public async Task<UploadRegistrationsResponseViewModel> ProcessBulkRegistrationsAsync(UploadRegistrationsRequestViewModel viewModel)
        {            
            var bulkRegistrationRequest = _mapper.Map<BulkRegistrationRequest>(viewModel);

            using (var fileStream = viewModel.File.OpenReadStream())
            {
                await _blobStorageService.UploadFileAsync(new BlobStorageData
                {
                    ContainerName = bulkRegistrationRequest.DocumentType.ToString(),
                    BlobFileName = bulkRegistrationRequest.BlobFileName,
                    SourceFilePath = $"{bulkRegistrationRequest.AoUkprn}/{BulkRegistrationProcessStatus.Processing}",
                    FileStream = fileStream,
                    UserName = bulkRegistrationRequest.PerformedBy
                });
            }

            var bulkRegistrationResponse = await _internalApiClient.ProcessBulkRegistrationsAsync(bulkRegistrationRequest);
            return _mapper.Map<UploadRegistrationsResponseViewModel>(bulkRegistrationResponse);
        }

        public async Task<Stream> GetRegistrationValidationErrorsFileAsync(long aoUkprn, Guid blobUniqueReference)
        {
            var tlevelDetails = await _internalApiClient.GetDocumentUploadHistoryDetailsAsync(aoUkprn, blobUniqueReference);

            if (tlevelDetails != null && tlevelDetails.Status == (int)DocumentUploadStatus.Failed)
            {
                var fileStream = await _blobStorageService.DownloadFileAsync(new BlobStorageData
                {
                    ContainerName = DocumentType.Registrations.ToString(),
                    BlobFileName = tlevelDetails.BlobFileName,
                    SourceFilePath = $"{aoUkprn}/{BulkRegistrationProcessStatus.ValidationErrors}"
                });

                if(fileStream == null)
                {
                    var blobReadError = $"No FileStream found to download registration validation errors. Method: DownloadFileAsync(ContainerName: {DocumentType.Registrations}, BlobFileName = {tlevelDetails.BlobFileName}, SourceFilePath = {aoUkprn}/{BulkRegistrationProcessStatus.ValidationErrors})";
                    _logger.LogWarning(LogEvent.FileStreamNotFound, blobReadError);
                }
                return fileStream;
            }
            else
            {
                _logger.LogWarning(LogEvent.NoDataFound, $"No DocumentUploadHistoryDetails found or the request is not valid. Method: GetDocumentUploadHistoryDetailsAsync(AoUkprn: {aoUkprn}, BlobUniqueReference = {blobUniqueReference})");
                return null;
            }            
        }

        public async Task<SelectProviderViewModel> GetRegisteredTqAoProviderDetailsAsync(long aoUkprn)
        {
            var providerDetails = await _internalApiClient.GetTqAoProviderDetailsAsync(aoUkprn);
            return _mapper.Map<SelectProviderViewModel>(providerDetails);
        }

        public async Task<SelectCoreViewModel> GetRegisteredProviderPathwayDetailsAsync(long aoUkprn, long providerUkprn)
        {
            var providerPathways = await _internalApiClient.GetRegisteredProviderPathwayDetailsAsync(aoUkprn, providerUkprn);
            return _mapper.Map<SelectCoreViewModel>(providerPathways);
        }

        public async Task<PathwaySpecialismsViewModel> GetPathwaySpecialismsByPathwayLarIdAsync(long aoUkprn, string pathwayLarId)
        {
            var pathwaySpecialisms = await _internalApiClient.GetPathwaySpecialismsByPathwayLarIdAsync(aoUkprn, pathwayLarId);
            return _mapper.Map<PathwaySpecialismsViewModel>(pathwaySpecialisms);
        }

        public async Task<UlnNotFoundViewModel> FindUlnAsync(long aoUkprn, long Uln)
        {
            var response = await _internalApiClient.FindUlnAsync(aoUkprn, Uln);
            return _mapper.Map<UlnNotFoundViewModel>(response);
        }

        public async Task<bool> AddRegistrationAsync(long aoUkprn, RegistrationViewModel model)
        {
            var registrationModel = _mapper.Map<RegistrationRequest>(model, opt => opt.Items["aoUkprn"] = aoUkprn);
            return await _internalApiClient.AddRegistrationAsync(registrationModel);
        }

        public async Task<RegistrationDetailsViewModel> GetRegistrationDetailsByProfileIdAsync(long aoUkprn, int profileId)
        {
            var response = await _internalApiClient.GetRegistrationDetailsByProfileIdAsync(aoUkprn, profileId);
            return _mapper.Map<RegistrationDetailsViewModel>(response);
        }

        public async Task<bool> DeleteRegistrationAsync(long aoUkprn, int profileId)
        {
            return await _internalApiClient.DeleteRegistrationAsync(aoUkprn, profileId);
        }

        public async Task<T> GetRegistrationProfileAsync<T>(long aoUkprn, int profileId)
        {
            var response = await _internalApiClient.GetRegistrationAsync(aoUkprn, profileId);
            return _mapper.Map<T>(response);
        }

        public async Task<ProviderChangeResponse> ProcessProviderChangesAsync(long aoUkprn, ChangeProviderViewModel viewModel)
        {
            var reg = await _internalApiClient.GetRegistrationAsync(aoUkprn, viewModel.ProfileId);

            if (reg == null) return null;

            if (reg.ProviderUkprn == viewModel.SelectedProviderUkprn.ToLong())
            {
                return new ProviderChangeResponse { IsModified = false };
            }
            else
            {
                var providerPathways = await _internalApiClient.GetRegisteredProviderPathwayDetailsAsync(aoUkprn, viewModel.SelectedProviderUkprn.ToLong());
                if (providerPathways != null && providerPathways.Count > 0 && providerPathways.Any(p => p.Code.Equals(reg.CoreCode)))
                {
                    _mapper.Map(viewModel, reg);
                    var isSuccess = await _internalApiClient.UpdateRegistrationAsync(reg);
                    return new ProviderChangeResponse { ProfileId = reg.ProfileId, Uln = reg.Uln, IsModified = true,  IsSuccess = isSuccess };
                }
                else
                {
                    return new ProviderChangeResponse { IsModified = true, IsCoreNotSupported = true };
                }
            }
        }

        public async Task<ManageRegistrationResponse> ProcessProfileNameChangeAsync(long aoUkprn, ChangeLearnersNameViewModel viewModel)
        {
            var reg = await _internalApiClient.GetRegistrationAsync(aoUkprn, viewModel.ProfileId);
            
            if (reg == null) 
                return null;
            
            if (viewModel.Firstname.Trim().Equals(reg.FirstName, StringComparison.InvariantCultureIgnoreCase) &&
                viewModel.Lastname.Trim().Equals(reg.LastName, StringComparison.InvariantCultureIgnoreCase))
                return new ManageRegistrationResponse { IsModified = false };

            _mapper.Map(viewModel, reg);
            var isSuccess = await _internalApiClient.UpdateRegistrationAsync(reg);

            return new ManageRegistrationResponse { ProfileId = reg.ProfileId, Uln = reg.Uln, IsModified = true, IsSuccess = isSuccess };
        }

        public async Task<ManageRegistrationResponse> ProcessSpecialismQuestionChangeAsync(long aoUkprn, ChangeSpecialismQuestionViewModel viewModel)
        {
            var reg = await _internalApiClient.GetRegistrationAsync(aoUkprn, viewModel.ProfileId);

            if (reg == null || viewModel.HasLearnerDecidedSpecialism == null) return null;

            if (viewModel.HasLearnerDecidedSpecialism.Value)
            {
                return new ManageRegistrationResponse { IsModified = false };
            }
            else
            {
                _mapper.Map(viewModel, reg);
                var isSuccess = await _internalApiClient.UpdateRegistrationAsync(reg);
                return new ManageRegistrationResponse { ProfileId = reg.ProfileId, Uln = reg.Uln, IsModified = true, IsSuccess = isSuccess };
            }
        }
    }
}
