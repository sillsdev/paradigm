// --------------------------------------------------------------------------------------------
// Copyright (C) 2003-2013 SIL International. All rights reserved.
//
// Distributable under the terms of the MIT License, as specified in the license.rtf file.
// --------------------------------------------------------------------------------------------
using System.Collections.Generic;
using System.Xml.Linq;
using NUnit.Framework;
using SIL.WordWorks.GAFAWS.PositionAnalysis;
using SIL.WordWorks.GAFAWS.PositionAnalysis.Impl;

namespace SIL.WordWorks.GAFAWS.PositionAnalyser
{
	/// <summary>
	/// Class to do general tests.
	/// </summary>
	[TestFixture]
	public class OtherTests : DataLayerBase
	{
		private string _fileName;
		private string _otherStuff;

		/// <summary>
		/// Initialize a class before each test is run.
		/// This is called by NUnit before each test.
		/// It ensures each test will have a brand new GAFAWSData object to work with.
		/// </summary>
		[SetUp]
		public void Init()
		{
			_fileName = MakeFile();
			_gd = new GafawsData();
			var otherStuff = new XElement("MyStuff",
				new XAttribute("val", "true"),
				new XElement("YourStuff",
					new XAttribute("ID", "YS1")));
			_otherStuff = otherStuff.ToString();
		}

		/// <summary>
		/// Clean out the stuff after running each test.
		/// </summary>
		[TearDown]
		public void TearDown()
		{
			_fileName = null;
			_gd = null;
		}

		public void AddOtherContents()
		{
			_gd.SaveData(_fileName);

			_gd = null;

			// Make sure it is there.
			_gd = GafawsData.LoadData(_fileName);
		}

		public void CheckOtherContents()
		{
			var element = XElement.Parse(_otherStuff);
			Assert.AreEqual("true", element.Attribute("val").Value, "Wrong value for 'val'");
			var ys = element.Element("YourStuff");
			Assert.IsNotNull(ys);
			Assert.AreEqual("YS1", ys.Attribute("ID").Value, "Wrong value for 'ID'");
		}

		/// <summary>
		/// Add some contents to an 'Other'.
		/// </summary>
		[Test]
		public void AddOtherToGAFAWSData()
		{
			try
			{
				_gd.Other = _otherStuff;

				AddOtherContents();

				_otherStuff = _gd.Other;

				CheckOtherContents();
			}
			finally
			{
				DeleteFile(_fileName);
			}
		}

		/// <summary>
		/// Add some contents to an 'Other' for Stem.
		/// </summary>
		[Test]
		public void AddOtherToStem()
		{
			try
			{
				_gd.Morphemes.Add(new Morpheme(MorphemeType.Stem, "S1"));

				var wr = new WordRecord("wr1");
				_gd.WordRecords.Add(wr);
				var stem = new Stem {Id = "S1"};
				wr.Stem = stem;
				stem.Other = _otherStuff;

				AddOtherContents();

				_otherStuff = _gd.WordRecords[0].Stem.Other;

				CheckOtherContents();
			}
			finally
			{
				DeleteFile(_fileName);
			}
		}

		/// <summary>
		/// Add some contents to an 'Other' for Stem.
		/// </summary>
		[Test]
		public void AddOtherToAffix()
		{
			try
			{
				_gd.Morphemes.Add(new Morpheme(MorphemeType.Prefix, "A1"));
				_gd.Morphemes.Add(new Morpheme(MorphemeType.Stem, "S1"));

				var wr = new WordRecord("wr1");
				_gd.WordRecords.Add(wr);
				var stem = new Stem { Id = "S1" };
				wr.Stem = stem;
				wr.Prefixes = new List<IAffix>();
				var afx = new Affix {Id = "A1"};
				wr.Prefixes.Add(afx);

				afx.Other = _otherStuff;

				AddOtherContents();

				_otherStuff = _gd.WordRecords[0].Prefixes[0].Other;

				CheckOtherContents();
			}
			finally
			{
				DeleteFile(_fileName);
			}
		}

		/// <summary>
		/// Add some contents to an 'Other' for the whole Word record.
		/// </summary>
		[Test]
		public void AddOtherToWordRecord()
		{
			try
			{
				_gd.Morphemes.Add(new Morpheme(MorphemeType.Prefix, "A1"));
				_gd.Morphemes.Add(new Morpheme(MorphemeType.Stem, "S1"));

				var wr = new WordRecord("wr1");
				_gd.WordRecords.Add(wr);
				wr.Prefixes = new List<IAffix>();
				var afx = new Affix { Id = "A1" };
				var stem = new Stem { Id = "S1" };
				wr.Stem = stem;
				wr.Prefixes.Add(afx);

				wr.Other = _otherStuff;

				AddOtherContents();

				_otherStuff = _gd.WordRecords[0].Other;

				CheckOtherContents();
			}
			finally
			{
				DeleteFile(_fileName);
			}
		}
	}
}
