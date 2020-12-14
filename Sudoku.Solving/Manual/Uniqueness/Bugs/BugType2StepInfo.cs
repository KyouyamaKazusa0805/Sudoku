using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;

namespace Sudoku.Solving.Manual.Uniqueness.Bugs
{
	/// <summary>
	/// Provides a usage of <b>bivalue universal grave</b> (BUG) type 2 technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Digit">The digit.</param>
	/// <param name="Cells">All cell offsets.</param>
	public sealed record BugType2StepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views, int Digit, IReadOnlyList<int> Cells)
		: UniquenessStepInfo(Conclusions, Views)
	{
		/// <summary>
		/// The table of extra difficulty values.
		/// </summary>
		private static readonly decimal[] ExtraDifficulty =
		{
			.1M, .2M, .2M, .3M, .3M, .3M, .4M, .4M, .4M, .4M,
			.5M, .5M, .5M, .5M, .5M, .6M, .6M, .6M, .6M, .6M
		};


		/// <inheritdoc/>
		public override decimal Difficulty => 5.6M + ExtraDifficulty[Cells.Count - 1];

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;

		/// <inheritdoc/>
		public override TechniqueCode TechniqueCode => TechniqueCode.BugType2;


		/// <inheritdoc/>
		public override string ToString()
		{
			int digit = Digit + 1;
			string cellsStr = new Cells(Cells).ToString();
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			return $"{Name}: {digit} with cells {cellsStr} => {elimStr}";
		}
	}
}
