using System.Collections.Generic;
using Sudoku.Drawing;
using Sudoku.Solving.Utils;

namespace Sudoku.Solving.Manual.Singles
{
	public sealed class NakedSingleTechniqueInfo : SingleTechniqueInfo
	{
		public NakedSingleTechniqueInfo(
			ICollection<Conclusion> conclusions, ICollection<View> views,
			int cellOffset, int digit)
			: base(conclusions, views) => (CellOffset, Digit) = (cellOffset, digit);


		public override string Name => "Naked single";

		public override decimal Difficulty => 2.3m;

		public int CellOffset { get; }

		public int Digit { get; }


		public override string ToString() =>
			$"{Name}: {CellUtils.ToString(CellOffset)} = {Digit + 1}";
	}
}
