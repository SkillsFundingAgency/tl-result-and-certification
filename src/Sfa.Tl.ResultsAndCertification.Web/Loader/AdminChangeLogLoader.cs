﻿using AutoMapper;
using Microsoft.AspNetCore.WebUtilities;
using Sfa.Tl.ResultsAndCertification.Api.Client.Interfaces;
using Sfa.Tl.ResultsAndCertification.Common.Enum;
using Sfa.Tl.ResultsAndCertification.Models.Contracts.AdminChangeLog;
using Sfa.Tl.ResultsAndCertification.Models.Contracts.Common;
using Sfa.Tl.ResultsAndCertification.Web.Loader.Interfaces;
using Sfa.Tl.ResultsAndCertification.Web.ViewModel.AdminChangeLog;
using System.Linq;
using System.Threading.Tasks;

namespace Sfa.Tl.ResultsAndCertification.Web.Loader
{
    public class AdminChangeLogLoader : IAdminChangeLogLoader
    {
        private readonly IResultsAndCertificationInternalApiClient _internalApiClient;
        private readonly IIndustryPlacementLoader _industryPlacementLoader;
        private readonly IMapper _mapper;

        public AdminChangeLogLoader(
            IResultsAndCertificationInternalApiClient internalApiClient,
            IIndustryPlacementLoader industryPlacementLoader,
            IMapper mapper)
        {
            _internalApiClient = internalApiClient;
            _industryPlacementLoader = industryPlacementLoader;
            _mapper = mapper;
        }

        public async Task<AdminSearchChangeLogViewModel> SearchChangeLogsAsync(string searchKey = "", int? pageNumber = null)
        {
            AdminSearchChangeLogRequest request = new()
            {
                SearchKey = searchKey,
                PageNumber = pageNumber
            };

            PagedResponse<AdminSearchChangeLog> apiResponse = await _internalApiClient.SearchChangeLogsAsync(request);
            return _mapper.Map<AdminSearchChangeLogViewModel>(apiResponse, opt =>
            {
                opt.Items["searchKey"] = searchKey;
                opt.Items["pageNumber"] = pageNumber;
            });

        }

        public async Task<AdminViewChangeRecordViewModel> GetAdminViewChangeStartYearRecord(int changeLogId)
        {
            var changeLogRecord = await _internalApiClient.GetAdminChangeLogRecordAsync(changeLogId);
            return _mapper.Map<AdminViewChangeRecordStartYearViewModel>(changeLogRecord);
        }

        public async Task<AdminViewChangeRecordViewModel> GetAdminViewChangeIPRecord(int changeLogId)
        {
            var changeLogRecord = await _internalApiClient.GetAdminChangeLogRecordAsync(changeLogId);
            var reasons = await _industryPlacementLoader.GetIpLookupDataAsync(IpLookupType.SpecialConsideration);

            var mappedChangeLogRecord = _mapper.Map<AdminViewChangeRecordIndustryPlacementViewModel>(changeLogRecord);

            mappedChangeLogRecord.Reasons = reasons.Where(e =>
                                                mappedChangeLogRecord.ChangeIPDetails.SpecialConsiderationReasonsTo
                                                .Contains(e.Id))
                                                .Select(e => e.Name)
                                                .ToList();
            return mappedChangeLogRecord;
        }
    }
}