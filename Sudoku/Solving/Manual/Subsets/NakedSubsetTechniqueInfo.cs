using System;
using System.Collections.Generic;
using Sudoku.Drawing;
using Sudoku.Solving.Utils;

namespace Sudoku.Solving.Manual.Subsets
{
	public sealed class NakedSubsetTechniqueInfo : SubsetTechniqueInfo
	{
		public NakedSubsetTechniqueInfo(
			ICollection<Conclusion> conclusions, ICollection<View> views,
			int regionOffset, ICollection<int> cellOffsets, ICollection<int> digits)
			: base(conclusions, views, regionOffset, cellOffsets, digits)
		{
		}


		public int Size => Digits.Count;

		public override string Name => $"Naked {base.Name}";

		public override decimal Difficulty
		{
			get
			{
				return Size switch
				{
					2 => 3.0m,
					3 => 3.6m,
					4 => 5.0m,
					_ => throw new InvalidOperationException($"{nameof(Size)} is out of valid range.")
				};
			}
		}


		public override string ToString() =>
			$"{Name}: {DigitCollection.ToString(Digits)} in {RegionUtils.ToString(RegionOffset)} => {ConclusionCollection.ToString(Conclusions)}";
	}
}
