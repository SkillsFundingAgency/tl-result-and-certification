﻿using AutoMapper;
using Sfa.Tl.ResultsAndCertification.Domain.Models;
using Sfa.Tl.ResultsAndCertification.Models.Contracts;
using System.Linq;

namespace Sfa.Tl.ResultsAndCertification.Application.Mappers
{
    public class AssessmentMapper : Profile
    {
        public AssessmentMapper()
        {
            CreateMap<TqRegistrationPathway, AssessmentDetails>()
                .ForMember(d => d.ProfileId, opts => opts.MapFrom(s => s.TqRegistrationProfileId))
                .ForMember(d => d.Uln, opts => opts.MapFrom(s => s.TqRegistrationProfile.UniqueLearnerNumber))
                .ForMember(d => d.Firstname, opts => opts.MapFrom(s => s.TqRegistrationProfile.Firstname))
                .ForMember(d => d.Lastname, opts => opts.MapFrom(s => s.TqRegistrationProfile.Lastname))
                .ForMember(d => d.DateofBirth, opts => opts.MapFrom(s => s.TqRegistrationProfile.DateofBirth))
                .ForMember(d => d.ProviderUkprn, opts => opts.MapFrom(s => s.TqProvider.TlProvider.UkPrn))
                .ForMember(d => d.ProviderName, opts => opts.MapFrom(s => s.TqProvider.TlProvider.Name))
                .ForMember(d => d.PathwayLarId, opts => opts.MapFrom(s => s.TqProvider.TqAwardingOrganisation.TlPathway.LarId))
                .ForMember(d => d.PathwayName, opts => opts.MapFrom(s => s.TqProvider.TqAwardingOrganisation.TlPathway.Name))
                .ForMember(d => d.TlevelTitle, opts => opts.MapFrom(s => s.TqProvider.TqAwardingOrganisation.TlPathway.TlevelTitle))
                .ForMember(d => d.PathwayAssessmentSeries, opts => opts.MapFrom(s => s.TqPathwayAssessments.Any() ? s.TqPathwayAssessments.OrderByDescending(a => a.AssessmentSeriesId).FirstOrDefault().AssessmentSeries.Name : null))
                .ForMember(d => d.PathwayAssessmentId, opts => opts.MapFrom(s => s.TqPathwayAssessments.Any() ? s.TqPathwayAssessments.OrderByDescending(a => a.AssessmentSeriesId).FirstOrDefault().Id : (int?)null))
                .ForMember(d => d.SpecialismLarId, opts => opts.MapFrom(s => s.TqRegistrationSpecialisms.Any() ? s.TqRegistrationSpecialisms.FirstOrDefault().TlSpecialism.LarId : null))
                .ForMember(d => d.SpecialismName, opts => opts.MapFrom(s => s.TqRegistrationSpecialisms.Any() ? s.TqRegistrationSpecialisms.FirstOrDefault().TlSpecialism.Name : null))
                .ForMember(d => d.SpecialismAssessmentSeries, opts => opts.MapFrom(s => s.TqRegistrationSpecialisms.Any() ? s.TqRegistrationSpecialisms.FirstOrDefault().TqSpecialismAssessments.Any() ? s.TqRegistrationSpecialisms.FirstOrDefault().TqSpecialismAssessments.FirstOrDefault().AssessmentSeries.Name : null : null))
                .ForMember(d => d.SpecialismAssessmentId, opts => opts.MapFrom(s => s.TqRegistrationSpecialisms.Any() ? s.TqRegistrationSpecialisms.FirstOrDefault().TqSpecialismAssessments.Any() ? s.TqRegistrationSpecialisms.FirstOrDefault().TqSpecialismAssessments.FirstOrDefault().Id : (int?)null : null))
                .ForMember(d => d.Specialisms, opts => opts.MapFrom(s => s.TqRegistrationSpecialisms.Select(x => new SpecialismDetails { Id = x.Id, Code = x.TlSpecialism.LarId, Name = x.TlSpecialism.Name })))
                .ForMember(d => d.PathwayResultId, opts =>
                                opts.MapFrom(s => s.TqPathwayAssessments.Any() && s.TqPathwayAssessments.OrderByDescending(a => a.AssessmentSeriesId).FirstOrDefault().TqPathwayResults.Any() ?
                                s.TqPathwayAssessments.OrderByDescending(a => a.AssessmentSeriesId).FirstOrDefault().TqPathwayResults.FirstOrDefault().Id : (int?)null))
                .ForMember(d => d.HasAnyOutstandingPathwayPrsActivities, opts => opts.MapFrom(s => s.TqPathwayAssessments.Any() ? s.TqPathwayAssessments.Any(a => a.TqPathwayResults.Any(r => r.IsOptedin && r.EndDate == null && r.PrsStatus == Common.Enum.PrsStatus.BeingAppealed)) : false))
                .ForMember(d => d.IsIndustryPlacementExist, opts => opts.MapFrom(s => s.IndustryPlacements.Any()))
                .ForMember(d => d.Status, opts => opts.MapFrom(s => s.Status));

            CreateMap<AssessmentSeries, AvailableAssessmentSeries>()
                .ForMember(d => d.ProfileId, opts => opts.MapFrom((src, dest, destMember, context) => (int)context.Items["profileId"]))
                .ForMember(d => d.AssessmentSeriesId, opts => opts.MapFrom(s => s.Id))
                .ForMember(d => d.AssessmentSeriesName, opts => opts.MapFrom(s => s.Name));

            CreateMap<TqPathwayAssessment, AssessmentEntryDetails>()
                .ForMember(d => d.ProfileId, opts => opts.MapFrom(s => s.TqRegistrationPathway.TqRegistrationProfile.Id))
                .ForMember(d => d.Uln, opts => opts.MapFrom(s => s.TqRegistrationPathway.TqRegistrationProfile.UniqueLearnerNumber))
                .ForMember(d => d.AssessmentId, opts => opts.MapFrom(s => s.Id))
                .ForMember(d => d.AssessmentSeriesId, opts => opts.MapFrom(s => s.AssessmentSeries.Id))
                .ForMember(d => d.AssessmentSeriesName, opts => opts.MapFrom(s => s.AssessmentSeries.Name));

            CreateMap<TqSpecialismAssessment, AssessmentEntryDetails>()
                .ForMember(d => d.ProfileId, opts => opts.MapFrom(s => s.TqRegistrationSpecialism.TqRegistrationPathway.TqRegistrationProfile.Id))
                .ForMember(d => d.Uln, opts => opts.MapFrom(s => s.TqRegistrationSpecialism.TqRegistrationPathway.TqRegistrationProfile.UniqueLearnerNumber))
                .ForMember(d => d.AssessmentId, opts => opts.MapFrom(s => s.Id))
                .ForMember(d => d.AssessmentSeriesId, opts => opts.MapFrom(s => s.AssessmentSeries.Id))
                .ForMember(d => d.AssessmentSeriesName, opts => opts.MapFrom(s => s.AssessmentSeries.Name));

            CreateMap<AssessmentSeries, AssessmentSeriesDetails>()
                .ForMember(d => d.Id, opts => opts.MapFrom(s => s.Id))
                .ForMember(d => d.ComponentType, opts => opts.MapFrom(s => s.ComponentType))
                .ForMember(d => d.Name, opts => opts.MapFrom(s => s.Name))
                .ForMember(d => d.Description, opts => opts.MapFrom(s => s.Description))
                .ForMember(d => d.Year, opts => opts.MapFrom(s => s.Year))
                .ForMember(d => d.StartDate, opts => opts.MapFrom(s => s.StartDate))
                .ForMember(d => d.EndDate, opts => opts.MapFrom(s => s.EndDate))
                .ForMember(d => d.AppealEndDate, opts => opts.MapFrom(s => s.AppealEndDate));
        }
    }
}
