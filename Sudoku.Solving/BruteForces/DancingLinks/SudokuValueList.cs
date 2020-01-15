using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Sudoku.Solving.BruteForces.DancingLinks
{
	internal sealed class SudokuValueList
	{
		public SudokuValueList() => Values = new int[9, 9];


		public int[,] Values { get; }


		public void SetRow(int[] row, int index)
		{
			for (int i = 0; i < row.Length; i++)
			{
				Values[index, i] = row[i];
			}
		}

		public (List<bool[]>, List<(int, int, int)>) CalculateMatrix()
		{
			var matrix = new List<bool[]>();
			var rcv = new List<(int, int, int)>();

			for (int row = 0; row < 9; row++)
			{
				for (int column = 0; column < 9; column++)
				{
					if (Values[row, column] == 0)
					{
						for (int value = 1; value <= 9; value++)
						{
							matrix.Add(new bool[9 * 9 << 2]);
							SetMatrixValues(matrix[^1], row, column, value);
							rcv.Add((row, column, value));
						}
					}
					else
					{
						matrix.Add(new bool[9 * 9 << 2]);
						SetMatrixValues(matrix[^1], row, column, Values[row, column]);
						rcv.Add((row, column, Values[row, column]));
					}
				}
			}

			return (matrix, rcv);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void SetMatrixValues(bool[] matrixRow, int row, int column, int value)
		{
			int positionConstraint = row * 9 + column;
			int rowConstraint = 81 + row * 9 + (value - 1);
			int columnConstraint = 162 + column * 9 + (value - 1);
			int blockConstraint = 243 + (row / 3 * 3 + column / 3) * 9 + (value - 1);

			matrixRow[positionConstraint] = true;
			matrixRow[rowConstraint] = true;
			matrixRow[columnConstraint] = true;
			matrixRow[blockConstraint] = true;
		}
	}
}
