﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.296
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SIL.WordWorks.GAFAWS.PlainWordlistConverter.Properties {
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
	internal class Resources {

		private static global::System.Resources.ResourceManager resourceMan;

		private static global::System.Globalization.CultureInfo resourceCulture;

		[global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		internal Resources() {
		}

		/// <summary>
		///   Returns the cached ResourceManager instance used by this class.
		/// </summary>
		[global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
		internal static global::System.Resources.ResourceManager ResourceManager {
			get {
				if (object.ReferenceEquals(resourceMan, null)) {
					global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("SIL.WordWorks.GAFAWS.PlainWordlistConverter.Properties.Resources", typeof(Resources).Assembly);
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
		///   Looks up a localized string similar to All files.
		/// </summary>
		internal static string kAllFiles {
			get {
				return ResourceManager.GetString("kAllFiles", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Process a wordlist file where each item is on its own line. The list must follow this pattern:{0}{0}p1-p2-&lt;stem&gt;-s1-s2{0}{0}A hyphen (-) follows each prefix and precedes each suffix.{0}{0}Affixes are optional, but the stem/root is not. The content between the stem markers (&lt; and &gt;) is up to the user.{0}{0}An optional comment may be included after the main data. Comments start with a semi-colon (;) and continue to the end of the line..
		/// </summary>
		internal static string kDescription {
			get {
				return ResourceManager.GetString("kDescription", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Wordlist converter.
		/// </summary>
		internal static string kName {
			get {
				return ResourceManager.GetString("kName", resourceCulture);
			}
		}

		/// <summary>
		///   Looks up a localized string similar to Plain Text files.
		/// </summary>
		internal static string kPlainTexFiles {
			get {
				return ResourceManager.GetString("kPlainTexFiles", resourceCulture);
			}
		}
	}
}
