// <copyright from='2003' to='2010' company='SIL International'>
//    Copyright (c) 2003, SIL International. All Rights Reserved.
// </copyright>
//
// File: GafawsData.cs
// Responsibility: Randy Regnier
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace SIL.WordWorks.GAFAWS.PositionAnalysis.Impl
{
	/// <summary>
	/// Main class in the GAFAWS data layer.
	/// </summary>
	public sealed class GafawsData : IGafawsData
	{
		#region Data members

		#endregion // Data members

		#region Construction

		public GafawsData()
		{
			WordRecords = new List<IWordRecord>();
			Morphemes = new List<IMorpheme>();
			Classes = new Classes();
			Challenges = new List<IChallenge>();
		}

		#endregion // Construction

		// [NB: Don't reorder these, or they won't be dumped in the right order,
		// and the resulting XML file won't pass the schema.]
		/// <summary>
		/// Collection of word records.
		/// </summary>
		public List<IWordRecord> WordRecords { get; private set; }

		/// <summary>
		/// Collection of morphemes.
		/// </summary>
		public List<IMorpheme> Morphemes { get; private set; }

		/// <summary>
		/// Collection of position classes. (Reserved for use by the Paradigm DLL.)
		/// </summary>
		public IClasses Classes { get; private set; }

		/// <summary>
		/// Collection of problems. (Reserved for use by the Paradigm DLL.)
		/// </summary>
		public List<IChallenge> Challenges { get; private set; }

		/// <summary>
		/// Model-specific data.
		/// </summary>
		public string Other { get; set; }

		/// <summary>
		/// An attribute for a date. (Reserved for use by the Paradigm DLL.)
		/// </summary>
		public string Date { get; set; }

		/// <summary>
		/// An attribute for a time. (Reserved for use by the Paradigm DLL.)
		/// </summary>
		public string Time { get; set; }

		public void Reset()
		{
			WordRecords.Clear();
			Morphemes.Clear();
			Classes = new Classes();
			Challenges.Clear();
		}

		#region Serialization

		/// <summary>
		/// Save the data to a file.
		/// </summary>
		/// <param name="pathname">Pathname of file to save to.</param>
		public void SaveData(string pathname)
		{
			var doc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"),
									new XElement("GAFAWSData",
										Date == null ? null : new XAttribute("date", Date),
										Time == null ? null : new XAttribute("time", Time),
												 new XElement("WordRecords", from wr in WordRecords
																			 select new XElement("WordRecord",
																				 new XAttribute("id", wr.Id),
																								 wr.Prefixes == null || wr.Prefixes.Count == 0
																									? null
																									: new XElement("Prefixes",
																												   from pfx in wr.Prefixes
																												   select
																													new XElement(
																													"Affix",
																													new XAttribute(
																														"id",
																														pfx.MidRef),
																														SerializationServices.WriteOtherElement(pfx.Other))),
																								 new XElement("Stem",
																											  new XAttribute(
																												"id",
																												wr.Stem.MidRef),
																												SerializationServices.WriteOtherElement(wr.Stem.Other)),
																								 wr.Suffixes == null || wr.Suffixes.Count == 0
																									? null
																									: new XElement("Suffixes",
																												   from sfx in
																													   wr.Suffixes
																												   select
																													new XElement(
																													"Affix",
																													new XAttribute(
																														"id",
																														sfx.MidRef),
																														SerializationServices.WriteOtherElement(sfx.Other))),
																							SerializationServices.WriteOtherElement(wr.Other))),
													new XElement("Morphemes", from morph in Morphemes
																			  select new XElement("Morpheme",
																				  new XAttribute("id", morph.Id),
																				  new XAttribute("type", morph.Type),
																				  morph.StartClass == null ? null : new XAttribute("startclass", morph.StartClass),
																				  morph.EndClass == null ? null : new XAttribute("endclass", morph.EndClass),
																				  SerializationServices.WriteOtherElement(morph.Other))),
												  new XElement("Classes",
													  new XElement("PrefixClasses", Classes.PrefixClasses.Count == 0 ? null : from pfxClass in Classes.PrefixClasses
																															  select new XElement("Class",
																																  new XAttribute("id", pfxClass.Id),
																																  pfxClass.Name == null ? null : new XAttribute("name", pfxClass.Name),
																																  new XAttribute("isfogbank", pfxClass.IsFogBank))),
													  new XElement("SuffixClasses", Classes.SuffixClasses.Count == 0 ? null : from sfxClass in Classes.SuffixClasses
																															  select new XElement("Class",
																																  new XAttribute("id", sfxClass.Id),
																																  sfxClass.Name == null ? null : new XAttribute("name", sfxClass.Name),
																																  new XAttribute("isfogbank", sfxClass.IsFogBank)))),
												new XElement("Challenges", Challenges.Count == 0 ? null : from challenge in Challenges
																										  select new XElement("Challenge", new XAttribute("message", challenge.Message))),
											   SerializationServices.WriteOtherElement(Other)));

			doc.Save(pathname);
		}

		/// <summary>
		/// Load data from file.
		/// </summary>
		/// <param name="pathname">Pathname of file to load.</param>
		/// <returns>An instance of GAFAWSData, if successful.</returns>
		/// <remarks>
		/// [NB: This may throw some exceptions, if pathname is bad,
		/// or it is not a suitable file.]
		/// </remarks>
		internal static IGafawsData LoadData(string pathname)
		{
			var doc = XDocument.Load(pathname);
			var root = doc.Root;
			if (root.Name != "GAFAWSData")
				throw new InvalidOperationException();

			var gd = new GafawsData
						{
							Date = root.Attribute("date") == null ? null : root.Attribute("date").Value,
							Time = root.Attribute("time") == null ? null : root.Attribute("time").Value
						};
			foreach (var wordRecordElement in root.Element("WordRecords").Elements("WordRecord"))
			{
				var wr = new WordRecord {Id = wordRecordElement.Attribute("id").Value};
				if (wordRecordElement.Element("Prefixes") != null)
				{
					wr.Prefixes = new List<IAffix>();
					foreach (var pfx in wordRecordElement
						.Element("Prefixes")
						.Elements("Affix")
						.Select(pfxElement => new Affix
												{
													MidRef = pfxElement.Attribute("id").Value,
													Other = SerializationServices.ReadOtherElement(pfxElement.Element("Other"))
												}))
					{
						wr.Prefixes.Add(pfx);
					}

				}
				var stemElement = wordRecordElement.Element("Stem");
				var stem = new Stem
							{
								MidRef = stemElement.Attribute("id").Value,
								Other = SerializationServices.ReadOtherElement(stemElement.Element("Other"))
							};
				wr.Stem = stem;
				if (wordRecordElement.Element("Suffixes") != null)
				{
					wr.Suffixes = new List<IAffix>();
					foreach (var sfx in wordRecordElement
						.Element("Suffixes")
						.Elements("Affix")
						.Select(sfxElement => new Affix
												{
													MidRef = sfxElement.Attribute("id").Value,
													Other = SerializationServices.ReadOtherElement(sfxElement.Element("Other"))
												}))
					{
						wr.Suffixes.Add(sfx);
					}
				}
				wr.Other = SerializationServices.ReadOtherElement(wordRecordElement.Element("Other"));
				gd.WordRecords.Add(wr);
			}

			foreach (var morpheme in root
				.Element("Morphemes")
				.Elements("Morpheme")
				.Select(morphemeElement => new Morpheme
				{
					Id = morphemeElement.Attribute("id").Value,
					Type = morphemeElement.Attribute("type").Value,
					StartClass = morphemeElement.Attribute("startclass") == null ? null : morphemeElement.Attribute("startclass").Value,
					EndClass = morphemeElement.Attribute("endclass") == null ? null : morphemeElement.Attribute("endclass").Value,
					Other = SerializationServices.ReadOtherElement(morphemeElement.Element("Other"))
				}))
			{
				gd.Morphemes.Add(morpheme);
			}

			foreach (var pfxClass in root
				.Element("Classes")
				.Element("PrefixClasses")
				.Elements("Class")
				.Select(newClassElement => new Class
				{
					Id = newClassElement.Attribute("id").Value,
					Name = newClassElement.Attribute("name") == null ? null : newClassElement.Attribute("name").Value,
					IsFogBank = newClassElement.Attribute("isfogbank").Value
				}))
			{
				gd.Classes.PrefixClasses.Add(pfxClass);
			}

			foreach (var sfxClass in root
				.Element("Classes")
				.Element("SuffixClasses")
				.Elements("Class")
				.Select(newClassElement => new Class
				{
					Id = newClassElement.Attribute("id").Value,
					Name = newClassElement.Attribute("name") == null ? null : newClassElement.Attribute("name").Value,
					IsFogBank = newClassElement.Attribute("isfogbank").Value
				}))
			{
				gd.Classes.PrefixClasses.Add(sfxClass);
			}

			foreach (var challenge in root
				.Element("Challenges")
				.Elements("Challenge")
				.Select(newChallengeElement => new Challenge
				{
					Message = newChallengeElement.Attribute("message").Value
				}))
			{
				gd.Challenges.Add(challenge);
			}
			gd.Other = SerializationServices.ReadOtherElement(root.Element("Other"));

			return gd;
		}
		#endregion	// Serialization
	}
}
