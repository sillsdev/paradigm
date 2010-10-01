#if USEMATRIXFORDISTINCTSETS
using System;
using System.Linq;

// Modified from: http://www.heatonresearch.com/
namespace SIL.WordWorks.GAFAWS.PositionAnalysis.Impl
{
	/// <summary>
	/// MatrixMath: This class can perform many different mathematical
	/// operations on matrixes.
	/// </summary>
	internal class MatrixMath
	{
		/// <summary>
		/// Add two matrixes together, producing a third.
		/// </summary>
		/// <param name="a">The first matrix to add.</param>
		/// <param name="b">The second matrix to add.</param>
		/// <returns>The two matrixes added together.</returns>
		internal static Matrix Add(Matrix a, Matrix b)
		{
			if (a.Rows != b.Rows)
			{
				throw new MatrixError(
						"To add the matrixes they must have the same number of rows and columns.  Matrix a has "
								+ a.Rows
								+ " rows and matrix b has "
								+ b.Rows + " rows.");
			}

			if (a.Cols != b.Cols)
			{
				throw new MatrixError(
						"To add the matrixes they must have the same number of rows and columns.  Matrix a has "
								+ a.Cols
								+ " cols and matrix b has "
								+ b.Cols + " cols.");
			}

			var result = new uint[a.Rows, a.Cols];

			for (var resultRow = 0; resultRow < a.Rows; resultRow++)
			{
				for (var resultCol = 0; resultCol < a.Cols; resultCol++)
				{
					result[resultRow, resultCol] = a[resultRow, resultCol]
							+ b[resultRow, resultCol];
				}
			}

			return new Matrix(result);
		}

		/// <summary>
		/// Copy the source matrix to the target matrix.  Both matrixes must have the same dimensions.
		/// </summary>
		/// <param name="source">The source matrix.</param>
		/// <param name="target">The target matrix.</param>
		internal static void Copy(Matrix source, Matrix target)
		{
			for (var row = 0; row < source.Rows; row++)
			{
				for (var col = 0; col < source.Cols; col++)
				{
					target[row, col] = source[row, col];
				}
			}

		}

		/// <summary>
		/// Delete a single column from a matrix.  A new matrix, with the delete is returned.
		/// </summary>
		/// <param name="matrix">The matrix to delete from.</param>
		/// <param name="deleted">The column to delete.</param>
		/// <returns>The matrix, with the delete.</returns>
		internal static Matrix DeleteCol(Matrix matrix, int deleted)
		{
			if (deleted >= matrix.Cols)
			{
				throw new MatrixError("Can't delete column " + deleted
						+ " from matrix, it only has " + matrix.Cols
						+ " columns.");
			}
			var newMatrix = new uint[matrix.Rows, matrix
					.Cols - 1];

			for (var row = 0; row < matrix.Rows; row++)
			{
				var targetCol = 0;

				for (var col = 0; col < matrix.Cols; col++)
				{
					if (col == deleted)
						continue;

					newMatrix[row, targetCol] = matrix[row, col];
					targetCol++;
				}

			}
			return new Matrix(newMatrix);
		}

		/// <summary>
		/// Delete a row from a matrix.  A new matrix, with the row deleted, is returned.
		/// </summary>
		/// <param name="matrix">The matrix to delete from.</param>
		/// <param name="deleted">The row to delete.</param>
		/// <returns>The matrix, with the row deleted.</returns>
		internal static Matrix DeleteRow(Matrix matrix, int deleted)
		{
			if (deleted >= matrix.Rows)
			{
				throw new MatrixError("Can't delete row " + deleted
						+ " from matrix, it only has " + matrix.Rows
						+ " rows.");
			}
			var newMatrix = new uint[matrix.Rows - 1, matrix
					.Cols];
			var targetRow = 0;
			for (var row = 0; row < matrix.Rows; row++)
			{
				if (row == deleted)
					continue;

				for (var col = 0; col < matrix.Cols; col++)
				{
					newMatrix[targetRow, col] = matrix[row, col];
				}
				targetRow++;
			}
			return new Matrix(newMatrix);
		}

		/// <summary>
		/// Divide every cell in the matrix by the specified number.
		/// </summary>
		/// <param name="a">The matrix to divide.</param>
		/// <param name="b">The number to divide by.</param>
		/// <returns>The divided matrix.</returns>
		internal static Matrix Divide(Matrix a, uint b)
		{
			var result = new uint[a.Rows, a.Cols];
			for (var row = 0; row < a.Rows; row++)
			{
				for (var col = 0; col < a.Cols; col++)
				{
					result[row, col] = a[row, col] / b;
				}
			}
			return new Matrix(result);
		}

		/// <summary>
		/// Compute the dot product for two matrixes.  NB: both matrixes must be vectors.
		/// </summary>
		/// <param name="a">The first matrix, must be a vector.</param>
		/// <param name="b">The second matrix, must be a vector.</param>
		/// <returns>The dot product of the two matrixes.</returns>
		internal static uint DotProduct(Matrix a, Matrix b)
		{
			if (!a.IsVector() || !b.IsVector())
			{
				throw new MatrixError(
						"To take the dot product, both matrixes must be vectors.");
			}

			var aArray = a.ToPackedArray();
			var bArray = b.ToPackedArray();

			if (aArray.Length != bArray.Length)
			{
				throw new MatrixError(
						"To take the dot product, both matrixes must be of the same length.");
			}

			uint result = 0;
			var length = aArray.Length;

			for (var i = 0; i < length; i++)
			{
				result += aArray[i] * bArray[i];
			}

			return result;
		}

		/// <summary>
		/// Multiply every cell in the matrix by the specified value.
		/// </summary>
		/// <param name="a">Multiply every cell in a matrix by the specified value.</param>
		/// <param name="b">The value to multiply by.</param>
		/// <returns>The new multiplied matrix.</returns>
		internal static Matrix Multiply(Matrix a, uint b)
		{
			var result = new uint[a.Rows, a.Cols];
			for (var row = 0; row < a.Rows; row++)
			{
				for (var col = 0; col < a.Cols; col++)
				{
					result[row, col] = a[row, col] * b;
				}
			}
			return new Matrix(result);
		}

		/// <summary>
		/// Multiply two matrixes.
		/// </summary>
		/// <param name="a">The first matrix.</param>
		/// <param name="b">The second matrix.</param>
		/// <returns>The resulting matrix.</returns>
		internal static Matrix Multiply(Matrix a, Matrix b)
		{
			if (a.Cols != b.Rows)
			{
				throw new MatrixError(
						"To use ordinary matrix multiplication the number of columns on the first matrix must mat the number of rows on the second.");
			}

			var result = new uint[a.Rows, b.Cols];

			for (var resultRow = 0; resultRow < a.Rows; resultRow++)
			{
				for (var resultCol = 0; resultCol < b.Cols; resultCol++)
				{
					uint value = 0;

					for (var i = 0; i < a.Cols; i++)
					{
						value += a[resultRow, i] * b[i, resultCol];
					}
					result[resultRow, resultCol] = value;
				}
			}

			return new Matrix(result);
		}

		/// <summary>
		/// Subtract one matrix from another.  The two matrixes must have the same number of rows and columns.
		/// </summary>
		/// <param name="a">The first matrix.</param>
		/// <param name="b">The second matrix.</param>
		/// <returns>The subtracted matrix.</returns>
		internal static Matrix Subtract(Matrix a, Matrix b)
		{
			if (a.Rows != b.Rows)
			{
				throw new MatrixError(
						"To subtract the matrixes they must have the same number of rows and columns.  Matrix a has "
								+ a.Rows
								+ " rows and matrix b has "
								+ b.Rows + " rows.");
			}

			if (a.Cols != b.Cols)
			{
				throw new MatrixError(
						"To subtract the matrixes they must have the same number of rows and columns.  Matrix a has "
								+ a.Cols
								+ " cols and matrix b has "
								+ b.Cols + " cols.");
			}

			var result = new uint[a.Rows, a.Cols];

			for (var resultRow = 0; resultRow < a.Rows; resultRow++)
			{
				for (var resultCol = 0; resultCol < a.Cols; resultCol++)
				{
					result[resultRow, resultCol] = a[resultRow, resultCol]
							- b[resultRow, resultCol];
				}
			}

			return new Matrix(result);
		}

		/// <summary>
		/// Transpose the specified matrix.
		/// </summary>
		/// <param name="input">The matrix to transpose.</param>
		/// <returns>The transposed matrix.</returns>
		internal static Matrix Transpose(Matrix input)
		{
			var inverseMatrix = new uint[input.Cols, input
					.Rows];

			for (var r = 0; r < input.Rows; r++)
			{
				for (var c = 0; c < input.Cols; c++)
				{
					inverseMatrix[c, r] = input[r, c];
				}
			}

			return new Matrix(inverseMatrix);
		}

		/// <summary>
		/// Calculate the vector length of the matrix.
		/// </summary>
		/// <param name="input">The vector to calculate for.</param>
		/// <returns>The vector length.</returns>
		internal static uint VectorLength(Matrix input)
		{
			if (!input.IsVector())
			{
				throw new MatrixError(
						"Can only take the vector length of a vector.");
			}
			var v = input.ToPackedArray();
			var rtn = v.Aggregate<uint, uint>(0, (current, t) => current + (uint)Math.Pow(t, 2));
			return (uint)Math.Sqrt(rtn);
		}

		/// <summary>
		/// Private constructor.  All methods are static.
		/// </summary>
		private MatrixMath()
		{
		}

	}

}
#endif