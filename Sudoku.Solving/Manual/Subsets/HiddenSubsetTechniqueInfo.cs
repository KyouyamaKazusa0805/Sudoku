using System.Collections.Generic;
using Sudoku.Constants;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;

namespace Sudoku.Solving.Manual.Subsets
{
	/// <summary>
	/// Provides a usage of <b>hidden subset</b> technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Region">The region that structure lies in.</param>
	/// <param name="Cells">All cells used.</param>
	/// <param name="Digits">All digits used.</param>
	public sealed record HiddenSubsetTechniqueInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views,
		int Region, IReadOnlyList<int> Cells, IReadOnlyList<int> Digits)
		: SubsetTechniqueInfo(Conclusions, Views, Region, Cells, Digits)
	{
		/// <inheritdoc/>
		public override decimal Difficulty =>
			Size switch { 2 => 3.4M, 3 => 4.0M, 4 => 5.4M, _ => throw Throwings.ImpossibleCase };

		/// <inheritdoc/>
		public override TechniqueCode TechniqueCode =>
			Size switch
			{
				2 => TechniqueCode.HiddenPair,
				3 => TechniqueCode.HiddenTriple,
				4 => TechniqueCode.HiddenQuadruple,
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
