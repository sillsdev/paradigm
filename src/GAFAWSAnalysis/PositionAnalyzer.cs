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

namespace SIL.WordWorks.GAFAWS.PositionAnalysis
{
	/// <summary>
	/// Main class for analyzing affix positions.
	/// </summary>
	public class PositionAnalyzer : IPositionAnalyzer
	{
		/// <summary>
		/// An instance of GAFAWSData.
		/// </summary>
		private GafawsData m_gd;

		/// <summary>
		/// The prefixes that need to be processed.
		/// </summary>
		private Dictionary<string, MorphemeWrapper> m_prefixes;

		/// <summary>
		/// The suffixes that need to be processed.
		/// </summary>
		private Dictionary<string, MorphemeWrapper> m_suffixes;

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
			m_gd.SaveData(outPath);
			return outPath;
		}

		/// <summary>
		/// Process the given input GAFAWS data object.
		/// </summary>
		/// <param name="gData">GAFAWSData to process.</param>
		public void Process(IGafawsData gData)
		{
			// Replaces 8 lines in C++.
			m_gd = (GafawsData)gData;
			if (m_gd == null)
				return;

			m_prefixes = new Dictionary<string, MorphemeWrapper>();
			m_suffixes = new Dictionary<string, MorphemeWrapper>();

			// Replaces CJGParadigm::GetAffixesFromDom()
			// 52 lines in C++.
			foreach(var m in m_gd.Morphemes)
			{
				switch (m.type)
				{
					case "pfx":
						m_prefixes.Add(m.MID, new MorphemeWrapper(m));
						break;
					case "sfx":
						m_suffixes.Add(m.MID, new MorphemeWrapper(m));
						break;
					//case "s":	// Skip stems.
				}
			}
			if ((m_prefixes.Count + m_suffixes.Count) == 0)
				return;	// Nothing to do.

			if (!AssignToSets())
				return;	// Nothing to do.

			// Find the classes for prefixes.
			var pca = new PrefixClassAssigner(m_gd);
			if (!pca.AssignClasses(m_prefixes))
				return;
			// Find the classes for suffixes.
			var sca = new SuffixClassAssigner(m_gd);
			if (!sca.AssignClasses(m_suffixes))
				return;

			foreach (var kvp in m_prefixes)
				kvp.Value.SetStartAndEnd();
			foreach (var kvp in m_suffixes)
				kvp.Value.SetStartAndEnd();

			// Set date and time.
			// Replaces 44 lines of C++ code.
			var dt = DateTime.Now;
			m_gd.date = dt.ToLongDateString();
			m_gd.time = dt.ToLongTimeString();
		}

		/// <summary>
		/// Assign affixes to predecessor and successor sets.
		/// </summary>
		/// <returns>True if it assigned affixes to sets, otherwise false.</returns>
		protected bool AssignToSets()
		{
			// C++ took 79 lines.
			if (m_gd.WordRecords.Count == 0)
				return false;	// Nothing to do.

			foreach(var wr in m_gd.WordRecords)
			{
				List<Affix> ac = null;
				Dictionary<string, MorphemeWrapper> affixes = null;
				for (var i = 0; i < 2; ++i)
				{
					switch (i)
					{
						case 0:	// Prefixes
							ac = wr.Prefixes;
							affixes = m_prefixes;
							break;
						case 1:	// Suffixes
							ac = wr.Suffixes;
							affixes = m_suffixes;
							break;
					}
					// NB: This will not run through the loop for cases where there is
					// only one affix, which is right, since it has nothing on either side.
					for (var iAfx = 1; ac != null && iAfx < ac.Count; ++iAfx)
					{
						var prevId = ac[iAfx -1].MIDREF;
						var curId = ac[iAfx].MIDREF;
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
		private readonly Morpheme m_morpheme;

		/// <summary>
		/// List of predecessor affixes.
		/// </summary>
		private readonly StringCollection m_predecessors;

		/// <summary>
		/// List of successor affixes.
		/// </summary>
		private readonly StringCollection m_successors;

		/// <summary>
		/// Starting class.
		/// </summary>
		private Class m_start;

		/// <summary>
		/// Ending class.
		/// </summary>
		private Class m_end;

		/// <summary>
		/// Cosntructor.
		/// </summary>
		/// <param name="m">The real morpheme that is wrapped.</param>
		public MorphemeWrapper(Morpheme m)
		{
			m_morpheme = m;
			m_predecessors = new StringCollection();
			m_successors = new StringCollection();
			m_start = null;
			m_end = null;
		}

		/// <summary>
		/// Adds a new affix as a successor, if it is not already in the list.
		/// </summary>
		/// <param name="succ">Affix ID.</param>
		public void AddAsSuccessor(string succ)
		{
			if (m_successors.Cast<string>().Any(t => t == succ))
				return;

			m_successors.Add(succ);
		}

		/// <summary>
		/// Adds a new affix as a predecessor, if it is not already in the list.
		/// </summary>
		/// <param name="pred">Affix ID.</param>
		public void AddAsPredecessor(string pred)
		{
			if (m_predecessors.Cast<string>().Any(t => t == pred))
				return;

			m_predecessors.Add(pred);
		}

		/// <summary>
		/// Set the starting and ending class IDREFs of the real morpheme.
		/// </summary>
		public void SetStartAndEnd()
		{
			// Replaces 41 lines in C++.
			if (m_start != null)
				m_morpheme.StartCLIDREF = m_start.CLID;
			if (m_end != null)
				m_morpheme.EndCLIDREF = m_end.CLID;
		}

		/// <summary>
		/// Remove an affix from the list of predecessors.
		/// </summary>
		/// <param name="pred">The affix ID to remove.</param>
		public void RemovePredecessor(string pred)
		{
			m_predecessors.Remove(pred);
		}

		/// <summary>
		/// Remove an affix from the list of successors.
		/// </summary>
		/// <param name="succ">The affix ID to remove.</param>
		public void RemoveSuccessor(string succ)
		{
			m_successors.Remove(succ);
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
					fRetVal = (m_predecessors.Count == 0);
					break;
				case ListType.Succ:
					fRetVal = (m_successors.Count == 0);
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
		public void SetAffixClass(bool isStartPoint, Class cls)
		{
			if (isStartPoint)
			{
				if (m_start == null)
					m_start = cls;
				else
					Debug.Assert(false);
			}
			else
			{
				if (m_end == null)
					m_end = cls;
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
			return m_morpheme.MID;
		}
	}
}
