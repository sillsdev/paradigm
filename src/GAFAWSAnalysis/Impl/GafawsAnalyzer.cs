// <copyright from='2003' to='2010' company='SIL International'>
//    Copyright (c) 2003, SIL International. All Rights Reserved.
// </copyright>
//
// File: GafawsAnalyzer.cs
// Responsibility: Randy Regnier
// Last reviewed:
//
// <remarks>
// Implementation of IGafawsAnalyzer
// </remarks>
using System;
using System.Collections.Generic;
using System.Linq;

namespace SIL.WordWorks.GAFAWS.PositionAnalysis.Impl
{
	/// <summary>
	/// Main class for analyzing affix positions and cooccurrence restrictions.
	/// </summary>
	internal class GafawsAnalyzer : IGafawsAnalyzer
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
		/// Analyze the given input file.
		/// </summary>
		/// <param name="pathInput">Pathname to the input data.</param>
		/// <returns>The pathname for the processed data.</returns>
		/// <remarks>Only used by tests.</remarks>
		internal string AnalyzeTestFile(string pathInput)
		{
			var gData = GafawsData.LoadData(pathInput);
			if (gData == null)
				return null;

			Analyze(gData);

			var outPath = OutputPathServices.GetOutputPathname(pathInput);
			_gd.SaveData(outPath);
			return outPath;
		}

		/// <summary>
		/// Analyze the given input GAFAWS data object.
		/// </summary>
		/// <param name="gafawsData">GafawsData to process.</param>
		public void Analyze(IGafawsData gafawsData)
		{
			_gd = (GafawsData)gafawsData;
			if (_gd == null)
				return;

			_prefixes = new Dictionary<string, MorphemeWrapper>();
			_suffixes = new Dictionary<string, MorphemeWrapper>();

			PreProcessMorphemes();

			if ((_prefixes.Count + _suffixes.Count) == 0)
				return;	// Nothing to do.

			if (!AnalyzePositions())
				return;

			if (!AnalyzeCooccurrences())
				return;

			// Set date and time. (Local date and tiome is fine.)
			var dt = DateTime.Now;
			_gd.Date = dt.ToLongDateString();
			_gd.Time = dt.ToLongTimeString();
		}

		private bool AnalyzeCooccurrences()
		{
			var allAffixMorphemes = new HashSet<IMorpheme>(_gd.Morphemes.Select(m => m).Where(m => m.Type != "s"));
			var allAffixMorphemesAsList = new List<IMorpheme>(allAffixMorphemes);
			var allAffixMorphemesAsDictionary = new Dictionary<string, IMorpheme>(allAffixMorphemes.Count);
			var allAffixMorphemeCooccurrences = new Dictionary<string, HashSet<IMorpheme>>(allAffixMorphemes.Count);
			foreach (var morpheme in allAffixMorphemes)
			{
				allAffixMorphemeCooccurrences.Add(morpheme.Id, new HashSet<IMorpheme>());
				allAffixMorphemesAsDictionary.Add(morpheme.Id, morpheme);
			}

			foreach (var wordRecord in _gd.WordRecords)
			{
				foreach (var afx in wordRecord.AllAffixes)
				{
					allAffixMorphemeCooccurrences[afx.Id].UnionWith(from affix in wordRecord.AllAffixes
																	select allAffixMorphemesAsDictionary[affix.Id]);
				}
			}
			_gd.AffixCooccurrences.AddRange(allAffixMorphemeCooccurrences.Values);

			var allNonCooccurrences = new Dictionary<string, HashSet<IMorpheme>>(allAffixMorphemeCooccurrences.Count);
			foreach (var kvp in allAffixMorphemeCooccurrences)
			{
				var key = kvp.Key;
				var nonCo = new HashSet<IMorpheme>(allAffixMorphemes.Except(kvp.Value))
								{
									allAffixMorphemesAsDictionary[kvp.Key] // Put it back in.
								};
				allNonCooccurrences.Add(key, nonCo);
			}
			_gd.AffixNonCooccurrences.AddRange(allNonCooccurrences.Values);

			var allNonCooccurrencesCopy = GetDistinctSets(allNonCooccurrences);
			_gd.DistinctSets.AddRange(allNonCooccurrencesCopy);

			return true;
		}

		private static IEnumerable<HashSet<IMorpheme>> GetDistinctSets(IDictionary<string, HashSet<IMorpheme>> sourceNonCooccurrences)
		{
			var allNonCooccurrencesCopy = new List<HashSet<IMorpheme>>();
			var mayNeedMoreChecking = new Dictionary<string, HashSet<IMorpheme>>();
			foreach (var kvp in sourceNonCooccurrences)
			{
				var currentKey = kvp.Key;
				var currentSet = kvp.Value;
				var newValue = new HashSet<IMorpheme>(currentSet);
				var moreCheckingSet = new HashSet<IMorpheme>(currentSet);
				mayNeedMoreChecking.Add(currentKey, moreCheckingSet);
				allNonCooccurrencesCopy.Add(newValue);
				foreach (var otherSet in from morpheme in currentSet
										 where newValue.Contains(morpheme)
										 select sourceNonCooccurrences[morpheme.Id])
				{
					newValue.IntersectWith(otherSet);
				}
				moreCheckingSet.ExceptWith(newValue);
			}

			var doesNeedMoreChecking = new Dictionary<string, HashSet<IMorpheme>>();
			var doesNeedMoreProcessing = false;
			foreach (var kvp in mayNeedMoreChecking)
			{
				var currentKey = kvp.Key;
				var currentSet = kvp.Value;
				foreach (var currentSetItem in currentSet)
				{
					var otherSet = mayNeedMoreChecking[currentSetItem.Id];
					var matchingItem = (from item in otherSet
										where item.Id == currentKey
										select item).FirstOrDefault();
					if (matchingItem == null)
						continue;

					doesNeedMoreProcessing = true;
					if (!doesNeedMoreChecking.ContainsKey(currentKey))
						doesNeedMoreChecking.Add(currentKey, currentSet);
					if (!doesNeedMoreChecking.ContainsKey(currentSetItem.Id))
						doesNeedMoreChecking.Add(currentSetItem.Id, otherSet);
				}
			}
			// See if any changes were made. If not (as can happen in hidden set checking),
			// Then reset doesNeedMoreProcessing to false.
			//doesNeedMoreProcessing = doesNeedMoreChecking.Aggregate(doesNeedMoreProcessing, (current, kvp) => current | !sourceNonCooccurrences[kvp.Key].SetEquals(kvp.Value));
			var eachSetIsEqual = true;
			foreach (var kvp in doesNeedMoreChecking)
			{
				var currentKey = kvp.Key;
				var currentSet = kvp.Value;
				foreach (var otherSet in sourceNonCooccurrences[currentKey])
				{
					if (sourceNonCooccurrences[kvp.Key].SetEquals(currentSet))
						continue;

					// Sets are not the same, so something happened.
					eachSetIsEqual = false;
					break;
				}
				if (!eachSetIsEqual)
					break;
			}
			if (eachSetIsEqual)
				doesNeedMoreProcessing = false;

			if (doesNeedMoreProcessing)
			{
				// Remove items in sets that have no key in dictionary.
				foreach (var kvp in doesNeedMoreChecking)
				{
					var goners = kvp.Value.Where(item => !doesNeedMoreChecking.ContainsKey(item.Id)).ToList();
					foreach (var morpheme in goners)
					{
						kvp.Value.Remove(morpheme);
					}
				}

				var hiddenSets = GetDistinctSets(doesNeedMoreChecking);
			}

			// Remove duplicate sets.
			var duplicatesByIndex = new List<int>();
			for (var i = allNonCooccurrencesCopy.Count - 1; i > 0; --i)
			{;
				var currentSet = allNonCooccurrencesCopy[i];
				for (var j = 0; j < i; ++j)
				{
					var earlierSet = allNonCooccurrencesCopy[j];
					if (!earlierSet.SetEquals(currentSet))
						continue;
					duplicatesByIndex.Add(i);
					break;
				}
			}
			foreach (var dupKey in duplicatesByIndex)
				allNonCooccurrencesCopy.RemoveAt(dupKey);

			return allNonCooccurrencesCopy;
		}

		private bool AnalyzePositions()
		{
			if (!AssignToSets())
				return false;	// Nothing to do.

			// Find the classes for prefixes.
			var pca = new PrefixClassAssigner(_gd);
			if (!pca.AssignClasses(_prefixes))
				return false;
			// Find the classes for suffixes.
			var sca = new SuffixClassAssigner(_gd);
			if (!sca.AssignClasses(_suffixes))
				return false;

			foreach (var kvp in _prefixes)
				kvp.Value.SetStartAndEnd();
			foreach (var kvp in _suffixes)
				kvp.Value.SetStartAndEnd();
			return true;
		}

		private void PreProcessMorphemes()
		{
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
		}

		/// <summary>
		/// Assign affixes to predecessor and successor sets.
		/// </summary>
		/// <returns>True if it assigned affixes to sets, otherwise false.</returns>
		protected bool AssignToSets()
		{
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
						var prevId = ac[iAfx -1].Id;
						var curId = ac[iAfx].Id;
						affixes[curId].AddAsSuccessor(prevId);
						affixes[prevId].AddAsPredecessor(curId);
					}
				}
			}
			return true;
		}
	}	// End class PositionAnalyzer
}
