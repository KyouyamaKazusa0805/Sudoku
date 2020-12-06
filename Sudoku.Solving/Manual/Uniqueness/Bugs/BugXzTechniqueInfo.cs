using System.Collections.Generic;
using System.Extensions;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;

namespace Sudoku.Solving.Manual.Uniqueness.Bugs
{
	/// <summary>
	/// Provides a usage of <b>bivalue universal grave XZ rule</b> (BUG-XZ) technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="DigitsMask">The digits mask.</param>
	/// <param name="Cells">All cells.</param>
	/// <param name="ExtraCell">The extra cell.</param>
	public sealed record BugXzTechniqueInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views,
		short DigitsMask, IReadOnlyList<int> Cells, int ExtraCell)
		: UniquenessTechniqueInfo(Conclusions, Views)
	{
		/// <inheritdoc/>
		public override decimal Difficulty => 5.8M;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;

		/// <inheritdoc/>
		public override TechniqueCode TechniqueCode => TechniqueCode.BugXz;


		/// <inheritdoc/>
		public override string ToString()
		{
			string digit = new DigitCollection(DigitsMask.GetAllSets()).ToString();
			string extraCellStr = new GridMap { ExtraCell }.ToString();
			string cellsStr = new GridMap(Cells).ToString();
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			return $"{Name}: {digit} with cells {cellsStr}, with extra cell {extraCellStr} => {elimStr}";
		}
	}
}
