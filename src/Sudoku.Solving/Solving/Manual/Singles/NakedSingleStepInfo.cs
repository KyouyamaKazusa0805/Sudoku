using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sudoku.Data;
using Sudoku.Drawing;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.Singles
{
	/// <summary>
	/// Indicates a usage of <b>naked single</b> technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Cell">The cell.</param>
	/// <param name="Digit">The digit.</param>
	public sealed record NakedSingleStepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views, int Cell, int Digit
	) : SingleStepInfo(Conclusions, Views, Cell, Digit)
	{
		/// <inheritdoc/>
		public override decimal Difficulty => 2.3M;

		/// <inheritdoc/>
		public override Technique TechniqueCode => Technique.NakedSingle;

		[FormatItem]
		private string CellStr
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new Cells { Cell }.ToString();
		}

		[FormatItem]
		private string DigitStr
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => (Digit + 1).ToString();
		}


		/// <inheritdoc/>
		public override string ToString() => $"{Name}: {CellStr} = {DigitStr}";
	}
}
