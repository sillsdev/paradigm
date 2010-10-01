#if USEMATRIXFORDISTINCTSETS
using System;

// Modified from: http://www.heatonresearch.com/
namespace SIL.WordWorks.GAFAWS.PositionAnalysis.Impl
{
	/// <summary>
	/// Thrown when a matrix error occured.
	/// </summary>
	internal class MatrixError : Exception
	{
		/// <summary>
		/// Constructor for a simple message exception.
		/// </summary>
		/// <param name="str">The message for the exception.</param>
		internal MatrixError(string str)
			: base(str)
		{
		}
	}
}
#endif