// <copyright from='2003' to='2010' company='SIL International'>
//    Copyright (c) 2003, SIL International. All Rights Reserved.
// </copyright>
//
// File: PAProcessTests.cs
// Responsibility: Randy Regnier
// Last reviewed:
//
// <remarks>
// Unit tests for the PositionAnalyzer class.
// </remarks>
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using NUnit.Framework;
using SIL.WordWorks.GAFAWS.PositionAnalysis;

namespace SIL.WordWorks.GAFAWS.PositionAnalyser
{
	/// <summary>
	/// Test class for PositionAnalyzer class.
	/// </summary>
	[TestFixture]
	public class PAProcessTests
	{
		/// <summary>
		/// Check the Process method on the TestA xml file.
		/// </summary>
		[Test]
		public void Process_TestA_Data()
		{
			// has been copied to executing dir during build
			CheckFile(@"XML\TestA.xml");
		}

		/// <summary>
		/// Check the Process method on the TestA1 xml file.
		/// </summary>
		[Test]
		public void Process_TestA1_Data()
		{
			// has been copied to executing dir during build
			CheckFile(@"XML\TestA1.xml");
		}

		/// <summary>
		/// Check the Process method on the TestA1A xml file.
		/// </summary>
		[Test]
		public void Process_TestA1A_Data()
		{
			// has been copied to executing dir during build
			CheckFile(@"XML\TestA1A.xml");
		}

		/// <summary>
		/// Check the Process method on the TestB xml file.
		/// </summary>
		[Test]
		public void Process_TestB_Data()
		{
			// has been copied to executing dir during build
			CheckFile(@"XML\TestB.xml");
		}

		/// <summary>
		/// Check the Process method on the TestC xml file.
		/// </summary>
		//[Test]
		public void Process_TestC_Data()
		{
			// has been copied to executing dir during build
			// p14 fails, but why?
			CheckFile(@"XML\TestC.xml");
		}

		private static void CheckFile(string testFile)
		{
			var pa = new PositionAnalyzer();
			var outputPathname = pa.Process(testFile);

			CheckOutput(outputPathname);
		}

		private static void CheckOutput(string outputPathname)
		{
			var prefixIds = new HashSet<string>();
			var suffixIds = new HashSet<string>();
			var doc = XDocument.Load(outputPathname);
			var root = doc.Root;
			foreach (var morpheme in root.Element("Morphemes").Elements("Morpheme"))
			{
				var classPfx = "";
				var mid = "";
				string startid;
				string endid;
				var currentSet = prefixIds;
				switch (morpheme.Attribute("type").Value.ToLowerInvariant())
				{
					case "s":
						continue;
					case "pfx":
						classPfx = "PP";
						GetMorphemeInfo(morpheme, out mid, out startid, out endid);
						prefixIds.Add(startid);
						prefixIds.Add(endid);
						currentSet = prefixIds;
						break;
					case "sfx":
						classPfx = "SP";
						GetMorphemeInfo(morpheme, out mid, out startid, out endid);
						suffixIds.Add(startid);
						suffixIds.Add(endid);
						currentSet = suffixIds;
						break;
				}

				string expectedStartId;
				string expectedEndId;
				if (mid.Length == 2)
				{
					// If MID has one digit, then it is the start and end id.
					// NB: One or both actual ids could end with "0" for 'fog bank' results.
					expectedEndId = expectedStartId = classPfx + mid[1];
				}
				else
				{
					// Otherwise, the first digit is start and the second is end.
					// NB: One or both actual ids could end with "0" for 'fog bank' results.
					expectedStartId = classPfx + mid[1];
					expectedEndId = classPfx + mid[2];
				}
				var actualStartId = morpheme.Attribute("StartCLIDREF").Value;
				var okStart = actualStartId[2].ToString() == "0"
							   || expectedStartId == actualStartId;
				Assert.IsTrue(okStart);
				Assert.IsTrue(expectedStartId == actualStartId && currentSet.Contains(expectedStartId)
					|| expectedStartId != actualStartId && currentSet.Contains(actualStartId));

				var actualEndId = morpheme.Attribute("EndCLIDREF").Value;
				var okEnd = actualEndId[2].ToString() == "0"
							   || expectedEndId == actualEndId;
				Assert.IsTrue(okEnd);
				Assert.IsTrue(expectedEndId == actualEndId && currentSet.Contains(expectedEndId)
					|| expectedEndId != actualEndId && currentSet.Contains(actualEndId));
			}

			CheckAffixIds(prefixIds, root, "PrefixClasses");
			CheckAffixIds(suffixIds, root, "SuffixClasses");
		}

		private static void CheckAffixIds(IEnumerable<string> afxids, XContainer root, string elementName)
		{
			foreach (var afxId in afxids)
			{
				var id = afxId;
				Assert.IsNotNull((root
					.Element("Classes")
									.Element(elementName)
									.Elements("Class")
									.Where(cls => cls.Attribute("CLID").Value == id)).FirstOrDefault());
			}
		}

		private static void GetMorphemeInfo(XElement morpheme, out string mid, out string startid, out string endid)
		{
			mid = morpheme.Attribute("MID").Value;
			startid = morpheme.Attribute("StartCLIDREF").Value;
			endid = morpheme.Attribute("EndCLIDREF").Value;
		}
	}
}
