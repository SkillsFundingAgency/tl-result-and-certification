﻿using Microsoft.AspNetCore.Mvc;
using Sfa.Tl.ResultsAndCertification.Application.Interfaces;
using Sfa.Tl.ResultsAndCertification.Common.Enum;
using Sfa.Tl.ResultsAndCertification.InternalApi.Interfaces;
using Sfa.Tl.ResultsAndCertification.InternalApi.Loader.Interfaces;
using Sfa.Tl.ResultsAndCertification.Models.Contracts;
using Sfa.Tl.ResultsAndCertification.Models.Contracts.IndustryPlacement;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sfa.Tl.ResultsAndCertification.InternalApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IndustryPlacementController : ControllerBase, IIndustryPlacementController
    {        
        protected IIndustryPlacementService _industryPlacementService;
        private readonly IBulkIndustryPlacementLoader _bulkIndustryPlacementProcess;

        public IndustryPlacementController(IIndustryPlacementService industryPlacementService, IBulkIndustryPlacementLoader bulkIndustryPlacementProcess = null)
        {
            _industryPlacementService = industryPlacementService;
            _bulkIndustryPlacementProcess = bulkIndustryPlacementProcess;
        }

        [HttpPost]
        [Route("ProcessBulkIndustryPlacements")]
        public async Task<BulkIndustryPlacementResponse> ProcessBulkIndustryPlacementsAsync(BulkProcessRequest request)
        {
            return await _bulkIndustryPlacementProcess.ProcessAsync(request);
        }

        [HttpGet]
        [Route("GetIpLookupData/{ipLookupType}/{pathwayId:int?}")]
        public async Task<IList<IpLookupData>> GetIpLookupDataAsync(IpLookupType ipLookupType, int? pathwayId = null)
        {
            return await _industryPlacementService.GetIpLookupDataAsync(ipLookupType, pathwayId);
        }

        [HttpGet]
        [Route("GetTempFlexNavigation/{pathwayId}/{academicYear}")]
        public async Task<IpTempFlexNavigation> GetTempFlexNavigationAsync(int pathwayId, int academicYear)
        {
            return await _industryPlacementService.GetTempFlexNavigationAsync(pathwayId, academicYear);
        }

        [HttpPost]
        [Route("ProcessIndustryPlacementDetails")]
        public async Task<bool> ProcessIndustryPlacementDetailsAsync(IndustryPlacementRequest request)
        {
            return await _industryPlacementService.ProcessIndustryPlacementDetailsAsync(request);
        }
    }
}