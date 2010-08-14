using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using SIL.WordWorks.GAFAWS.PositionAnalysis;

namespace SIL.WordWorks.GAFAWS.FW70Converter
{
	internal class FwAnalysis
	{
		private readonly XElement _analElement;
		private readonly List<FwMorphBundle> _prefixMorphBundles = new List<FwMorphBundle>();
		private readonly List<FwMorphBundle> _stemMorphBundles = new List<FwMorphBundle>();
		private readonly List<FwMorphBundle> _suffixMorphBundles = new List<FwMorphBundle>();

		private FwAnalysis(XElement analElement)
		{
			_analElement = analElement;
		}

		internal string Id
		{
			get { return _analElement.Attribute("guid").Value.ToLowerInvariant(); }
		}

		internal static FwAnalysis Create(XElement analElement,
			Dictionary<string, XElement> morphBundles,
			Dictionary<string, XElement> msas,
			Dictionary<string, XElement> entries,
			Dictionary<string, XElement> forms)
		{
			var newby = new FwAnalysis(analElement);
			var mbProp = analElement.Element("MorphBundles");
			if (mbProp != null)
			{
				var myMbIds = from objsur in mbProp.Elements("objsur")
							  select objsur.Attribute("guid").Value.ToLowerInvariant();
				var newBundles = new List<FwMorphBundle>(myMbIds.Count());
				foreach (var myMbId in myMbIds)
				{
					newBundles.Add(FwMorphBundle.Create(morphBundles[myMbId], msas, entries, forms));
					morphBundles.Remove(myMbId);
				}
				if (newBundles.Count == 1)
					return newby;// Only a stem, so uninteresting.

				/*
						case "MoUnclassifiedAffixMsa":
							return 5117;
						case "MoDerivAffMsa":
							return 5031;
						case "MoDerivStepMsa":
							return 5032;
						case "MoInflAffMsa":
							return 5038;
						case "MoStemMsa":
							return 5001;
				*/
				// Partition them into prefixes, stems (including deriv affixes), and suffixes.
				// Deal with prefixes, if any.
				var startStemIdx = 0;
				foreach (var morphBundle in newBundles)
				{
					if (morphBundle.Msa == null)
					{
						newby._prefixMorphBundles.Clear();
						return newby;
					}
					if (morphBundle.Msa.Class == 5001 || morphBundle.Msa.Class == 5031 || morphBundle.Msa.Class == 5032) //  || morphBundle.Msa.Class == 5117 What about 5117-MoUnclassifiedAffixMsa?
					{
						// stem or derivational prefix, so bail out of this loop.
						break;
					}
					startStemIdx++;

					// It's a prefix.
					newby._prefixMorphBundles.Add(morphBundle);
				}
				// Deal with suffixes, if any.
				// Work through the suffixes from the end of the word.
				// We stop when we hit the stem or a derivational suffix.
				var endStemIdx = 0;
				for (var i = newBundles.Count - 1; i > -1; --i)
				{
					var mb = newBundles[i];
					if (mb.Msa == null)
					{
						newby._prefixMorphBundles.Clear();
						newby._suffixMorphBundles.Clear();
						return newby;
					}
					if (mb.Msa.Class == 5001 || mb.Msa.Class == 5031 || mb.Msa.Class == 5032) //  || mb.Msa.Class == 5117 What about 5117-MoUnclassifiedAffixMsa?
					{
						// stem or derivational suffix, so bail out of this loop.
						endStemIdx = i;
						break;
					}

					// It's a suffix.
					newby._suffixMorphBundles.Add(mb);
				}

				// Deal with stem(s) (including compound roots and derivational affixes, fore and/or aft.
				if (startStemIdx == endStemIdx && endStemIdx == 0)
				{
					newby._stemMorphBundles.Add(newBundles[0]);
				}
				else
				{
					for (var currentIdx = startStemIdx; currentIdx <= endStemIdx; ++currentIdx)
					{
						try
						{
							newby._stemMorphBundles.Add(newBundles[currentIdx]);
						}
						catch (Exception e)
						{
							Console.WriteLine(e);
						}
					}
				}
			}

			return newby;
		}

		private bool CanConvert
		{
			get
			{
				var allBundles = new List<FwMorphBundle>();
				if (_prefixMorphBundles.Count > 0)
					allBundles.AddRange(_prefixMorphBundles);
				allBundles.AddRange(_stemMorphBundles);
				if (_suffixMorphBundles.Count > 0)
					allBundles.AddRange(_suffixMorphBundles);
				return (allBundles.Count > 1) && (!allBundles.Any(mb => mb.Msa == null));
			}
		}

		internal void Convert(IGafawsData gData,
			IWordRecordFactory wordRecordFactory,
			IMorphemeFactory morphemeFactory,
			IAffixFactory affixFactory,
			IStemFactory stemFactory,
			Dictionary<string, FwMsa> prefixes, Dictionary<string, List<FwMsa>> stems, Dictionary<string, FwMsa> suffixes)
		{
			if (!CanConvert)
				return;

			var wr = wordRecordFactory.Create();
			// Deal with prefixes, if any.
			foreach (var prefixMorphBundle in _prefixMorphBundles)
			{
				// Add prefix, if not already present.
				if (wr.Prefixes == null)
					wr.Prefixes = new List<IAffix>();
				var msaKey = prefixMorphBundle.Msa.Key;
				if (!prefixes.ContainsKey(msaKey))
				{
					prefixes.Add(msaKey, prefixMorphBundle.Msa);
					gData.Morphemes.Add(morphemeFactory.Create(MorphemeType.Prefix, msaKey));
				}
				var afx = affixFactory.Create();
				afx.Id = msaKey;
				wr.Prefixes.Add(afx);
			}

			// Deal with suffixes, if any.
			foreach (var suffixMorphBundle in _suffixMorphBundles)
			{
				// Add suffix, if not already present.
				if (wr.Suffixes == null)
					wr.Suffixes = new List<IAffix>();
				var msaKey = suffixMorphBundle.Msa.Key;
				if (!suffixes.ContainsKey(msaKey))
				{
					suffixes.Add(msaKey, suffixMorphBundle.Msa);
					gData.Morphemes.Add(morphemeFactory.Create(MorphemeType.Suffix, msaKey));
				}
				var afx = affixFactory.Create();
				afx.Id = msaKey;
				wr.Suffixes.Insert(0, afx);
			}
			// Deal with stem(s).
			var sStem = "";
			var foundFirstOrMore = false;
			var spacer = string.Empty;
			foreach (var stemMorphBundle in _stemMorphBundles)
			{
				var msaKey = stemMorphBundle.Msa.Key;
				if (foundFirstOrMore && spacer != string.Empty)
					spacer = " ";
				sStem += spacer + msaKey;
				if (!foundFirstOrMore)
					foundFirstOrMore = true;
			}
			if (!stems.ContainsKey(sStem))
			{
				stems.Add(sStem, new List<FwMsa>());
				gData.Morphemes.Add(morphemeFactory.Create(MorphemeType.Stem, sStem));
			}
			var stem = stemFactory.Create();
			stem.Id = sStem;
			wr.Stem = stem;

			// Add wr.
			gData.WordRecords.Add(wr);
		}

		internal bool IsCategory(List<string> allIds)
		{
			var catProp = _analElement.Element("Category");
			if (catProp != null)
				return allIds.Contains(catProp.Element("objsur").Attribute("guid").Value.ToLowerInvariant());

			// Do it the hard way from a stem.
			// Just pick the first stem, and don't fret (for now, that is) about others or deriv affixes on either end.
			var firstStemMsa = (from stemMb in _stemMorphBundles
							   where stemMb.Msa.Class == 5001
							   select stemMb.Msa).FirstOrDefault();
			return firstStemMsa != null && allIds.Contains(firstStemMsa.CatId);
		}
	}
}