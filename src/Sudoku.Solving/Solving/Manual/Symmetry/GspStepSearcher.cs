using System;
using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.Symmetry
{
	/// <summary>
	/// Encapsulates a <b>Gurth's symmetrical placement</b> (GSP) technique searcher.
	/// </summary>
	[DirectSearcher, IsOptionsFixed]
	public sealed partial class GspStepSearcher : SymmetryStepSearcher
	{
		/// <inheritdoc/>
		public override SearchingOptions Options { get; set; } = new(default);

		/// <summary>
		/// Indicates the searcher properties.
		/// </summary>
		/// <remarks>
		/// Please note that all technique searches should contain
		/// this static property in order to display on settings window. If the searcher doesn't contain,
		/// when we open the settings window, it'll throw an exception to report about this.
		/// </remarks>
		[Obsolete("Please use the property '" + nameof(Options) + "' instead.", false)]
		public static TechniqueProperties Properties { get; } = new(default, nameof(Technique.Gsp))
		{
			IsReadOnly = true
		};


		/// <inheritdoc/>
		public override void GetAll(IList<StepInfo> accumulator, in SudokuGrid grid)
		{
			// To verify all kinds of symmetry.
			var conclusions = new List<Conclusion>();
			CheckDiagonal(conclusions, grid, out var diagonalInfo);
			CheckAntiDiagonal(conclusions, grid, out var antidiagonalInfo);
			CheckCentral(conclusions, grid, out var centralInfo);

			if (conclusions.Count == 0)
			{
				return;
			}

			accumulator.Add((diagonalInfo | antidiagonalInfo | centralInfo)!);
		}


		private static unsafe partial void CheckDiagonal(IList<Conclusion> result, in SudokuGrid grid, out GspStepInfo? info);
		private static unsafe partial void CheckAntiDiagonal(IList<Conclusion> result, in SudokuGrid grid, out GspStepInfo? info);
		private static unsafe partial void CheckCentral(IList<Conclusion> result, in SudokuGrid grid, out GspStepInfo? info);
	}
}
