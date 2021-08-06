using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Resources;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.Uniqueness.Rects
{
	/// <summary>
	/// Provides a usage of <b>unique rectangle + unknown covering</b> technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Digit1">The digit 1.</param>
	/// <param name="Digit2">The digit 2.</param>
	/// <param name="TargetCell">The target cell.</param>
	/// <param name="ExtraDigit">The extra digit.</param>
	/// <param name="Cells">All UR cells.</param>
	/// <param name="AbsoluteOffset">The absolute offset.</param>
	public sealed record UrWithUnknownCoveringStepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views,
		int Digit1, int Digit2, int TargetCell, int ExtraDigit, int[] Cells, int AbsoluteOffset
	) : UrStepInfo(Conclusions, Views, Technique.UrUnknownCovering, Digit1, Digit2, Cells, false, AbsoluteOffset)
	{
		/// <inheritdoc/>
		public override decimal Difficulty => 4.9M;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;

		/// <inheritdoc/>
		public override TechniqueGroup TechniqueGroup => TechniqueGroup.UrPlus;

		/// <inheritdoc/>
		[FormatItem]
		protected override string AdditionalFormat =>
			TextResources.Current.Format_UrWithUnknownCoveringStepInfo_Additional;

		[FormatItem]
		private string TargetCellStr
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new Cells { TargetCell }.ToString();
		}

		[FormatItem]
		private string DigitsStr
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new DigitCollection(
				(short)(1 << Digit1 | 1 << Digit2)
			).ToString((string)TextResources.Current.OrKeyword);
		}

		[FormatItem]
		private string ExtraDigitStr
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => (ExtraDigit + 1).ToString();
		}


		/// <inheritdoc/>
		public override string ToString() => base.ToString();

		/// <inheritdoc/>
		protected override string GetAdditional() =>
			$"unknown covering: Suppose {TargetCellStr} is filled with the unknown digit X (X is {DigitsStr}), then 4 cells form a UR deadly pattern of digit X and {ExtraDigitStr}";
	}
}
