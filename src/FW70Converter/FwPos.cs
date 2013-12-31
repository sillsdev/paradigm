// --------------------------------------------------------------------------------------------
// Copyright (C) 2003-2013 SIL International. All rights reserved.
//
// Distributable under the terms of the MIT License, as specified in the license.rtf file.
// --------------------------------------------------------------------------------------------
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace SIL.WordWorks.GAFAWS.FW70Converter
{
	internal class FwPos
	{
		private readonly List<FwPos> _subs = new List<FwPos>();
		private XElement _element;

		internal static FwPos Create(XElement posElement, Dictionary<string, XElement> otherCats)
		{
			// Make one top level FwPos out of "posElement",
			// then create/add sub cats of it.
			var newby = new FwPos
							{
								_element = posElement
							};
			var subPosElement = posElement.Element("SubPossibilities");
			if (subPosElement != null)
			{
				foreach (var mysubPosId in from objsur in subPosElement.Elements("objsur")
									   select objsur.Attribute("guid").Value.ToLowerInvariant())
				{
					newby._subs.Add(Create(otherCats[mysubPosId], otherCats));
					otherCats.Remove(mysubPosId);
				}
			}

			return newby;
		}

		internal string Id
		{
			get { return _element.Attribute("guid").Value.ToLowerInvariant(); }
		}

		internal List<string> AllIds
		{
			get
			{
				var result = new List<string> {Id};
				foreach (var sub in _subs)
					result.AddRange(sub.AllIds);
				return result;
			}
		}

		internal IEnumerable<FwPos> SubCats
		{
			get { return _subs; }
		}

		public override string ToString()
		{
			return _element.Element("Name").Element("AUni").Value;
		}
	}
}