// <copyright from='2007' to='2010' company='SIL International'>
//		Copyright (c) 2007, SIL International. All Rights Reserved.
//
//		Distributable under the terms of either the Common Public License or the
//		GNU Lesser General Public License, as specified in the LICENSING.txt file.
// </copyright>
//
// File: Program.cs
// Responsibility: Randy Regnier
using System;
using System.Windows.Forms;
using StructureMap;

namespace SIL.WordWorks.GAFAWS.AffixPositionAnalyzer
{
	/// <summary></summary>
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			var container = new Container();
			container.Configure(c=> c.AddRegistry(new AppRegistry()));
			Application.Run(container.GetInstance<AffixPositionAnalyzer>());
		}
	}
}