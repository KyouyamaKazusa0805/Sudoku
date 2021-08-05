using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.Uniqueness.Polygons
{
	/// <summary>
	/// Provides a usage of <b>Borescoper's deadly pattern type 3</b> (BDP) technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Map">The cells used.</param>
	/// <param name="DigitsMask">The digits mask.</param>
	/// <param name="ExtraCells">The extra cells.</param>
	/// <param name="ExtraDigitsMask">The extra digits mask.</param>
	public sealed record BdpType3StepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views,
		in Cells Map, short DigitsMask, in Cells ExtraCells, short ExtraDigitsMask
	) : BdpStepInfo(Conclusions, Views, Map, DigitsMask)
	{
		/// <inheritdoc/>
		public override decimal Difficulty => 5.2M + ExtraCells.Count * .1M;

		/// <inheritdoc/>
		public override Technique TechniqueCode => Technique.BdpType3;

#if SOLUTION_WIDE_CODE_ANALYSIS
		[FormatItem]
#endif
		private string ExtraDigitsStr
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new DigitCollection(ExtraDigitsMask).ToString();
		}

#if SOLUTION_WIDE_CODE_ANALYSIS
		[FormatItem]
#endif
		private string ExtraCellsStr
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => ExtraCells.ToString();
		}


		/// <inheritdoc/>
		public override string ToString() =>
			$"{Name}: {DigitsStr} in cells {CellsStr} with the digits {ExtraDigitsStr} in cells {ExtraCellsStr} => {ElimStr}";
	}
}
