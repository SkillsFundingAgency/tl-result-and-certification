﻿using Sfa.Tl.ResultsAndCertification.Application.Interfaces;
using Sfa.Tl.ResultsAndCertification.Common.Enum;
using Sfa.Tl.ResultsAndCertification.Common.Extensions;
using Sfa.Tl.ResultsAndCertification.Common.Services.BlobStorage.Interface;
using Sfa.Tl.ResultsAndCertification.Common.Services.Mapper;
using Sfa.Tl.ResultsAndCertification.InternalApi.Loader.Interfaces;
using Sfa.Tl.ResultsAndCertification.Models.BlobStorage;
using Sfa.Tl.ResultsAndCertification.Models.Contracts.DataExport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sfa.Tl.ResultsAndCertification.InternalApi.Loader
{
    public class DataExportLoader : IDataExportLoader
    {
        private readonly IDataExportService _dataExportService;
        private readonly IBlobStorageService _blobStorageService;

        public DataExportLoader(IDataExportService dataExportService, IBlobStorageService blobStorageService)
        {
            _dataExportService = dataExportService;
            _blobStorageService = blobStorageService;
        }

        public async Task<IList<DataExportResponse>> ProcessDataExportAsync(long aoUkprn, DataExportType requestType, string requestedBy)
        {
            return requestType switch
            {
                DataExportType.Registrations => await ProcessRegistrationsRequestAsync(aoUkprn, requestType, requestedBy),
                DataExportType.Assessments => await ProcessAssessmentsRequestAsync(aoUkprn, requestType, requestedBy),
                DataExportType.Results => null,
                _ => null,
            };
        }

        private async Task<IList<DataExportResponse>> ProcessRegistrationsRequestAsync(long aoUkprn, DataExportType requestType, string requestedBy)
        {
            var registrations = await _dataExportService.GetDataExportRegistrationsAsync(aoUkprn);
            var exportResponse = await ProcessDataExportResponseAsync(registrations, aoUkprn, requestType, requestedBy, typeof(RegistrationsExportMap));
            return new List<DataExportResponse>() { exportResponse };
        }

        private async Task<IList<DataExportResponse>> ProcessAssessmentsRequestAsync(long aoUkprn, DataExportType requestType, string requestedBy)
        {
            // Core Assessments
            var coreAssessments = await _dataExportService.GetDataExportCoreAssessmentsAsync(aoUkprn);

            // Specialism Assessments
            var specialismAssessments = await _dataExportService.GetDataExportSpecialismAssessmentsAsync(aoUkprn);

            var response = new List<DataExportResponse>();

            var coreAssessmentsResponse = await ProcessDataExportResponseAsync(coreAssessments, aoUkprn, requestType, requestedBy);
            coreAssessmentsResponse.ComponentType = ComponentType.Core;
            response.Add(coreAssessmentsResponse);

            var specialismAssessmentsResponse = await ProcessDataExportResponseAsync(specialismAssessments, aoUkprn, requestType, requestedBy);
            specialismAssessmentsResponse.ComponentType = ComponentType.Specialism;
            response.Add(specialismAssessmentsResponse);

            return response;
        }

        private async Task<DataExportResponse> ProcessDataExportResponseAsync<T>(IList<T> data, long aoUkprn, DataExportType requestType, string requestedBy, Type classMapType = null)
        {
            if (data == null || !data.Any())
                return new DataExportResponse { IsDataFound = false };

            var byteData = await CsvExtensions.WriteFileAsync(data, classMapType: classMapType);
            var response = await WriteCsvToBlobAsync(aoUkprn, requestType, requestedBy, byteData);
            return response;
        }

        private async Task<DataExportResponse> WriteCsvToBlobAsync(long aoUkprn, DataExportType requestType, string requestedBy, byte[] byteData)
        {
            // 3. Save to Blob
            var blobUniqueReference = Guid.NewGuid();
            await _blobStorageService.UploadFromByteArrayAsync(new BlobStorageData
            {
                ContainerName = DocumentType.DataExports.ToString(),
                SourceFilePath = $"{aoUkprn}/{requestType}",
                BlobFileName = $"{blobUniqueReference}.{FileType.Csv}",
                FileData = byteData,
                UserName = requestedBy
            });

            // 4. Return response
            return new DataExportResponse
            {
                FileSize = Math.Round((byteData.Length / 1024D), 2),
                BlobUniqueReference = blobUniqueReference,
                IsDataFound = true
            };
        }
    }
}