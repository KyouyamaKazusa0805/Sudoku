using System.Collections.Generic;
using Sudoku.Data.Collections;
using Sudoku.Drawing;

namespace Sudoku.Solving.Manual.Singles
{
	/// <summary>
	/// Indicates a usage of <b>full house</b> technique.
	/// </summary>
	public sealed class FullHouseTechniqueInfo : SingleTechniqueInfo
	{
		/// <summary>
		/// Initializes an instance with information.
		/// </summary>
		/// <param name="conclusions">The conclusion.</param>
		/// <param name="views">The views.</param>
		/// <param name="cellOffset">The cell offset.</param>
		/// <param name="digit">The digit.</param>
		public FullHouseTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views, int cellOffset, int digit)
			: base(conclusions, views, cellOffset, digit)
		{
		}


		/// <inheritdoc/>
		public override string Name => "Full House";

		/// <inheritdoc/>
		public override decimal Difficulty => 1M;


		/// <inheritdoc/>
		public override string ToString()
		{
			string cellStr = new CellCollection(stackalloc[] { Cell }).ToString();
			int value = Digit + 1;
			return $"{Name}: {cellStr} = {value}";
		}
	}
}
