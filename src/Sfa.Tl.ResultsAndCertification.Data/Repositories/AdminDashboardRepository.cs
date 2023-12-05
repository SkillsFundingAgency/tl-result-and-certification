﻿using Microsoft.EntityFrameworkCore;
using Sfa.Tl.ResultsAndCertification.Common.Extensions;
using Sfa.Tl.ResultsAndCertification.Data.Interfaces;
using Sfa.Tl.ResultsAndCertification.Domain.Models;
using Sfa.Tl.ResultsAndCertification.Models.Contracts.AdminDashboard;
using Sfa.Tl.ResultsAndCertification.Models.Contracts.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Sfa.Tl.ResultsAndCertification.Data.Repositories
{
    public class AdminDashboardRepository : IAdminDashboardRepository
    {
        private readonly ResultsAndCertificationDbContext _dbContext;

        public AdminDashboardRepository(ResultsAndCertificationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IList<FilterLookupData>> GetAwardingOrganisationFiltersAsync()
        {
            return await _dbContext.TlAwardingOrganisation
                .OrderBy(x => x.DisplayName)
                .Select(x => new FilterLookupData { Id = x.Id, Name = x.DisplayName, IsSelected = false })
                .ToListAsync();
        }

        public async Task<IList<FilterLookupData>> GetAcademicYearFiltersAsync(DateTime searchDate)
        {
            return await _dbContext.AcademicYear
                .Where(x => searchDate >= x.EndDate || (searchDate >= x.StartDate && searchDate <= x.EndDate))
                .OrderByDescending(x => x.Year)
                .Take(5)
                .Select(x => new FilterLookupData { Id = x.Year, Name = $"{x.Year} to {x.Year + 1}", IsSelected = false })
                .ToListAsync();
        }

        public async Task<PagedResponse<AdminSearchLearnerDetail>> SearchLearnerDetailsAsync(AdminSearchLearnerRequest request)
        {
            IQueryable<TqRegistrationPathway> registrationPathwayQueryable = _dbContext.TqRegistrationPathway
                                                                                        .Include(p => p.TqProvider)
                                                                                            .ThenInclude(p => p.TlProvider)
                                                                                        .Include(p => p.TqProvider)
                                                                                            .ThenInclude(p => p.TqAwardingOrganisation)
                                                                                            .ThenInclude(p => p.TlAwardingOrganisaton)
                                                                                       .Where(p => !_dbContext.TqRegistrationPathway.Any(p2 => p2.TqRegistrationProfileId == p.TqRegistrationProfileId && p2.Id > p.Id))
                                                                                       .AsQueryable();

            int totalCount = registrationPathwayQueryable.Count();

            if (!string.IsNullOrWhiteSpace(request.SearchKey))
            {
                bool isSearchKeyUln = request.SearchKey.IsLong();

                Expression<Func<TqRegistrationPathway, bool>> expression = isSearchKeyUln
                    ? p => p.TqRegistrationProfile.UniqueLearnerNumber == request.SearchKey.ToLong()
                    : p => EF.Functions.Like(p.TqRegistrationProfile.Lastname, request.SearchKey.ToLower());

                registrationPathwayQueryable = registrationPathwayQueryable.Where(expression);
            }

            if (!request.SelectedAwardingOrganisations.IsNullOrEmpty())
            {
                registrationPathwayQueryable = registrationPathwayQueryable.Where(p => request.SelectedAwardingOrganisations.Contains(p.TqProvider.TqAwardingOrganisation.TlAwardingOrganisatonId));
            }

            if (!request.SelectedAcademicYears.IsNullOrEmpty())
            {
                registrationPathwayQueryable = registrationPathwayQueryable.Where(p => request.SelectedAcademicYears.Contains(p.AcademicYear));
            }

            if (request.ProviderUkprn.HasValue)
            {
                registrationPathwayQueryable = registrationPathwayQueryable.Where(p => request.ProviderUkprn == p.TqProvider.TlProvider.UkPrn);
            }

            int filteredRecordsCount = await registrationPathwayQueryable.CountAsync();
            var pager = new Pager(filteredRecordsCount, request.PageNumber, 10);

            IQueryable<AdminSearchLearnerDetail> learnerRecordsQueryable = registrationPathwayQueryable
                .Select(x => new AdminSearchLearnerDetail
                {
                    RegistrationPathwayId = x.Id,
                    Uln = x.TqRegistrationProfile.UniqueLearnerNumber,
                    Firstname = x.TqRegistrationProfile.Firstname,
                    Lastname = x.TqRegistrationProfile.Lastname,
                    Provider = x.TqProvider.TlProvider.Name,
                    ProviderUkprn = x.TqProvider.TlProvider.UkPrn,
                    AwardingOrganisation = x.TqProvider.TqAwardingOrganisation.TlAwardingOrganisaton.DisplayName,
                    AcademicYear = x.AcademicYear
                })
                .OrderBy(x => x.Lastname)
                .Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize);

            List<AdminSearchLearnerDetail> learnerRecords = await learnerRecordsQueryable.ToListAsync();
            return new PagedResponse<AdminSearchLearnerDetail> { Records = learnerRecords, TotalRecords = totalCount, PagerInfo = pager };
        }

        public async Task<TqRegistrationPathway> GetLearnerRecordAsync(int profileId)
        {
            return await _dbContext.TqRegistrationPathway
                .Include(p => p.TqRegistrationSpecialisms)
                .Include(p => p.TqRegistrationProfile)
                .Include(p => p.TqProvider)
                    .ThenInclude(p => p.TlProvider)
                .Include(p => p.TqProvider)
                    .ThenInclude(p => p.TqAwardingOrganisation)
                    .ThenInclude(p => p.TlPathway)
            .Where(p => p.TqRegistrationProfileId == profileId)
           .FirstOrDefaultAsync();
        }

    }
}