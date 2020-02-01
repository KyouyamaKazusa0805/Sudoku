using System;
using System.Collections.Generic;
using Sudoku.Drawing;
using Sudoku.Solving.Utils;

namespace Sudoku.Solving.Manual.Chaining
{
	/// <summary>
	/// Provides a usage of two strong link technique.
	/// </summary>
	public sealed class TwoStrongLinkTechniqueInfo : TechniqueInfo
	{
		/// <summary>
		/// Initializes an instance with the specified information.
		/// </summary>
		/// <param name="conclusions">All conclusions.</param>
		/// <param name="views">All views.</param>
		/// <param name="digit">The digit.</param>
		/// <param name="baseRegion">The base region.</param>
		/// <param name="targetRegion">The target region.</param>
		public TwoStrongLinkTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views,
			int digit, int baseRegion, int targetRegion)
			: base(conclusions, views) =>
			(Digit, BaseRegion, TargetRegion) = (digit, baseRegion, targetRegion);


		/// <summary>
		/// Indicates the digit.
		/// </summary>
		public int Digit { get; }

		/// <summary>
		/// Indicates the base region.
		/// </summary>
		public int BaseRegion { get; }

		/// <summary>
		/// Indicates the target region.
		/// </summary>
		public int TargetRegion { get; }

		/// <inheritdoc/>
		public override string Name
		{
			get
			{
				return (BaseRegion / 9, TargetRegion / 9) switch
				{
					(0, _) => "Turbot Fish",
					(_, 0) => "Turbot Fish",
					(1, 1) => "Skyscraper",
					(2, 2) => "Skyscraper",
					(1, 2) => "Two-string Kite",
					(2, 1) => "Two-string Kite",
					_ => throw new NotSupportedException("The specified value is invalid.")
				};
			}
		}

		/// <inheritdoc/>
		public override decimal Difficulty
		{
			get
			{
				return Name switch
				{
					"Turbot Fish" => 4.2m,
					"Skyscraper" => 4.0m,
					"Two-string Kite" => 4.1m,
					_ => throw new NotSupportedException("The specified value is invalid.")
				};
			}
		}

		/// <inheritdoc/>
		public override DifficultyLevels DifficultyLevel => DifficultyLevels.VeryHard;


		/// <inheritdoc/>
		public override string ToString()
		{
			int digit = Digit + 1;
			string baseRegionStr = RegionUtils.ToString(BaseRegion);
			string targetRegionStr = RegionUtils.ToString(TargetRegion);
			string elimStr = ConclusionCollection.ToString(Conclusions);
			return $@"{Name}: {digit} in {baseRegionStr}\{targetRegionStr} => {elimStr}";
		}
	}
}
