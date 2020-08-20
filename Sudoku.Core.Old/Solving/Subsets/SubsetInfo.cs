using System.Collections.Generic;
using Sudoku.Data.Meta;
using Sudoku.Drawing;

namespace Sudoku.Solving.Subsets
{
	public abstract class SubsetInfo : TechniqueInfo
	{
		protected SubsetInfo(
			Conclusion conclusion, ICollection<View> views, Region region, IEnumerable<int> digits, int size)
			: base(conclusion, views) => (Region, Digits, Size) = (region, digits, size);


		public int Size { get; }

		public Region Region { get; }

		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Moderate;

		public IEnumerable<int> Digits { get; }
	}
}
