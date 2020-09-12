using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;

namespace Sudoku.Solving.Manual.Singles
{
	/// <summary>
	/// Indicates a usage of <b>full house</b> technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Cell">The cell.</param>
	/// <param name="Digit">The digit.</param>
	public sealed record FullHouseTechniqueInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views, int Cell, int Digit)
		: SingleTechniqueInfo(Conclusions, Views, Cell, Digit)
	{
		/// <inheritdoc/>
		public override decimal Difficulty => 1.0M;

		/// <inheritdoc/>
		public override TechniqueCode TechniqueCode => TechniqueCode.FullHouse;


		/// <inheritdoc/>
		public override string ToString()
		{
			string cellStr = new CellCollection(Cell).ToString();
			int value = Digit + 1;
			return $"{Name}: {cellStr} = {value}";
		}
	}
}
