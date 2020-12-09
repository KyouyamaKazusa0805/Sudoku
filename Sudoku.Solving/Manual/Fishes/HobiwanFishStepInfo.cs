using System;
using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using static Sudoku.Solving.Constants.Processings;

namespace Sudoku.Solving.Manual.Fishes
{
	/// <summary>
	/// Provides a usage of <b>Hobiwan's fish</b> technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Digit">The digit used.</param>
	/// <param name="BaseSets">The base sets.</param>
	/// <param name="CoverSets">The cover sets.</param>
	/// <param name="Exofins">All exo-fins.</param>
	/// <param name="Endofins">All endo-fins.</param>
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
	public sealed record HobiwanFishStepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views, int Digit, IReadOnlyList<int> BaseSets,
		IReadOnlyList<int> CoverSets, in GridMap Exofins, in GridMap Endofins, bool? IsSashimi)
		: FishStepInfo(Conclusions, Views, Digit, BaseSets, CoverSets)
	{
		/// <summary>
		/// The basic difficulty rating table.
		/// </summary>
		private static readonly decimal[] BasicDiff = { 0, 0, 3.2M, 3.8M, 5.2M, 6.0M, 6.0M, 6.6M, 7.0M };

		/// <summary>
		/// The finned difficulty rating table.
		/// </summary>
		private static readonly decimal[] FinnedDiff = { 0, 0, .2M, .2M, .2M, .3M, .3M, .3M, .4M };

		/// <summary>
		/// The sashimi difficulty rating table.
		/// </summary>
		private static readonly decimal[] SashimiDiff = { 0, 0, .3M, .3M, .4M, .4M, .5M, .6M, .7M };

		/// <summary>
		/// The Franken shape extra difficulty rating table.
		/// </summary>
		private static readonly decimal[] FrankenShapeDiffExtra = { 0, 0, .2M, 1.2M, 1.2M, 1.3M, 1.3M, 1.3M, 1.4M };

		/// <summary>
		/// The mutant shape extra difficulty rating table.
		/// </summary>
		private static readonly decimal[] MutantShapeDiffExtra = { 0, 0, .3M, 1.4M, 1.4M, 1.5M, 1.5M, 1.5M, 1.6M };


		/// <inheritdoc/>
		public override decimal Difficulty =>
			BaseDifficulty + SashimiExtraDifficulty + ShapeExtraDifficulty;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel =>
			Size is 2 or 3 or 4 ? DifficultyLevel.Fiendish : DifficultyLevel.Nightmare;

		/// <inheritdoc/>
		public override TechniqueCode TechniqueCode =>
			InternalName switch
			{
				"X-Wing" => TechniqueCode.XWing,
				"Finned X-Wing" => TechniqueCode.FinnedXWing,
				"Sashimi X-Wing" => TechniqueCode.SashimiXWing,
				"Franken X-Wing" => TechniqueCode.FrankenXWing,
				"Finned Franken X-Wing" => TechniqueCode.FinnedFrankenXWing,
				"Sashimi Franken X-Wing" => TechniqueCode.SashimiFrankenXWing,
				"Mutant X-Wing" => TechniqueCode.MutantXWing,
				"Finned Mutant X-Wing" => TechniqueCode.FinnedMutantXWing,
				"Sashimi Mutant X-Wing" => TechniqueCode.SashimiMutantXWing,
				"Swordfish" => TechniqueCode.Swordfish,
				"Finned Swordfish" => TechniqueCode.FinnedSwordfish,
				"Sashimi Swordfish" => TechniqueCode.SashimiSwordfish,
				"Franken Swordfish" => TechniqueCode.FrankenSwordfish,
				"Finned Franken Swordfish" => TechniqueCode.FinnedFrankenSwordfish,
				"Sashimi Franken Swordfish" => TechniqueCode.SashimiFrankenSwordfish,
				"Mutant Swordfish" => TechniqueCode.MutantSwordfish,
				"Finned Mutant Swordfish" => TechniqueCode.FinnedMutantSwordfish,
				"Sashimi Mutant Swordfish" => TechniqueCode.SashimiMutantSwordfish,
				"Jellyfish" => TechniqueCode.Jellyfish,
				"Finned Jellyfish" => TechniqueCode.FinnedJellyfish,
				"Sashimi Jellyfish" => TechniqueCode.SashimiJellyfish,
				"Franken Jellyfish" => TechniqueCode.FrankenJellyfish,
				"Finned Franken Jellyfish" => TechniqueCode.FinnedFrankenJellyfish,
				"Sashimi Franken Jellyfish" => TechniqueCode.SashimiFrankenJellyfish,
				"Mutant Jellyfish" => TechniqueCode.MutantJellyfish,
				"Finned Mutant Jellyfish" => TechniqueCode.FinnedMutantJellyfish,
				"Sashimi Mutant Jellyfish" => TechniqueCode.SashimiMutantJellyfish,
				"Squirmbag" => TechniqueCode.Squirmbag,
				"Finned Squirmbag" => TechniqueCode.FinnedSquirmbag,
				"Sashimi Squirmbag" => TechniqueCode.SashimiSquirmbag,
				"Franken Squirmbag" => TechniqueCode.FrankenSquirmbag,
				"Finned Franken Squirmbag" => TechniqueCode.FinnedFrankenSquirmbag,
				"Sashimi Franken Squirmbag" => TechniqueCode.SashimiFrankenSquirmbag,
				"Mutant Squirmbag" => TechniqueCode.MutantSquirmbag,
				"Finned Mutant Squirmbag" => TechniqueCode.FinnedMutantSquirmbag,
				"Sashimi Mutant Squirmbag" => TechniqueCode.SashimiMutantSquirmbag,
				"Whale" => TechniqueCode.Whale,
				"Finned Whale" => TechniqueCode.FinnedWhale,
				"Sashimi Whale" => TechniqueCode.SashimiWhale,
				"Franken Whale" => TechniqueCode.FrankenWhale,
				"Finned Franken Whale" => TechniqueCode.FinnedFrankenWhale,
				"Sashimi Franken Whale" => TechniqueCode.SashimiFrankenWhale,
				"Mutant Whale" => TechniqueCode.MutantWhale,
				"Finned Mutant Whale" => TechniqueCode.FinnedMutantWhale,
				"Sashimi Mutant Whale" => TechniqueCode.SashimiMutantWhale,
				"Leviathan" => TechniqueCode.Leviathan,
				"Finned Leviathan" => TechniqueCode.FinnedLeviathan,
				"Sashimi Leviathan" => TechniqueCode.SashimiLeviathan,
				"Franken Leviathan" => TechniqueCode.FrankenLeviathan,
				"Finned Franken Leviathan" => TechniqueCode.FinnedFrankenLeviathan,
				"Sashimi Franken Leviathan" => TechniqueCode.SashimiFrankenLeviathan,
				"Mutant Leviathan" => TechniqueCode.MutantLeviathan,
				"Finned Mutant Leviathan" => TechniqueCode.FinnedMutantLeviathan,
				"Sashimi Mutant Leviathan" => TechniqueCode.SashimiMutantLeviathan,
				_ => throw new NotSupportedException("The current instance doesn't support this kind of fish.")
			};

		/// <summary>
		/// Indicates the base difficulty.
		/// </summary>
		private decimal BaseDifficulty => BasicDiff[Size];

		/// <summary>
		/// Indicates the extra difficulty on sashimi judgement.
		/// </summary>
		private decimal SashimiExtraDifficulty =>
			IsSashimi switch { false => FinnedDiff[Size], true => SashimiDiff[Size], null => 0 };

		/// <summary>
		/// Indicates the extra difficulty on shape.
		/// </summary>
		private decimal ShapeExtraDifficulty =>
			IsBasic() ? 0 : IsFranken() ? FrankenShapeDiffExtra[Size] : MutantShapeDiffExtra[Size];

		/// <summary>
		/// The internal name.
		/// </summary>
		private string InternalName =>
			$@"{IsSashimi switch { null => string.Empty, true => "Sashimi ", false => "Finned " }}{true switch
			{
				_ when IsBasic() => string.Empty,
				_ when IsFranken() => "Franken ",
				_ => "Mutant ",
			}}{FishNames[Size]}";


		/// <inheritdoc/>
		public override string ToString()
		{
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			string baseSets = new RegionCollection(BaseSets).ToString();
			string coverSets = new RegionCollection(CoverSets).ToString();
			string exo = Exofins.IsEmpty ? string.Empty : $"f{Exofins} ";
			string endo = Endofins.IsEmpty ? string.Empty : $"ef{Endofins} ";
			return $@"{Name}: {Digit + 1} in {baseSets}\{coverSets} {exo}{endo}=> {elimStr}";
		}

		/// <summary>
		/// To check whether the specified structure is basic.
		/// </summary>
		/// <returns>A <see cref="bool"/> value.</returns>
		private bool IsBasic() =>
#if OBSOLETE
			static bool r(int region) => region / 9 == 1;
			static bool c(int region) => region / 9 == 2;
			return BaseSets.All(r) && CoverSets.All(c) || BaseSets.All(c) && CoverSets.All(r);
#else
			false;
#endif


		/// <summary>
		/// To check whether the specified structure is Franken.
		/// </summary>
		/// <returns>A <see cref="bool"/> value.</returns>
		private bool IsFranken()
		{
			for (int i = 0, count = BaseSets.Count; i < count - 1; i++)
			{
				for (int j = i + 1; j < count; j++)
				{
					int bs1 = BaseSets[i];
					int bs2 = BaseSets[j];
					if (bs1 / 9 == 0 || bs2 / 9 == 0)
					{
						goto RowColumnCheck;
					}
				}
			}
			for (int i = 0, count = CoverSets.Count; i < count - 1; i++)
			{
				for (int j = i + 1; j < count; j++)
				{
					int cs1 = CoverSets[i];
					int cs2 = CoverSets[j];
					if (cs1 / 9 == 0 || cs2 / 9 == 0)
					{
						goto RowColumnCheck;
					}
				}
			}

			return false;

		RowColumnCheck:
			for (int i = 0, count = BaseSets.Count; i < count - 1; i++)
			{
				for (int j = i + 1; j < count; j++)
				{
					int bs1 = BaseSets[i];
					int bs2 = BaseSets[j];
					if (bs1 / 9 == 1 && bs2 / 9 == 2)
					{
						return false;
					}
				}
			}
			for (int i = 0, count = CoverSets.Count; i < count - 1; i++)
			{
				for (int j = i + 1; j < count; j++)
				{
					int cs1 = CoverSets[i];
					int cs2 = CoverSets[j];
					if (cs1 / 9 == 1 && cs2 / 9 == 2)
					{
						return false;
					}
				}
			}

			return true;
		}
	}
}
