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
	public sealed class TwoStrongLinksTechniqueInfo : SdpTechniqueInfo
	{
		/// <include file='SolvingDocComments.xml' path='comments/constructor[@type="TechniqueInfo"]'/>
		/// <param name="digit">The digit.</param>
		/// <param name="baseRegion">The base region.</param>
		/// <param name="targetRegion">The target region.</param>
		public TwoStrongLinksTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views,
			int digit, int baseRegion, int targetRegion) : base(conclusions, views, digit) =>
			(Digit, BaseRegion, TargetRegion) = (digit, baseRegion, targetRegion);


		/// <summary>
		/// Indicates the base region.
		/// </summary>
		public int BaseRegion { get; }

		/// <summary>
		/// Indicates the target region.
		/// </summary>
		public int TargetRegion { get; }

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
				(0, _) => TechniqueCode.TurbotFish,
				(_, 0) => TechniqueCode.TurbotFish,
				(1, 1) => TechniqueCode.Skyscraper,
				(2, 2) => TechniqueCode.Skyscraper,
				(1, 2) => TechniqueCode.TwoStringKite,
				(2, 1) => TechniqueCode.TwoStringKite,
				_ => throw new NotSupportedException("The specified value is invalid.")
			};


		/// <inheritdoc/>
		public override string ToString()
		{
			int digit = Digit + 1;
			string baseRegionStr = new RegionCollection(BaseRegion).ToString();
			string targetRegionStr = new RegionCollection(TargetRegion).ToString();
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			return $@"{Name}: {digit} in {baseRegionStr}\{targetRegionStr} => {elimStr}";
		}
	}
}
