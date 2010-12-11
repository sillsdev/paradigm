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
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Windows.Forms;

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

			//An aggregate catalog that combines multiple catalogs
			using (var catalog = new AggregateCatalog())
			{
				//catalog.Catalogs.Add(new AssemblyCatalog(typeof(Program).Assembly));
				catalog.Catalogs.Add(new DirectoryCatalog(Path.GetDirectoryName(typeof (Program).Assembly.CodeBase.Substring(8))));

				//Create the CompositionContainer with the parts in the catalog
				using (var container = new CompositionContainer(catalog))
				{
					var mainWind = new AffixPositionAnalyzer();
					container.ComposeParts(mainWind); // Import parts for main window.
					Application.Run(mainWind);
				}
			}
		}
	}
}