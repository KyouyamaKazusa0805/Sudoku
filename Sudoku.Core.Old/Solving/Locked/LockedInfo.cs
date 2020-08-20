using System.Collections.Generic;
using Sudoku.Data.Meta;
using Sudoku.Drawing;

namespace Sudoku.Solving.Locked
{
	public sealed class LockedInfo : TechniqueInfo
	{
		public LockedInfo(Conclusion conclusion, ICollection<View> views, int digit, Region baseSet, Region coverSet)
			: base(conclusion, views) => (Digit, BaseSet, CoverSet) = (digit, baseSet, coverSet);


		public override decimal Difficulty => BaseSet.RegionType == RegionType.Block ? 2.6m : 2.8m;

		public int Digit { get; }

		public override string Name => BaseSet.RegionType == RegionType.Block ? "Pointing" : "Claiming";

		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Moderate;

		public Region BaseSet { get; }

		public Region CoverSet { get; }


		public override string ToString() => $@"{Name}: {Digit + 1} in {BaseSet}\{CoverSet} => {Conclusion}";
	}
}
