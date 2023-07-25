﻿using Microsoft.Extensions.Logging;
using Sfa.Tl.ResultsAndCertification.Api.Client.Interfaces;
using Sfa.Tl.ResultsAndCertification.Application.Interfaces;
using Sfa.Tl.ResultsAndCertification.Common.Enum;
using Sfa.Tl.ResultsAndCertification.Common.Extensions;
using Sfa.Tl.ResultsAndCertification.Common.Helpers;
using Sfa.Tl.ResultsAndCertification.Common.Services.BlobStorage.Interface;
using Sfa.Tl.ResultsAndCertification.Common.Services.Mapper;
using Sfa.Tl.ResultsAndCertification.Functions.Helpers;
using Sfa.Tl.ResultsAndCertification.Functions.Interfaces;
using Sfa.Tl.ResultsAndCertification.Models.BlobStorage;
using Sfa.Tl.ResultsAndCertification.Models.Contracts.Ucas;
using Sfa.Tl.ResultsAndCertification.Models.Functions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Sfa.Tl.ResultsAndCertification.Functions.Services
{
    public class UcasDataTransferService : IUcasDataTransferService
    {
        private readonly IUcasDataService _ucasDataService;
        private readonly IBlobStorageService _blobStorageService;
        private readonly IUcasApiClient _ucasApiClient;
        private readonly ILogger _logger;

        public UcasDataTransferService(IUcasDataService ucasDataService,
            IBlobStorageService blobStorageService,
            IUcasApiClient ucasApiClient, ILogger<IUcasDataTransferService> logger)
        {
            _ucasDataService = ucasDataService;
            _blobStorageService = blobStorageService;
            _ucasApiClient = ucasApiClient;
            _logger = logger;
        }

        public async Task<UcasDataTransferResponse> ProcessUcasDataRecordsAsync(UcasDataType ucasDataType)
        {
            // 1. Get Entries data
            var ucasData = await _ucasDataService.ProcessUcasDataRecordsAsync(ucasDataType);
            if (ucasData == null || !ucasData.UcasDataRecords.Any())
            {
                var message = $"No entries are found. Method: GetUcasEntriesAsync()";
                _logger.LogWarning(LogEvent.NoDataFound, message);
                return new UcasDataTransferResponse { IsSuccess = true, Message = message };
            }

            // 2. Write to the file (in byte format)
            var ucasDataRecords = new List<dynamic> { ucasData.Header };
            ucasDataRecords.AddRange(ucasData.UcasDataRecords);
            ucasDataRecords.Add(ucasData.Trailer);
            
            var byteData = await CsvExtensions.WriteFileAsync(ucasDataRecords, delimeter: Constants.PipeSeperator, writeHeader: false, typeof(CsvMapper));
            
            if (byteData.Length <= 0)
            {
                var message = $"No byte data available to send Ucas. Method: Csv WriteFileAsync()";
                throw new ApplicationException(message);
            }            

            // 3. Send data to Ucas using ApiClient
            var filename = $"{Guid.NewGuid()}.{Constants.FileExtensionTxt}";

            //Todo: Not sending the file to UCas API 

            //var fileHash = CommonHelper.ComputeSha256Hash(byteData);

            //var ucasFileId = await _ucasApiClient.SendDataAsync(new UcasDataRequest { FileName = filename, FileData = byteData, FileHash = fileHash });

            // 4. Write response to blob
            await _blobStorageService.UploadFromByteArrayAsync(new BlobStorageData
            {
                ContainerName = DocumentType.Ucas.ToString().ToLower(),
                SourceFilePath = ucasDataType.ToString().ToLower(),
                BlobFileName = $"{filename}-{DateTime.UtcNow.Date.ToString()}",
                FileData = byteData,
                UserName = Constants.FunctionPerformedBy
            });

            // 5. Update response
            return new UcasDataTransferResponse { IsSuccess = true };
        }
    }
}