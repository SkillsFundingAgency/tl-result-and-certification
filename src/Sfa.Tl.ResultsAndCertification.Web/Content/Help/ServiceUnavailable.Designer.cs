﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Sfa.Tl.ResultsAndCertification.Web.Content.Help {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class ServiceUnavailable {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal ServiceUnavailable() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Sfa.Tl.ResultsAndCertification.Web.Content.Help.ServiceUnavailable", typeof(ServiceUnavailable).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Sorry, the service is unavailable.
        /// </summary>
        public static string Heading_Sorry_Service_Unavailable {
            get {
                return ResourceManager.GetString("Heading_Sorry_Service_Unavailable", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Service unavailable.
        /// </summary>
        public static string Page_Title {
            get {
                return ResourceManager.GetString("Page_Title", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to If you have any queries, contact:.
        /// </summary>
        public static string Para_Contact {
            get {
                return ResourceManager.GetString("Para_Contact", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The Manage T Level results service is unavailable while we process and calculate overall T Level results..
        /// </summary>
        public static string Para_Unavailable_While_Results_Calculation {
            get {
                return ResourceManager.GetString("Para_Unavailable_While_Results_Calculation", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You will be able to use the service from 12:00{0}..
        /// </summary>
        public static string Para_Use_Service_From {
            get {
                return ResourceManager.GetString("Para_Use_Service_From", resourceCulture);
            }
        }
    }
}