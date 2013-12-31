// --------------------------------------------------------------------------------------------
// Copyright (C) 2003-2013 SIL International. All rights reserved.
//
// Distributable under the terms of the MIT License, as specified in the license.rtf file.
// --------------------------------------------------------------------------------------------
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using SIL.WordWorks.GAFAWS.PositionAnalysis.Properties;

namespace SIL.WordWorks.GAFAWS.PositionAnalysis.Impl
{
	/// <summary>
	/// List type enumeration.
	/// </summary>
	internal enum ListType
	{
		Pred,
		Succ
	}

	/// <summary>
	/// Abstract superclass for prefix and suffix class assigners.
	/// </summary>
	internal abstract class ClassAssigner
	{
		/// <summary>
		/// Affixes remaining to be assigned.
		/// </summary>
		protected Dictionary<string, MorphemeWrapper> _toCheck;

		/// <summary></summary>
		protected bool _assignedOk;

		/// <summary>
		/// Main GAFAWS data layer object.
		/// </summary>
		protected IGafawsData _gd;

		/// <summary>
		/// Collection of classes.
		/// </summary>
		protected List<IClass> _classes;

		/// <summary>
		/// The class ID prefix.
		/// </summary>
		protected string _classPrefix;

		/// <summary>
		/// The class that is the unknown group of classes.
		/// </summary>
		protected IClass _fogBank;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="gd">The main data layer object.</param>
		protected ClassAssigner(IGafawsData gd)
		{
			_assignedOk = true;
			_gd = gd;
			_fogBank = null;
		}

		/// <summary>
		/// Assign all affixes to the relevant position class.
		/// </summary>
		/// <param name="htToCheck">The set of affixes to check.</param>
		/// <returns>True if the classes were assigned, otherwise false.</returns>
		internal bool AssignClasses(Dictionary<string, MorphemeWrapper> htToCheck)
		{
			_toCheck = new Dictionary<string, MorphemeWrapper>(htToCheck);
			AssignClassesFromStem();
			_toCheck = new Dictionary<string, MorphemeWrapper>(htToCheck);
			AssignClassesFromEnd();

			// Number the classes.
			var i = 1;
			foreach(var c in _classes)
			{
				switch (c.IsFogBank)
				{
					case "0":
						c.Id = _classPrefix + i++;
						break;
					case "1":
						c.Id = _classPrefix + "0";
						break;
				}
			}
			return _assignedOk;
		}

		/// <summary>
		/// Assign the classes from the stem out to the end.
		/// </summary>
		protected void AssignClassesFromStem()
		{
			AssignClassesCore(
				GetListType(true),
				true,
				0,
				null);
		}

		/// <summary>
		/// Assign classes from the end to the stem.
		/// </summary>
		protected void AssignClassesFromEnd()
		{
			var listType = GetListType(false);
			var idxFog = 0;
			List<IClass> oldClasses = null;

			if (_fogBank != null)
			{
				for (idxFog = 0; idxFog < _classes.Count; ++idxFog)
				{
					if (_classes[idxFog] == _fogBank)
						break;
				}
			}
			else oldClasses = new List<IClass>(_classes);

			AssignClassesCore(listType, false, idxFog, oldClasses);
		}

		///  <summary>
		/// Abstract method that is to return a ListType.
		/// </summary>
		/// <param name="fromStem">True, if working out from the stem,
		/// otherwise false.</param>
		/// <returns>The ListType.</returns>
		abstract protected ListType GetListType(bool fromStem);

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
		protected void AssignClassesCore(ListType listType, bool isStartPoint,
			int idxFog, List<IClass> oldClasses)
		{
			while (_toCheck.Count > 0)
			{
				bool isInFog;
				var toBeAssigned = GetCandidates(listType, out isInFog);
				var cls = GetClass(listType, isStartPoint, isInFog, idxFog, oldClasses);
				DoAssignment(toBeAssigned, listType, isStartPoint, cls);
			}
		}

		/// <summary>
		/// Gather up a set of affixes to classify.
		/// </summary>
		/// <param name="listType">Type of list to process.</param>
		/// <param name="isInFog">Set to True if the remaining classes are unknowable,
		/// otherwise set to false.</param>
		/// <returns>Set of affixes to work on.</returns>
		protected Dictionary<string, MorphemeWrapper> GetCandidates(ListType listType, out bool isInFog)
		{
			isInFog = false;
			var ht = _toCheck
				.Select(kvp => kvp.Value)
				.Where(mr => mr.CanAssignClass(listType))
				.ToDictionary(mr => mr.GetId());
			if (ht.Count == 0)
			{
				// Couldn't find any, so we have bad input data.
				// Assign all remaining affixes to 'fog' class.
				isInFog = true;
				ht = new Dictionary<string, MorphemeWrapper>(_toCheck);
			}
			return ht;
		}

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
		protected IClass GetClass(ListType listType, bool isStartPoint, bool isInFog,
			int idxFog, List<IClass> oldClasses)
		{
			IClass cls = null;

			if (isStartPoint)
			{
				// Working out from stem.
				cls = new Class();
				_classes.Add(cls);
				if (isInFog)
				{
					_fogBank = cls;
					cls.IsFogBank = "1";
					var chl = new Challenge();
					_gd.Challenges.Add(chl);
					chl.Message = GetMessage();
				}
			}
			else
			{
				// Working towards stem.
				if (isInFog)
				{
					if (_fogBank != null)
						cls = _fogBank;	// Other edge of fog bank.
					else
					{
						// How is it that the fog is unidirectional?
						Debug.Assert(false);
					}
				}
				else
				{
					if (_fogBank != null)
					{
						cls = new Class();
						// Insert it between fog and other outer classes.
						_classes.Insert(idxFog + 1, cls);
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

		/// <summary>
		/// Really do the assignment.
		/// </summary>
		/// <param name="toBeAssigned">Set of affixes to be assigned.</param>
		/// <param name="listType">Type of list to process.</param>
		/// <param name="isStartPoint">True if setting the starting class,
		/// otherwise false.</param>
		/// <param name="cls">The class to set to.</param>
		protected void DoAssignment(Dictionary<string, MorphemeWrapper> toBeAssigned,
			ListType listType, bool isStartPoint, IClass cls)
		{
			foreach (var mr in toBeAssigned.Select(kvp => kvp.Value))
			{
				mr.SetAffixClass(isStartPoint, cls);
				var id = mr.GetId();
				_toCheck.Remove(id);
				foreach (var kvpInner in _toCheck)
				{
					if (listType == ListType.Succ)
						kvpInner.Value.RemoveSuccessor(id);
					else
						kvpInner.Value.RemovePredecessor(id);
				}
			}
		}

		/// <summary>
		/// Get the message, when there is a problem with the setting the class.
		/// Subclasses msut override this to return a suitable message.
		/// </summary>
		/// <returns>The problem message.</returns>
		protected virtual string GetMessage()
		{
			return "";
		}

	}


	/// <summary>
	/// The class that assigns prefixes.
	/// </summary>
	internal class PrefixClassAssigner : ClassAssigner
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="gd">Main GAFAWS data layer object.</param>
		internal PrefixClassAssigner(IGafawsData gd)
			: base(gd)
		{
			_classes = gd.Classes.PrefixClasses;
			_classPrefix = "PP";
		}

		/// <summary>
		/// Overrides method to return proper ListType.
		/// </summary>
		/// <param name="fromStem">True if working out from stem,
		/// otherwise false.</param>
		/// <returns>The ListType.</returns>
		protected override ListType GetListType(bool fromStem)
		{
			return fromStem ? ListType.Pred : ListType.Succ;
		}

		/// <summary>
		/// Overrides method to return a better error message for this class.
		/// </summary>
		/// <returns>A problem report string.</returns>
		protected override string GetMessage()
		{
			return StringResources.kstidBadPrefixes;
		}
	}

	/// <summary>
	/// The class that assigns suffixes.
	/// </summary>
	internal class SuffixClassAssigner : ClassAssigner
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="gd">Main GAFAWS data layer object.</param>
		internal SuffixClassAssigner(IGafawsData gd)
			: base(gd)
		{
			_classes = gd.Classes.SuffixClasses;
			_classPrefix = "SP";
		}

		/// <summary>
		/// Overrides method to return proper ListType.
		/// </summary>
		/// <param name="fromStem">True if working out from stem,
		/// otherwise false.</param>
		/// <returns>The ListType.</returns>
		protected override ListType GetListType(bool fromStem)
		{
			return fromStem ? ListType.Succ : ListType.Pred;
		}

		///  <summary>
		/// Overrides method to return a better error message for this class.
		/// </summary>
		/// <returns>A problem report string.</returns>
		protected override string GetMessage()
		{
			return StringResources.kstidBadSuffixes;
		}
	}
}
