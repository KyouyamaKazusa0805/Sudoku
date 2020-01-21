using System.Collections.Generic;
using Sudoku.Drawing;
using Sudoku.Solving.Utils;

namespace Sudoku.Solving.Manual.Singles
{
	/// <summary>
	/// Indicates a usage of full house technique.
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
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views,
			int cellOffset, int digit)
			: base(conclusions, views) => (CellOffset, Digit) = (cellOffset, digit);


		/// <inheritdoc/>
		public override string Name => "Full House";

		/// <inheritdoc/>
		public override decimal Difficulty => 1.0m;

		/// <summary>
		/// Indicates the cell offset.
		/// </summary>
		public int CellOffset { get; }

		/// <summary>
		/// Indicates the digit.
		/// </summary>
		public int Digit { get; }


		/// <inheritdoc/>
		public override string ToString() =>
			$"{Name}: {CellUtils.ToString(CellOffset)} = {Digit + 1}";
	}
}
