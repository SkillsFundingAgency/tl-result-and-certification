﻿using System;

namespace Sfa.Tl.ResultsAndCertification.Web.ViewModel.PostResultsService
{
    public class UploadRommsResponseViewModel
    {
        public bool IsSuccess { get; set; }
        public Guid BlobUniqueReference { get; set; }
        public double ErrorFileSize { get; set; }
        public BulkUploadStatsViewModel Stats { get; set; }

        public bool ShowProblemWithServicePage { get { return !IsSuccess && BlobUniqueReference == Guid.Empty; } }
    }
}
