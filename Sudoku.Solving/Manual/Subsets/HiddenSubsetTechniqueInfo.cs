using System.Collections.Generic;
using Sudoku.Drawing;
using Sudoku.Solving.Utils;

namespace Sudoku.Solving.Manual.Subsets
{
	/// <summary>
	/// Provides a usage of <b>hidden subset</b> technique.
	/// </summary>
	public sealed class HiddenSubsetTechniqueInfo : SubsetTechniqueInfo
	{
		/// <inheritdoc/>
		public HiddenSubsetTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views,
			int regionOffset, IReadOnlyList<int> cellOffsets, IReadOnlyList<int> digits)
			: base(conclusions, views, regionOffset, cellOffsets, digits)
		{
		}


		/// <inheritdoc/>
		public override decimal Difficulty
		{
			get
			{
				return Size switch
				{
					2 => 3.4M,
					3 => 4M,
					4 => 5.4M,
					_ => throw Throwing.ImpossibleCase
				};
			}
		}

		/// <summary>
		/// Indicates the size of this instance.
		/// </summary>
		public int Size => Digits.Count;

		/// <inheritdoc/>
		public override string Name => $"Hidden {base.Name}";


		/// <inheritdoc/>
		public override string ToString()
		{
			string digitsStr = DigitCollection.ToString(Digits);
			string regionStr = RegionUtils.ToString(RegionOffset);
			string elimStr = ConclusionCollection.ToString(Conclusions);
			return $"{Name}: {digitsStr} in {regionStr} => {elimStr}";
		}
	}
}
