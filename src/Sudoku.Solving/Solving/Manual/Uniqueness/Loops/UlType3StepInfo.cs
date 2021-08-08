using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Resources;

namespace Sudoku.Solving.Manual.Uniqueness.Loops
{
	/// <summary>
	/// Provides a usage of <b>unique loop type 3</b> technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Digit1">The digit 1.</param>
	/// <param name="Digit2">The digit 2.</param>
	/// <param name="Loop">The loop.</param>
	/// <param name="SubsetDigitsMask">The subset digits mask.</param>
	/// <param name="SubsetCells">The subset cells.</param>
	public sealed record UlType3StepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views, int Digit1, int Digit2,
		in Cells Loop, short SubsetDigitsMask, IReadOnlyList<int> SubsetCells
	) : UlStepInfo(Conclusions, Views, Digit1, Digit2, Loop)
	{
		/// <inheritdoc/>
		public override int Type => 3;

		[FormatItem]
		private string SubsetCellsStr
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new Cells(SubsetCells).ToString();
		}

		[FormatItem]
		private string DigitsStr
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new DigitCollection(SubsetDigitsMask).ToString();
		}

		[FormatItem]
		private string SubsetName
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => TextResources.Current[$"SubsetNames{(SubsetCells.Count + 1).ToString()}"];
		}


		/// <inheritdoc/>
		public bool Equals(UlType3StepInfo? other) =>
			base.Equals(other) && SubsetDigitsMask == other.SubsetDigitsMask;

		/// <inheritdoc/>
		public override int GetHashCode() => base.GetHashCode();
	}
}
