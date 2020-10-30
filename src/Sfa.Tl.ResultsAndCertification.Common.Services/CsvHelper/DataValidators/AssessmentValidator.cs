﻿using FluentValidation;
using Sfa.Tl.ResultsAndCertification.Common.Services.CsvHelper.Helpers.Extensions;
using Sfa.Tl.ResultsAndCertification.Models.Registration.BulkProcess;

namespace Sfa.Tl.ResultsAndCertification.Common.Services.CsvHelper.DataValidators
{
    public class AssessmentValidator : AbstractValidator<AssessmentCsvRecordRequest>
    {
        public AssessmentValidator()
        {
            // Uln
            RuleFor(r => r.Uln)
                .Required()
                .MustBeNumberWithLength(10);

            //// Core
            //RuleFor(r => r.CoreCode)
            //    .Required()
            //    .MustBeStringWithLength(8);

            //// Core
            //RuleFor(r => r.SpecialismCode)
            //    .Required()
            //    .MustBeStringWithLength(8);

        }
    }
}
