using System.Collections.Generic;
using Sudoku.Drawing;
using Sudoku.Solving.Utils;

namespace Sudoku.Solving.Manual.Subsets
{
	public abstract class SubsetTechniqueInfo : TechniqueInfo
	{
		protected SubsetTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views,
			int regionOffset, IReadOnlyList<int> cellOffsets, IReadOnlyList<int> digits)
			: base(conclusions, views) =>
			(RegionOffset, CellOffsets, Digits) = (regionOffset, cellOffsets, digits);


		public int RegionOffset { get; }

		public IReadOnlyList<int> Digits { get; }

		public IReadOnlyList<int> CellOffsets { get; }

		public override string Name => SubsetUtils.GetNameBy(Digits.Count);

		public sealed override DifficultyLevels DifficultyLevel => DifficultyLevels.Moderate;
	}
}
