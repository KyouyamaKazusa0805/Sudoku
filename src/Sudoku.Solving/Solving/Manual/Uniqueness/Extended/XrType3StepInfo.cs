using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Techniques;
using static System.Numerics.BitOperations;

namespace Sudoku.Solving.Manual.Uniqueness.Extended
{
	/// <summary>
	/// Provides a usage of <b>extended rectangle</b> (XR) type 3 technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Cells">All cells.</param>
	/// <param name="DigitsMask">All digits mask.</param>
	/// <param name="ExtraCells">All extra cells.</param>
	/// <param name="ExtraDigitsMask">All extra digits mask.</param>
	/// <param name="Region">The region.</param>
	public sealed record XrType3StepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views, in Cells Cells, short DigitsMask,
		IReadOnlyList<int> ExtraCells, short ExtraDigitsMask, int Region
	) : XrStepInfo(Conclusions, Views, Cells, DigitsMask)
	{
		/// <inheritdoc/>
		public override decimal Difficulty => base.Difficulty + .1M * PopCount((uint)ExtraDigitsMask);

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;

		/// <inheritdoc/>
		public override Technique TechniqueCode => Technique.XrType3;

		[FormatItem]
		private string ExtraDigitsStr
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new DigitCollection(ExtraDigitsMask).ToString();
		}

		[FormatItem]
		private string ExtraCellsStr
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new Cells(ExtraCells).ToString();
		}

		[FormatItem]
		private string RegionStr
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new RegionCollection(Region).ToString();
		}
	}
}
