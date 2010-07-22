// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2007, SIL International. All Rights Reserved.
// <copyright from='2007' to='2010' company='SIL International'>
//		Copyright (c) 2007, SIL International. All Rights Reserved.
//
//		Distributable under the terms of either the Common Public License or the
//		GNU Lesser General Public License, as specified in the LICENSING.txt file.
// </copyright>
#endregion
//
// File: Program.cs
// Responsibility: Randy Regnier
//
// <remarks>
// </remarks>
// ---------------------------------------------------------------------------------------------
using System;
using System.Windows.Forms;

namespace SIL.GAFAWS.Apps.AffixPositionAnalyzer
{
	/// ----------------------------------------------------------------------------------------
	/// <summary></summary>
	/// ----------------------------------------------------------------------------------------
	static class Program
	{
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new AffixPositionAnalyzer());
		}
	}
}