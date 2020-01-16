using System;
using System.Collections.Generic;
using Sudoku.Drawing;
using Sudoku.Solving.Utils;

namespace Sudoku.Solving.Manual.Fishes.Basic
{
	public sealed class BasicFishTechniqueInfo : FishTechniqueInfo
	{
		public BasicFishTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views,
			int digit, IReadOnlyList<int> baseSets, IReadOnlyList<int> coverSets)
			: base(conclusions, views, digit, baseSets, coverSets)
		{
		}


		public override string Name => FishUtils.GetNameBy(Size);

		public override decimal Difficulty
		{
			get
			{
				return Size switch
				{
					2 => 3.2m,
					3 => 3.8m,
					4 => 5.2m,
					_ => throw new InvalidOperationException("Higher sized fish will be replaced by lower one.")
				};
			}
		}

		public override DifficultyLevels DifficultyLevel => DifficultyLevels.Hard;


		public override string ToString() =>
			$@"{Name}: {Digit + 1} in {RegionCollection.ToString(BaseSets)}\{RegionCollection.ToString(CoverSets)} => {ConclusionCollection.ToString(Conclusions)}";
	}
}
