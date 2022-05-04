﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Sfa.Tl.ResultsAndCertification.Application.Interfaces;
using Sfa.Tl.ResultsAndCertification.Common.Enum;
using Sfa.Tl.ResultsAndCertification.Data.Interfaces;
using Sfa.Tl.ResultsAndCertification.Domain.Models;
using Sfa.Tl.ResultsAndCertification.Models.Contracts.IndustryPlacement;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contract = Sfa.Tl.ResultsAndCertification.Models.Contracts.IndustryPlacement;
using DbModel = Sfa.Tl.ResultsAndCertification.Domain.Models;

namespace Sfa.Tl.ResultsAndCertification.Application.Services
{
    public class IndustryPlacementService : IIndustryPlacementService
    {
        private readonly IRepository<IpLookup> _ipLookupRepository;
        private readonly IRepository<IpModelTlevelCombination> _ipModelTlevelCombinationRepository;
        private readonly IRepository<IpTempFlexTlevelCombination> _ipTempFlexTlevelCombinationRepository;
        private readonly IRepository<DbModel.IpTempFlexNavigation> _ipTempFlexNavigationRepository;

        private readonly IMapper _mapper;

        public IndustryPlacementService(
            IRepository<IpLookup> ipLookupRepository,
            IRepository<IpModelTlevelCombination> ipModelTlevelCombinationRepository,
            IRepository<IpTempFlexTlevelCombination> ipTempFlexTlevelCombinationRepository,
            IRepository<DbModel.IpTempFlexNavigation> ipTempFlexNavigationRepository,
            IMapper mapper)
        {
            _ipLookupRepository = ipLookupRepository;
            _ipModelTlevelCombinationRepository = ipModelTlevelCombinationRepository;
            _ipTempFlexTlevelCombinationRepository = ipTempFlexTlevelCombinationRepository;
            _ipTempFlexNavigationRepository = ipTempFlexNavigationRepository;
            _mapper = mapper;
        }

        public async Task<IList<IpLookupData>> GetIpLookupDataAsync(IpLookupType ipLookupType, int? pathwayId)
        {
            return ipLookupType switch
            {
                IpLookupType.SpecialConsideration => await SpecialConsiderationReasonsAsync(),
                IpLookupType.IndustryPlacementModel => await IndustryPlacementModelsAsync(pathwayId),
                IpLookupType.TemporaryFlexibility => await TemporaryFlexibilitiesAsync(pathwayId),
                _ => null
            };
        }

        private async Task<IList<IpLookupData>> SpecialConsiderationReasonsAsync()
        {
            var lookupData = await _ipLookupRepository.GetManyAsync(x => x.TlLookup.Category == IpLookupType.SpecialConsideration.ToString()).OrderBy(x => x.SortOrder).ToListAsync();
            return _mapper.Map<IList<IpLookupData>>(lookupData);
        }

        private async Task<IList<IpLookupData>> IndustryPlacementModelsAsync(int? pathwayId)
        {
            var lookupData = await _ipModelTlevelCombinationRepository
                                    .GetManyAsync(x => x.IsActive && x.TlPathwayId == pathwayId && x.IpLookup.TlLookup.Category == IpLookupType.IndustryPlacementModel.ToString())
                                    .Select(x => x.IpLookup)
                                    .OrderBy(x => x.SortOrder).ToListAsync();

            return _mapper.Map<IList<IpLookupData>>(lookupData);
        }

        private async Task<IList<IpLookupData>> TemporaryFlexibilitiesAsync(int? pathwayId)
        {
            var lookupData = await _ipTempFlexTlevelCombinationRepository
                                    .GetManyAsync(x => x.IsActive && x.TlPathwayId == pathwayId && x.IpLookup.TlLookup.Category == IpLookupType.TemporaryFlexibility.ToString())
                                    .Select(x => x.IpLookup)
                                    .OrderBy(x => x.SortOrder).ToListAsync();

            return _mapper.Map<IList<IpLookupData>>(lookupData);
        }

        public async Task<Contract.IpTempFlexNavigation> GetTempFlexNavigationAsync(int pathwayId, int academicYear)
        {
            var navigation = await _ipTempFlexNavigationRepository.GetFirstOrDefaultAsync(x => x.TlPathwayId == pathwayId && x.AcademicYear == academicYear);
            return _mapper.Map<Contract.IpTempFlexNavigation>(navigation);
        }
    }
}
