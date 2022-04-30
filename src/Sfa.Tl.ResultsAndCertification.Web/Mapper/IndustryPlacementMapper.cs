﻿using AutoMapper;
using Sfa.Tl.ResultsAndCertification.Models.Contracts.TrainingProvider;
using Sfa.Tl.ResultsAndCertification.Web.ViewModel.IndustryPlacement.Manual;

namespace Sfa.Tl.ResultsAndCertification.Web.Mapper
{
    public class IndustryPlacementMapper : Profile
    {
        public IndustryPlacementMapper()
        {
            CreateMap<LearnerRecordDetails, IpCompletionViewModel>()
               .ForMember(d => d.ProfileId, opts => opts.MapFrom(s => s.ProfileId))
               .ForMember(d => d.PathwayId, opts => opts.MapFrom(s => s.RegistrationPathwayId))
               .ForMember(d => d.AcademicYear, opts => opts.MapFrom(s => s.AcademicYear))
               .ForMember(d => d.LearnerName, opts => opts.MapFrom(s => s.Name))
               .ForMember(d => d.IndustryPlacementStatus, opts => opts.MapFrom(s => s.IndustryPlacementStatus));

            CreateMap<IpCompletionViewModel, IpModelUsedViewModel>()
               .ForMember(d => d.ProfileId, opts => opts.MapFrom(s => s.ProfileId))
               .ForMember(d => d.LearnerName, opts => opts.MapFrom(s => s.LearnerName))
               .ForMember(d => d.IndustryPlacementStatus, opts => opts.MapFrom(s => s.IndustryPlacementStatus));

            CreateMap<IpCompletionViewModel, IpMultiEmployerUsedViewModel>()
               .ForMember(d => d.LearnerName, opts => opts.MapFrom(s => s.LearnerName));            
        }
    }
}
