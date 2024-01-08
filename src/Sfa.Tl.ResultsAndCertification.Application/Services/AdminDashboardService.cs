﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Sfa.Tl.ResultsAndCertification.Application.Interfaces;
using Sfa.Tl.ResultsAndCertification.Common.Enum;
using Sfa.Tl.ResultsAndCertification.Common.Helpers;
using Sfa.Tl.ResultsAndCertification.Common.Services.System.Interface;
using Sfa.Tl.ResultsAndCertification.Data.Interfaces;
using Sfa.Tl.ResultsAndCertification.Data.Repositories;
using Sfa.Tl.ResultsAndCertification.Domain.Models;
using Sfa.Tl.ResultsAndCertification.Models.Contracts.AdminDashboard;
using Sfa.Tl.ResultsAndCertification.Models.Contracts.Common;
using Sfa.Tl.ResultsAndCertification.Models.Contracts.IndustryPlacement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sfa.Tl.ResultsAndCertification.Application.Services
{
    public class AdminDashboardService : IAdminDashboardService
    {
        private readonly IAdminDashboardRepository _adminDashboardRepository;
        private readonly ISystemProvider _systemProvider;
        private readonly IMapper _mapper;
        private readonly IRepository<TqRegistrationPathway> _tqRegistrationPathwayRepository;
        private readonly ILogger _logger;
        private readonly IRepository<ChangeLog> _changeLog;

        public AdminDashboardService(IAdminDashboardRepository adminDashboardRepository,
            ISystemProvider systemProvider,
            IMapper mapper,
            IRepository<TqRegistrationPathway> tqRegistrationPathwayRepository,
            ILogger logger,
            IRepository<ChangeLog> changeLog)
        {
            _adminDashboardRepository = adminDashboardRepository;
            _systemProvider = systemProvider;
            _mapper = mapper;
            _tqRegistrationPathwayRepository = tqRegistrationPathwayRepository;
            _logger = logger;
            _changeLog = changeLog;
        }

        public async Task<AdminSearchLearnerFilters> GetAdminSearchLearnerFiltersAsync()
        {
            return new AdminSearchLearnerFilters
            {
                AwardingOrganisations = await _adminDashboardRepository.GetAwardingOrganisationFiltersAsync(),
                AcademicYears = await _adminDashboardRepository.GetAcademicYearFiltersAsync(_systemProvider.UtcToday)
            };
        }

        public Task<PagedResponse<AdminSearchLearnerDetail>> GetAdminSearchLearnerDetailsAsync(AdminSearchLearnerRequest request)
        {
            return _adminDashboardRepository.SearchLearnerDetailsAsync(request);
        }

        public async Task<AdminLearnerRecord> GetAdminLearnerRecordAsync(int pathwayId)
        {
            var _academicYearToBe = new List<int>();

            var result = await _adminDashboardRepository.GetAdminLearnerRecordAsync(pathwayId);
            var _adminLearnerRecord = _mapper.Map<AdminLearnerRecord>(result);

            for (int i = result.AcademicYear - 1, j = 1; i >= result.TlevelStartYear && j <= 2; i--, j++)
                _academicYearToBe.Add(i);

            _adminLearnerRecord.AcademicStartYearsToBe = _academicYearToBe;

            return _adminLearnerRecord;
        }

        public async Task<bool> ProcessChangeStartYearAsync(ReviewChangeStartYearRequest request)
        {
            var pathway = await _tqRegistrationPathwayRepository.GetManyAsync(p => p.Id == request.PathwayId                                                                              
                                                                              && (p.Status == RegistrationPathwayStatus.Active))
                                                                 .OrderByDescending(p => p.CreatedOn)
                                                                 .FirstOrDefaultAsync();

            if (pathway == null)
            {
                _logger.LogWarning(LogEvent.NoDataFound, $"No record found to update tqregistrationpathwayid for pathwayid = {request.PathwayId}. Method: ProcessChangeStartYearAsync({request})");
                return false;
            }

            pathway.AcademicYear = request.AcademicYear;
        
            var  status = await _tqRegistrationPathwayRepository.UpdateWithSpecifedColumnsOnlyAsync(pathway, u => u.AcademicYear, u => u.ModifiedBy, u => u.ModifiedOn);

            var changeLog = new ChangeLog();

            if (status > 0) await _changeLog.CreateAsync(changeLog);
            
            return status > 0;
        }



    }

}