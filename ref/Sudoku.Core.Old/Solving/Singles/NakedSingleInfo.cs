using System.Collections.Generic;
using Sudoku.Drawing;

namespace Sudoku.Solving.Singles
{
	public sealed class NakedSingleInfo : SingleInfo
	{
		public NakedSingleInfo(Conclusion conclusion, ICollection<View> views)
			: base(conclusion, views)
		{
		}


		public override decimal Difficulty => 2.3m;

		public override string Name => "Naked Single";


		public override string ToString() => $"{Name}: {Conclusion}";
	}
}
