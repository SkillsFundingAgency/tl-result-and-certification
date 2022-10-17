﻿using AutoMapper;
using Sfa.Tl.ResultsAndCertification.Models.Contracts.TrainingProvider;
using Sfa.Tl.ResultsAndCertification.Web.Mapper;
using Sfa.Tl.ResultsAndCertification.Web.Mapper.Resolver;
using Sfa.Tl.ResultsAndCertification.Web.ViewModel.TrainingProvider.Manual;
using System.Threading.Tasks;

namespace Sfa.Tl.ResultsAndCertification.Web.UnitTests.Loader.TrainingProviderTests.CreateReplacementDocumentPrintingRequest
{
    public abstract class TestSetup : TrainingProviderLoaderTestBase
    {
        protected long ProviderUkprn;
        protected RequestReplacementDocumentViewModel RequestReplacementDocumentViewModel { get; set; }
        protected bool ActualResult { get; set; }

        public async override Task When()
        {
            ActualResult = await Loader.CreateReplacementDocumentPrintingRequestAsync(ProviderUkprn, RequestReplacementDocumentViewModel);
        }

        public void CreateMapper()
        {
            var mapperConfig = new MapperConfiguration(c =>
            {
                c.AddMaps(typeof(TrainingProviderMapper).Assembly);
                c.ConstructServicesUsing(type =>
                            type.Name.Contains("UserNameResolver") ?
                                new UserNameResolver<RequestReplacementDocumentViewModel, ReplacementPrintRequest>(HttpContextAccessor) : null);

            });
            Mapper = new AutoMapper.Mapper(mapperConfig);
        }
    }
}
