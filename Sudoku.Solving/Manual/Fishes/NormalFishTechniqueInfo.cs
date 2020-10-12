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
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Digit">The digit used.</param>
	/// <param name="BaseSets">The base sets.</param>
	/// <param name="CoverSets">The cover sets.</param>
	/// <param name="FinCells">All fin cells.</param>
	/// <param name="IsSashimi">
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
	/// </param>
	public sealed record NormalFishTechniqueInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views, int Digit, IReadOnlyList<int> BaseSets,
		IReadOnlyList<int> CoverSets, IReadOnlyList<int>? Fins, bool? IsSashimi)
		: FishTechniqueInfo(Conclusions, Views, Digit, BaseSets, CoverSets)
	{
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
				_ => throw new NotSupportedException("The current instance doesn't support this kind of fish.")
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
			string? finStr = Fins is { Count: not 0 } ? $" f{new GridMap(Fins)}" : string.Empty;
			using var elims = new ConclusionCollection(Conclusions);
			string elimStr = elims.ToString();
			return $@"{Name}: {value} in {baseSetStr}\{coverSetStr}{finStr} => {elimStr}";
		}
	}
}
