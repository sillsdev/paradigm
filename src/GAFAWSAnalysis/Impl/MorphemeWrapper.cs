using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;

namespace SIL.WordWorks.GAFAWS.PositionAnalysis.Impl
{
	/// <summary>
	/// A wrapper class for the data layer morpheme object.
	/// </summary>
	/// <remarks>
	/// This wrapper class allows us to keep track of useful information of a morpheme
	/// that is not supported by the data layer.
	/// </remarks>
	internal class MorphemeWrapper
	{
		/// <summary>
		/// The real morpheme.
		/// </summary>
		private readonly IMorpheme _morpheme;

		/// <summary>
		/// List of predecessor affixes.
		/// </summary>
		private readonly StringCollection _predecessors;

		/// <summary>
		/// List of successor affixes.
		/// </summary>
		private readonly StringCollection _successors;

		/// <summary>
		/// Starting class.
		/// </summary>
		private IClass _start;

		/// <summary>
		/// Ending class.
		/// </summary>
		private IClass _end;

		/// <summary>
		/// Cosntructor.
		/// </summary>
		/// <param name="m">The real morpheme that is wrapped.</param>
		public MorphemeWrapper(IMorpheme m)
		{
			_morpheme = m;
			_predecessors = new StringCollection();
			_successors = new StringCollection();
			_start = null;
			_end = null;
		}

		/// <summary>
		/// Adds a new affix as a successor, if it is not already in the list.
		/// </summary>
		/// <param name="succ">Affix ID.</param>
		public void AddAsSuccessor(string succ)
		{
			if (_successors.Cast<string>().Any(t => t == succ))
				return;

			_successors.Add(succ);
		}

		/// <summary>
		/// Adds a new affix as a predecessor, if it is not already in the list.
		/// </summary>
		/// <param name="pred">Affix ID.</param>
		public void AddAsPredecessor(string pred)
		{
			if (_predecessors.Cast<string>().Any(t => t == pred))
				return;

			_predecessors.Add(pred);
		}

		/// <summary>
		/// Set the starting and ending class IDREFs of the real morpheme.
		/// </summary>
		public void SetStartAndEnd()
		{
			if (_start != null)
				_morpheme.StartClass = _start.Id;
			if (_end != null)
				_morpheme.EndClass = _end.Id;
		}

		/// <summary>
		/// Remove an affix from the list of predecessors.
		/// </summary>
		/// <param name="pred">The affix ID to remove.</param>
		public void RemovePredecessor(string pred)
		{
			_predecessors.Remove(pred);
		}

		/// <summary>
		/// Remove an affix from the list of successors.
		/// </summary>
		/// <param name="succ">The affix ID to remove.</param>
		public void RemoveSuccessor(string succ)
		{
			_successors.Remove(succ);
		}

		/// <summary>
		/// Check to see if the morpheme can be assigned a class.
		/// </summary>
		/// <param name="listType">The type of list to check.</param>
		/// <returns>True if the class can be assigne, otherwise false.</returns>
		public bool CanAssignClass(ListType listType)
		{
			bool fRetVal;
			switch (listType)
			{
				default:
					fRetVal = false;
					break;
				case ListType.Pred:
					fRetVal = (_predecessors.Count == 0);
					break;
				case ListType.Succ:
					fRetVal = (_successors.Count == 0);
					break;
			}
			return fRetVal;
		}

		/// <summary>
		/// Set the starting or ending class.
		/// </summary>
		/// <param name="isStartPoint">True if the class is the starting class,
		/// otherwise false.</param>
		/// <param name="cls">The class to set.</param>
		public void SetAffixClass(bool isStartPoint, IClass cls)
		{
			if (isStartPoint)
			{
				if (_start == null)
					_start = cls;
				else
					Debug.Assert(false);
			}
			else
			{
				if (_end == null)
					_end = cls;
				else
					Debug.Assert(false);
			}
		}

		/// <summary>
		/// Get the ID of the wrapped morpheme.
		/// </summary>
		/// <returns>The ID of the wrapped morpheme.</returns>
		public string GetId()
		{
			return _morpheme.Id;
		}
	}
}