using System;
using System.Collections.Generic;
using Sudoku.Drawing;
using Sudoku.Solving.Utils;

namespace Sudoku.Solving.Manual.Fishes.Basic
{
	/// <summary>
	/// Provides a usage of normal fish technique.
	/// </summary>
	public sealed class NormalFishTechniqueInfo : FishTechniqueInfo
	{
		/// <summary>
		/// Initializes an instance with information.
		/// </summary>
		/// <param name="conclusions">The concluisons.</param>
		/// <param name="views">The views.</param>
		/// <param name="digit">The digit.</param>
		/// <param name="baseSets">The base sets.</param>
		/// <param name="coverSets">The cover sets.</param>
		/// <param name="finCellOffsets">All candidate offsets of fins' position.</param>
		/// <param name="isSashimi">
		/// Indicates whether the fish instance is sashimi.
		/// </param>
		public NormalFishTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views,
			int digit, IReadOnlyList<int> baseSets, IReadOnlyList<int> coverSets,
			IReadOnlyList<int> finCellOffsets, bool? isSashimi)
			: base(conclusions, views, digit, baseSets, coverSets) =>
			(IsSashimi, FinCandidateOffsets) = (isSashimi, finCellOffsets);


		/// <summary>
		/// Indicates whether the fish instance is sashimi.
		/// The value can be:
		/// <list type="table">
		/// <item>
		/// <term><c>true</c></term><description>Sashimi finned fish.</description>
		/// </item>
		/// <item>
		/// <term><c>false</c></term><description>Normal finned fish.</description>
		/// </item>
		/// <item>
		/// <term><c>null</c></term><description>Normal fish.</description>
		/// </item>
		/// </list>
		/// </summary>
		public bool? IsSashimi { get; }

		/// <summary>
		/// Indicates all fin candidates in this fish information instance.
		/// </summary>
		public IReadOnlyList<int> FinCandidateOffsets { get; }

		/// <inheritdoc/>
		public override string Name =>
			$"{(FinCandidateOffsets.Count != 0 ? "Finned " : string.Empty)}{FishUtils.GetNameBy(Size)}";

		/// <inheritdoc/>
		public override decimal Difficulty
		{
			get
			{
				var ex = new NotSupportedException($"{nameof(Size)} is out of valid range.");
				return Size switch
				{
					2 => 3.2m,
					3 => 3.8m,
					4 => 5.2m,
					_ => throw ex
				} + IsSashimi switch
				{
					null => 0,
					true => Size switch
					{
						2 => 0.3m,
						3 => 0.3m,
						4 => 0.4m,
						_ => throw ex
					},
					false => 0.2m
				};
			}
		}

		/// <inheritdoc/>
		public override DifficultyLevels DifficultyLevel
		{
			get
			{
				return IsSashimi switch
				{
					null => DifficultyLevels.Hard,
					true => DifficultyLevels.VeryHard,
					false => Size < 3 ? DifficultyLevels.Hard : DifficultyLevels.VeryHard
				};
			}
		}


		/// <inheritdoc/>
		public override string ToString()
		{
			return $"{Name}: {Digit + 1} in {RegionCollection.ToString(BaseSets)}"
				+ $@"\{RegionCollection.ToString(CoverSets)}"
				+ $@"{(FinCandidateOffsets.Count == 0
					? string.Empty
					: $"(With fin(s): {CandidateCollection.ToString(FinCandidateOffsets)})")}"
				+ $" => {ConclusionCollection.ToString(Conclusions)}";
		}
	}
}
