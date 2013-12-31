// --------------------------------------------------------------------------------------------
// Copyright (C) 2003-2013 SIL International. All rights reserved.
//
// Distributable under the terms of the MIT License, as specified in the license.rtf file.
// --------------------------------------------------------------------------------------------
using System.Collections.Generic;
using System.Xml.Linq;

namespace SIL.WordWorks.GAFAWS.FW70Converter
{
	internal class FwMorphBundle
	{
		private readonly XElement _element;

		internal string Id
		{
			get { return _element.Attribute("guid").Value.ToLowerInvariant(); }
		}

		internal static FwMorphBundle Create(XElement mbElement,
			Dictionary<string, XElement> msas,
			Dictionary<string, XElement> entries,
			Dictionary<string, XElement> forms)
		{
			var newBy = new FwMorphBundle(mbElement);
			var msaPropElement = newBy._element.Element("Msa");
			if (msaPropElement != null)
			{
				newBy.Msa = FwMsa.Create(
					msas[msaPropElement.Element("objsur").Attribute("guid").Value.ToLowerInvariant()],
					entries,
					forms);
			}
			return newBy;
		}

		private FwMorphBundle(XElement element)
		{
			_element = element;
		}

		internal FwMsa Msa { get; private set; }
	}
}