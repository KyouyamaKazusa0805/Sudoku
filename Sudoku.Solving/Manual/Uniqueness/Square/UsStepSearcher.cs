using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.DocComments;
using Sudoku.Solving.Annotations;

namespace Sudoku.Solving.Manual.Uniqueness.Square
{
	/// <summary>
	/// Encapsulates a <b>uniqueness square</b> (US) technique searcher.
	/// </summary>
	public sealed partial class UsStepSearcher : UniquenessStepSearcher
	{
		/// <inheritdoc cref="SearchingProperties"/>
		public static TechniqueProperties Properties { get; } = new(53, nameof(TechniqueCode.UsType1))
		{
			DisplayLevel = 2
		};


		/// <inheritdoc/>
		public override void GetAll(IList<StepInfo> accumulator, in SudokuGrid grid)
		{
			foreach (var pattern in Patterns)
			{
				if ((EmptyMap | pattern) != EmptyMap)
				{
					continue;
				}

				short mask = 0;
				foreach (int cell in pattern)
				{
					mask |= grid.GetCandidateMask(cell);
				}

				CheckType1(accumulator, grid, pattern, mask);
				CheckType2(accumulator, pattern, mask);
				CheckType3(accumulator, grid, pattern, mask);
				CheckType4(accumulator, grid, pattern, mask);
			}
		}

		partial void CheckType1(IList<StepInfo> accumulator, in SudokuGrid grid, in Cells pattern, short mask);
		partial void CheckType2(IList<StepInfo> accumulator, in Cells pattern, short mask);
		partial void CheckType3(IList<StepInfo> accumulator, in SudokuGrid grid, in Cells pattern, short mask);
		partial void CheckType4(IList<StepInfo> accumulator, in SudokuGrid grid, in Cells pattern, short mask);
	}
}
