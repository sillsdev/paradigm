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
using System.Xml.Xsl;
using SIL.WordWorks.GAFAWS.PositionAnalysis;
using SIL.WordWorks.GAFAWS.PositionAnalysis.Properties;

namespace SIL.WordWorks.GAFAWS.ANAConverter
{
	/// <summary>
	/// Converts an Ample ANA file into an XML document suitable for input to GAFAWSAnalysis.
	/// </summary>
	public class AnaConverter : IGafawsConverter
	{
		private readonly IPositionAnalyzer m_analyzer;

		/// <summary>
		/// An instance of GAFAWSData.
		/// </summary>
		private IGafawsData m_gd;

		/// <summary>
		/// Constructor.
		/// </summary>
		public AnaConverter(IPositionAnalyzer analyzer, IGafawsData gd)
		{
			m_analyzer = analyzer;
			m_gd = gd;
			AnaObject.Reset();
		}

		#region IGAFAWSConverter implementation

		/// <summary>
		/// Do whatever it takes to convert the input this processor knows about.
		/// </summary>
		public void Convert()
		{
			using (var dlg = new AnaConverterDlg())
			{
				dlg.ShowDialog();
				if (dlg.DialogResult == DialogResult.OK)
				{
					string outputPathname = null;
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
							AnaObject.DataLayer = m_gd;

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
												record.Convert();
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
							record.Convert(); // Process last record.
						}

						// Main processing.
						m_analyzer.Process(m_gd);

						// Do any post-analysis processing here, if needed.
						// End of any optional post-processing.

						// Save, so it can be transformed.
						outputPathname = OutputPathServices.GetOutputPathname(anaPathname);
						m_gd.SaveData(outputPathname);

						// Transform.
						var trans = new XslCompiledTransform();
						try
						{
							trans.Load(XSLPathname);
						}
						catch
						{
							MessageBox.Show(PublicResources.kCouldNotLoadFile, PublicResources.kInformation);
							return;
						}

						var htmlOutput = Path.GetTempFileName() + ".html";
						try
						{
							trans.Transform(outputPathname, htmlOutput);
						}
						catch
						{
							MessageBox.Show(PublicResources.kCouldNotTransform, PublicResources.kInformation);
							return;
						}
						Process.Start(htmlOutput);
					}
					finally
					{
						if (parametersPathname != null && File.Exists(parametersPathname))
							File.Delete(parametersPathname);
						if (outputPathname != null && File.Exists(outputPathname))
							File.Delete(outputPathname);
					}
				}
			}

			// Reset m_gd, in case it gets called for another file.
			m_gd.Reset();
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
		public string XSLPathname
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
		/// <param name="message">The message to use in the exception.</param>
		private static void ThrowFileLoadException(TextReader reader, string pathInput, string message)
		{
			reader.Close();
			throw new FileLoadException(message, pathInput);
		}
	}
}
