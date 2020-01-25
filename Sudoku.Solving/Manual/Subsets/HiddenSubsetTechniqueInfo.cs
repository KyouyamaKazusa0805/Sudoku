using System;
using System.Collections.Generic;
using Sudoku.Drawing;
using Sudoku.Solving.Utils;

namespace Sudoku.Solving.Manual.Subsets
{
	/// <summary>
	/// Provides a usage of hidden subset technique.
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
					2 => 3.4m,
					3 => 4.0m,
					4 => 5.4m,
					_ => throw new NotSupportedException($"{nameof(Size)} is out of valid range.")
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
