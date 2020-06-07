using System.Collections.Generic;
using Sudoku.Constants;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;

namespace Sudoku.Solving.Manual.Subsets
{
	/// <summary>
	/// Provides a usage of <b>naked subset</b> technique.
	/// </summary>
	public sealed class NakedSubsetTechniqueInfo : SubsetTechniqueInfo
	{
		/// <include file='SolvingDocComments.xml' path='comments/constructor[@type="TechniqueInfo"]'/>
		/// <param name="regionOffset">The region offset.</param>
		/// <param name="cellOffsets">The cell offsets.</param>
		/// <param name="digits">The digits.</param>
		/// <param name="isLocked">Indicates whether the technique is locked. </param>
		public NakedSubsetTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views,
			int regionOffset, IReadOnlyList<int> cellOffsets, IReadOnlyList<int> digits,
			bool? isLocked) : base(conclusions, views, regionOffset, cellOffsets, digits) =>
			IsLocked = isLocked;


		/// <summary>
		/// Represents a value for this technique is a locked,
		/// partial locked or normal subset.
		/// The technique is one when the value is:
		/// <list type="table">
		/// <item><term><see langword="true"/></term><description>Locked subset,</description></item>
		/// <item><term><see langword="false"/></term><description>Partial locked subset,</description></item>
		/// <item><term><see langword="null"/></term><description>Normal subset.</description></item>
		/// </list>
		/// </summary>
		public bool? IsLocked { get; }

		/// <summary>
		/// Indicates the size.
		/// </summary>
		public int Size => Digits.Count;

		/// <inheritdoc/>
		public override decimal Difficulty =>
			Size switch
			{
				2 => 3M,
				3 => 3.6M,
				4 => 5M,
				_ => throw Throwings.ImpossibleCase
			} + IsLocked switch
			{
				null => 0,
				true => Size switch
				{
					2 => -1M,
					3 => -1.1M,
					_ => throw Throwings.ImpossibleCase
				},
				false => .1M
			};

		/// <inheritdoc/>
		public override TechniqueCode TechniqueCode =>
			(IsLocked, Digits.Count) switch
			{
				(true, 2) => TechniqueCode.LockedPair,
				(false, 2) => TechniqueCode.NakedPairPlus,
				(null, 2) => TechniqueCode.NakedPair,
				(true, 3) => TechniqueCode.LockedTriple,
				(false, 3) => TechniqueCode.NakedTriplePlus,
				(null, 3) => TechniqueCode.NakedTriple,
				(false, 4) => TechniqueCode.NakedQuadruplePlus,
				(null, 4) => TechniqueCode.NakedQuadruple,
				_ => throw Throwings.ImpossibleCase
			};


		/// <inheritdoc/>
		public override string ToString()
		{
			string digitsStr = new DigitCollection(Digits).ToString();
			string regionStr = new RegionCollection(RegionOffset).ToString();
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			return $"{Name}: {digitsStr} in {regionStr} => {elimStr}";
		}
	}
}
