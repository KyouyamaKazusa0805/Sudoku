using System;
using System.Collections.Generic;
using Sudoku.Constants;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using static Sudoku.Solving.Constants.Processings;

namespace Sudoku.Solving.Manual.Fishes
{
	/// <summary>
	/// Provides a usage of <b>normal fish</b> technique.
	/// </summary>
	public sealed class NormalFishTechniqueInfo : FishTechniqueInfo
	{
		/// <include file='SolvingDocComments.xml' path='comments/constructor[@type="TechniqueInfo"]'/>
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
		public override decimal Difficulty =>
			Size switch
			{
				2 => 3.2M,
				3 => 3.8M,
				4 => 5.2M,
				_ => throw Throwings.ImpossibleCase
			} + IsSashimi switch
			{
				null => 0,
				true => Size switch
				{
					2 => .3M,
					3 => .3M,
					4 => .4M,
					_ => throw Throwings.ImpossibleCase
				},
				false => .2M
			};

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;

		/// <inheritdoc/>
		public override TechniqueCode TechniqueCode =>
			InternalName switch
			{
				"X-Wing" => TechniqueCode.XWing,
				"Finned X-Wing" => TechniqueCode.FinnedXWing,
				"Sashimi X-Wing" => TechniqueCode.SashimiXWing,
				"Swordfish" => TechniqueCode.Swordfish,
				"Finned Swordfish" => TechniqueCode.FinnedSwordfish,
				"Sashimi Swordfish" => TechniqueCode.SashimiSwordfish,
				"Jellyfish" => TechniqueCode.Jellyfish,
				"Finned Jellyfish" => TechniqueCode.FinnedJellyfish,
				"Sashimi Jellyfish" => TechniqueCode.SashimiJellyfish,
				_ => throw new NotSupportedException("The current instance does not support this kind of fish.")
			};

		/// <summary>
		/// Indicates the internal name.
		/// </summary>
		private string InternalName =>
			$@"{IsSashimi switch
			{
				null => "",
				true => "Sashimi ",
				false => "Finned "
			}}{FishNames[Size]}";


		/// <inheritdoc/>
		public override string ToString()
		{
			int value = Digit + 1;
			string baseSetStr = new RegionCollection(BaseSets).ToString();
			string coverSetStr = new RegionCollection(CoverSets).ToString();
			string? finStr = FinCellOffsets is not null && FinCellOffsets.Count != 0
				? $" f{new CellCollection(FinCellOffsets).ToString()}"
				: string.Empty;
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			return $@"{Name}: {value} in {baseSetStr}\{coverSetStr}{finStr} => {elimStr}";
		}
	}
}
