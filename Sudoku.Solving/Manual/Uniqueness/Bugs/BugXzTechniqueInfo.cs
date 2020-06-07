using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Extensions;

namespace Sudoku.Solving.Manual.Uniqueness.Bugs
{
	/// <summary>
	/// Provides a usage of <b>bivalue universal grave XZ rule</b> (BUG-XZ) technique.
	/// </summary>
	public sealed class BugXzTechniqueInfo : UniquenessTechniqueInfo
	{
		/// <include file='SolvingDocComments.xml' path='comments/constructor[@type="TechniqueInfo"]'/>
		/// <param name="digitMask">The digits mask.</param>
		/// <param name="cells">All cell offsets.</param>
		/// <param name="extraCell">The extra cell.</param>
		public BugXzTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views,
			short digitMask, IReadOnlyList<int> cells, int extraCell) : base(conclusions, views) =>
			(DigitsMask, Cells, ExtraCell) = (digitMask, cells, extraCell);


		/// <summary>
		/// Indicates the digits mask.
		/// </summary>
		public short DigitsMask { get; }

		/// <summary>
		/// Indicates the cell offsets.
		/// </summary>
		public IReadOnlyList<int> Cells { get; }

		/// <summary>
		/// Indicates the extra cell.
		/// </summary>
		public int ExtraCell { get; }

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
			string extraCellStr = new CellCollection(ExtraCell).ToString();
			string cellsStr = new CellCollection(Cells).ToString();
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			return $"{Name}: {digit} with cells {cellsStr}, with extra cell {extraCellStr} => {elimStr}";
		}
	}
}
