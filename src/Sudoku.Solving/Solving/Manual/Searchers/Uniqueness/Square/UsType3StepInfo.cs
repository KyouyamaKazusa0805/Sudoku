using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Resources;
using Sudoku.Solving.Text;
using Sudoku.Techniques;
using static System.Numerics.BitOperations;

namespace Sudoku.Solving.Manual.Uniqueness.Square
{
	/// <summary>
	/// Provides a usage of <b>unique square type 3</b> (US) technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Cells">The cells.</param>
	/// <param name="DigitsMask">The digits mask.</param>
	/// <param name="ExtraDigitsMask">The extra digits mask.</param>
	/// <param name="ExtraCells">The extra cells.</param>
	public sealed record UsType3StepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views, in Cells Cells, short DigitsMask,
		short ExtraDigitsMask, IReadOnlyList<int> ExtraCells) : UsStepInfo(Conclusions, Views, Cells, DigitsMask)
	{
		/// <inheritdoc/>
		public override decimal Difficulty => base.Difficulty + ExtraDifficulty;

		/// <inheritdoc/>
		public override Technique TechniqueCode => Technique.UsType3;

		/// <summary>
		/// Indicates the extra difficulty.
		/// </summary>
		private decimal ExtraDifficulty => PopCount((uint)ExtraDigitsMask) * .1M;

		[FormatItem]
		private string ExtraCellsStr
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new Cells(ExtraCells).ToString();
		}

		[FormatItem]
		private string ExtraDigitStr
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new DigitCollection(ExtraDigitsMask).ToString();
		}

		[FormatItem]
		private string SubsetName
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => TextResources.Current[$"SubsetNames{(ExtraCells.Count + 1).ToString()}"];
		}
	}
}
