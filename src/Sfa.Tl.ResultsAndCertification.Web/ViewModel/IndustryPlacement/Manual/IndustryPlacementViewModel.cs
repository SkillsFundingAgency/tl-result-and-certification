﻿namespace Sfa.Tl.ResultsAndCertification.Web.ViewModel.IndustryPlacement.Manual
{
    public class IndustryPlacementViewModel
    {
        public IpCompletionViewModel IpCompletion { get; set; }
        public IpModelViewModel IpModelViewModel { get; set; }
        public SpecialConsiderationViewModel SpecialConsideration { get; set; }
        public IpTempFlexibilityViewModel TempFlexibility { get; set; }

        public bool IsChangeModeAllowed { get; set; }


        public void ResetChangeMode()
        {
            IpCompletion.IsChangeMode = false;

            if (SpecialConsideration != null)
            {
                SpecialConsideration.Hours.IsChangeMode = false;
                SpecialConsideration.Reasons.IsChangeMode = false;
            }

            //// TODO: Delete below.
            //if (IpModelViewModel.IpModelUsed != null ||
            //    IpModelViewModel.IpMultiEmployerUsed != null ||
            //    IpModelViewModel.IpMultiEmployerOther != null ||
            //    IpModelViewModel.IpMultiEmployerSelect != null ||
            //    TempFlexibility != null)
            //{
            //    throw new System.Exception();
            //}
        }
    }
}