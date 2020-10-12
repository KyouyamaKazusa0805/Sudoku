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
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Region">The region that structure lies in.</param>
	/// <param name="Cells">All cells used.</param>
	/// <param name="Digits">All digits used.</param>
	/// <param name="IsLocked">Indicates whether the subset is locked.</param>
	public sealed record NakedSubsetTechniqueInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views,
		int Region, IReadOnlyList<int> Cells, IReadOnlyList<int> Digits, bool? IsLocked)
		: SubsetTechniqueInfo(Conclusions, Views, Region, Cells, Digits)
	{
		/// <inheritdoc/>
		public override decimal Difficulty =>
			Size switch { 2 => 3.0M, 3 => 3.6M, 4 => 5.0M, _ => throw Throwings.ImpossibleCase }
			+ IsLocked switch
			{
				null => 0,
				true => Size switch { 2 => -1.0M, 3 => -1.1M, _ => throw Throwings.ImpossibleCase },
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
			string regionStr = new RegionCollection(Region).ToString();
			using var elims = new ConclusionCollection(Conclusions);
			string elimStr = elims.ToString();
			return $"{Name}: {digitsStr} in {regionStr} => {elimStr}";
		}
	}
}
