﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Sfa.Tl.ResultsAndCertification.Web.Content.IndustryPlacement {
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
    public class IndustryPlacementBanner {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal IndustryPlacementBanner() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Sfa.Tl.ResultsAndCertification.Web.Content.IndustryPlacement.IndustryPlacementBan" +
                            "ner", typeof(IndustryPlacementBanner).Assembly);
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
        ///   Looks up a localized string similar to Industry placement status added.
        /// </summary>
        public static string Banner_HeaderMesage {
            get {
                return ResourceManager.GetString("Banner_HeaderMesage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;p&gt;You have told us that this learner has completed their industry placement. You can change the industry placement status by selecting ‘change’.&lt;/p&gt;.
        /// </summary>
        public static string Success_Message_Completed {
            get {
                return ResourceManager.GetString("Success_Message_Completed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;p&gt;You have told us that this learner has completed their industry placement with special consideration. You can change the industry placement status by selecting ‘change’.&lt;/p&gt;.
        /// </summary>
        public static string Success_Message_Completed_With_Special_Consideration {
            get {
                return ResourceManager.GetString("Success_Message_Completed_With_Special_Consideration", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;p&gt;You have told us that this learner still needs to complete their industry placement. You can change the industry placement status once the learner has completed this.&lt;/p&gt;.
        /// </summary>
        public static string Success_Message_Still_Need_To_Complete {
            get {
                return ResourceManager.GetString("Success_Message_Still_Need_To_Complete", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;p&gt;You have told us that this learner will not complete their industry placement. If this changes, you can update the industry placement status by selecting ‘change’.&lt;/p&gt;.
        /// </summary>
        public static string Success_Message_Will_Not_Complete {
            get {
                return ResourceManager.GetString("Success_Message_Will_Not_Complete", resourceCulture);
            }
        }
    }
}
