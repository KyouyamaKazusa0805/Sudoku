using System;
using System.Collections.Generic;
using System.Linq;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.Extensions;
using static Sudoku.GridProcessings;

namespace Sudoku.Solving.Manual.Uniqueness.Rectangles
{
	/// <summary>
	/// Encapsulates an <b>unique rectangle extension</b> (UR+) technique searcher.
	/// </summary>
	[TechniqueDisplay("Unique Rectangle Extension")]
	public sealed class UrExtensionTechniqueSearcher : RectangleTechniqueSearcher
	{
		/// <summary>
		/// Indicates whether the solver should check uncompleted URs.
		/// </summary>
		private readonly bool _checkIncompleted;


		/// <summary>
		/// Initializes an instance with the checking option.
		/// </summary>
		/// <param name="checkIncompletedUniquenessPatterns">
		/// Indicates whether the solver should check uncompleted URs.
		/// </param>
		public UrExtensionTechniqueSearcher(bool checkIncompletedUniquenessPatterns) =>
			_checkIncompleted = checkIncompletedUniquenessPatterns;


		/// <summary>
		/// Indicates the priority of this technique.
		/// </summary>
		public static int Priority { get; set; } = 47;

		/// <summary>
		/// Indicates whether the technique is enabled.
		/// </summary>
		public static bool IsEnabled { get; set; } = true;


		/// <inheritdoc/>
		public override void GetAll(IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid)
		{

		}


		/// <summary>
		/// To check whether the specified region contains only two cells containing the specified
		/// digit, and these two cells are same as the argument <paramref name="cells"/>.
		/// </summary>
		/// <param name="cells">The cells.</param>
		/// <param name="digit">The digits.</param>
		/// <param name="region">The regions.</param>
		/// <param name="grid">The grid.</param>
		/// <returns>A <see cref="bool"/> value indicating that.</returns>
		private static bool IsExact(int[] cells, int digit, int region, IReadOnlyGrid grid)
		{
			if (grid.HasDigitValue(digit, region))
			{
				return false;
			}

			var map = new GridMap(cells);
			var z = GridMap.Empty;
			foreach (int cell in RegionCells[region])
			{
				if (!(grid.Exists(cell, digit) is true))
				{
					continue;
				}

				z.Add(cell);
			}

			return map == z;
		}
	}
}
