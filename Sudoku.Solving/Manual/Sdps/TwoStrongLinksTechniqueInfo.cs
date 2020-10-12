using System;
using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;

namespace Sudoku.Solving.Manual.Sdps
{
	/// <summary>
	/// Provides a usage of two strong links technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Digit">The digit used.</param>
	/// <param name="BaseRegion">The base region.</param>
	/// <param name="TargetRegion">The target region.</param>
	public sealed record TwoStrongLinksTechniqueInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views, int Digit, int BaseRegion, int TargetRegion)
		: SdpTechniqueInfo(Conclusions, Views, Digit)
	{
		/// <inheritdoc/>
		public override decimal Difficulty =>
			TechniqueCode switch
			{
				TechniqueCode.TurbotFish => 4.2M,
				TechniqueCode.Skyscraper => 4.0M,
				TechniqueCode.TwoStringKite => 4.1M,
				_ => throw new NotSupportedException("The specified value is invalid.")
			};

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;

		/// <inheritdoc/>
		public override TechniqueCode TechniqueCode =>
			(BaseRegion / 9, TargetRegion / 9) switch
			{
				(0, _) or (_, 0) => TechniqueCode.TurbotFish,
				(1, 1) or (2, 2) => TechniqueCode.Skyscraper,
				(1, 2) or (2, 1) => TechniqueCode.TwoStringKite,
				_ => throw new NotSupportedException("The specified value is invalid.")
			};


		/// <inheritdoc/>
		public override string ToString()
		{
			int digit = Digit + 1;
			string baseRegionStr = new RegionCollection(BaseRegion).ToString();
			string targetRegionStr = new RegionCollection(TargetRegion).ToString();
			using var elims = new ConclusionCollection(Conclusions);
			string elimStr = elims.ToString();
			return $@"{Name}: {digit} in {baseRegionStr}\{targetRegionStr} => {elimStr}";
		}
	}
}
