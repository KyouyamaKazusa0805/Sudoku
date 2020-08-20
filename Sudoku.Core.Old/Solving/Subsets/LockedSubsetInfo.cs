using System.Collections.Generic;
using Sudoku.Data.Literals;
using Sudoku.Data.Meta;
using Sudoku.Drawing;

namespace Sudoku.Solving.Subsets
{
	public sealed class LockedSubsetInfo : NakedSubsetInfo
	{
		public LockedSubsetInfo(
			Conclusion conclusion, ICollection<View> views, Region region, IEnumerable<int> digits, int size)
			: base(conclusion, views, region, digits, size)
		{
		}


		public override decimal Difficulty => Size switch { 2 => 2m, _ => 2.5m, };

		public override string Name => $"Locked {Values.SubsetNames[Size]}";
	}
}
