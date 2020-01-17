using System.Collections.Generic;
using Sudoku.Drawing;
using Sudoku.Solving.Utils;

namespace Sudoku.Solving.Manual.Intersections
{
	public sealed class IntersectionTechniqueInfo : TechniqueInfo
	{
		public IntersectionTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views,
			int digit, int baseRegion, int coverRegion)
			: base(conclusions, views) =>
			(Digit, BaseRegion, CoverRegion) = (digit, baseRegion, coverRegion);


		public override string Name => BaseRegion < 9 ? "Pointing" : "Claiming";

		public override decimal Difficulty => BaseRegion < 9 ? 2.6m : 2.8m;

		public override DifficultyLevels DifficultyLevel => DifficultyLevels.Moderate;

		public int Digit { get; }

		public int BaseRegion { get; }

		public int CoverRegion { get; }


		public override string ToString() =>
			$@"{Name}: {Digit + 1} in {RegionUtils.ToString(BaseRegion)}\{RegionUtils.ToString(CoverRegion)} => {ConclusionCollection.ToString(Conclusions)}";
	}
}
