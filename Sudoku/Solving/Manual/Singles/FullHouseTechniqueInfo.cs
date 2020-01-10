using System.Collections.Generic;
using Sudoku.Drawing;
using Sudoku.Solving.Utils;

namespace Sudoku.Solving.Manual.Singles
{
	public sealed class FullHouseTechniqueInfo : SingleTechniqueInfo
	{
		public FullHouseTechniqueInfo(
			ICollection<Conclusion> conclusions, ICollection<View> views,
			int cellOffset, int digit)
			: base(conclusions, views) => (CellOffset, Digit) = (cellOffset, digit);


		public override string Name => "Full House";

		public override decimal Difficulty => 1.0m;

		public int CellOffset { get; }

		public int Digit { get; }


		public override string ToString() =>
			$"{Name}: {CellUtils.ToString(CellOffset)} = {Digit + 1}";
	}
}
