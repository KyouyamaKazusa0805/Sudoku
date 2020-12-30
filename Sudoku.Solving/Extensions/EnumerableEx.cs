using System.Collections.Generic;
using System.Extensions;
using System.Runtime.CompilerServices;
using Sudoku.Data;
using Sudoku.Data.Extensions;

namespace Sudoku.Solving.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="IEnumerable{T}"/>.
	/// </summary>
	/// <seealso cref="IEnumerable{T}"/>
	public static class EnumerableEx
	{
		/// <summary>
		/// Check whether the current loop is valid UL-shaped loop.
		/// </summary>
		/// <param name="loop">(<see langword="this"/> parameter) The loop.</param>
		/// <returns>A <see cref="bool"/> value indicating that.</returns>
		[SkipLocalsInit]
		public static unsafe bool IsValidLoop(this IEnumerable<int> loop)
		{
			int visitedOddRegions = 0, visitedEvenRegions = 0;
			bool isOdd;
			foreach (int cell in loop)
			{
				for (var label = RegionLabel.Block; label <= RegionLabel.Column; label++)
				{
					int region = label.ToRegion(cell);
					if (*&isOdd)
					{
						if (visitedOddRegions.ContainsBit(region))
						{
							return false;
						}
						else
						{
							visitedOddRegions |= 1 << region;
						}
					}
					else
					{
						if (visitedEvenRegions.ContainsBit(region))
						{
							return false;
						}
						else
						{
							visitedEvenRegions |= 1 << region;
						}
					}
				}

				*&isOdd = !*&isOdd;
			}

			return visitedEvenRegions == visitedOddRegions;
		}
	}
}
