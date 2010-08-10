using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace SIL.WordWorks.GAFAWS.FW70Converter
{
	internal class FwMsa
	{
		private readonly XElement _element;

		private FwMsa(XElement msaElement)
		{
			_element = msaElement;
		}

		internal int Class
		{
			get
			{
				var className = _element.Attribute("class").Value;
				switch (className)
				{
					default:
						throw new InvalidOperationException("MSA class not recognized/supported.");
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
				}
			}
		}

		internal string Id
		{
			get { return _element.Attribute("guid").Value.ToLowerInvariant(); }
		}

		internal string Key { get; private set; }

		public string CatId
		{
			get { return (Class == 5001) ? _element.Element("PartOfSpeech").Element("objsur").Attribute("guid").Value.ToLowerInvariant() : null; }
		}

		internal static FwMsa Create(XElement msaElement,
			Dictionary<string, XElement> entries,
			Dictionary<string, XElement> forms)
		{
			var newby = new FwMsa(msaElement);
			var entryElement = entries[msaElement.Attribute("ownerguid").Value.ToLowerInvariant()];
			var aUniElement = forms[entryElement.Element("LexemeForm").Element("objsur").Attribute("guid").Value.ToLowerInvariant()]
				.Element("Form").Element("AUni");
			var txt = (aUniElement == null) ? "???" : aUniElement.Value;
			newby.Key = txt + "_" + newby.Id;

			return newby;
		}
	}
}