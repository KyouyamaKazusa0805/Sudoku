using System.Collections.Generic;
using Sudoku.Data.Meta;
using Sudoku.Drawing;

namespace Sudoku.Solving.Subsets
{
	public sealed class NakedSubsetPlusInfo : NakedSubsetInfo
	{
		public NakedSubsetPlusInfo(
			Conclusion conclusion, ICollection<View> views, Region region, IEnumerable<int> digits, int size)
			: base(conclusion, views, region, digits, size)
		{
		}


		public override decimal Difficulty => base.Difficulty + new[] { 0, 0, 0.1m, 0.1m, 0.2m }[Size];

		public override string Name => $"{base.Name} (+)";
	}
}
