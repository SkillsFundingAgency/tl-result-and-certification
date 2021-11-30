﻿using AutoMapper;
using Sfa.Tl.ResultsAndCertification.Domain.Models;
using Sfa.Tl.ResultsAndCertification.Models.Contracts.Learner;
using System.Collections.Generic;

namespace Sfa.Tl.ResultsAndCertification.Application.Mappers
{
    public class LearnerMapper : Profile
    {
        public LearnerMapper()
        {
            CreateMap<TqRegistrationPathway, LearnerRecord>()
                .ForMember(d => d.ProfileId, opts => opts.MapFrom(s => s.TqRegistrationProfile.Id))
                .ForMember(d => d.Uln, opts => opts.MapFrom(s => s.TqRegistrationProfile.UniqueLearnerNumber))
                .ForMember(d => d.Firstname, opts => opts.MapFrom(s => s.TqRegistrationProfile.Firstname))
                .ForMember(d => d.Lastname, opts => opts.MapFrom(s => s.TqRegistrationProfile.Lastname))
                .ForMember(d => d.DateofBirth, opts => opts.MapFrom(s => s.TqRegistrationProfile.DateofBirth))
                .ForMember(d => d.Gender, opts => opts.MapFrom(s => s.TqRegistrationProfile.Gender))
                .ForMember(d => d.Pathway, opts => opts.MapFrom(s => s));

            //CreateMap<TqRegistrationPathway, ICollection<Pathway>>()
            //    .ConstructUsing((m, context) =>
            //    {
            //        return new List<Pathway>
            //        {
            //            new Pathway
            //            {
            //                Id = m.Id,
            //                LarId = m.TqProvider.TqAwardingOrganisation.TlPathway.LarId,
            //                Name = m.TqProvider.TqAwardingOrganisation.TlPathway.Name,
            //                Title = m.TqProvider.TqAwardingOrganisation.TlPathway.TlevelTitle,
            //                Status = m.Status,
            //                Provider = context.Mapper.Map<Provider>(m.TqProvider),
            //                PathwayAssessments = context.Mapper.Map<IList<Assessment>>(m.TqPathwayAssessments),
            //                Specialisms = context.Mapper.Map<IList<Specialism>>(m.TqRegistrationSpecialisms)
            //            }
            //        };
            //    });

            CreateMap<TqRegistrationPathway, Pathway>()
               .ForMember(d => d.Id, opts => opts.MapFrom(s => s.Id))
               .ForMember(d => d.LarId, opts => opts.MapFrom(s => s.TqProvider.TqAwardingOrganisation.TlPathway.LarId))
               .ForMember(d => d.Name, opts => opts.MapFrom(s => s.TqProvider.TqAwardingOrganisation.TlPathway.Name))
               .ForMember(d => d.Title, opts => opts.MapFrom(s => s.TqProvider.TqAwardingOrganisation.TlPathway.TlevelTitle))
               .ForMember(d => d.AcademicYear, opts => opts.MapFrom(s => s.AcademicYear))
               .ForMember(d => d.Status, opts => opts.MapFrom(s => s.Status))
               .ForMember(d => d.Provider, opts => opts.MapFrom(s => s.TqProvider))
               .ForMember(d => d.PathwayAssessments, opts => opts.MapFrom(s => s.TqPathwayAssessments))
               .ForMember(d => d.Specialisms, opts => opts.MapFrom(s => s.TqRegistrationSpecialisms))
               .ForMember(d => d.IndustryPlacements, opts => opts.MapFrom(s => s.IndustryPlacements));

            CreateMap<TqProvider, Provider>()
               .ForMember(d => d.Id, opts => opts.MapFrom(s => s.TlProvider.Id))
               .ForMember(d => d.Ukprn, opts => opts.MapFrom(s => s.TlProvider.UkPrn))
               .ForMember(d => d.Name, opts => opts.MapFrom(s => s.TlProvider.Name))
               .ForMember(d => d.DisplayName, opts => opts.MapFrom(s => s.TlProvider.DisplayName));

            CreateMap<TqPathwayAssessment, Assessment>()
               .ForMember(d => d.Id, opts => opts.MapFrom(s => s.Id))
               .ForMember(d => d.SeriesId, opts => opts.MapFrom(s => s.AssessmentSeries.Id))
               .ForMember(d => d.SeriesName, opts => opts.MapFrom(s => s.AssessmentSeries.Name))
               .ForMember(d => d.AppealEndDate, opts => opts.MapFrom(s => s.AssessmentSeries.AppealEndDate))
               .ForMember(d => d.LastUpdatedOn, opts => opts.MapFrom(s => s.CreatedOn))
               .ForMember(d => d.LastUpdatedBy, opts => opts.MapFrom(s => s.CreatedBy))
               .ForMember(d => d.Results, opts => opts.MapFrom(s => s.TqPathwayResults));

            CreateMap<TqPathwayResult, Result>()
               .ForMember(d => d.Id, opts => opts.MapFrom(s => s.Id))
               .ForMember(d => d.Grade, opts => opts.MapFrom(s => s.TlLookup.Value))
               .ForMember(d => d.PrsStatus, opts => opts.MapFrom(s => s.PrsStatus))
               .ForMember(d => d.LastUpdatedOn, opts => opts.MapFrom(s => s.CreatedOn))
               .ForMember(d => d.LastUpdatedBy, opts => opts.MapFrom(s => s.CreatedBy));

            CreateMap<TqRegistrationSpecialism, Specialism>()
               .ForMember(d => d.Id, opts => opts.MapFrom(s => s.Id))
               .ForMember(d => d.LarId, opts => opts.MapFrom(s => s.TlSpecialism.LarId))
               .ForMember(d => d.Name, opts => opts.MapFrom(s => s.TlSpecialism.Name))
               .ForMember(d => d.Assessments, opts => opts.MapFrom(s => s.TqSpecialismAssessments));

            CreateMap<TqSpecialismAssessment, Assessment>()
               .ForMember(d => d.Id, opts => opts.MapFrom(s => s.Id))
               .ForMember(d => d.SeriesId, opts => opts.MapFrom(s => s.AssessmentSeries.Id))
               .ForMember(d => d.SeriesName, opts => opts.MapFrom(s => s.AssessmentSeries.Name))
               .ForMember(d => d.AppealEndDate, opts => opts.MapFrom(s => s.AssessmentSeries.AppealEndDate))
               .ForMember(d => d.LastUpdatedOn, opts => opts.MapFrom(s => s.CreatedOn))
               .ForMember(d => d.LastUpdatedBy, opts => opts.MapFrom(s => s.CreatedBy))
               .ForMember(d => d.Results, opts => opts.MapFrom(s => new List<Result>()));

            CreateMap<Domain.Models.IndustryPlacement, Models.Contracts.Learner.IndustryPlacement>()
               .ForMember(d => d.Id, opts => opts.MapFrom(s => s.Id))
               .ForMember(d => d.Status, opts => opts.MapFrom(s => s.Status));
        }
    }
}
