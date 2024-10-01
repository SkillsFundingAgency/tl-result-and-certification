﻿using System.IO;
using System.Threading.Tasks;

namespace Sfa.Tl.ResultsAndCertification.Web.Loader.Interfaces
{
    public interface IDocumentLoader
    {
        Task<Stream> GetBulkUploadRegistrationsTechSpecFileAsync(string fileName);
        Task<Stream> GetBulkUploadWithdrawalsTechSpecFileAsync(string fileName);
        Task<Stream> GetBulkUploadRommTechSpecFileAsync(string fileName);
        Task<Stream> GetBulkUploadAssessmentEntriesTechSpecFileAsync(string fileName);
        Task<Stream> GetTechSpecFileAsync(string folderName, string fileName);
    }
}
