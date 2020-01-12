using System.Collections.Generic;
using Sudoku.Drawing;

namespace Sudoku.Solving.Manual.Subsets
{
	public abstract class SubsetTechniqueInfo : TechniqueInfo
	{
		protected SubsetTechniqueInfo(
			ICollection<Conclusion> conclusions, ICollection<View> views,
			int regionOffset, ICollection<int> cellOffsets)
			: base(conclusions, views) =>
			(RegionOffset, CellOffsets) = (regionOffset, cellOffsets);


		public int RegionOffset { get; }

		public ICollection<int> CellOffsets { get; }

		public override DifficultyLevels DifficultyLevel => DifficultyLevels.Moderate;
	}
}
