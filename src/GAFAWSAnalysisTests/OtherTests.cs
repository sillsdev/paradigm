// <copyright from='2003' to='2010' company='SIL International'>
//    Copyright (c) 2003, SIL International. All Rights Reserved.
// </copyright>
//
// File: OtherTests.cs
// Responsibility: Randy Regnier
// Last reviewed:
//
// <remarks>
// Misc. unit tests for the GAFAWS data layer.
// </remarks>
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
		private string m_fileName;
		private string m_otherStuff;

		/// <summary>
		/// Initialize a class before each test is run.
		/// This is called by NUnit before each test.
		/// It ensures each test will have a brand new GAFAWSData object to work with.
		/// </summary>
		[SetUp]
		public void Init()
		{
			m_fileName = MakeFile();
			m_gd = new GafawsData();
			var otherStuff = new XElement("MyStuff",
				new XAttribute("val", "true"),
				new XElement("YourStuff",
					new XAttribute("ID", "YS1")));
			m_otherStuff = otherStuff.ToString();
		}

		/// <summary>
		/// Clean out the stuff after running each test.
		/// </summary>
		[TearDown]
		public void TearDown()
		{
			m_fileName = null;
			m_gd = null;
		}

		public void AddOtherContents()
		{
			m_gd.SaveData(m_fileName);

			m_gd = null;

			// Make sure it is there.
			m_gd = GafawsData.LoadData(m_fileName);
		}

		public void CheckOtherContents()
		{
			var element = XElement.Parse(m_otherStuff);
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
				m_gd.Other = m_otherStuff;

				AddOtherContents();

				m_otherStuff = m_gd.Other;

				CheckOtherContents();
			}
			finally
			{
				DeleteFile(m_fileName);
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
				m_gd.Morphemes.Add(new Morpheme(MorphemeType.Stem, "S1"));

				var wr = new WordRecord("wr1");
				m_gd.WordRecords.Add(wr);
				var stem = new Stem {Id = "S1"};
				wr.Stem = stem;
				stem.Other = m_otherStuff;

				AddOtherContents();

				m_otherStuff = m_gd.WordRecords[0].Stem.Other;

				CheckOtherContents();
			}
			finally
			{
				DeleteFile(m_fileName);
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
				m_gd.Morphemes.Add(new Morpheme(MorphemeType.Prefix, "A1"));
				m_gd.Morphemes.Add(new Morpheme(MorphemeType.Stem, "S1"));

				var wr = new WordRecord("wr1");
				m_gd.WordRecords.Add(wr);
				var stem = new Stem { Id = "S1" };
				wr.Stem = stem;
				wr.Prefixes = new List<IAffix>();
				var afx = new Affix {Id = "A1"};
				wr.Prefixes.Add(afx);

				afx.Other = m_otherStuff;

				AddOtherContents();

				m_otherStuff = m_gd.WordRecords[0].Prefixes[0].Other;

				CheckOtherContents();
			}
			finally
			{
				DeleteFile(m_fileName);
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
				m_gd.Morphemes.Add(new Morpheme(MorphemeType.Prefix, "A1"));
				m_gd.Morphemes.Add(new Morpheme(MorphemeType.Stem, "S1"));

				var wr = new WordRecord("wr1");
				m_gd.WordRecords.Add(wr);
				wr.Prefixes = new List<IAffix>();
				var afx = new Affix { Id = "A1" };
				var stem = new Stem { Id = "S1" };
				wr.Stem = stem;
				wr.Prefixes.Add(afx);

				wr.Other = m_otherStuff;

				AddOtherContents();

				m_otherStuff = m_gd.WordRecords[0].Other;

				CheckOtherContents();
			}
			finally
			{
				DeleteFile(m_fileName);
			}
		}
	}
}
