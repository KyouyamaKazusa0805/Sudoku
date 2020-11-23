using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Extensions;

namespace Sudoku.Solving.Manual.Alses
{
	/// <summary>
	/// Provides a usage of <b>sue de coq</b> (SdC) technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Block">The block.</param>
	/// <param name="Line">The line.</param>
	/// <param name="BlockMask">The block mask.</param>
	/// <param name="LineMask">The line mask.</param>
	/// <param name="IntersectionMask">The intersection mask.</param>
	/// <param name="IsCannibalistic">Indicates whether the SdC is cannibalistic.</param>
	/// <param name="IsolatedDigitsMask">The isolated digits mask.</param>
	/// <param name="BlockCells">The map of block cells.</param>
	/// <param name="LineCells">The map of line cells.</param>
	/// <param name="IntersectionCells">The map of intersection cells.</param>
	public sealed record SdcTechniqueInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views,
		int Block, int Line, short BlockMask, short LineMask, short IntersectionMask,
		bool IsCannibalistic, short IsolatedDigitsMask, in GridMap BlockCells, in GridMap LineCells,
		in GridMap IntersectionCells)
		: AlsTechniqueInfo(Conclusions, Views)
	{
		/// <inheritdoc/>
		public override decimal Difficulty => 5.0M + (IsolatedDigitsMask != 0 ? .1M : 0) + (IsCannibalistic ? .2M : 0);

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;

		/// <inheritdoc/>
		public override TechniqueCode TechniqueCode =>
			IsCannibalistic ? TechniqueCode.CannibalizedSdc : TechniqueCode.Sdc;


		/// <inheritdoc/>
		public override string ToString()
		{
			string blockCellsStr = BlockCells.ToString();
			string blockDigitsStr = new DigitCollection(BlockMask.GetAllSets()).ToString(null);
			string lineCellsStr = LineCells.ToString();
			string lineDigitsStr = new DigitCollection(LineMask.GetAllSets()).ToString(null);
			string interCellsStr = IntersectionCells.ToString();
			string interDigitsStr = new DigitCollection(IntersectionMask.GetAllSets()).ToString(null);
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			return
				$"{Name}: {interCellsStr}({interDigitsStr}) - " +
				$"{blockCellsStr}({blockDigitsStr}) & {lineCellsStr}({lineDigitsStr}) => {elimStr}";
		}
	}
}
