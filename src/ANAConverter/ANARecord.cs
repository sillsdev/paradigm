// --------------------------------------------------------------------------------------------
// Copyright (C) 2003-2013 SIL International. All rights reserved.
//
// Distributable under the terms of the MIT License, as specified in the license.rtf file.
// --------------------------------------------------------------------------------------------
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
		static protected IGafawsData Gd;

		/// <summary>
		/// Get or set the data layer object.
		/// </summary>
		internal static IGafawsData DataLayer
		{
			set { Gd = value; }
			get { return Gd; }
		}

		internal static void Reset()
		{
			Gd = null;
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
		static private char _ambiguityCharacter = '%';
		protected List<AnaAnalysis> Analyses;

		new internal static void Reset()
		{
			_ambiguityCharacter = '%';
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
			_ambiguityCharacter = pams.Marker.Ambiguity;
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
			Analyses = new List<AnaAnalysis>();
			ProcessALine(line);
		}

		/// <summary>
		/// Convert the object to the data layer.
		/// </summary>
		internal void Convert(IWordRecordFactory wordRecordFactory, IMorphemeFactory morphemeFactory, IStemFactory stemFactory, IAffixFactory affixFactory)
		{
			foreach (var t in Analyses)
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
					Analyses.Add(new AnaAnalysis(contents[i]));
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
						   && (possibleAmbChar == _ambiguityCharacter);
				}
			return false;
		}

		/// <summary>
		/// Process the \w field.
		/// </summary>
		/// <param name="originalForm">The original wordform.</param>
		internal void ProcessWLine(string originalForm)
		{
			foreach (var t in Analyses)
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
			if (Analyses.Count != contents.Length - 3)
				return;
			for (var i = 0; i < Analyses.Count; ++i)
				Analyses[i].ProcessContent(type, contents[i + 2]);
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
			var contents = line.Trim().Split(_ambiguityCharacter);
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
