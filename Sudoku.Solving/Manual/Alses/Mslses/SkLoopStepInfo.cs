using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;

namespace Sudoku.Solving.Manual.Alses.Mslses
{
	/// <summary>
	/// Provides a usage of <b>domino loop</b> technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Cells">All cells used.</param>
	public sealed record SkLoopStepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views, IReadOnlyList<int> Cells)
		: MslsStepInfo(Conclusions, Views)
	{
		/// <inheritdoc/>
		public override decimal Difficulty => 9.6M;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Nightmare;

		/// <inheritdoc/>
		public override TechniqueCode TechniqueCode => TechniqueCode.SkLoop;


		/// <inheritdoc/>
		public override string ToString()
		{
			string cellsStr = new Cells(Cells).ToString();
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			return $"{Name}: {Cells.Count} Cells {cellsStr} => {elimStr}";
		}
	}
}
