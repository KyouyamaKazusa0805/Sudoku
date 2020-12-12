using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.DocComments;
using Sudoku.Solving.Annotations;

namespace Sudoku.Solving.Manual.Symmetry
{
	/// <summary>
	/// Encapsulates a <b>Gurth's symmetrical placement 2</b> (GSP2) technique searcher.
	/// </summary>
	public sealed class Gsp2StepSearcher : SymmetryStepSearcher
	{
		/// <summary>
		/// Indicates the iteration list for row swapping.
		/// </summary>
		private static readonly int[][]?[] RowIterationList =
		{
			new int[][] { null!, new[] { 9, 10 }, new[] { 9, 11 }, new[] { 10, 11 } },
			new int[][] { null!, new[] { 12, 13 }, new[] { 12, 14 }, new[] { 13, 14 } },
			new int[][] { null!, new[] { 15, 16 }, new[] { 15, 17 }, new[] { 16, 17 } }
		};

		/// <summary>
		/// Indicates the iteration list for column swapping.
		/// </summary>
		private static readonly int[][]?[] ColumnIterationList =
		{
			new int[][] { null!, new[] { 18, 19 }, new[] { 18, 20 }, new[] { 19, 20 } },
			new int[][] { null!, new[] { 21, 22 }, new[] { 21, 23 }, new[] { 22, 23 } },
			new int[][] { null!, new[] { 24, 25 }, new[] { 24, 26 }, new[] { 25, 26 } }
		};


		/// <inheritdoc cref="SearchingProperties"/>
		public static TechniqueProperties Properties { get; } = new(default, nameof(TechniqueCode.Gsp))
		{
			IsReadOnly = true
		};


		/// <inheritdoc/>
		public override void GetAll(IList<StepInfo> accumulator, in SudokuGrid grid)
		{
		}
	}
}
