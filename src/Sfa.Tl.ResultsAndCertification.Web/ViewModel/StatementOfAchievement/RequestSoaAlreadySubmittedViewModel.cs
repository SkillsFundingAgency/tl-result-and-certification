﻿using Sfa.Tl.ResultsAndCertification.Common.Enum;
using Sfa.Tl.ResultsAndCertification.Common.Extensions;
using Sfa.Tl.ResultsAndCertification.Common.Helpers;
using Sfa.Tl.ResultsAndCertification.Web.ViewComponents.Breadcrumb;
using Sfa.Tl.ResultsAndCertification.Web.ViewComponents.Summary.SummaryItem;
using System;
using System.Collections.Generic;
using System.Linq;
using SoaRequestedAlreadyContent = Sfa.Tl.ResultsAndCertification.Web.Content.StatementOfAchievement.RequestSoaAlreadySubmitted;
using BreadcrumbContent = Sfa.Tl.ResultsAndCertification.Web.Content.ViewComponents.Breadcrumb;
using Sfa.Tl.ResultsAndCertification.Web.Helpers;
using Sfa.Tl.ResultsAndCertification.Models.Contracts.StatementOfAchievement;

namespace Sfa.Tl.ResultsAndCertification.Web.ViewModel.StatementOfAchievement
{
    public class RequestSoaAlreadySubmittedViewModel
    {
        public SoaPrintingDetails SnapshotDetails { get; set; }
        public RegistrationPathwayStatus PathwayStatus { get; set; }
        public string RequestedBy { get; set; }
        public DateTime RequestedOn { get; set; }
        public string RequestedDate { get { return RequestedOn.ToDobFormat(); } }

        public SummaryItemModel SummaryUln => new()
        {
            Id = "uln",
            Title = SoaRequestedAlreadyContent.Title_Uln_Text,
            Value = SnapshotDetails.Uln.ToString()
        };

        public SummaryItemModel SummaryLearnerName => new()
        {
            Id = "learnername",
            Title = SoaRequestedAlreadyContent.Title_Name_Text,
            Value = SnapshotDetails.Name
        };

        public SummaryItemModel SummaryDateRequested => new()
        {
            Id = "daterequested",
            Title = SoaRequestedAlreadyContent.Title_DateRequested_Text,
            Value = RequestedDate
        };

        public SummaryItemModel SummaryRequestedBy => new()
        {
            Id = "requestedby",
            Title = SoaRequestedAlreadyContent.Title_RequestedBy_Text,
            Value = RequestedBy
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
                        new BreadcrumbItem { DisplayName = BreadcrumbContent.Request_Statement_Of_Achievement, RouteName = RouteConstants.RequestStatementOfAchievement }                    }
                };
            }
        }
        
        public bool IsValid(int requestAllowedInDays)
        {
            return PathwayStatus == RegistrationPathwayStatus.Withdrawn && CommonHelper.IsSoaAlreadyRequested(requestAllowedInDays, RequestedOn);
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
}
