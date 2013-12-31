// --------------------------------------------------------------------------------------------
// Copyright (C) 2003-2013 SIL International. All rights reserved.
//
// Distributable under the terms of the MIT License, as specified in the license.rtf file.
// --------------------------------------------------------------------------------------------
#if USEMATRIXFORDISTINCTSETS
using System;

// Modified from: http://www.heatonresearch.com/
namespace SIL.WordWorks.GAFAWS.PositionAnalysis.Impl
{
	/// <summary>
	/// Matrix: This class implements a mathematical matrix.  Matrix
	/// math is very important to neural network processing.  Many
	/// of the classes developed in this book will make use of the
	/// matrix classes in this package.
	/// </summary>
	internal class Matrix
	{
		/// <summary>
		/// The matrix data, stored as a 2D array.
		/// </summary>
		readonly uint[,] _matrix;

		/// <summary>
		/// Allows index access to the elements of the matrix.
		/// </summary>
		/// <param name="row">The row to access.</param>
		/// <param name="col">The column to access.</param>
		/// <returns>The element at the specified position in the matrix.</returns>
		internal uint this[int row, int col]
		{
			get
			{
				Validate(row, col);
				return _matrix[row, col];
			}
			set
			{
				Validate(row, col);
				_matrix[row, col] = value;
			}
		}

		/// <summary>
		/// Create a matrix that is a single column.
		/// </summary>
		/// <param name="input">A 1D array to make the matrix from.</param>
		/// <returns>A matrix that contains a single column.</returns>
		internal static Matrix CreateColumnMatrix(uint[] input)
		{
			var d = new uint[input.Length, 1];
			for (var row = 0; row < d.Length; row++)
			{
				d[row, 0] = input[row];
			}
			return new Matrix(d);
		}

		/// <summary>
		/// Create a matrix that is a single row.
		/// </summary>
		/// <param name="input">A 1D array to make the matrix from.</param>
		/// <returns>A matrix that contans a single row.</returns>
		internal static Matrix CreateRowMatrix(uint[] input)
		{
			var d = new uint[1, input.Length];

			for (var i = 0; i < input.Length; i++)
			{
				d[0, i] = input[i];
			}

			return new Matrix(d);
		}

		/// <summary>
		/// Construct a matrix from a 2D double array.
		/// </summary>
		/// <param name="sourceMatrix">A 2D double array.</param>
		internal Matrix(uint[,] sourceMatrix)
		{
			_matrix = new uint[sourceMatrix.GetUpperBound(0) + 1, sourceMatrix.GetUpperBound(1) + 1];
			for (var r = 0; r < Rows; r++)
			{
				for (var c = 0; c < Cols; c++)
				{
					this[r, c] = sourceMatrix[r, c];
				}
			}
		}

		/// <summary>
		/// Construct a blank matrix with the specified number of rows and columns.
		/// </summary>
		/// <param name="rows">How many rows.</param>
		/// <param name="cols">How many columns.</param>
		internal Matrix(int rows, int cols)
		{
			_matrix = new uint[rows, cols];
		}

		/// <summary>
		/// Add the specified value to the specified row and column of the matrix.
		/// </summary>
		/// <param name="row">The row to add to.</param>
		/// <param name="col">The column to add to.</param>
		/// <param name="value">The value to add.</param>
		internal void Add(int row, int col, uint value)
		{
			Validate(row, col);
			var newValue = this[row, col] + value;
			this[row, col] = newValue;
		}

		/// <summary>
		/// Clear the matrix.
		/// </summary>
		internal void Clear()
		{
			for (var r = 0; r < Rows; r++)
			{
				for (var c = 0; c < Cols; c++)
				{
					this[r, c] = 0;
				}
			}
		}

		/// <summary>
		/// Clone the matrix.
		/// </summary>
		/// <returns>A cloned copy of the matrix.</returns>
		internal Matrix Clone()
		{
			return new Matrix(_matrix);
		}

		/// <summary>
		/// Determine if this matrix is equal to another.  Use a precision of 10 decimal places.
		/// </summary>
		/// <param name="matrix">The other matrix to compare.</param>
		/// <returns>True if the two matrixes are equal.</returns>
		internal bool Equals(Matrix matrix)
		{
			return Equals(matrix, 10);
		}

		/// <summary>
		/// Compare the matrix to another with the specified level of precision.
		/// </summary>
		/// <param name="matrix">The other matrix to compare.</param>
		/// <param name="precision">The number of decimal places of precision to use.</param>
		/// <returns>True if the two matrixes are equal.</returns>
		internal bool Equals(Matrix matrix, int precision)
		{

			if (precision < 0)
			{
				throw new MatrixError("Precision can't be a negative number.");
			}

			var test = Math.Pow(10.0, precision);
			if (Double.IsInfinity(test) || (test > Int64.MaxValue))
			{
				throw new MatrixError("Precision of " + precision
						+ " decimal places is not supported.");
			}

			precision = (int)Math.Pow(10, precision);

			for (var r = 0; r < Rows; r++)
			{
				for (var c = 0; c < Cols; c++)
				{
					if (this[r, c] * precision != matrix[r, c] * precision)
					{
						return false;
					}
				}
			}

			return true;
		}


		/// <summary>
		/// Take the values of thie matrix from a packed array.
		/// </summary>
		/// <param name="array">The packed array to read the matrix from.</param>
		/// <param name="index">The index to begin reading at in the array.</param>
		/// <returns>The new index after this matrix has been read.</returns>
		internal int FromPackedArray(uint[] array, int index)
		{

			for (var r = 0; r < Rows; r++)
			{
				for (var c = 0; c < Cols; c++)
				{
					_matrix[r, c] = array[index++];
				}
			}

			return index;
		}

		/// <summary>
		/// Get one column from this matrix as a column matrix.
		/// </summary>
		/// <param name="col">The desired column.</param>
		/// <returns>The column matrix.</returns>
		internal Matrix GetCol(int col)
		{
			if (col > Cols)
			{
				throw new MatrixError("Can't get column #" + col
						+ " because it does not exist.");
			}

			var newMatrix = new uint[Rows, 1];

			for (var row = 0; row < Rows; row++)
			{
				newMatrix[row, 0] = _matrix[row, col];
			}

			return new Matrix(newMatrix);
		}

		/// <summary>
		/// Get the number of columns in this matrix
		/// </summary>
		internal int Cols
		{
			get
			{
				return _matrix.GetUpperBound(1) + 1;
			}
		}

		/// <summary>
		/// Get the specified row as a row matrix.
		/// </summary>
		/// <param name="row">The desired row.</param>
		/// <returns>A row matrix.</returns>
		internal Matrix GetRow(int row)
		{
			if (row > Rows)
			{
				throw new MatrixError("Can't get row #" + row
						+ " because it does not exist.");
			}

			var newMatrix = new uint[1, Cols];

			for (var col = 0; col < Cols; col++)
			{
				newMatrix[0, col] = _matrix[row, col];
			}

			return new Matrix(newMatrix);
		}

		/// <summary>
		/// Get the number of rows in this matrix
		/// </summary>
		internal int Rows
		{
			get
			{
				return _matrix.GetUpperBound(0) + 1;
			}
		}


		/// <summary>
		/// Determine if this matrix is a vector.  A vector matrix only has a single row or column.
		/// </summary>
		/// <returns>True if this matrix is a vector.</returns>
		internal bool IsVector()
		{
			if (Rows == 1)
			{
				return true;
			}

			return Cols == 1;
		}

		/// <summary>
		/// Determine if all of the values in the matrix are zero.
		/// </summary>
		/// <returns>True if all of the values in the matrix are zero.</returns>
		internal bool IsZero()
		{
			for (var row = 0; row < Rows; row++)
			{
				for (var col = 0; col < Cols; col++)
				{
					if (_matrix[row, col] != 0)
						return false;
				}
			}
			return true;
		}

		///// <summary>
		///// Fill the matrix with random values in the specified range.
		///// </summary>
		///// <param name="min">The minimum value for the random numbers.</param>
		///// <param name="max">The maximum value for the random numbers.</param>
		//internal void Ramdomize(uint min, uint max)
		//{
		//    var rand = new Random();

		//    for (var r = 0; r < Rows; r++)
		//    {
		//        for (var c = 0; c < Cols; c++)
		//        {
		//            _matrix[r, c] = (uint)(rand.Next() * (max - min)) + min;
		//        }
		//    }
		//}

		/// <summary>
		/// Get the size fo the matrix.  This is thr rows times the columns.
		/// </summary>
		internal int Size
		{
			get { return Rows * Cols; }
		}

		/// <summary>
		/// Sum all of the values in the matrix.
		/// </summary>
		/// <returns>The sum of all of the values in the matrix.</returns>
		internal uint Sum()
		{
			uint result = 0;
			for (var r = 0; r < Rows; r++)
			{
				for (var c = 0; c < Cols; c++)
				{
					result += _matrix[r, c];
				}
			}
			return result;
		}

		/// <summary>
		/// Convert the matrix to a packed array.
		/// </summary>
		/// <returns>A packed array.</returns>
		internal uint[] ToPackedArray()
		{
			var result = new uint[Rows * Cols];

			var index = 0;
			for (var r = 0; r < Rows; r++)
			{
				for (var c = 0; c < Cols; c++)
				{
					result[index++] = _matrix[r, c];
				}
			}

			return result;
		}

		/// <summary>
		/// Validate that the specified row and column are inside of the range of the matrix.
		/// </summary>
		/// <param name="row">The row to check.</param>
		/// <param name="col">The column to check.</param>
		internal void Validate(int row, int col)
		{
			if ((row >= Rows) || (row < 0))
			{
				throw new MatrixError("The row:" + row + " is out of range:"
						+ Rows);
			}

			if ((col >= Cols) || (col < 0))
			{
				throw new MatrixError("The col:" + col + " is out of range:"
						+ Cols);
			}
		}

		/// <summary>
		/// Create an identity matrix, of the specified size.  An identity matrix is always square.
		/// </summary>
		/// <param name="size"></param>
		/// <returns></returns>
		internal static Matrix Identity(int size)
		{
			if (size < 1)
			{
				throw new MatrixError("Identity matrix must be at least of size 1.");
			}

			var result = new Matrix(size, size);

			for (var i = 0; i < size; i++)
			{
				result[i, i] = 1;
			}

			return result;
		}
	}
}
#endif