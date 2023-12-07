﻿using Sfa.Tl.ResultsAndCertification.Models.Contracts.AdminDashboard;
using Sfa.Tl.ResultsAndCertification.Models.Contracts.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sfa.Tl.ResultsAndCertification.Data.Interfaces
{
    public interface IAdminDashboardRepository
    {
        Task<IList<FilterLookupData>> GetAwardingOrganisationFiltersAsync();
        Task<IList<FilterLookupData>> GetAcademicYearFiltersAsync(DateTime searchDate);
        Task<AdminLearnerRecord> GetAdminLearnerRecordAsync(int pathwayId);
        Task<PagedResponse<AdminSearchLearnerDetail>> SearchLearnerDetailsAsync(AdminSearchLearnerRequest request);
    }
}