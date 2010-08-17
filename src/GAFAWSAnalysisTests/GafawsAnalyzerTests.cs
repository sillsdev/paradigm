// <copyright from='2003' to='2010' company='SIL International'>
//    Copyright (c) 2003, SIL International. All Rights Reserved.
// </copyright>
//
// File: GafawsAnalyzerTests.cs
// Responsibility: Randy Regnier
// Last reviewed:
//
// <remarks>
// Unit tests for the GafawsAnalyzer class.
// </remarks>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using NUnit.Framework;
using SIL.WordWorks.GAFAWS.PositionAnalysis;
using SIL.WordWorks.GAFAWS.PositionAnalysis.Impl;

namespace SIL.WordWorks.GAFAWS.PositionAnalyser
{
	/// <summary>
	/// Test class for GafawsAnalyzer class.
	/// </summary>
	[TestFixture]
	public class GafawsAnalyzerTests
	{
		/// <summary>
		/// Check the Analyze method on the TestA xml file.
		/// </summary>
		[Test]
		public void Process_TestA_Data()
		{
			// has been copied to executing dir during build
			CheckFile(@"XML\TestA.xml");
		}

		/// <summary>
		/// Check the Analyze method on the TestA1 xml file.
		/// </summary>
		[Test]
		public void Process_TestA1_Data()
		{
			// has been copied to executing dir during build
			CheckFile(@"XML\TestA1.xml");
		}

		/// <summary>
		/// Check the Analyze method on the TestA1A xml file.
		/// </summary>
		[Test]
		public void Process_TestA1A_Data()
		{
			// has been copied to executing dir during build
			CheckFile(@"XML\TestA1A.xml");
		}

		/// <summary>
		/// Check the Analyze method on the TestB xml file.
		/// </summary>
		[Test]
		public void Process_TestB_Data()
		{
			// has been copied to executing dir during build
			CheckFile(@"XML\TestB.xml");
		}

		/// <summary>
		/// Check the Analyze method on the TestB xml file.
		/// </summary>
		[Test]
		public void Process_Huichol_Data()
		{
			/*
			Mapping between original affix forms and the test data:
				ni = p1
				ka2 = p2
				p& = p3
				ka1 = p4
				ke = p24
				m& = p34
			*/
			// has been copied to executing dir during build
			// Basic position checking.
			var outputPathname  = CheckFile(@"XML\Huichol.xml");

			// Distinct sets checking.
			var gd = GafawsData.LoadData(outputPathname);

			var cooccurrenceSets = gd.AffixCooccurrences;
			Assert.AreEqual(6, cooccurrenceSets.Count);
			var sets = new List<List<string>>(cooccurrenceSets.Count)
				{
					new List<string>(3) { "p3", "p2", "p4" },
					new List<string>(5) { "p2", "p3", "p4", "p34", "p1" },
					new List<string>(4) { "p4", "p3", "p2", "p1" },
					new List<string>(3) { "p34", "p2", "p1" },
					new List<string>(5) { "p1", "p2", "p4", "p34", "p24" },
					new List<string>(2) { "p24", "p1" }
				};
			CheckSets(cooccurrenceSets, sets);

			var noncooccurrenceSets = gd.AffixNonCooccurrences;
			Assert.AreEqual(6, noncooccurrenceSets.Count);
			sets = new List<List<string>>(cooccurrenceSets.Count)
				{
					new List<string>(4) { "p3", "p34", "p1", "p24" },
					new List<string>(2) { "p2", "p24" },
					new List<string>(3) { "p4", "p34", "p24" },
					new List<string>(4) { "p3", "p4", "p34", "p24" },
					new List<string>(2) { "p3", "p1" },
					new List<string>(5) { "p3", "p2", "p4", "p34", "p24" }
				};
			CheckSets(noncooccurrenceSets, sets);

			var distSets = gd.DistinctSets;
			Assert.AreEqual(4, distSets.Count);
			sets = new List<List<string>>(distSets.Count)
				{
					new List<string>(3) { "p3", "p34", "p24" }, // Removes p1 from first set, above
					new List<string>(2) { "p2", "p24" }, // Same as second set, above.
					new List<string>(3) { "p4", "p34", "p24" }, // Same as third set, above.
																// Fourth set not present. Were they removed, because they were empty?
					new List<string>(2) { "p3", "p1" } // Same as fifth set, above,
																// Sixth set not present. Were they removed, because they were empty?
				};
			CheckSets(distSets, sets);
		}

		private static void CheckSets(IList<HashSet<IMorpheme>> sourceSets, IList<List<string>> sets)
		{
			Assert.IsTrue(sourceSets.Count == sets.Count);
			for (var i = 0; i < sourceSets.Count; i++)
			{
				var sourceSet = sourceSets[i];
				var sourceMorphemeIds = new List<string>(from morpheme in sourceSet
															 select morpheme.Id);
				var checkingSet = sets[i];
				Assert.IsTrue(sourceSet.Count == checkingSet.Count);
				foreach (var afxId in checkingSet)
				{
					Assert.IsTrue(sourceMorphemeIds.Contains(afxId));
				}
			}
		}

		/// <summary>
		/// Check the Analyze method on the TestC xml file.
		/// </summary>
		//[Test]
		public void Process_TestC_Data()
		{
			// has been copied to executing dir during build
			// p14 fails, but why?
			CheckFile(@"XML\TestC.xml");
		}

		private static string CheckFile(string testFile)
		{
			var pa = new GafawsAnalyzer();
			var outputPathname = pa.AnalyzeTestFile(testFile);

			CheckOutput(outputPathname);

			return outputPathname;
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
				var id = "";
				string startid;
				string endid;
				var currentSet = prefixIds;
				switch (morpheme.Attribute("type").Value.ToLowerInvariant())
				{
					case "s":
						continue;
					case "pfx":
						classPfx = "PP";
						GetMorphemeInfo(morpheme, out id, out startid, out endid);
						prefixIds.Add(startid);
						prefixIds.Add(endid);
						currentSet = prefixIds;
						break;
					case "sfx":
						classPfx = "SP";
						GetMorphemeInfo(morpheme, out id, out startid, out endid);
						suffixIds.Add(startid);
						suffixIds.Add(endid);
						currentSet = suffixIds;
						break;
				}

				string expectedStartId;
				string expectedEndId;
				if (id.Length == 2)
				{
					// If id has one digit, then it is the start and end id.
					// NB: One or both actual ids could end with "0" for 'fog bank' results.
					expectedEndId = expectedStartId = classPfx + id[1];
				}
				else
				{
					// Otherwise, the first digit is start and the second is end.
					// NB: One or both actual ids could end with "0" for 'fog bank' results.
					expectedStartId = classPfx + id[1];
					expectedEndId = classPfx + id[2];
				}
				var actualStartId = morpheme.Attribute("startclass").Value;
				var okStart = actualStartId[2].ToString() == "0"
							   || expectedStartId == actualStartId;
				Assert.IsTrue(okStart);
				Assert.IsTrue(expectedStartId == actualStartId && currentSet.Contains(expectedStartId)
					|| expectedStartId != actualStartId && currentSet.Contains(actualStartId));

				var actualEndId = morpheme.Attribute("endclass").Value;
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
									.Where(cls => cls.Attribute("id").Value == id)).FirstOrDefault());
			}
		}

		private static void GetMorphemeInfo(XElement morpheme, out string id, out string startid, out string endid)
		{
			id = morpheme.Attribute("id").Value;
			startid = morpheme.Attribute("startclass").Value;
			endid = morpheme.Attribute("endclass").Value;
		}
	}
}
