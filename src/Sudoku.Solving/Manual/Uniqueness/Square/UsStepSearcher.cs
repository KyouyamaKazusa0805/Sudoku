using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Techniques;
using static Sudoku.Solving.Manual.FastProperties;

namespace Sudoku.Solving.Manual.Uniqueness.Square
{
	/// <summary>
	/// Encapsulates a <b>uniqueness square</b> (US) technique searcher.
	/// </summary>
	public sealed partial class UsStepSearcher : UniquenessStepSearcher
	{
		/// <summary>
		/// Indicates the searcher properties.
		/// </summary>
		/// <remarks>
		/// Please note that all technique searches should contain
		/// this static property in order to display on settings window. If the searcher doesn't contain,
		/// when we open the settings window, it'll throw an exception to report about this.
		/// </remarks>
		public static TechniqueProperties Properties { get; } = new(16, nameof(Technique.UsType1))
		{
			DisplayLevel = 2
		};


		/// <inheritdoc/>
		public override void GetAll(IList<StepInfo> accumulator, in SudokuGrid grid)
		{
			foreach (var pattern in Patterns)
			{
				if ((EmptyMap & pattern) != pattern)
				{
					continue;
				}

				short mask = 0;
				foreach (int cell in pattern)
				{
					mask |= grid.GetCandidates(cell);
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
