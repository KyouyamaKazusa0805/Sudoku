using System.Collections.Generic;
using Sudoku.Drawing;

namespace Sudoku.Solving.Manual.Fishes
{
	public abstract class FishTechniqueInfo : TechniqueInfo
	{
		protected FishTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views,
			int digit, IReadOnlyList<int> baseSets, IReadOnlyList<int> coverSets)
			: base(conclusions, views) =>
			(Digit, BaseSets, CoverSets) = (digit, baseSets, coverSets);


		public int Digit { get; }

		public int Size => BaseSets.Count;

		public int Rank => CoverSets.Count - BaseSets.Count;

		public IReadOnlyList<int> BaseSets { get; }

		public IReadOnlyList<int> CoverSets { get; }
	}
}
