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
		/// <param name="conclusions">The conclusions.</param>
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
			IReadOnlyList<int>? finCellOffsets, bool? isSashimi)
			: base(conclusions, views, digit, baseSets, coverSets) =>
			(IsSashimi, FinCellOffsets) = (isSashimi, finCellOffsets);


		/// <summary>
		/// Indicates whether the fish instance is sashimi.
		/// The value can be:
		/// <list type="table">
		/// <item>
		/// <term><see langword="true"/></term><description>Sashimi finned fish.</description>
		/// </item>
		/// <item>
		/// <term><see langword="false"/></term><description>Normal finned fish.</description>
		/// </item>
		/// <item>
		/// <term><see langword="null"/></term><description>Normal fish.</description>
		/// </item>
		/// </list>
		/// </summary>
		public bool? IsSashimi { get; }

		/// <summary>
		/// Indicates all fin candidates in this fish information instance.
		/// </summary>
		public IReadOnlyList<int>? FinCellOffsets { get; }

		/// <inheritdoc/>
		public override string Name
		{
			get
			{
				return $@"{IsSashimi switch
				{
					null => "",
					true => "Sashimi ",
					false => "Finned "
				}}{FishUtils.GetNameBy(Size)}";
			}
		}

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
		public override DifficultyLevel DifficultyLevel
		{
			get
			{
				return IsSashimi switch
				{
					null => DifficultyLevel.Hard,
					true => DifficultyLevel.VeryHard,
					false => Size < 3 ? DifficultyLevel.Hard : DifficultyLevel.VeryHard
				};
			}
		}


		/// <inheritdoc/>
		public override string ToString()
		{
			int value = Digit + 1;
			string baseSetStr = RegionCollection.ToString(BaseSets);
			string coverSetStr = RegionCollection.ToString(CoverSets);
			string elimStr = ConclusionCollection.ToString(Conclusions);
			string? finCellStr = FinCellOffsets is null ? null : CellCollection.ToString(FinCellOffsets);
			bool condition = !(FinCellOffsets is null) && FinCellOffsets.Count != 0;
			return $@"{Name}: {value} in {baseSetStr}\{coverSetStr}"
				+ $@"{(condition
					? $" (With {(FinCellOffsets!.Count == 1 ? "a fin cell" : "fin cells")}: {finCellStr})"
					: "")}"
				+ $" => {elimStr}";
		}
	}
}
