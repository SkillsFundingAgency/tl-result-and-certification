﻿using Microsoft.Extensions.Logging;
using Sfa.Tl.ResultsAndCertification.Application.Interfaces;
using Sfa.Tl.ResultsAndCertification.Common.Constants;
using Sfa.Tl.ResultsAndCertification.Common.Enum;
using Sfa.Tl.ResultsAndCertification.Common.Helpers;
using Sfa.Tl.ResultsAndCertification.Common.Services.BlobStorage.Interface;
using Sfa.Tl.ResultsAndCertification.Common.Services.CsvHelper.Service.Interface;
using Sfa.Tl.ResultsAndCertification.InternalApi.Loader.Interfaces;
using Sfa.Tl.ResultsAndCertification.Models.BlobStorage;
using Sfa.Tl.ResultsAndCertification.Models.BulkProcess;
using Sfa.Tl.ResultsAndCertification.Models.Contracts;
using Sfa.Tl.ResultsAndCertification.Models.Registration.BulkProcess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sfa.Tl.ResultsAndCertification.InternalApi.Loader
{
    public class BulkRegistrationLoader : IBulkRegistrationLoader
    {
        private readonly ICsvHelperService<RegistrationCsvRecordRequest, CsvResponseModel<RegistrationCsvRecordResponse>, RegistrationCsvRecordResponse> _csvService;
        private readonly IRegistrationService _registrationService;
        private readonly IBlobStorageService _blobStorageService;
        private readonly IDocumentUploadHistoryService _documentUploadHistoryService;
        private readonly ILogger<BulkRegistrationLoader> _logger;

        public BulkRegistrationLoader(ICsvHelperService<RegistrationCsvRecordRequest,
            CsvResponseModel<RegistrationCsvRecordResponse>, RegistrationCsvRecordResponse> csvService,
            IRegistrationService registrationService, IBlobStorageService blobStorageService,
            IDocumentUploadHistoryService documentUploadHistoryService, ILogger<BulkRegistrationLoader> logger)
        {
            _csvService = csvService;
            _registrationService = registrationService;
            _blobStorageService = blobStorageService;
            _documentUploadHistoryService = documentUploadHistoryService;
            _logger = logger;
        }

        public async Task<BulkRegistrationResponse> ProcessBulkRegistrationsAsync(BulkRegistrationRequest request)
        {
            var response = new BulkRegistrationResponse();

            CsvResponseModel<RegistrationCsvRecordResponse> csvResponse = null;

            // Step: 1 Read file from Blob
            using (var fileStream = await _blobStorageService.DownloadFileAsync(new BlobStorageData
            {
                ContainerName = request.DocumentType.ToString(),
                BlobFileName = request.BlobFileName,
                SourceFilePath = $"{request.AoUkprn}/{BulkRegistrationProcessStatus.Processing}",
                UserName = request.PerformedBy
            }))
            {
                if (fileStream == null)
                {
                    var blobReadError = $"No FileStream found to process bluk registrations. Method: DownloadFileAsync(ContainerName: {request.DocumentType}, BlobFileName = {request.BlobFileName}, SourceFilePath = {request.AoUkprn}/{BulkRegistrationProcessStatus.Processing}, UserName = {request.PerformedBy}), User: {request.PerformedBy}";
                    _logger.LogInformation(LogEvent.FileStreamNotFound, blobReadError);
                    throw new Exception(blobReadError);
                }

                // Stage 2 validation
                csvResponse = await _csvService.ReadAndParseFileAsync(new RegistrationCsvRecordRequest { FileStream = fileStream });

                if (!csvResponse.IsDirty)
                    CheckUlnDuplicates(csvResponse.Rows);
            }

            if (csvResponse.IsDirty || csvResponse.Rows.Any(x => !x.IsValid))
                return await SaveErrorsAndUpdateResponse(request, response, csvResponse);

            // Stage 3 valiation. 
            var registrationRecordStage3Response = await _registrationService.ValidateRegistrationTlevelsAsync(request.AoUkprn, csvResponse.Rows.Where(x => x.IsValid).ToList());
            if (csvResponse.Rows.Any(x => !x.IsValid))
                return await SaveErrorsAndUpdateResponse(request, response, csvResponse);

            // Step: Map data to DB model type.
            var tqRegistrations = _registrationService.TransformRegistrationModel();

            // Step: Process DB operation
            var result = await _registrationService.CompareAndProcessRegistrations();

            response.IsSuccess = true;
            return response;
        }

        private async Task<BulkRegistrationResponse> SaveErrorsAndUpdateResponse(BulkRegistrationRequest request, BulkRegistrationResponse response, CsvResponseModel<RegistrationCsvRecordResponse> csvResponse)
        {
            var errorFile = await CreateErrorFileAsync(csvResponse);
            await UploadErrorsFileToBlobStorage(request, errorFile);
            await MoveFileFromProcessingToFailedAsync(request);
            await CreateDocumentUploadHistory(request, DocumentUploadStatus.Failed);

            response.IsSuccess = false;
            response.BlobUniqueReference = request.BlobUniqueReference;
            response.ErrorFileSize = Math.Round((errorFile.Length / 1024D), 2);

            return response;
        }

        private static void CheckUlnDuplicates(IList<RegistrationCsvRecordResponse> registrations)
        {
            // check if this expression simplified?
            var duplicateRegistrations = registrations
                .Where(x => x.Uln != 0)
                .GroupBy(x => x.Uln)
                .Where(g => g.Count() > 1)
                .Select(y => y)
                .ToList();

            duplicateRegistrations.ForEach(x =>
            {
                x.ToList().ForEach(s => s.ValidationErrors.Add(new RegistrationValidationError
                {
                    RowNum = s.RowNum.ToString(),
                    Uln = s.Uln != 0 ? s.Uln.ToString() : string.Empty,
                    ErrorMessage = ValidationMessages.DuplicateRecord
                }));
            });
        }

        private async Task<byte[]> CreateErrorFileAsync(CsvResponseModel<RegistrationCsvRecordResponse> csvResponse)
        {
            var validationErrors = ExtractAllValidationErrors(csvResponse);
            var errorFile = await _csvService.WriteFileAsync(validationErrors);
            return errorFile;
        }

        private List<RegistrationValidationError> ExtractAllValidationErrors(CsvResponseModel<RegistrationCsvRecordResponse> csvResponse)
        {
            if (csvResponse.IsDirty)
                return new List<RegistrationValidationError> { new RegistrationValidationError { ErrorMessage = csvResponse.ErrorMessage } };

            var errors = new List<RegistrationValidationError>();
            var invalidReg = csvResponse.Rows?.Where(x => !x.IsValid).ToList();
            invalidReg.ForEach(x => { errors.AddRange(x.ValidationErrors); });

            return errors;
        }

        private async Task<bool> CreateDocumentUploadHistory(BulkRegistrationRequest request, DocumentUploadStatus status = DocumentUploadStatus.Processed)
        {
            if (request == null) return false;

            var model = new DocumentUploadHistoryDetails
            {
                AoUkprn = request.AoUkprn,
                BlobFileName = request.BlobFileName,
                BlobUniqueReference = request.BlobUniqueReference,
                DocumentType = (int)request.DocumentType,
                FileType = (int)request.FileType,
                Status = (int)status,
                CreatedBy = request.PerformedBy
            };
            return await _documentUploadHistoryService.CreateDocumentUploadHistory(model);
        }

        private async Task<bool> UploadErrorsFileToBlobStorage(BulkRegistrationRequest request, byte[] errorFile)
        {
            if (errorFile == null || errorFile.Length == 0) return false;
            await _blobStorageService.UploadFromByteArrayAsync(new BlobStorageData
            {
                ContainerName = request.DocumentType.ToString(),
                SourceFilePath = $"{request.AoUkprn}/{BulkRegistrationProcessStatus.ValidationErrors}",
                BlobFileName = request.BlobFileName,
                UserName = request.PerformedBy,
                FileData = errorFile
            });
            return true;
        }

        private async Task<bool> MoveFileFromProcessingToProcessedAsync(BulkRegistrationRequest request)
        {
            if (request == null) return false;

            await _blobStorageService.MoveFileAsync(new BlobStorageData
            {
                ContainerName = request.DocumentType.ToString(),
                BlobFileName = request.BlobFileName,
                SourceFilePath = $"{request.AoUkprn}/{BulkRegistrationProcessStatus.Processing}",
                DestinationFilePath = $"{request.AoUkprn}/{BulkRegistrationProcessStatus.Processed}"
            });
            return true;
        }

        private async Task<bool> MoveFileFromProcessingToFailedAsync(BulkRegistrationRequest request)
        {
            if (request == null) return false;

            await _blobStorageService.MoveFileAsync(new BlobStorageData
            {
                ContainerName = request.DocumentType.ToString(),
                SourceFilePath = $"{request.AoUkprn}/{BulkRegistrationProcessStatus.Processing}",
                BlobFileName = request.BlobFileName,
                DestinationFilePath = $"{request.AoUkprn}/{BulkRegistrationProcessStatus.Failed}"
            });
            return true;
        }
    }
}
