using System.Collections.Generic;
using Sudoku.Drawing;
using Sudoku.Solving.Utils;

namespace Sudoku.Solving.Manual.Subsets
{
	public abstract class SubsetTechniqueInfo : TechniqueInfo
	{
		protected SubsetTechniqueInfo(
			ICollection<Conclusion> conclusions, ICollection<View> views,
			int regionOffset, ICollection<int> cellOffsets, ICollection<int> digits)
			: base(conclusions, views) =>
			(RegionOffset, CellOffsets, Digits) = (regionOffset, cellOffsets, digits);


		public int RegionOffset { get; }

		public ICollection<int> Digits { get; }

		public ICollection<int> CellOffsets { get; }

		public override string Name => SubsetUtils.GetNameBy(Digits.Count);

		public sealed override DifficultyLevels DifficultyLevel => DifficultyLevels.Moderate;
	}
}
