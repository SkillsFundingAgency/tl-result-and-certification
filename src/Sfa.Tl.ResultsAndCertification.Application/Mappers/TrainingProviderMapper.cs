﻿using AutoMapper;
using Sfa.Tl.ResultsAndCertification.Common.Enum;
using Sfa.Tl.ResultsAndCertification.Domain.Models;
using Sfa.Tl.ResultsAndCertification.Models.Contracts.TrainingProvider;
using System.Linq;

namespace Sfa.Tl.ResultsAndCertification.Application.Mappers
{
    public class TrainingProviderMapper : Profile
    {
        public TrainingProviderMapper()
        {
            CreateMap<TqRegistrationPathway, LearnerRecordDetails>()
                .ForMember(d => d.ProfileId, opts => opts.MapFrom(s => s.TqRegistrationProfile.Id))
                .ForMember(d => d.Uln, opts => opts.MapFrom(s => s.TqRegistrationProfile.UniqueLearnerNumber))
                .ForMember(d => d.Name, opts => opts.MapFrom(s => $"{s.TqRegistrationProfile.Firstname} {s.TqRegistrationProfile.Lastname}"))
                .ForMember(d => d.DateofBirth, opts => opts.MapFrom(s => s.TqRegistrationProfile.DateofBirth))
                .ForMember(d => d.ProviderName, opts => opts.MapFrom(s => $"{s.TqProvider.TlProvider.Name} ({s.TqProvider.TlProvider.UkPrn})"))
                .ForMember(d => d.PathwayName, opts => opts.MapFrom(s => $"{s.TqProvider.TqAwardingOrganisation.TlPathway.Name} ({s.TqProvider.TqAwardingOrganisation.TlPathway.LarId})"))
                .ForMember(d => d.IsLearnerRegistered, opts => opts.MapFrom(s => s.Status == RegistrationPathwayStatus.Active || s.Status == RegistrationPathwayStatus.Withdrawn))
                .ForMember(d => d.IsLearnerRecordAdded, opts => opts.MapFrom(s => s.TqRegistrationProfile.IsEnglishAndMathsAchieved.HasValue && s.IndustryPlacements.Any()))
                .ForMember(d => d.IsEnglishAndMathsAchieved, opts => opts.MapFrom(s => s.TqRegistrationProfile.IsEnglishAndMathsAchieved))
                .ForMember(d => d.IsSendLearner, opts => opts.MapFrom(s => s.TqRegistrationProfile.IsSendLearner))
                .ForMember(d => d.HasLrsEnglishAndMaths, opts => opts.MapFrom(s => s.TqRegistrationProfile.IsRcFeed == false && s.TqRegistrationProfile.QualificationAchieved.Any()))
                .ForMember(d => d.IndustryPlacementId, opts => opts.MapFrom(s => s.IndustryPlacements.Any() ? s.IndustryPlacements.FirstOrDefault().Id : 0))
                .ForMember(d => d.IndustryPlacementStatus, opts => opts.MapFrom(s => s.IndustryPlacements.Any() ? s.IndustryPlacements.FirstOrDefault().Status : (IndustryPlacementStatus?) null));

            CreateMap<TqRegistrationPathway, FindLearnerRecord>()
                .ForMember(d => d.ProfileId, opts => opts.MapFrom(s => s.TqRegistrationProfile.Id))
                .ForMember(d => d.Uln, opts => opts.MapFrom(s => s.TqRegistrationProfile.UniqueLearnerNumber))
                .ForMember(d => d.Name, opts => opts.MapFrom(s => $"{s.TqRegistrationProfile.Firstname} {s.TqRegistrationProfile.Lastname}"))
                .ForMember(d => d.DateofBirth, opts => opts.MapFrom(s => s.TqRegistrationProfile.DateofBirth))
                .ForMember(d => d.ProviderName, opts => opts.MapFrom(s => $"{s.TqProvider.TlProvider.Name} ({s.TqProvider.TlProvider.UkPrn})"))
                .ForMember(d => d.IsLearnerRegistered, opts => opts.MapFrom(s => s.Status == RegistrationPathwayStatus.Active || s.Status == RegistrationPathwayStatus.Withdrawn))
                .ForMember(d => d.IsLearnerRecordAdded, opts => opts.MapFrom(s => s.TqRegistrationProfile.IsEnglishAndMathsAchieved.HasValue && s.IndustryPlacements.Any()))
                .ForMember(d => d.IsEnglishAndMathsAchieved, opts => opts.MapFrom(s => s.TqRegistrationProfile.IsEnglishAndMathsAchieved))                
                .ForMember(d => d.HasLrsEnglishAndMaths, opts => opts.MapFrom(s => s.TqRegistrationProfile.IsRcFeed == false && s.TqRegistrationProfile.QualificationAchieved.Any()))
                .ForMember(d => d.HasSendQualification, opts => opts.MapFrom(s => s.TqRegistrationProfile.QualificationAchieved.Any(q => q.Qualification != null && q.Qualification.IsSendQualification)));
        }
    }
}