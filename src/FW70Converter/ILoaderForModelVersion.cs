// --------------------------------------------------------------------------------------------
// Copyright (C) 2003-2013 SIL International. All rights reserved.
//
// Distributable under the terms of the MIT License, as specified in the license.rtf file.
// --------------------------------------------------------------------------------------------
using System.Collections.Generic;
using System.Xml.Linq;

namespace SIL.WordWorks.GAFAWS.FW70Converter
{
	internal interface ILoaderForModelVersion
	{
		uint SupportedModelVersion { get; }
		XElement LoadFile(XDocument doc,
			List<XElement> lists,
			Dictionary<string, XElement> cats,
			List<XElement> wordforms,
			Dictionary<string, XElement> analyses,
			Dictionary<string, XElement> morphBundles,
			Dictionary<string, XElement> msas,
			Dictionary<string, XElement> entries,
			Dictionary<string, XElement> forms,
			HashSet<string> humanApprovedEvalIds);
	}
}
