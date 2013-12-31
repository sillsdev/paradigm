// --------------------------------------------------------------------------------------------
// Copyright (C) 2003-2013 SIL International. All rights reserved.
//
// Distributable under the terms of the MIT License, as specified in the license.rtf file.
// --------------------------------------------------------------------------------------------
using System;
using System.IO;
using System.Xml;
using NUnit.Framework;
using SIL.WordWorks.GAFAWS.PositionAnalysis;
using SIL.WordWorks.GAFAWS.PositionAnalysis.Impl;

namespace SIL.WordWorks.GAFAWS.PositionAnalyser
{
	/// <summary>
	/// GAFAWS data layer serialization testing class.
	/// </summary>
	[TestFixture]
	public class SerializationTests : DataLayerBase
	{
		/// <summary>
		/// A known set of data.
		/// </summary>
		private readonly string _dataBefore = "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>" + Environment.NewLine +
			"<GAFAWSData>" + Environment.NewLine +
			"  <WordRecords>" + Environment.NewLine +
			"    <WordRecord id=\"WR1\">" + Environment.NewLine +
			"      <Stem id=\"M1\" />" + Environment.NewLine +
			"    </WordRecord>" + Environment.NewLine +
			"  </WordRecords>" + Environment.NewLine +
			"  <Morphemes>" + Environment.NewLine +
			"    <Morpheme id=\"M1\" type=\"s\" />" + Environment.NewLine +
			"  </Morphemes>" + Environment.NewLine +
			"  <AffixSets>" + Environment.NewLine +
			"    <AffixCooccurrences />" + Environment.NewLine +
			"    <AffixNonCooccurrences />" + Environment.NewLine +
			"    <DistinctSets />" + Environment.NewLine +
			"  </AffixSets>" + Environment.NewLine +
			"  <SubgraphSets />" + Environment.NewLine +
			"  <Classes>" + Environment.NewLine +
			"    <PrefixClasses />" + Environment.NewLine +
			"    <SuffixClasses />" + Environment.NewLine +
			"  </Classes>" + Environment.NewLine +
			"  <Challenges />" + Environment.NewLine +
			"</GAFAWSData>";

		/// <summary>
		/// Initialize a class before each test is run.
		/// This is called by NUnit before each test.
		/// It ensures each test will have a brand new GAFAWSData object to work with.
		/// </summary>
		[SetUp]
		public void Init()
		{
			_gd = new GafawsData();
		}

		/// <summary>
		/// Try to save with null as the pathname.
		/// </summary>
		[Test]
		public void SaveDataWithNullPathname()
		{
			Assert.Throws<ArgumentNullException>(() => _gd.SaveData(null));
		}

		/// <summary>
		/// Save a good set of data.
		/// </summary>
		[Test]
		public void SaveData()
		{
			string fileName = null;
			StreamReader reader = null;
			try
			{
				var cls = _gd.Classes;
				Assert.IsNotNull(cls);
				var m = new Morpheme(MorphemeType.Stem, "M1");
				_gd.Morphemes.Add(m);
				var wr = new WordRecord("WR1");
				_gd.WordRecords.Add(wr);
				wr.Stem = new Stem {Id = m.Id};
				fileName = MakeFile();
				_gd.SaveData(fileName);
				reader = new StreamReader(fileName);
				var dataAfter = reader.ReadToEnd();
				Assert.AreEqual(_dataBefore, dataAfter, "Before and After");
			}
			finally
			{
				if (reader != null)
					reader.Close();
				DeleteFile(fileName);
			}
		}

		/// <summary>
		/// Try loading a null pathname.
		/// </summary>
		[Test]
		public void LoadDataWithNullPathname()
		{
			Assert.Throws<ArgumentNullException>(() => _gd = GafawsData.LoadData(null));
		}

		/// <summary>
		/// Load good data set.
		/// </summary>
		[Test]
		public void LoadGoodData()
		{
			string fileName = null;
			try
			{
				Assert.AreEqual(0, _gd.Morphemes.Count);	// Shouldn't have any at this point.
				fileName = MakeFile(_dataBefore);
				_gd = GafawsData.LoadData(fileName);
				Assert.AreEqual(1, _gd.Morphemes.Count);	// Should be 1 of them now.
				Assert.AreEqual(1, _gd.WordRecords.Count, "Wrong word record count.");
				var wr = _gd.WordRecords[0];
				Assert.IsNull(wr.Prefixes, "Should have null preffix collection.");
				Assert.IsNull(wr.Suffixes, "Should have null suffix collection.");
			}
			finally
			{
				DeleteFile(fileName);
			}
		}

		/// <summary>
		/// Try loading data file that is empty.
		/// </summary>
		[Test]
		public void LoadDataWithEmptyFile()
		{
			string fileName = null;
			try
			{
				fileName = MakeFile();
				Assert.Throws<XmlException>(() => _gd = GafawsData.LoadData(fileName));
			}
			finally
			{
				DeleteFile(fileName);
			}
		}

		/// <summary>
		/// Try loading XML data that isn't in the right model of data.
		/// </summary>
		[Test]
		public void LoadDataWrongXml()
		{
			string fileName = null;
			try
			{
				fileName = MakeFile("<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n" +
					"<NOTGAFAWSData xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"" +
						" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">\r\n" +
					"</NOTGAFAWSData>");
				Assert.Throws<InvalidOperationException>(() => _gd = GafawsData.LoadData(fileName));
			}
			finally
			{
				DeleteFile(fileName);
			}
		}
	}
}
