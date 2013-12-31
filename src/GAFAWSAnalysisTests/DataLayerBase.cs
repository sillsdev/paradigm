// --------------------------------------------------------------------------------------------
// Copyright (C) 2003-2013 SIL International. All rights reserved.
//
// Distributable under the terms of the MIT License, as specified in the license.rtf file.
// --------------------------------------------------------------------------------------------
using System.IO;
using SIL.WordWorks.GAFAWS.PositionAnalysis;

namespace SIL.WordWorks.GAFAWS.PositionAnalyser
{
	/// <summary>
	/// Base class for unit tests that work with a GAFAWSData object.
	/// </summary>
	public class DataLayerBase
	{
		/// <summary>
		/// Main Data layer object.
		/// </summary>
		protected IGafawsData _gd;

		/// <summary>
		/// Make a temporary file with the given contents.
		/// </summary>
		/// <param name="contents">The file's contents.</param>
		/// <returns>Pathname for the new file.</returns>
		protected string MakeFile(string contents)
		{
			var fileName = Path.GetTempFileName();
			if (string.IsNullOrEmpty(contents))
				return fileName;

			File.WriteAllText(fileName, contents);

			return fileName;
		}

		/// <summary>
		/// Make an empty temporary file.
		/// </summary>
		/// <returns>Pathname for the new file.</returns>
		protected string MakeFile()
		{
			return MakeFile(null);
		}

		/// <summary>
		/// Delete a file.
		/// </summary>
		/// <param name="fileName">Name of file to delete.</param>
		protected void DeleteFile(string fileName)
		{
			if (string.IsNullOrEmpty(fileName)) return;

			if (File.Exists(fileName))
				File.Delete(fileName);
		}
	}
}
