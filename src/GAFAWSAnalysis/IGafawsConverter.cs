// <copyright from='2003' to='2010' company='SIL International'>
//    Copyright (c) 2003, SIL International. All Rights Reserved.
// </copyright>
//
// File: IGafawsConverter.cs
// Responsibility: Randy Regnier
// Last reviewed:
//
// <remarks>
// Declaration of the IGafawsConverter interface.
// </remarks>
namespace SIL.WordWorks.GAFAWS.PositionAnalysis
{
	/// <summary>
	/// Interface declaration.
	/// Data converters should implement this interface, if they want to take source data and
	/// convert it into data to be fed into the PositionAnalyzer class in the GAFAWSAnalysis
	/// assembly (which does the work of analyzing the affix positions)
	/// </summary>
	public interface IGafawsConverter
	{
		/// <summary>
		/// Do whatever it takes to convert the input this processor knows about.
		/// </summary>
		/// <param name="gafawsData">
		/// An empty IGafawsData instance,
		/// which is where the converted data ends up.
		/// </param>
		/// <returns>
		/// The pathname of the converted data.
		/// </returns>
		/// <remarks>
		/// Implementors may call "Save" on <paramref name="gafawsData"/>, or not, as they choose.
		/// </remarks>
		string Convert(IGafawsData gafawsData);

		/// <summary>
		/// Optional processing after the conversion and analysis has been done.
		/// </summary>
		/// <param name="gafawsData"></param>
		void PostAnalysisProcessing(IGafawsData gafawsData);

		/// <summary>
		/// Gets the name of the converter that is suitable for display in a list
		/// of other converters.
		/// </summary>
		string Name
		{
			get;
		}

		/// <summary>
		/// Gets a description of the converter that is suitable for display.
		/// </summary>
		string Description
		{
			get;
		}

		/// <summary>
		/// Gets the pathname of the XSL file used to turn the XML into HTML.
		/// </summary>
		string XslPathname
		{
			get;
		}
	}
}
