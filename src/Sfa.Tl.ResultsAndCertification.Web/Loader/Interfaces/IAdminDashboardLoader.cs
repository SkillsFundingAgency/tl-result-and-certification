﻿using Sfa.Tl.ResultsAndCertification.Web.ViewModel.AdminDashboard;
using Sfa.Tl.ResultsAndCertification.Web.ViewModel.AdminDashboard.LearnerRecord;
using System.Threading.Tasks;

namespace Sfa.Tl.ResultsAndCertification.Web.Loader.Interfaces
{
    public interface IAdminDashboardLoader
    {
        Task<AdminSearchLearnerFiltersViewModel> GetAdminSearchLearnerFiltersAsync();

        Task<LearnerRecordViewModel> GetAdminLearnerRecordAsync<LearnerRecordViewModel>(int pathwayId);

        Task<AdminSearchLearnerDetailsListViewModel> GetAdminSearchLearnerDetailsListAsync(AdminSearchLearnerCriteriaViewModel adminSearchCriteria);
    }
}