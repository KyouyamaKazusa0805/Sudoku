using System;
using System.Collections.Generic;
using Sudoku.Drawing;
using Sudoku.Solving.Utils;

namespace Sudoku.Solving.Manual.Subsets
{
	public sealed class NakedSubsetTechniqueInfo : SubsetTechniqueInfo
	{
		public NakedSubsetTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views,
			int regionOffset, IReadOnlyList<int> cellOffsets, IReadOnlyList<int> digits,
			bool? isLocked)
			: base(conclusions, views, regionOffset, cellOffsets, digits) =>
			IsLocked = isLocked;


		/// <summary>
		/// Represents a value for this technique is a locked,
		/// partial locked or normal subset.
		/// The technique is one when the value is:
		/// <list type="table">
		/// <item><term><c>true</c></term><description>Locked subset,</description></item>
		/// <item><term><c>false</c></term><description>Partial locked subset,</description></item>
		/// <item><term><c>null</c></term><description>Normal subset.</description></item>
		/// </list>
		/// </summary>
		public bool? IsLocked { get; }

		public int Size => Digits.Count;

		public override string Name
		{
			get
			{
				return IsLocked switch
				{
					null => $"Naked {base.Name}",
					true => $"Locked {base.Name}",
					false => $"Naked {base.Name} (+)"
				};
			}
		}

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
				} + IsLocked switch
				{
					null => 0,
					true => Size switch
					{
						2 => -1.0m,
						3 => -1.1m,
						_ => throw new Exception("No valid 'Locked Subset'(size > 4) in 9-block sudoku.")
					},
					false => 0.1m
				};
			}
		}


		public override string ToString() =>
			$"{Name}: {DigitCollection.ToString(Digits)} in {RegionUtils.ToString(RegionOffset)} => {ConclusionCollection.ToString(Conclusions)}";
	}
}
