using System.Collections.Generic;
using System.Extensions;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;

namespace Sudoku.Solving.Manual.Alses
{
	/// <summary>
	/// Provides a usage of <b>3-dimension sue de coq</b> technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="RowDigitsMask">The row digits mask.</param>
	/// <param name="ColumnDigitsMask">The column digits mask.</param>
	/// <param name="BlockDigitsMask">The block digits mask.</param>
	/// <param name="RowCells">The row cells map.</param>
	/// <param name="ColumnCells">The column cells map.</param>
	/// <param name="BlockCells">The block cells map.</param>
	public sealed record Sdc3dStepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views,
		short RowDigitsMask, short ColumnDigitsMask, short BlockDigitsMask,
		in Cells RowCells, in Cells ColumnCells, in Cells BlockCells) : AlsStepInfo(Conclusions, Views)
	{
		/// <inheritdoc/>
		public override decimal Difficulty => 5.5M;

		/// <inheritdoc/>
		public override TechniqueCode TechniqueCode => TechniqueCode.Sdc3d;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;


		/// <inheritdoc/>
		public override string ToString()
		{
			string cells1Str = RowCells.ToString();
			string cells2Str = ColumnCells.ToString();
			string cells3Str = BlockCells.ToString();
			string digits1Str = new DigitCollection(RowDigitsMask.GetAllSets()).ToString();
			string digits2Str = new DigitCollection(ColumnDigitsMask.GetAllSets()).ToString();
			string digits3Str = new DigitCollection(BlockDigitsMask.GetAllSets()).ToString();
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			return
				$"{Name}: {cells1Str}({digits1Str}) + {cells2Str}({digits2Str}) + " +
				$"{cells3Str}({digits3Str}) => {elimStr}";
		}
	}
}
