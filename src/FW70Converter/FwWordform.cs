// --------------------------------------------------------------------------------------------
// Copyright (C) 2003-2013 SIL International. All rights reserved.
//
// Distributable under the terms of the MIT License, as specified in the license.rtf file.
// --------------------------------------------------------------------------------------------
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using SIL.WordWorks.GAFAWS.PositionAnalysis;

namespace SIL.WordWorks.GAFAWS.FW70Converter
{
	internal class FwWordform
	{
		private readonly List<FwAnalysis> _analyses = new List<FwAnalysis>();
		private readonly XElement _element;

		internal static FwWordform Create(XElement wfElement,
			Dictionary<string, XElement> analyses,
			HashSet<string> humanApprovedEvalIds,
			List<string> allIds,
			Dictionary<string, XElement> morphBundles,
			Dictionary<string, XElement> msas,
			Dictionary<string, XElement> entries,
			Dictionary<string, XElement> forms)
		{
			var newby = new FwWordform(wfElement);
			var analProp = wfElement.Element("Analyses");
			if (analProp != null)
			{
				foreach (var myAnalId in from objsur in analProp.Elements("objsur")
									   select objsur.Attribute("guid").Value.ToLowerInvariant())
				{
					var analElement = analyses[myAnalId];
					var analEvalProperty = analElement.Element("Evaluations");
					if (analEvalProperty == null ||
						!analEvalProperty.Elements("objsur").Any(osElement => humanApprovedEvalIds.Contains(osElement.Attribute("guid").Value.ToLowerInvariant())))
					{
						// Skip making anals that are not human approved
						// Remove mbs for these unapproved anals
						var mbProp = analElement.Element("MorphBundles");
						if (mbProp != null)
						{
							foreach (var myMbId in from objsur in mbProp.Elements("objsur")
												   select objsur.Attribute("guid").Value.ToLowerInvariant())
							{
								morphBundles.Remove(myMbId);
							}
						}
						continue;
					}

					var newAnal = FwAnalysis.Create(analElement, morphBundles, msas, entries, forms);
					if (newAnal.IsCategory(allIds))
						newby._analyses.Add(newAnal);
					analyses.Remove(myAnalId);
				}
			}

			return newby;
		}

		private FwWordform(XElement element)
		{
			_element = element;
		}

		internal string Id
		{
			get { return _element.Attribute("guid").Value.ToLowerInvariant(); }
		}

		internal void Convert(IGafawsData gData,
			IWordRecordFactory wordRecordFactory,
			IMorphemeFactory morphemeFactory,
			IAffixFactory affixFactory,
			IStemFactory stemFactory,
			Dictionary<string, FwMsa> prefixes, Dictionary<string, List<FwMsa>> stems, Dictionary<string, FwMsa> suffixes)
		{
			foreach (var anal in _analyses)
				anal.Convert(gData, wordRecordFactory, morphemeFactory, affixFactory, stemFactory, prefixes, stems, suffixes);
		}
	}
}