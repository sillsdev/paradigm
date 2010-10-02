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

			if (!AnalyzeCompontentSubgraphs())
				return;

			// Set date and time. (Local date and tiome is fine.)
			var dt = DateTime.Now;
			_gd.Date = dt.ToLongDateString();
			_gd.Time = dt.ToLongTimeString();
		}

		private bool AnalyzeCompontentSubgraphs()
		{
			// Get unique sets of affixes from the wordforms.
			// That is, if multiple wordforms have the same set of affixes,
			// then only use one set of them for this part of the analysis.
			// NB: 'key' is a concatenation of the afx ids.
			var affixesFromWordforms = new Dictionary<string, HashSet<IMorpheme>>();
			foreach (var wordRecord in _gd.WordRecords)
			{
				var afxSet = new HashSet<IMorpheme>();
				var key = "";
				foreach (var afxId in wordRecord.AllAffixes.Select(affix => affix.Id))
				{
					var id = afxId;
					key += id;
					afxSet.Add((from m in _gd.Morphemes
								where m.Id == id
								select m).First());
				}
				if (!affixesFromWordforms.ContainsKey(key))
					affixesFromWordforms.Add(key, afxSet);
			}
			if (!affixesFromWordforms.ContainsKey(""))
				affixesFromWordforms.Add("", new HashSet<IMorpheme>()); // Make sure it has an empty set.
			affixesFromWordforms[""].Add(_gd.NothingMorpheme);

			Dictionary<string, IMorpheme> allAffixMorphemesAsDictionary;
			List<IMorpheme> allAffixMorphemesAsList;
			var allAffixMorphemes = GetAllAffixMorphemes(out allAffixMorphemesAsDictionary, out allAffixMorphemesAsList);
			var subgraphSets = new Dictionary<string, List<HashSet<IMorpheme>>>();

#if USEMATRIXFORDISTINCTSETS
			// allAffixMorphemesAsList.Count + 1 is where we put the 'row sum'.
			// allAffixMorphemesAsList.Count + 2 is where we put the sub graph number.
			var maxColumns = allAffixMorphemesAsList.Count + 2;
			var dataMatrix = new Matrix(affixesFromWordforms.Count, maxColumns);

			// Set up main matrix.
			for (var col = 0; col < allAffixMorphemesAsList.Count; ++col)
			{
				var currentAfx = allAffixMorphemesAsList[col];
				for (var row = 0; row < affixesFromWordforms.Count; ++row)
				{
					var currentWfAffixes = affixesFromWordforms.Values.ElementAt(row);
					if (currentWfAffixes.Contains(currentAfx))
						dataMatrix[row, col] = 1;
				}
			}

			// Set 'row sum' column counts.
			var rowSumCol = allAffixMorphemesAsList.Count;
			for (var row = 0; row < affixesFromWordforms.Count; ++row)
			{
				for (var col = 0; col < allAffixMorphemesAsList.Count; ++col)
				{
					if (dataMatrix[row, col] == 1)
						dataMatrix[row, rowSumCol] = dataMatrix[row, rowSumCol] + 1;
				}
			}

			// Column Sums
			long colSumTotal = 0;
			var columnSum = new Matrix(1, allAffixMorphemesAsList.Count);
			uint currentSubGraph = 1;
			var subGraphCol = allAffixMorphemesAsList.Count + 1;
			var subgraphNumberToSmallestColMap = new List<KeyValuePair<uint, int>>();
			uint emptySubgraph;
			while (true)
			{
				var smallestColIdx = 0;
				var smallestColumnTotal = uint.MaxValue;
				// Add the column sum.
				for (var col = 0; col < allAffixMorphemesAsList.Count; ++col)
				{
					var currentValue = columnSum[0, col];
					for (var row = 0; row < affixesFromWordforms.Count; ++row)
					{
						if (dataMatrix[row, col] == 1 && dataMatrix[row, subGraphCol] == 0)
							columnSum[0, col] = currentValue + dataMatrix[row, rowSumCol];
						currentValue = columnSum[0, col];
						colSumTotal += currentValue;
					}
					if (currentValue == 0 || currentValue >= smallestColumnTotal)
						continue;

					smallestColIdx = col;
					smallestColumnTotal = currentValue;
				}

				// Figure out the subgraph stuff.
				if (colSumTotal == 0)
				{
					emptySubgraph = currentSubGraph;
					for (var row = 0; row < affixesFromWordforms.Count; ++row)
					{
						// But skip rows that already have been classified.
						if (dataMatrix[row, subGraphCol] > 0)
							continue;
						dataMatrix[row, subGraphCol] = currentSubGraph;
					}
					break;
				}

				for (var row = 0; row < affixesFromWordforms.Count; ++row)
				{
					// 1. Look for 1s in the 'smallestColIdx' column.
					if (dataMatrix[row, smallestColIdx] != 1)
						continue;
					// But skip rows that already have been classified.
					if (dataMatrix[row, subGraphCol] > 0)
						continue;

					// 2. Stuff the currentSubGraph number for each '1' out in the rightmost cell of dataMatrix.
					dataMatrix[row, subGraphCol] = currentSubGraph;
				//	// 3. Substract the 'row sum' of the the row with the 1 in it from the col sum and put the new col sum in place in columnSum.
				//	columnSum[0, smallestColIdx] = columnSum[0, smallestColIdx] - dataMatrix[row, rowSumCol];
				}
				subgraphSets.Add(allAffixMorphemesAsList[smallestColIdx].Id, new List<HashSet<IMorpheme>>());
				subgraphNumberToSmallestColMap.Add(new KeyValuePair<uint, int>(currentSubGraph, smallestColIdx));
				currentSubGraph++; // Get ready for the next sub graph.
				colSumTotal = 0;
				columnSum = new Matrix(1, allAffixMorphemesAsList.Count);
			}

			for (var row = 0; row < affixesFromWordforms.Count; ++row)
			{
				string key;
				var subGraphNumber = dataMatrix[row, subGraphCol];
				List<HashSet<IMorpheme>> currentSubGraphSets;
				if (subGraphNumber == emptySubgraph)
				{
					key = "xxx";
				}
				else
				{
					var idx = (from kvp in subgraphNumberToSmallestColMap
						   where kvp.Key == subGraphNumber
						   select kvp.Value).First();
					key = allAffixMorphemesAsList[idx].Id;
				}
				if (!subgraphSets.TryGetValue(key, out currentSubGraphSets))
				{
					currentSubGraphSets = new List<HashSet<IMorpheme>>();
					subgraphSets.Add(key, currentSubGraphSets);
				}
				currentSubGraphSets.Add(affixesFromWordforms.ElementAt(row).Value);
			}
#endif
			foreach (var kvp in subgraphSets)
				_gd.ElementarySubgraphs.Add(kvp.Key, kvp.Value);

			return true;
		}

		private bool AnalyzeCooccurrences()
		{
			Dictionary<string, IMorpheme> allAffixMorphemesAsDictionary;
			List<IMorpheme> allAffixMorphemesAsList;
			var allAffixMorphemes = GetAllAffixMorphemes(out allAffixMorphemesAsDictionary, out allAffixMorphemesAsList);
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

#if USEMATRIXFORDISTINCTSETS
			var masterChart = new Matrix(allAffixMorphemes.Count, allAffixMorphemes.Count);

			// @V: Fill union matrix
			for (var row = 0; row < allAffixMorphemesAsList.Count; ++row)
			{
				var currentMorpheme = allAffixMorphemesAsList[row];
				foreach (var col in allAffixMorphemeCooccurrences[currentMorpheme.Id].Select(cooccurrance => allAffixMorphemesAsList.IndexOf(cooccurrance)))
					if (row == col)
						masterChart[row, col] = 0; // Same afx does not cooccur with itself.
					else
						masterChart[row, col] = 1;
			}

			// Extra: Noncooccurrances.
			var nonCooccurrances = new List<HashSet<IMorpheme>>();
			for (var row = 0; row < masterChart.Rows; ++row)
			{
				var noCoo = new HashSet<IMorpheme>();
				nonCooccurrances.Add(noCoo);
				for (var col = 0; col < masterChart.Cols; ++col)
				{
					if (masterChart[row, col] == 0)
						noCoo.Add(allAffixMorphemesAsList[col]);
				}
				noCoo.Add(allAffixMorphemesAsList[row]);
			}
			_gd.AffixNonCooccurrences.AddRange(nonCooccurrances);

			// @w: Complement union matrix. (aka Work Chart in book).
			var workChart = masterChart.Clone();
			for (var row = 0; row < masterChart.Rows; ++row)
			{
				for (var col = 0; col < masterChart.Cols; ++col)
				{
					if (row == col)
					{
						workChart[row, col] = 1; // Identity.
					}
					else
					{
						// Switch 1s and 0s.
						if (masterChart[row, col] == 1)
							workChart[row, col] = 0;
						else
							workChart[row, col] = 1;
					}
				}
			}

			_gd.DistinctSets.AddRange(GetDistinctSets(workChart, allAffixMorphemesAsList));
#else
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

#endif
			return true;
		}

		private HashSet<IMorpheme> GetAllAffixMorphemes(out Dictionary<string, IMorpheme> allAffixMorphemesAsDictionary, out List<IMorpheme> allAffixMorphemesAsList)
		{
			var allAffixMorphemes = new HashSet<IMorpheme>(_gd.Morphemes.Select(m => m).Where(m => m.Type != "s"));
			allAffixMorphemesAsList = new List<IMorpheme>(allAffixMorphemes);
			allAffixMorphemesAsDictionary = new Dictionary<string, IMorpheme>(allAffixMorphemes.Count);
			return allAffixMorphemes;
		}

#if USEMATRIXFORDISTINCTSETS
		private static IEnumerable<HashSet<IMorpheme>> GetDistinctSets(Matrix currentMatrix, IList<IMorpheme> affixMorphemes)
		{
			var distinctSets = new List<HashSet<IMorpheme>>();
			for (var currentRow = 0; currentRow < currentMatrix.Rows; ++currentRow)
			{
				for (var currentColumn = 0; currentColumn < currentMatrix.Cols; ++currentColumn)
				{
					if (currentRow == currentColumn)
						continue; // Skip Identity cell.
					var currentValue = currentMatrix[currentRow, currentColumn];
					if (currentValue != 1)
						continue;

					// "Untagged 1s"
					var otherRow = currentColumn;
					for (var otherColumn = 0; otherColumn < currentMatrix.Cols; ++otherColumn)
					{
						// If corresponding column (otherColumn) in current row (currentRow) is 1,
						// AND corresponding column (otherColumn) in other row (otherRow) is 0,
						// then set corresponding column (otherColumn) in current row (currentRow) to 2.
						if (currentMatrix[currentRow, otherColumn] == 1 && currentMatrix[otherRow, otherColumn] == 0)
							currentMatrix[currentRow, otherColumn] = 2; // "Tag" the 1 by making it a 2.
					}
				}
			}

			// Get Distinct Sets (not counting hidden sets).
			// These are the cells in each row that now have 1s in them.
			for (var row = 0; row < currentMatrix.Rows; ++row)
			{
				var distinctSet = new HashSet<IMorpheme>();
				distinctSets.Add(distinctSet);
				for (var col = 0; col < currentMatrix.Cols; ++col)
				{
					var currentValue = currentMatrix[row, col];
					if (currentValue == 1)
						distinctSet.Add(affixMorphemes[col]);
				}
			}

			// Identify potentially hidden sets (2s), and mark them with pairs of 3s.
			var conserveRows = new SortedList<int, int>();
			var conserveColumns = new SortedList<int, int>();
			var hiddenAffixMorphemesAsList = new SortedList<int, IMorpheme>();
			for (var currentRow = 0; currentRow < currentMatrix.Rows; ++currentRow)
			{
				for (var currentColumn = 0; currentColumn < currentMatrix.Cols; ++currentColumn)
				{
					var currentValue = currentMatrix[currentRow, currentColumn];
					var symmetricValue = currentMatrix[currentColumn, currentRow];

					if (currentValue <= 1 || (symmetricValue <= 1))
						continue;

					// Hidden set member.
					if (!hiddenAffixMorphemesAsList.ContainsKey(currentRow))
						hiddenAffixMorphemesAsList.Add(currentRow, affixMorphemes[currentRow]);
					currentMatrix[currentRow, currentColumn] = 3;
					currentMatrix[currentColumn, currentRow] = 3;

					if (!conserveRows.ContainsKey(currentRow))
						conserveRows.Add(currentRow, 0);
					if (!conserveRows.ContainsKey(currentColumn))
						conserveRows.Add(currentColumn, 0);

					if (!conserveColumns.ContainsKey(currentColumn))
						conserveColumns.Add(currentColumn, 0);
					if (!conserveColumns.ContainsKey(currentRow))
						conserveColumns.Add(currentRow, 0);
				}
			}

			// If hiddenAffixMorphemesAsList count is > 0,
			// then get the reduced matrix and recurse,
			// until no more hidden sets are found.
			if (hiddenAffixMorphemesAsList.Count > 0)
			{
				var hiddenSetMatrix = new Matrix(conserveRows.Count, conserveColumns.Count);
				for (var row = 0; row < conserveRows.Count; ++row)
				{
					for (var col = 0; col < conserveColumns.Count; ++col)
					{
						hiddenSetMatrix[row, col] = currentMatrix[conserveRows.Keys[row], conserveColumns.Keys[col]];
						var currentValue = hiddenSetMatrix[row, col];
						if (currentValue > 1)
							hiddenSetMatrix[row, col] = 1;
					}
				}

				// Add hidden sets, recursively.
				distinctSets.AddRange(GetDistinctSets(hiddenSetMatrix, hiddenAffixMorphemesAsList.Values));
			}

			// Remove duplicate sets.
			RemoveDuplicateSets(distinctSets);

			return distinctSets;
		}
#else
		private static IEnumerable<HashSet<IMorpheme>> GetDistinctSets(IDictionary<string, HashSet<IMorpheme>> sourceNonCooccurrences)
		{
			var allNonCooccurrencesCopy = new List<HashSet<IMorpheme>>();
			var mayNeedMoreChecking = new Dictionary<string, HashSet<IMorpheme>>();
			var allNewValuesAreEmptySets = true;
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
					if (newValue.Count > 0)
						allNewValuesAreEmptySets = false;
				}
				moreCheckingSet.ExceptWith(newValue);
			}

			// If allNonCooccurrencesCopy has sets that are all empty, then just return the union of all input data.
			if (allNewValuesAreEmptySets)
			{

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
			RemoveDuplicateSets(allNonCooccurrencesCopy);

			return allNonCooccurrencesCopy;
		}
#endif

		private static void RemoveDuplicateSets(IList<HashSet<IMorpheme>> sets)
		{
			var duplicatesByIndex = new List<int>();
			for (var i = sets.Count - 1; i > 0; --i)
			{
				var currentSet = sets[i];
				for (var j = 0; j < i; ++j)
				{
					var earlierSet = sets[j];
					if (!earlierSet.SetEquals(currentSet))
						continue;
					duplicatesByIndex.Add(i);
					break;
				}
			}
			foreach (var dupKey in duplicatesByIndex)
				sets.RemoveAt(dupKey);
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
