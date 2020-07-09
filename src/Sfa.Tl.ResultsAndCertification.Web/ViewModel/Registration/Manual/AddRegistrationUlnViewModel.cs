﻿using Sfa.Tl.ResultsAndCertification.Common.Helpers;
using Sfa.Tl.ResultsAndCertification.Web.ViewComponents.BackLink;
using System.ComponentModel.DataAnnotations;
using ErrorResource = Sfa.Tl.ResultsAndCertification.Web.Content.Registration;

namespace Sfa.Tl.ResultsAndCertification.Web.ViewModel.Registration.Manual
{
    public class AddRegistrationUlnViewModel
    {
        [Required(ErrorMessageResourceType = typeof(ErrorResource.AddRegistrationUln), ErrorMessageResourceName = "Validation_Uln_Required")]
        [RegularExpression("^((?!(0))[0-9]{10})$", ErrorMessageResourceType = typeof(ErrorResource.AddRegistrationUln), ErrorMessageResourceName = "Validation_Uln_Must_Be_Digits")]
        public string Uln { get; set; }

        public bool IsEditMode { get; set; }

        public BackLinkModel BackLink
        {
            get
            {
                return new BackLinkModel
                {
                    RouteName = RouteConstants.RegistrationDashboard,
                };
            }
        }
    }
}