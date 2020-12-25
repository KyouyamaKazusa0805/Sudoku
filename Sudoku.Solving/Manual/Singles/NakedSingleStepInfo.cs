using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Drawing;

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
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views, int Cell, int Digit)
		: SingleStepInfo(Conclusions, Views, Cell, Digit)
	{
		/// <inheritdoc/>
		public override decimal Difficulty => 2.3M;

		/// <inheritdoc/>
		public override TechniqueCode TechniqueCode => TechniqueCode.NakedSingle;


		/// <inheritdoc/>
		public override string ToString() => $"{Name}: {new Cells { Cell }} = {Digit + 1}";
	}
}
