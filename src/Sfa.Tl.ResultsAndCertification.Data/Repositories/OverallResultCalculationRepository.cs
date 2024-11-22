﻿using Microsoft.EntityFrameworkCore;
using Sfa.Tl.ResultsAndCertification.Common.Enum;
using Sfa.Tl.ResultsAndCertification.Data.Interfaces;
using Sfa.Tl.ResultsAndCertification.Domain.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sfa.Tl.ResultsAndCertification.Data.Repositories
{
    public class OverallResultCalculationRepository : IOverallResultCalculationRepository
    {
        private readonly ResultsAndCertificationDbContext _dbContext;

        public OverallResultCalculationRepository(ResultsAndCertificationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IList<TqRegistrationPathway>> GetLearnersForOverallGradeCalculation(int academicYearFrom, int academicYearTo)
        {
            var activeLearners = _dbContext.TqRegistrationPathway
                .Include(x => x.TqRegistrationProfile)
                .Include(x => x.IndustryPlacements)
                .Include(x => x.TqPathwayAssessments.Where(pa => pa.IsOptedin && pa.EndDate == null))
                    .ThenInclude(x => x.TqPathwayResults.Where(pr => pr.IsOptedin && pr.EndDate == null))
                    .ThenInclude(x => x.TlLookup)
                .Include(x => x.TqRegistrationSpecialisms.Where(s => s.IsOptedin && s.EndDate == null))
                    .ThenInclude(x => x.TlSpecialism)
                .Include(x => x.TqRegistrationSpecialisms.Where(s => s.IsOptedin && s.EndDate == null))
                    .ThenInclude(x => x.TqSpecialismAssessments.Where(sa => sa.IsOptedin && sa.EndDate == null))
                    .ThenInclude(x => x.TqSpecialismResults.Where(sr => sr.IsOptedin && sr.EndDate == null))
                    .ThenInclude(x => x.TlLookup)
                .Include(x => x.OverallResults.Where(o => o.IsOptedin && o.EndDate == null))
                .Include(x => x.TqProvider)
                    .ThenInclude(x => x.TqAwardingOrganisation)
                    .ThenInclude(x => x.TlPathway)
                .Where(pw => pw.Status == RegistrationPathwayStatus.Active &&
                             pw.AcademicYear >= academicYearFrom && pw.AcademicYear <= academicYearTo &&
                             (!pw.OverallResults.Any() ||                         // if overall result not yet calculated OR
                              pw.OverallResults.Any(ovr => ovr.IsOptedin && ovr.EndDate == null && // ip or core-result or spl result are updated after the calculation
                                 (pw.IndustryPlacements.Any(ip => ip.CreatedOn > ovr.CreatedOn || ip.ModifiedOn > ovr.CreatedOn) ||
                                  pw.TqPathwayAssessments.SelectMany(pa => pa.TqPathwayResults).Any(pr => pr.CreatedOn > ovr.CreatedOn || pr.ModifiedOn > ovr.CreatedOn) ||
                                  pw.TqRegistrationSpecialisms.SelectMany(s => s.TqSpecialismAssessments.SelectMany(sa => sa.TqSpecialismResults)).Any(sr => sr.CreatedOn > ovr.CreatedOn || sr.ModifiedOn > ovr.CreatedOn)))
                             ));

            return await activeLearners.ToListAsync();
        }
    }
}
