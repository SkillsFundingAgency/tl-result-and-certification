﻿using Sfa.Tl.ResultsAndCertification.Models.BulkProcess;
using System;
using System.Collections.Generic;

namespace Sfa.Tl.ResultsAndCertification.Models.Registration.BulkProcess
{
    public class WithdrawalCsvRecordResponse : ValidationState<BulkProcessValidationError>
    {
        public WithdrawalCsvRecordResponse()
        {
            SpecialismCodes = new List<string>();
        }

        public int RowNum { get; set; }

        public long Uln { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime DateOfBirth { get; set; }

        public long ProviderUkprn { get; set; }

        public int AcademicYear { get; set; }

        public string AcademicYearName { get; set; }

        public string CoreCode { get; set; }

        public IEnumerable<string> SpecialismCodes { get; set; }
    }
}
