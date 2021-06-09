﻿using Sfa.Tl.ResultsAndCertification.Common.Enum;
using Sfa.Tl.ResultsAndCertification.Common.Extensions;
using Sfa.Tl.ResultsAndCertification.Common.Helpers;
using Sfa.Tl.ResultsAndCertification.Web.ViewComponents.Breadcrumb;
using Sfa.Tl.ResultsAndCertification.Web.ViewComponents.Summary.SummaryItem;
using Sfa.Tl.ResultsAndCertification.Web.ViewModel.ProviderAddress;
using System;
using System.Collections.Generic;
using System.Linq;
using SoaRequestedAlreadyContent = Sfa.Tl.ResultsAndCertification.Web.Content.StatementOfAchievement.RequestSoaSubmittedAlready;
using BreadcrumbContent = Sfa.Tl.ResultsAndCertification.Web.Content.ViewComponents.Breadcrumb;

namespace Sfa.Tl.ResultsAndCertification.Web.ViewModel.StatementOfAchievement
{
    public class RequestSoaSubmittedAlreadyViewModel
    {
        public RequestSnapshotDetails SnapshotDetails { get; set; }
        public RegistrationPathwayStatus PathwayStatus { get; set; }
        public string RequestedBy { get; set; }
        public DateTime RequestedOn { get; set; }
        public string RequestedDate { get { return RequestedOn.ToDobFormat(); } }

        public SummaryItemModel SummaryUln => new SummaryItemModel
        {
            Id = "uln",
            Title = SoaRequestedAlreadyContent.Title_Uln_Text,
            Value = SnapshotDetails.Uln.ToString()
        };

        public SummaryItemModel SummaryLearnerName => new SummaryItemModel
        {
            Id = "learnername",
            Title = SoaRequestedAlreadyContent.Title_Name_Text,
            Value = SnapshotDetails.Name
        };

        public SummaryItemModel SummaryDateofBirth => new SummaryItemModel
        {
            Id = "dateofbirth",
            Title = SoaRequestedAlreadyContent.Title_DateofBirth_Text,
            Value = SnapshotDetails.Dateofbirth.ToDobFormat()
        };

        public SummaryItemModel SummaryProvider => new SummaryItemModel
        {
            Id = "providername",
            Title = SoaRequestedAlreadyContent.Title_Provider_Text,
            Value = SnapshotDetails.ProviderName
        };

        public SummaryItemModel SummaryTlevelTitle => new SummaryItemModel
        {
            Id = "tleveltitle",
            Title = SoaRequestedAlreadyContent.Title_Tlevel_Title_Text,
            Value = SnapshotDetails.TlevelTitle
        };

        public SummaryItemModel SummaryCoreCode => new SummaryItemModel
        {
            Id = "corecode",
            Title = SoaRequestedAlreadyContent.Title_Core_Code_Text,
            Value = string.Format(SoaRequestedAlreadyContent.Core_Code_Value, SnapshotDetails.Core, SnapshotDetails.CoreGrade),
            IsRawHtml = true
        };

        public SummaryItemModel SummarySpecialismCode => new SummaryItemModel
        {
            Id = "specialismcode",
            Title = SoaRequestedAlreadyContent.Title_Occupational_Specialism_Text,
            Value = string.Format(SoaRequestedAlreadyContent.Occupational_Specialism_Value, SnapshotDetails.Specialism, SnapshotDetails.SpecialismGrade),
            IsRawHtml = true
        };

        public SummaryItemModel SummaryEnglishAndMaths => new SummaryItemModel
        {
            Id = "englishandmaths",
            Title = SoaRequestedAlreadyContent.Title_English_And_Maths_Text,
            Value = SnapshotDetails.EnglishAndMaths
        };

        public SummaryItemModel SummaryIndustryPlacement => new SummaryItemModel
        {
            Id = "industryplacement",
            Title = SoaRequestedAlreadyContent.Title_Industry_Placement_Text,
            Value = SnapshotDetails.IndustryPlacement
        };

        public SummaryItemModel SummaryDepartment => new SummaryItemModel
        {
            Id = "department",
            Title = SoaRequestedAlreadyContent.Title_Department_Text,
            Value = SnapshotDetails.ProviderAddress?.DepartmentName
        };

        public SummaryItemModel SummaryAddress => new SummaryItemModel
        {
            Id = "address",
            Title = SoaRequestedAlreadyContent.Title_Organisation_Address_Text,
            Value = string.Format(SoaRequestedAlreadyContent.Organisation_Address_Value, FormatedAddress),
            IsRawHtml = true
        };

        public BreadcrumbModel Breadcrumb
        {
            get
            {
                return new BreadcrumbModel
                {
                    BreadcrumbItems = new List<BreadcrumbItem>
                    {
                        new BreadcrumbItem { DisplayName = BreadcrumbContent.Home, RouteName = RouteConstants.Home },
                        new BreadcrumbItem { DisplayName = BreadcrumbContent.Request_Statement_Of_Achievement, RouteName = RouteConstants.RequestStatementOfAchievement },
                        new BreadcrumbItem { DisplayName = BreadcrumbContent.Search_For_Learner, RouteName = RouteConstants.RequestSoaUniqueLearnerNumber },
                        new BreadcrumbItem { DisplayName = BreadcrumbContent.StatementOfAchievementRequested }
                    }
                };
            }
        }
        
        public bool IsValid(int requestAllowedInDays)
        {
            return PathwayStatus == RegistrationPathwayStatus.Withdrawn && IsSoaRequestedAlready(requestAllowedInDays);
        }

        private bool IsSoaRequestedAlready(int reRequestAllowedInDays)
        {
            return RequestedOn > DateTime.Now.AddDays(-reRequestAllowedInDays);
        }

        private string FormatedAddress
        {
            get
            {
                var addressLines = new List<string> { SnapshotDetails.ProviderAddress?.OrganisationName, SnapshotDetails.ProviderAddress?.AddressLine1, SnapshotDetails.ProviderAddress?.AddressLine2, SnapshotDetails.ProviderAddress?.Town, SnapshotDetails.ProviderAddress?.Postcode };
                return string.Join(SoaRequestedAlreadyContent.Html_Line_Break, addressLines.Where(x => !string.IsNullOrWhiteSpace(x)));
            }
        }
    }

    public class RequestSnapshotDetails 
    {
        //Learner's registration details
        public int ProfileId { get; set; }
        public long Uln { get; set; }
        public string Name { get; set; }
        public DateTime Dateofbirth { get; set; }
        public string ProviderName { get; set; }

        //Learner's technical qualification details
        public string TlevelTitle { get; set; }
        public string Core { get; set; }
        public string CoreGrade { get; set; }
        public string Specialism { get; set; }
        public string SpecialismGrade { get; set; }

        //Learner's T level component achievements
        public string EnglishAndMaths { get; set; }
        public string IndustryPlacement { get; set; }

        // Provider Organisation's postal address
        public AddressViewModel ProviderAddress { get; set; }
    }
}
