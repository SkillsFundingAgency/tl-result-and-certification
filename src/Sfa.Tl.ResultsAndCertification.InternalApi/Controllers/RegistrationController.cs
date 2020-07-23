﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sfa.Tl.ResultsAndCertification.Application.Interfaces;
using Sfa.Tl.ResultsAndCertification.InternalApi.Interfaces;
using Sfa.Tl.ResultsAndCertification.InternalApi.Loader.Interfaces;
using Sfa.Tl.ResultsAndCertification.Models.Contracts;

namespace Sfa.Tl.ResultsAndCertification.InternalApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistrationController : ControllerBase, IRegistrationController
    {
        private readonly IRegistrationService _registrationService;
        private readonly IBulkRegistrationLoader _bulkRegistrationProcess;
        private readonly ILogger<ProviderController> _logger;

        public RegistrationController(IRegistrationService registrationService, IBulkRegistrationLoader bulkRegistrationProcess, ILogger<ProviderController> logger)
        {
            _registrationService = registrationService;
            _bulkRegistrationProcess = bulkRegistrationProcess; 
            _logger = logger;
        }

        [HttpPost]
        [Route("ProcessBulkRegistrations")]
        public async Task<BulkRegistrationResponse> ProcessBulkRegistrationsAsync(BulkRegistrationRequest request)
        {
            return await _bulkRegistrationProcess.ProcessBulkRegistrationsAsync(request);
        }

        [HttpPost]
        [Route("AddRegistration")]
        public async Task<bool> AddRegistrationAsync(RegistrationRequest model)
        {
            return await _registrationService.AddRegistrationAsync(model);
        }

        [HttpGet]
        [Route("FindUln/{aoUkprn}/{uln}")]
        public async Task<FindUlnResponse> FindUlnAsync(long aoUkprn, long uln)
        {
            return await _registrationService.FindUlnAsync(aoUkprn, uln);
        }
    }
}