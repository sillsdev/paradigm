﻿// <copyright from='2003' to='2010' company='SIL International'>
//    Copyright (c) 2007, SIL International. All Rights Reserved.
// </copyright>
//
// File: ANARecord.cs
// Responsibility: Randy Regnier
// Last reviewed:
using System;
using System.Collections.Generic;
using System.IO;
using SIL.WordWorks.GAFAWS.PositionAnalysis;

namespace SIL.WordWorks.GAFAWS.ANAConverter
{
	/// <summary>
	/// Enumeration of field types that get processed.
	/// </summary>
	internal enum LineType
	{
		UnderlyingForm,
		Decomposition,
		Category
	};

	/// <summary>
	/// Abstract superclass of all other ANA objects, which serves to hold the data layer.
	/// </summary>
	internal abstract class AnaObject
	{
		static protected IGafawsData s_gd;

		/// <summary>
		/// Get or set the data layer object.
		/// </summary>
		internal static IGafawsData DataLayer
		{
			set { s_gd = value; }
			get { return s_gd; }
		}

		internal static void Reset()
		{
			s_gd = null;
			AnaRecord.Reset();
			AnaAnalysis.Reset();
		}
	}

	/// <summary>
	/// Representation of one ANA record in the file.
	/// </summary>
	internal class AnaRecord : AnaObject
	{
		/// <summary>
		/// Ambiguity character, default is: '%'.
		/// </summary>
		static private char s_ambiguityCharacter = '%';
		protected List<AnaAnalysis> m_analyses;

		new internal static void Reset()
		{
			s_ambiguityCharacter = '%';
		}

		/// <summary>
		/// Set the input parameters.
		/// </summary>
		/// <param name="parms">The pathname to the parameters file.</param>
		/// <exception cref="FileLoadException">
		/// Thrown when the input file is empty.
		/// </exception>
		internal static void SetParameters(string parms)
		{
			if (parms == null)
				return;

			var pams = Parameters.DeSerialize(parms);
			s_ambiguityCharacter = pams.Marker.Ambiguity;
			AnaAnalysis.OpenRootDelimiter = pams.RootDelimiter.OpenDelimiter;
			AnaAnalysis.CloseRootDelimiter = pams.RootDelimiter.CloseDelimiter;
			AnaAnalysis.SeparatorCharacter = pams.Marker.Decomposition;
			AnaAnalysis.PartsOfSpeech = pams.Categories;
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="line">The \a field without the field marker.</param>
		internal AnaRecord(string line)
		{
			m_analyses = new List<AnaAnalysis>();
			ProcessALine(line);
		}

		/// <summary>
		/// Convert the object to the data layer.
		/// </summary>
		internal void Convert(IWordRecordFactory wordRecordFactory, IMorphemeFactory morphemeFactory, IStemFactory stemFactory, IAffixFactory affixFactory)
		{
			foreach (var t in m_analyses)
				t.Convert(wordRecordFactory, morphemeFactory, stemFactory, affixFactory);
		}

		/// <summary>
		/// Process the \a field.
		/// </summary>
		/// <param name="line">The complete \a field, without the field marker.</param>
		protected void ProcessALine(string line)
		{
			if (IsCorrectAmbiguityCharacter(line))
			{
				var contents = TokenizeLine(line, true);
				for (var i = 2; i < contents.Length - 1; ++i)
					m_analyses.Add(new AnaAnalysis(contents[i]));
				return;
			}
			throw new ApplicationException("Incorrect ambiguity character.");
		}

		/// <summary>
		/// IsCorrectAmbiguityCharacter
		/// </summary>
		/// <param name="line"></param>
		/// <returns></returns>
		private static bool IsCorrectAmbiguityCharacter(string line)
		{
			// Not ambiguous.
			if (line.Length < 5) // Too short to be ambiguous.
				return true;
			var possibleAmbChar = line[line.Length - 1];
			if (possibleAmbChar != line[0]) // First and last not the same.
				return true;
			if (!Char.IsDigit(line[1])) // At least one digit
				return true;

			// Maybe ambiguous - find closing ambig char if there is one.
			for (var i = 2; i < line.Length - 2; ++i)
				if(!Char.IsDigit(line[i]))
				{
					return (line[i] == possibleAmbChar)
						   && (possibleAmbChar == s_ambiguityCharacter);
				}
			return false;
		}

		/// <summary>
		/// Process the \w field.
		/// </summary>
		/// <param name="originalForm">The original wordform.</param>
		internal void ProcessWLine(string originalForm)
		{
			foreach (var t in m_analyses)
				t.OriginalForm = originalForm;
		}

		/// <summary>
		/// Process the \d, \u, and \cat fields.
		/// </summary>
		/// <param name="type">The field being processed.</param>
		/// <param name="line">The complete line, without the field marker.</param>
		internal void ProcessOtherLine(LineType type, string line)
		{
			var contents = TokenizeLine(line, false);
			if (m_analyses.Count != contents.Length - 3)
				return;
			for (var i = 0; i < m_analyses.Count; ++i)
				m_analyses[i].ProcessContent(type, contents[i + 2]);
		}

		/// <summary>
		/// Tokenize the given line.
		/// </summary>
		/// <param name="line">The line to tokenize.</param>
		/// <param name="isALine">True, if the line being processed is the \a line, otherwise false.</param>
		/// <returns>An array of tokenized strings.
		/// [NB: The array is returned, as if it all were ambiguous.]</returns>
		protected string[] TokenizeLine(string line, bool isALine)
		{
			var contents = line.Trim().Split(s_ambiguityCharacter);
			if (contents.Length == 4 && isALine)
				contents[2] = null; // Analysis failure.
			else if (contents.Length == 1)
			{
				// Treat it the same as if it were ambiguous.
				// This makes other processing easier.
				var tempLine = contents[0];
				contents = new String[4];
				contents[2] = tempLine;
			}
			return contents;
		}
	}
}
