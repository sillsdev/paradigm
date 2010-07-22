// --------------------------------------------------------------------------------------------
// <copyright from='2003' to='2010' company='SIL International'>
//    Copyright (c) 2009, SIL International. All Rights Reserved.
// </copyright>
//
// File: DataLayerBase.cs
// Responsibility: Randy Regnier
//
// <remarks>
// Superclass for unit tests that work with a GAFAWSData object.
// </remarks>
// --------------------------------------------------------------------------------------------
using System.IO;

namespace SIL.WordWorks.GAFAWS.PositionAnalyser
{
	/// ---------------------------------------------------------------------------------------
	/// <summary>
	/// Base class for unit tests that work with a GAFAWSData object.
	/// </summary>
	/// ---------------------------------------------------------------------------------------
	public class DataLayerBase
	{
		/// -----------------------------------------------------------------------------------
		/// <summary>
		/// Main Data layer object.
		/// </summary>
		/// -----------------------------------------------------------------------------------
		protected GAFAWSData m_gd;

		/// -----------------------------------------------------------------------------------
		/// <summary>
		/// Make a temporary file with the given contents.
		/// </summary>
		/// <param name="contents">The file's contents.</param>
		/// <returns>Pathname for the new file.</returns>
		/// -----------------------------------------------------------------------------------
		protected string MakeFile(string contents)
		{
			var fileName = Path.GetTempFileName();
			if (string.IsNullOrEmpty(contents))
				return fileName;

			File.WriteAllText(fileName, contents);

			return fileName;
		}

		/// -----------------------------------------------------------------------------------
		/// <summary>
		/// Make an empty temporary file.
		/// </summary>
		/// <returns>Pathname for the new file.</returns>
		/// -----------------------------------------------------------------------------------
		protected string MakeFile()
		{
			return MakeFile(null);
		}

		/// -----------------------------------------------------------------------------------
		/// <summary>
		/// Delete a file.
		/// </summary>
		/// <param name="fileName">Name of file to delete.</param>
		/// -----------------------------------------------------------------------------------
		protected void DeleteFile(string fileName)
		{
			if (string.IsNullOrEmpty(fileName)) return;

			if (File.Exists(fileName))
				File.Delete(fileName);
		}
	}
}
