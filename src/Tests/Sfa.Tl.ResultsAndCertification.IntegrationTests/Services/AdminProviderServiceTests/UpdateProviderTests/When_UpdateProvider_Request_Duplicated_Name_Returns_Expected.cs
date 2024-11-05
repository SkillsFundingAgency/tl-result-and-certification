﻿using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Sfa.Tl.ResultsAndCertification.Domain.Models;
using Sfa.Tl.ResultsAndCertification.Tests.Common.DataBuilders;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Sfa.Tl.ResultsAndCertification.IntegrationTests.Services.AdminProviderServiceTests.UpdateProviderTests
{
    public class When_UpdateProvider_Request_Duplicated_Name_Returns_Expected : UpdateProviderBaseTest
    {
        private const long Ukprn = 10000536;
        private IList<TlProvider> _providersDb;

        public override void Given()
        {
            int providerId = SeedTestData();

            Request = new()
            {
                ProviderId = providerId,
                UkPrn = Ukprn,
                Name = "Walsall Studio School",
                DisplayName = "Walsall Studio School of English",
                IsActive = true,
                ModifiedBy = "modified-by"
            };
        }

        [Fact]
        public async void Then_Should_Return_False()
        {
            Result.IsRequestValid.Should().BeFalse();
            Result.DuplicatedUkprnFound.Should().BeFalse();
            Result.DuplicatedNameFound.Should().BeTrue();
            Result.Success.Should().BeFalse();

            TlProvider provider = await DbContext.TlProvider.SingleAsync(p => p.Id == Request.ProviderId);
            provider.Should().BeEquivalentTo(_providersDb.First());
        }

        private int SeedTestData()
        {
            TlProviderBuilder providerBuilder = new();
            _providersDb = providerBuilder.BuildList();

            DbContext.AddRange(_providersDb);
            DbContext.SaveChanges();

            return _providersDb.First().Id;
        }
    }
}