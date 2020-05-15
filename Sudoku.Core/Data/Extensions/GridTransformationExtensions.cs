using System;
using System.Runtime.CompilerServices;
using Sudoku.Constants;
using static Sudoku.Constants.Processings;

namespace Sudoku.Data.Extensions
{
	/// <summary>
	/// Provides a series of methods for <see cref="IReadOnlyGrid"/>
	/// using in transformations.
	/// </summary>
	/// <seealso cref="IReadOnlyGrid"/>
	public static class GridTransformationExtensions
	{
		/// <summary>
		/// The table of clockwise rotation.
		/// </summary>
		private static readonly int[] ClockwiseTable =
		{
			72, 63, 54, 45, 36, 27, 18, 9, 0,
			73, 64, 55, 46, 37, 28, 19, 10, 1,
			74, 65, 56, 47, 38, 29, 20, 11, 2,
			75, 66, 57, 48, 39, 30, 21, 12, 3,
			76, 67, 58, 49, 40, 31, 22, 13, 4,
			77, 68, 59, 50, 41, 32, 23, 14, 5,
			78, 69, 60, 51, 42, 33, 24, 15, 6,
			79, 70, 61, 52, 43, 34, 25, 16, 7,
			80, 71, 62, 53, 44, 35, 26, 17, 8
		};

		/// <summary>
		/// The table of counter-clockwise rotation.
		/// </summary>
		private static readonly int[] CounterClockwiseTable =
		{
			8, 17, 26, 35, 44, 53, 62, 71, 80,
			7, 16, 25, 34, 43, 52, 61, 70, 79,
			6, 15, 24, 33, 42, 51, 60, 69, 78,
			5, 14, 23, 32, 41, 50, 59, 68, 77,
			4, 13, 22, 31, 40, 49, 58, 67, 76,
			3, 12, 21, 30, 39, 48, 57, 66, 75,
			2, 11, 20, 29, 38, 47, 56, 65, 74,
			1, 10, 19, 28, 37, 46, 55, 64, 73,
			0, 9, 18, 27, 36, 45, 54, 63, 72
		};

		/// <summary>
		/// The table of pi-rotation.
		/// </summary>
		private static readonly int[] PiRotateTable =
		{
			80, 79, 78, 77, 76, 75, 74, 73, 72,
			71, 70, 69, 68, 67, 66, 65, 64, 63,
			62, 61, 60, 59, 58, 57, 56, 55, 54,
			53, 52, 51, 50, 49, 48, 47, 46, 45,
			44, 43, 42, 41, 40, 39, 38, 37, 36,
			35, 34, 33, 32, 31, 30, 29, 28, 27,
			26, 25, 24, 23, 22, 21, 20, 19, 18,
			17, 16, 15, 14, 13, 12, 11, 10, 9,
			8, 7, 6, 5, 4, 3, 2, 1, 0
		};


		/// <summary>
		/// Mirror left-right the grid.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The grid.</param>
		/// <returns>The result grid.</returns>
		public static Grid MirrorLeftRight(this IReadOnlyGrid @this)
		{
			var result = @this.Clone();
			for (int i = 0; i < 4; i++)
			{
				for (int j = 0; j < 9; j++)
				{
					short temp = result.GetMask(i * 9 + j);
					result.SetMask(i * 9 + j, result.GetMask(i * 9 + (8 - j)));
					result.SetMask(i * 9 + (8 - j), temp);
				}
			}

			return result;
		}

		/// <summary>
		/// Mirror top-bottom the grid.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The grid.</param>
		/// <returns>The result grid.</returns>
		public static Grid MirrorTopBottom(this IReadOnlyGrid @this)
		{
			var result = @this.Clone();
			for (int i = 0; i < 9; i++)
			{
				for (int j = 0; j < 4; j++)
				{
					short temp = result.GetMask(i * 9 + j);
					result.SetMask(i * 9 + j, result.GetMask((8 - i) * 9 + j));
					result.SetMask((8 - i) * 9 + j, temp);
				}
			}

			return result;
		}

		/// <summary>
		/// Mirror diagonal the grid.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The grid.</param>
		/// <returns>The result grid.</returns>
		public static Grid MirrorDiagonal(this IReadOnlyGrid @this)
		{
			var result = @this.Clone();
			for (int i = 1; i < 9; i++)
			{
				for (int j = 0; j < i; j++)
				{
					short temp = result.GetMask(i * 9 + j);
					result.SetMask(i * 9 + j, result.GetMask(j * 9 + i));
					result.SetMask(j * 9 + i, temp);
				}
			}

			return result;
		}

		/// <summary>
		/// Transpose the grid.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The grid.</param>
		/// <returns>The result grid.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Grid Transpose(this IReadOnlyGrid @this) => @this.MirrorDiagonal();

		/// <summary>
		/// Mirror anti-diagonal the grid.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The grid.</param>
		/// <returns>The result grid.</returns>
		public static Grid MirrorAntidiagonal(this IReadOnlyGrid @this)
		{
			var result = @this.Clone();
			for (int i = 0; i < 9; i++)
			{
				for (int j = 0; j < 8 - i; j++)
				{
					short temp = result.GetMask(i * 9 + j);
					result.SetMask(i * 9 + j, result.GetMask((8 - j) * 9 + (8 - i)));
					result.SetMask((8 - j) * 9 + (8 - i), temp);
				}
			}

			return result;
		}

		/// <summary>
		/// Rotate the grid clockwise.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The grid.</param>
		/// <returns>The result.</returns>
		public static Grid RotateClockwise(this IReadOnlyGrid @this)
		{
			var result = @this.Clone();
			for (int i = 0; i < 81; i++)
			{
				int z = ClockwiseTable[i];
				short temp = result.GetMask(i);
				result.SetMask(i, result.GetMask(z));
				result.SetMask(z, temp);
			}
			return result;
		}

		/// <summary>
		/// Rotate the grid counter-clockwise.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The grid.</param>
		/// <returns>The result.</returns>
		public static Grid RotateCounterClockwise(this IReadOnlyGrid @this)
		{
			var result = @this.Clone();
			for (int i = 0; i < 81; i++)
			{
				int z = CounterClockwiseTable[i];
				short temp = result.GetMask(i);
				result.SetMask(i, result.GetMask(z));
				result.SetMask(z, temp);
			}
			return result;
		}

		/// <summary>
		/// Rotate the grid <c><see cref="Math.PI"/></c> degrees.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The grid.</param>
		/// <returns>The result.</returns>
		public static Grid RotatePi(this IReadOnlyGrid @this)
		{
			var result = @this.Clone();
			for (int i = 0; i < 81; i++)
			{
				int z = PiRotateTable[i];
				short temp = result.GetMask(i);
				result.SetMask(i, result.GetMask(z));
				result.SetMask(z, temp);
			}
			return result;
		}

		/// <summary>
		/// Swap to regions.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The grid.</param>
		/// <param name="region1">The region 1.</param>
		/// <param name="region2">The region 2.</param>
		/// <returns>The result.</returns>
		/// <exception cref="ArgumentException">
		/// Throws when two specified region argument is not in valid range (0..27)
		/// or two regions are not in same region type.
		/// </exception>
		public static Grid SwapTwoRegions(this IReadOnlyGrid @this, int region1, int region2)
		{
			if (region1 < 0 || region1 >= 18)
			{
				throw new ArgumentException("The specified argument is out of valid range.", nameof(region1));
			}
			if (region2 < 0 || region2 >= 18)
			{
				throw new ArgumentException("The specified argument is out of valid range.", nameof(region2));
			}
			if (region1 / 9 != region2 / 9)
			{
				throw new ArgumentException("Two region should be the same region type.");
			}

			var result = @this.Clone();
			for (int i = 0; i < 9; i++)
			{
				int c1 = RegionCells[region1][i];
				int c2 = RegionCells[region2][i];
				short temp = result.GetMask(c1);
				result.SetMask(c1, result.GetMask(c2));
				result.SetMask(c2, temp);
			}

			return result;
		}
	}
}
