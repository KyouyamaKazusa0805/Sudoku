using System.Collections.Generic;
using Sudoku.Drawing;
using Sudoku.Solving.Utils;

namespace Sudoku.Solving.Manual.Exocets
{
	/// <summary>
	/// Provides a usage of <b>senior exocet</b> (SE) technique.
	/// </summary>
	public sealed class SeniorExocetTechniqueInfo : ExocetTechniqueInfo
	{
		/// <summary>
		/// Initializes an instance with the specified information.
		/// </summary>
		/// <param name="conclusions">All conclusions.</param>
		/// <param name="views">All views.</param>
		/// <param name="exocet">The exocet.</param>
		/// <param name="digits">All digits.</param>
		/// <param name="endoTargetCell">The endo target cell.</param>
		public SeniorExocetTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views, Exocet exocet,
			IEnumerable<int> digits, int endoTargetCell)
			: base(
				  conclusions, views, exocet, digits, ExocetTypeCode.Senior, null, null,
				  default, default, default, default, default) =>
			EndoTargetCell = endoTargetCell;


		/// <summary>
		/// Indicates the target cell that lies on the cross-line.
		/// </summary>
		public int EndoTargetCell { get; }

		/// <inheritdoc/>
		public override decimal Difficulty => 9.6M;


		/// <inheritdoc/>
		protected override string? GetAdditional() => $"Endo-target: {CellUtils.ToString(EndoTargetCell)}";
	}
}
