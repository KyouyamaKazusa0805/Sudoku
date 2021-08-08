using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.RankTheory
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
		in Cells RowCells, in Cells ColumnCells, in Cells BlockCells
	) : RankTheoryStepInfo(Conclusions, Views)
	{
		/// <inheritdoc/>
		public override decimal Difficulty => 5.5M;

		/// <inheritdoc/>
		public override Technique TechniqueCode => Technique.Sdc3d;

		/// <inheritdoc/>
		public override TechniqueTags TechniqueTags => base.TechniqueTags | TechniqueTags.Als;

		/// <inheritdoc/>
		public override TechniqueGroup TechniqueGroup => TechniqueGroup.Sdc;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;

		[FormatItem]
		private string Cells1Str
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => RowCells.ToString();
		}

		[FormatItem]
		private string Digits1Str
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new DigitCollection(RowDigitsMask).ToString();
		}

		[FormatItem]
		private string Cells2Str
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => ColumnCells.ToString();
		}

		[FormatItem]
		private string Digits2Str
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new DigitCollection(ColumnDigitsMask).ToString();
		}

		[FormatItem]
		private string Cells3Str
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => BlockCells.ToString();
		}

		[FormatItem]
		private string Digits3Str
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new DigitCollection(BlockDigitsMask).ToString();
		}
	}
}
