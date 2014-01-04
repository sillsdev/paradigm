// --------------------------------------------------------------------------------------------
// Copyright (C) 2003-2013 SIL International. All rights reserved.
//
// Distributable under the terms of the MIT License, as specified in the license.rtf file.
// --------------------------------------------------------------------------------------------
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
				var runningAssemblyPath = typeof(Program).Assembly.CodeBase.Substring (8);
				var runningAssemblyFolder = Path.GetDirectoryName(typeof (Program).Assembly.CodeBase.Substring(8));
				if (runningAssemblyFolder.StartsWith ("home"))
					runningAssemblyFolder = "/" + runningAssemblyFolder;
				catalog.Catalogs.Add(new DirectoryCatalog(runningAssemblyFolder));

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