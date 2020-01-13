using System;
using System.Collections.Generic;
using Sudoku.Drawing;
using Sudoku.Solving.Utils;

namespace Sudoku.Solving.Manual.Subsets
{
	public sealed class HiddenSubsetTechniqueInfo : SubsetTechniqueInfo
	{
		public HiddenSubsetTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views,
			int regionOffset, IReadOnlyList<int> cellOffsets, IReadOnlyList<int> digits)
			: base(conclusions, views, regionOffset, cellOffsets, digits)
		{
		}


		public override decimal Difficulty
		{
			get
			{
				return Size switch
				{
					2 => 3.4m,
					3 => 4.0m,
					4 => 5.4m,
					_ => throw new InvalidOperationException($"{nameof(Size)} is out of valid range.")
				};
			}
		}
		
		public int Size => Digits.Count;

		public override string Name => $"Hidden {base.Name}";


		public override string ToString() =>
			$"{Name}: {DigitCollection.ToString(Digits)} in {RegionUtils.ToString(RegionOffset)} => {ConclusionCollection.ToString(Conclusions)}";
	}
}
