﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18052
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ZillaIoc.Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class ExceptionMessages {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal ExceptionMessages() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("ZillaIoc.Resources.ExceptionMessages", typeof(ExceptionMessages).Assembly);
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
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot create an instance of abstract type &apos;{0}&apos;..
        /// </summary>
        internal static string CannotCreateInstanceOfAbstractType {
            get {
                return ResourceManager.GetString("CannotCreateInstanceOfAbstractType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Multiple unnamed bindings for Service type &apos;{0}&apos; were found. Cannot have multiple default bindings for the same Service type. {1} Bindings found : {2}..
        /// </summary>
        internal static string CannotHaveMultipleDefaultBindingsForService {
            get {
                return ResourceManager.GetString("CannotHaveMultipleDefaultBindingsForService", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Multiple named bindings for Service type &apos;{0}&apos; with the name &apos;{1}&apos; were found. Cannot have multiple named bindings for the same Service type with the same name. {2} Bindings found : {3}..
        /// </summary>
        internal static string CannotHaveMultipleNamedBindingsForServiceWithSameName {
            get {
                return ResourceManager.GetString("CannotHaveMultipleNamedBindingsForServiceWithSameName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No bindings found for Service type &apos;{0}&apos;. Cannot create an instance because type is abstract..
        /// </summary>
        internal static string CannotResolveAbstractServiceTypeWithNoBinding {
            get {
                return ResourceManager.GetString("CannotResolveAbstractServiceTypeWithNoBinding", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot plug in abstract type &apos;{0}&apos; to Service type &apos;{1}&apos;. Abstract types cannot be used as Plugin types..
        /// </summary>
        internal static string CannotUseAnAbstractTypeForAPluginType {
            get {
                return ResourceManager.GetString("CannotUseAnAbstractTypeForAPluginType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot determine default binding for service type &apos;{0}&apos;. Multiple default bindings found..
        /// </summary>
        internal static string MultipleDefaultBindingsFoundForService {
            get {
                return ResourceManager.GetString("MultipleDefaultBindingsFoundForService", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot determine named binding for service type &apos;{0}&apos; with name &apos;{1}&apos;. Multiple bindings found with same name..
        /// </summary>
        internal static string MultipleNamedBindingsFoundForServiceWithSameName {
            get {
                return ResourceManager.GetString("MultipleNamedBindingsFoundForServiceWithSameName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No named bindings for Service type &apos;{0}&apos; with name &apos;{1}&apos; were found..
        /// </summary>
        internal static string UnknownNamedService {
            get {
                return ResourceManager.GetString("UnknownNamedService", resourceCulture);
            }
        }
    }
}