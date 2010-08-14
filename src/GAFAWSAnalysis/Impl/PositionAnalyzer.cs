// <copyright from='2003' to='2010' company='SIL International'>
//    Copyright (c) 2003, SIL International. All Rights Reserved.
// </copyright>
//
// File: PositionAnalyzer.cs
// Responsibility: Randy Regnier
// Last reviewed:
//
// <remarks>
// Implementation of PositionAnalyzer and its supporting class: MorphemeWrapper
// </remarks>
using System;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace SIL.WordWorks.GAFAWS.PositionAnalysis.Impl
{
	/// <summary>
	/// Main class for analyzing affix positions.
	/// </summary>
	public class PositionAnalyzer : IPositionAnalyzer
	{
		/// <summary>
		/// An instance of GAFAWSData.
		/// </summary>
		private GafawsData _gd;

		/// <summary>
		/// The prefixes that need to be processed.
		/// </summary>
		private Dictionary<string, MorphemeWrapper> _prefixes;

		/// <summary>
		/// The suffixes that need to be processed.
		/// </summary>
		private Dictionary<string, MorphemeWrapper> _suffixes;

		/// <summary>
		/// Process the given input file.
		/// </summary>
		/// <param name="pathInput">Pathname to the input data.</param>
		/// <returns>The pathname for the processed data.</returns>
		/// <remarks>Obly used by tests.</remarks>
		internal string Process(string pathInput)
		{
			// Replaces 8 lines in C++.
			var gData = GafawsData.LoadData(pathInput);
			if (gData == null)
				return null;

			Process(gData);

			var outPath = OutputPathServices.GetOutputPathname(pathInput);
			_gd.SaveData(outPath);
			return outPath;
		}

		/// <summary>
		/// Process the given input GAFAWS data object.
		/// </summary>
		/// <param name="gData">GAFAWSData to process.</param>
		public void Process(IGafawsData gData)
		{
			// Replaces 8 lines in C++.
			_gd = (GafawsData)gData;
			if (_gd == null)
				return;

			_prefixes = new Dictionary<string, MorphemeWrapper>();
			_suffixes = new Dictionary<string, MorphemeWrapper>();

			// Replaces CJGParadigm::GetAffixesFromDom()
			// 52 lines in C++.
			foreach(var m in _gd.Morphemes)
			{
				switch (m.Type)
				{
					case "pfx":
						_prefixes.Add(m.Id, new MorphemeWrapper(m));
						break;
					case "sfx":
						_suffixes.Add(m.Id, new MorphemeWrapper(m));
						break;
					//case "s":	// Skip stems.
				}
			}
			if ((_prefixes.Count + _suffixes.Count) == 0)
				return;	// Nothing to do.

			if (!AssignToSets())
				return;	// Nothing to do.

			// Find the classes for prefixes.
			var pca = new PrefixClassAssigner(_gd);
			if (!pca.AssignClasses(_prefixes))
				return;
			// Find the classes for suffixes.
			var sca = new SuffixClassAssigner(_gd);
			if (!sca.AssignClasses(_suffixes))
				return;

			foreach (var kvp in _prefixes)
				kvp.Value.SetStartAndEnd();
			foreach (var kvp in _suffixes)
				kvp.Value.SetStartAndEnd();

			// Set date and time.
			// Replaces 44 lines of C++ code.
			var dt = DateTime.Now.ToUniversalTime();
			_gd.Date = dt.ToLongDateString();
			_gd.Time = dt.ToLongTimeString();
		}

		/// <summary>
		/// Assign affixes to predecessor and successor sets.
		/// </summary>
		/// <returns>True if it assigned affixes to sets, otherwise false.</returns>
		protected bool AssignToSets()
		{
			// C++ took 79 lines.
			if (_gd.WordRecords.Count == 0)
				return false;	// Nothing to do.

			foreach(var wr in _gd.WordRecords)
			{
				List<IAffix> ac = null;
				Dictionary<string, MorphemeWrapper> affixes = null;
				for (var i = 0; i < 2; ++i)
				{
					switch (i)
					{
						case 0:	// Prefixes
							ac = wr.Prefixes;
							affixes = _prefixes;
							break;
						case 1:	// Suffixes
							ac = wr.Suffixes;
							affixes = _suffixes;
							break;
					}
					// NB: This will not run through the loop for cases where there is
					// only one affix, which is right, since it has nothing on either side.
					for (var iAfx = 1; ac != null && iAfx < ac.Count; ++iAfx)
					{
						var prevId = ac[iAfx -1].MidRef;
						var curId = ac[iAfx].MidRef;
						affixes[curId].AddAsSuccessor(prevId);
						affixes[prevId].AddAsPredecessor(curId);
					}
				}
			}
			return true;
		}
	}	// End class PositionAnalyzer


	/// <summary>
	/// A wrapper class for the data layer morpheme object.
	/// </summary>
	/// <remarks>
	/// This wrapper class allows us to keep track of useful information of a morpheme
	/// that is not supported by the data layer.
	/// </remarks>
	internal class MorphemeWrapper
	{
		/// <summary>
		/// The real morpheme.
		/// </summary>
		private readonly IMorpheme _morpheme;

		/// <summary>
		/// List of predecessor affixes.
		/// </summary>
		private readonly StringCollection _predecessors;

		/// <summary>
		/// List of successor affixes.
		/// </summary>
		private readonly StringCollection _successors;

		/// <summary>
		/// Starting class.
		/// </summary>
		private IClass _start;

		/// <summary>
		/// Ending class.
		/// </summary>
		private IClass _end;

		/// <summary>
		/// Cosntructor.
		/// </summary>
		/// <param name="m">The real morpheme that is wrapped.</param>
		public MorphemeWrapper(IMorpheme m)
		{
			_morpheme = m;
			_predecessors = new StringCollection();
			_successors = new StringCollection();
			_start = null;
			_end = null;
		}

		/// <summary>
		/// Adds a new affix as a successor, if it is not already in the list.
		/// </summary>
		/// <param name="succ">Affix ID.</param>
		public void AddAsSuccessor(string succ)
		{
			if (_successors.Cast<string>().Any(t => t == succ))
				return;

			_successors.Add(succ);
		}

		/// <summary>
		/// Adds a new affix as a predecessor, if it is not already in the list.
		/// </summary>
		/// <param name="pred">Affix ID.</param>
		public void AddAsPredecessor(string pred)
		{
			if (_predecessors.Cast<string>().Any(t => t == pred))
				return;

			_predecessors.Add(pred);
		}

		/// <summary>
		/// Set the starting and ending class IDREFs of the real morpheme.
		/// </summary>
		public void SetStartAndEnd()
		{
			// Replaces 41 lines in C++.
			if (_start != null)
				_morpheme.StartClass = _start.Id;
			if (_end != null)
				_morpheme.EndClass = _end.Id;
		}

		/// <summary>
		/// Remove an affix from the list of predecessors.
		/// </summary>
		/// <param name="pred">The affix ID to remove.</param>
		public void RemovePredecessor(string pred)
		{
			_predecessors.Remove(pred);
		}

		/// <summary>
		/// Remove an affix from the list of successors.
		/// </summary>
		/// <param name="succ">The affix ID to remove.</param>
		public void RemoveSuccessor(string succ)
		{
			_successors.Remove(succ);
		}

		/// <summary>
		/// Check to see if the morpheme can be assigned a class.
		/// </summary>
		/// <param name="listType">The type of list to check.</param>
		/// <returns>True if the class can be assigne, otherwise false.</returns>
		public bool CanAssignClass(ListType listType)
		{
			bool fRetVal;
			switch (listType)
			{
				default:
					fRetVal = false;
					break;
				case ListType.Pred:
					fRetVal = (_predecessors.Count == 0);
					break;
				case ListType.Succ:
					fRetVal = (_successors.Count == 0);
					break;
			}
			return fRetVal;
		}

		/// <summary>
		/// Set the starting or ending class.
		/// </summary>
		/// <param name="isStartPoint">True if the class is the starting class,
		/// otherwise false.</param>
		/// <param name="cls">The class to set.</param>
		public void SetAffixClass(bool isStartPoint, IClass cls)
		{
			if (isStartPoint)
			{
				if (_start == null)
					_start = cls;
				else
					Debug.Assert(false);
			}
			else
			{
				if (_end == null)
					_end = cls;
				else
					Debug.Assert(false);
			}
		}

		/// <summary>
		/// Get the ID of the wrapped morpheme.
		/// </summary>
		/// <returns>The ID of the wrapped morpheme.</returns>
		public string GetId()
		{
			return _morpheme.Id;
		}
	}
}
