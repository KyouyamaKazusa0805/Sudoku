using System;
using System.Collections.Generic;
using Sudoku.Drawing;
using Sudoku.Solving.Utils;

namespace Sudoku.Solving.Manual.Fishes.Basic
{
	/// <summary>
	/// Provides a usage of basic fish technique.
	/// </summary>
	public sealed class BasicFishTechniqueInfo : FishTechniqueInfo
	{
		/// <inheritdoc/>
		public BasicFishTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views,
			int digit, IReadOnlyList<int> baseSets, IReadOnlyList<int> coverSets)
			: base(conclusions, views, digit, baseSets, coverSets)
		{
		}


		/// <inheritdoc/>
		public override string Name => FishUtils.GetNameBy(Size);

		/// <inheritdoc/>
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
		
		/// <inheritdoc/>
		public override DifficultyLevels DifficultyLevel => DifficultyLevels.Hard;


		/// <inheritdoc/>
		public override string ToString() =>
			$@"{Name}: {Digit + 1} in {RegionCollection.ToString(BaseSets)}\{RegionCollection.ToString(CoverSets)} => {ConclusionCollection.ToString(Conclusions)}";
	}
}
