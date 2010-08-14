// <copyright from='2003' to='2010' company='SIL International'>
//    Copyright (c) 2007, SIL International. All Rights Reserved.
// </copyright>
//
// File: ANAConverter.cs
// Responsibility: Randy Regnier
// Last reviewed:
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using SIL.WordWorks.GAFAWS.PositionAnalysis;

namespace SIL.WordWorks.GAFAWS.ANAConverter
{
	/// <summary>
	/// Converts an Ample ANA file into an XML document suitable for input to GAFAWSAnalysis.
	/// </summary>
	public class AnaConverter : IGafawsConverter
	{
		private readonly IWordRecordFactory _wordRecordFactory;
		private readonly IAffixFactory _affixFactory;
		private readonly IStemFactory _stemFactory;
		private readonly IMorphemeFactory _morphemeFactory;

		/// <summary>
		/// Constructor.
		/// </summary>
		internal AnaConverter(
			IWordRecordFactory wordRecordFactory,
			IAffixFactory affixFactory,
			IStemFactory stemFactory,
			IMorphemeFactory morphemeFactory)
		{
			_wordRecordFactory = wordRecordFactory;
			_affixFactory = affixFactory;
			_stemFactory = stemFactory;
			_morphemeFactory = morphemeFactory;
			AnaObject.Reset();
		}

		#region IGAFAWSConverter implementation

		/// <summary>
		/// Do whatever it takes to convert the input this processor knows about.
		/// </summary>
		public string Convert(IGafawsData gafawsData)
		{
			using (var dlg = new AnaConverterDlg())
			{
				dlg.ShowDialog();
				if (dlg.DialogResult != DialogResult.OK)
					return null;

				string outputPathname;
				string parametersPathname = null;
				try
				{
					parametersPathname = dlg.ParametersPathname;
					var anaPathname = dlg.AnaPathname;
					using (var reader = new StreamReader(anaPathname)) // Client to catch any exception.
					{
						AnaRecord record = null;
						var line = reader.ReadLine();
						AnaRecord.SetParameters(parametersPathname);
						AnaObject.DataLayer = gafawsData;

						// Sanity checks.
						if (line == null)
							ThrowFileLoadException(reader, anaPathname, "ANA File is empty");

						while (!line.StartsWith("\\a"))
						{
							line = line.Trim();
							if ((line != "") || ((line = reader.ReadLine()) == null))
								ThrowFileLoadException(reader, anaPathname, "Does not appear to be an ANA file.");
						}

						while (line != null)
						{
							switch (line.Split()[0])
							{
								case "\\a":
									{
										if (record != null)
											record.Convert(_wordRecordFactory, _morphemeFactory, _stemFactory, _affixFactory);
										record = new AnaRecord(line.Substring(3));
										break;
									}
								case "\\w":
									{
										record.ProcessWLine(line.Substring(3));
										break;
									}
								case "\\u":
									{
										record.ProcessOtherLine(LineType.UnderlyingForm, line.Substring(3));
										break;
									}
								case "\\d":
									{
										record.ProcessOtherLine(LineType.Decomposition, line.Substring(3));
										break;
									}
								case "\\cat":
									{
										record.ProcessOtherLine(LineType.Category, line.Substring(5));
										break;
									}
								default:
									// Eat this line.
									break;
							}
							line = reader.ReadLine();
						}
						Debug.Assert(record != null);
						record.Convert(_wordRecordFactory, _morphemeFactory, _stemFactory, _affixFactory); // Process last record.
					}
					outputPathname = OutputPathServices.GetOutputPathname(anaPathname);
				}
				finally
				{
					if (parametersPathname != null && File.Exists(parametersPathname))
						File.Delete(parametersPathname);
				}
				return outputPathname;
			}
		}

		/// <summary>
		/// Optional processing after the conversion and analysis has been done.
		/// </summary>
		/// <param name="gafawsData"></param>
		public void PostAnalysisProcessing(IGafawsData gafawsData)
		{
		}

		/// <summary>
		/// Gets the name of the converter that is suitable for display in a list
		/// of other converts.
		/// </summary>
		public string Name
		{
			get { return "ANA converter"; }
		}

		/// <summary>
		/// Gets a description of the converter that is suitable for display.
		/// </summary>
		public string Description
		{
			get
			{
				return "Process an AMPLE ANA output file."
					+ Environment.NewLine + Environment.NewLine
					+ "The '\\a' and '\\w' fields are required to be in the ANA file. Other filedes are optional.";
			}
		}

		/// <summary>
		/// Gets the pathname of the XSL file used to turn the XML into HTML.
		/// </summary>
		public string XslPathname
		{
			get
			{
				return OutputPathServices.GetXslPathname("AffixPositionChart_ANA.xsl");
			}
		}

		#endregion IGAFAWSConverter implementation

		/// <summary>
		/// Close the reader, and throw a FileLoadException.
		/// </summary>
		/// <param name="reader">The reader to close.</param>
		/// <param name="pathInput">Input pathname for invalid file.</param>
		/// <param name="message">The Message to use in the exception.</param>
		private static void ThrowFileLoadException(TextReader reader, string pathInput, string message)
		{
			reader.Close();
			throw new FileLoadException(message, pathInput);
		}
	}
}
