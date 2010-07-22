// --------------------------------------------------------------------------------------------
// <copyright from='2003' to='2010' company='SIL International'>
//    Copyright (c) 2003, SIL International. All Rights Reserved.
// </copyright>
//
// File: ClassAssigner.cs
// Responsibility: Randy Regnier
// Last reviewed:
//
// <remarks>
// Implementation of ClassAssigner and it two sublasses:
// PrefixClassAssigner and SuffixClassAssigner
// </remarks>
//
// --------------------------------------------------------------------------------------------
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

namespace SIL.WordWorks.GAFAWS
{
	/// ---------------------------------------------------------------------------------------
	/// <summary>
	/// List type enumeration.
	/// </summary>
	/// ---------------------------------------------------------------------------------------
	internal enum ListType
	{
		Pred,
		Succ
	}

	/// ---------------------------------------------------------------------------------------
	/// <summary>
	/// Abstract superclass for prefix and suffix class assigners.
	/// </summary>
	/// ---------------------------------------------------------------------------------------
	internal abstract class ClassAssigner
	{
		/// -----------------------------------------------------------------------------------
		/// <summary>
		/// Affixes remaining to be assigned.
		/// </summary>
		/// -----------------------------------------------------------------------------------
		protected Dictionary<string, MorphemeWrapper> m_toCheck;
		/// -----------------------------------------------------------------------------------
		/// <summary>
		/// Set of possible status messages.
		/// </summary>
		/// -----------------------------------------------------------------------------------
		protected Dictionary<string, string> m_messages;
		/// -----------------------------------------------------------------------------------
		/// <summary>
		///
		/// </summary>
		/// -----------------------------------------------------------------------------------
		protected bool m_assignedOk;
		/// -----------------------------------------------------------------------------------
		/// <summary>
		/// Main GAFAWS data layer object.
		/// </summary>
		/// -----------------------------------------------------------------------------------
		protected GAFAWSData m_gd;
		/// -----------------------------------------------------------------------------------
		/// <summary>
		/// Collection of classes.
		/// </summary>
		/// -----------------------------------------------------------------------------------
		protected List<Class> m_classes;
		/// -----------------------------------------------------------------------------------
		/// <summary>
		/// The class ID prefix.
		/// </summary>
		/// -----------------------------------------------------------------------------------
		protected string m_classPrefix;
		/// -----------------------------------------------------------------------------------
		/// <summary>
		/// The class that is the unknown group of classes.
		/// </summary>
		/// -----------------------------------------------------------------------------------
		protected Class m_fogBank;

		/// -----------------------------------------------------------------------------------
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="gd">The main data layer object.</param>
		/// <param name="messages">List of status messages.</param>
		/// -----------------------------------------------------------------------------------
		protected ClassAssigner(GAFAWSData gd, Dictionary<string, string> messages)
		{
			m_messages = messages;
			m_assignedOk = true;
			m_gd = gd;
			m_fogBank = null;
		}

		/// -----------------------------------------------------------------------------------
		/// <summary>
		/// Assign all affixes to the relevant position class.
		/// </summary>
		/// <param name="htToCheck">The set of affixes to check.</param>
		/// <returns>True if the classes were assigned, otherwise false.</returns>
		/// -----------------------------------------------------------------------------------
		public bool AssignClasses(Dictionary<string, MorphemeWrapper> htToCheck)
		{
			m_toCheck = new Dictionary<string, MorphemeWrapper>(htToCheck);
			AssignClassesFromStem();
			m_toCheck = new Dictionary<string, MorphemeWrapper>(htToCheck);
			AssignClassesFromEnd();

			// Number the classes.
			var i = 1;
			foreach(var c in m_classes)
			{
				switch (c.isFogBank)
				{
					case "0":
						c.CLID = m_classPrefix + i++;
						break;
					case "1":
						c.CLID = m_classPrefix + "0";
						break;
				}
			}
			return m_assignedOk;
		}

		/// -----------------------------------------------------------------------------------
		/// <summary>
		/// Assign the classes from the stem out to the end.
		/// </summary>
		/// -----------------------------------------------------------------------------------
		protected void AssignClassesFromStem()
		{
			AssignClassesCore(
				GetListType(true),
				true,
				0,
				null);
		}

		/// -----------------------------------------------------------------------------------
		/// <summary>
		/// Assign classes from the end to the stem.
		/// </summary>
		/// -----------------------------------------------------------------------------------
		protected void AssignClassesFromEnd()
		{
			var listType = GetListType(false);
			var idxFog = 0;
			List<Class> oldClasses = null;

			if (m_fogBank != null)
			{
				for (idxFog = 0; idxFog < m_classes.Count; ++idxFog)
				{
					if (m_classes[idxFog] == m_fogBank)
						break;
				}
			}
			else oldClasses = new List<Class>(m_classes);

			AssignClassesCore(listType, false, idxFog, oldClasses);
		}

		/// -----------------------------------------------------------------------------------
		/// <summary>
		/// Abstract method that is to return a ListType.
		/// </summary>
		/// <param name="fromStem">True, if working out from the stem,
		/// otherwise false.</param>
		/// <returns>The ListType.</returns>
		/// -----------------------------------------------------------------------------------
		abstract protected ListType GetListType(bool fromStem);

		/// -----------------------------------------------------------------------------------
		/// <summary>
		/// Do the main class assignment.
		/// </summary>
		/// <param name="listType">Tyep of list to process.</param>
		/// <param name="isStartPoint">True if the classes are the starting point,
		/// otherwise false.</param>
		/// <param name="idxFog">True if the remaining classes are unknowable,
		/// otherwise false.</param>
		/// <param name="oldClasses">Old set of classes, if working towards stem,
		/// otherwise null.</param>
		/// -----------------------------------------------------------------------------------
		protected void AssignClassesCore(ListType listType, bool isStartPoint,
			int idxFog, List<Class> oldClasses)
		{
			while (m_toCheck.Count > 0)
			{
				bool isInFog;
				var toBeAssigned = GetCandidates(listType, out isInFog);
				var cls = GetClass(listType, isStartPoint, isInFog, idxFog, oldClasses);
				DoAssignment(toBeAssigned, listType, isStartPoint, cls);
			}
		}

		/// -----------------------------------------------------------------------------------
		/// <summary>
		/// Gather up a set of affixes to classify.
		/// </summary>
		/// <param name="listType">Type of list to process.</param>
		/// <param name="isInFog">Set to True if the remaining classes are unknowable,
		/// otherwise set to false.</param>
		/// <returns>Set of affixes to work on.</returns>
		/// -----------------------------------------------------------------------------------
		protected Dictionary<string, MorphemeWrapper> GetCandidates(ListType listType, out bool isInFog)
		{
			isInFog = false;
			var ht = m_toCheck
				.Select(kvp => kvp.Value)
				.Where(mr => mr.CanAssignClass(listType))
				.ToDictionary(mr => mr.GetID());
			if (ht.Count == 0)
			{
				// Couldn't find any, so we have bad input data.
				// Assign all remaining affixes to 'fog' class.
				isInFog = true;
				ht = new Dictionary<string, MorphemeWrapper>(m_toCheck);
			}
			return ht;
		}

		/// -----------------------------------------------------------------------------------
		/// <summary>
		/// Find (create, if needed) the class to use for the assignment.
		/// </summary>
		/// <param name="listType">Type of list to process.</param>
		/// <param name="isStartPoint">True if assigning starting class,
		/// otherwise false.</param>
		/// <param name="isInFog">True if the remaining classes are unknowable,
		/// otherwise false.</param>
		/// <param name="idxFog">Index for the unknown class.</param>
		/// <param name="oldClasses">Old set of classes, if working towards stem,
		/// otherwise null.</param>
		/// <returns>The class to use for the assignment.</returns>
		/// -----------------------------------------------------------------------------------
		protected Class GetClass(ListType listType, bool isStartPoint, bool isInFog,
			int idxFog, List<Class> oldClasses)
		{
			Class cls = null;

			if (isStartPoint)
			{
				// Working out from stem.
				cls = new Class();
				m_classes.Add(cls);
				if (isInFog)
				{
					m_fogBank = cls;
					cls.isFogBank = "1";
					var chl = new Challenge();
					m_gd.Challenges.Add(chl);
					chl.message = GetMessage();
				}
			}
			else
			{
				// Working towards stem.
				if (isInFog)
				{
					if (m_fogBank != null)
						cls = m_fogBank;	// Other edge of fog bank.
					else
					{
						// How is it that the fog is unidirectional?
						Debug.Assert(false);
					}
				}
				else
				{
					if (m_fogBank != null)
					{
						cls = new Class();
						// Insert it between fog and other outer classes.
						m_classes.Insert(idxFog + 1, cls);
					}
					else
					{
						// Use old classes.
						var idx = oldClasses.Count - 1;
						cls = oldClasses[idx];
						oldClasses.RemoveAt(idx);
					}
				}
			}
			return cls;
		}

		/// -----------------------------------------------------------------------------------
		/// <summary>
		/// Really do the assignment.
		/// </summary>
		/// <param name="toBeAssigned">Set of affixes to be assigned.</param>
		/// <param name="listType">Type of list to process.</param>
		/// <param name="isStartPoint">True if setting the starting class,
		/// otherwise false.</param>
		/// <param name="cls">The class to set to.</param>
		/// -----------------------------------------------------------------------------------
		protected void DoAssignment(Dictionary<string, MorphemeWrapper> toBeAssigned,
			ListType listType, bool isStartPoint, Class cls)
		{
			foreach (var mr in toBeAssigned.Select(kvp => kvp.Value))
			{
				mr.SetAffixClass(isStartPoint, cls);
				var mid = mr.GetID();
				m_toCheck.Remove(mid);
				foreach (var kvpInner in m_toCheck)
				{
					if (listType == ListType.Succ)
						kvpInner.Value.RemoveSuccessor(mid);
					else
						kvpInner.Value.RemovePredecessor(mid);
				}
			}
		}

		/// -----------------------------------------------------------------------------------
		/// <summary>
		/// Get the message, when there is a problem with the setting the class.
		/// Subclasses msut override this to return a suitable message.
		/// </summary>
		/// <returns>The problem message.</returns>
		/// -----------------------------------------------------------------------------------
		protected virtual string GetMessage()
		{
			return "";
		}

	}	// End of class ClassAssigner


	/// ---------------------------------------------------------------------------------------
	/// <summary>
	/// The class that assigns prefixes.
	/// </summary>
	/// ---------------------------------------------------------------------------------------
	internal class PrefixClassAssigner : ClassAssigner
	{
		/// -----------------------------------------------------------------------------------
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="gd">Main GAFAWS data layer object.</param>
		/// <param name="messages">Set of status messages.</param>
		/// -----------------------------------------------------------------------------------
		public PrefixClassAssigner(GAFAWSData gd, Dictionary<string, string> messages)
			: base(gd, messages)
		{
			m_classes = gd.Classes.PrefixClasses;
			m_classPrefix = "PP";
		}

		/// -----------------------------------------------------------------------------------
		/// <summary>
		/// Overrides method to return proper ListType.
		/// </summary>
		/// <param name="fromStem">True if working out from stem,
		/// otherwise false.</param>
		/// <returns>The ListType.</returns>
		/// -----------------------------------------------------------------------------------
		protected override ListType GetListType(bool fromStem)
		{
			return fromStem ? ListType.Pred : ListType.Succ;
		}

		/// -----------------------------------------------------------------------------------
		/// <summary>
		/// Overrides method to return a better error message for this class.
		/// </summary>
		/// <returns>A problem report string.</returns>
		/// -----------------------------------------------------------------------------------
		protected override string GetMessage()
		{
			return m_messages["kstidBadPrefixes"];
		}
	}

	/// ---------------------------------------------------------------------------------------
	/// <summary>
	/// The class that assigns suffixes.
	/// </summary>
	/// ---------------------------------------------------------------------------------------
	internal class SuffixClassAssigner : ClassAssigner
	{
		/// -----------------------------------------------------------------------------------
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="gd">Main GAFAWS data layer object.</param>
		/// <param name="messages">Set of status messages.</param>
		/// -----------------------------------------------------------------------------------
		public SuffixClassAssigner(GAFAWSData gd, Dictionary<string, string> messages)
			: base(gd, messages)
		{
			m_classes = gd.Classes.SuffixClasses;
			m_classPrefix = "SP";
		}

		/// -----------------------------------------------------------------------------------
		/// <summary>
		/// Overrides method to return proper ListType.
		/// </summary>
		/// <param name="fromStem">True if working out from stem,
		/// otherwise false.</param>
		/// <returns>The ListType.</returns>
		/// -----------------------------------------------------------------------------------
		protected override ListType GetListType(bool fromStem)
		{
			return fromStem ? ListType.Succ : ListType.Pred;
		}

		/// -----------------------------------------------------------------------------------
		/// <summary>
		/// Overrides method to return a better error message for this class.
		/// </summary>
		/// <returns>A problem report string.</returns>
		/// -----------------------------------------------------------------------------------
		protected override string GetMessage()
		{
			return m_messages["kstidBadSuffixes"];
		}
	}
}
