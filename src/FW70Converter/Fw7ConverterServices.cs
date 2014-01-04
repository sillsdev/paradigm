// --------------------------------------------------------------------------------------------
// Copyright (C) 2003-2013 SIL International. All rights reserved.
//
// Distributable under the terms of the MIT License, as specified in the license.rtf file.
// --------------------------------------------------------------------------------------------
using System.Collections.Generic;
using System.Xml.Linq;

namespace SIL.WordWorks.GAFAWS.FW70Converter
{
	internal static class Fw7ConverterServices
	{

		internal static XElement LoadFileForGeneralVersionNumbers(XDocument doc, List<XElement> lists, Dictionary<string, XElement> cats, List<XElement> wordforms, Dictionary<string, XElement> analyses, Dictionary<string, XElement> morphBundles, Dictionary<string, XElement> msas,
			Dictionary<string, XElement> entries, Dictionary<string, XElement> forms, HashSet<string> humanApprovedEvalIds)
		{
			XElement langProj = null;
			foreach (var currentRtElement in doc.Root.Elements("rt"))
			{
				switch (currentRtElement.Attribute("class").Value)
				{
					// Skip it.
					case "LangProject":
						langProj = currentRtElement;
						break;
					case "CmAgent":
						var agentElement = currentRtElement;
						var humanElement = agentElement.Element("Human");
						if (humanElement != null && humanElement.Attribute("val").Value.ToLowerInvariant() == "true")
						{
							humanApprovedEvalIds.Add(agentElement.Element("Approves").Element("objsur").Attribute("guid").Value.ToLowerInvariant());
						}
						break;
					// There can be any number of lists, but in the end we only want the main POS list (and none of them for reversals, or anything else).
					case "CmPossibilityList":
						lists.Add(currentRtElement);
						break;
					case "WfiWordform":
						wordforms.Add(currentRtElement);
						break;
					// PartOfSpeech are in the main list, and ni reversal POS lists, but we only care about those in the main list.
					case "PartOfSpeech":
						AddItem(currentRtElement, cats);
						break;
					case "WfiAnalysis":
						AddItem(currentRtElement, analyses);
						break;
					case "WfiMorphBundle":
						AddItem(currentRtElement, morphBundles);
						break;
					case "LexEntry":
						AddItem(currentRtElement, entries);
						break;
					case "MoStemAllomorph":
					case "MoAffixProcess":
					case "MoAffixAllomorph":
						AddItem(currentRtElement, forms);
						break;
					// MSAs can be owned elsewhere other than by a LexEntry, but we only want those owned by entries.
					case "MoUnclassifiedAffixMsa":
					case "MoDerivAffMsa":
					case "MoDerivStepMsa":
					case "MoInflAffMsa":
					case "MoStemMsa":
						AddItem(currentRtElement, msas);
						break;
				}
			}

			return langProj;
		}

		private static void AddItem(XElement item, IDictionary<string, XElement> holder)
		{
			holder.Add(item.Attribute("guid").Value.ToLowerInvariant(), item);
		}

	}
}
