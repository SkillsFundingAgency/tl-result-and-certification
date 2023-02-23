﻿using System;

namespace Sfa.Tl.ResultsAndCertification.Domain.Models
{
    public partial class DocumentUploadHistory : BaseEntity
    {
        public int? TlAwardingOrganisationId { get; set; }
        public int? TlProviderId { get; set; }
        public Guid BlobUniqueReference { get; set; }
        public string BlobFileName { get; set; }
        public int DocumentType { get; set; }
        public int FileType { get; set; }
        public int Status { get; set; }

        public virtual TlAwardingOrganisation TlAwardingOrganisation { get; set; }
        public virtual TlProvider TlProvider { get; set; }
    }
}