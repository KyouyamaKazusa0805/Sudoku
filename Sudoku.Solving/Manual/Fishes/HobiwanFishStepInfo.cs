using System;
using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Techniques;

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
		IReadOnlyList<int> CoverSets, in Cells Exofins, in Cells Endofins, bool? IsSashimi)
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
		public override Technique TechniqueCode =>
			InternalName switch
			{
				"X-Wing" => Technique.XWing,
				"Finned X-Wing" => Technique.FinnedXWing,
				"Sashimi X-Wing" => Technique.SashimiXWing,
				"Franken X-Wing" => Technique.FrankenXWing,
				"Finned Franken X-Wing" => Technique.FinnedFrankenXWing,
				"Sashimi Franken X-Wing" => Technique.SashimiFrankenXWing,
				"Mutant X-Wing" => Technique.MutantXWing,
				"Finned Mutant X-Wing" => Technique.FinnedMutantXWing,
				"Sashimi Mutant X-Wing" => Technique.SashimiMutantXWing,
				"Swordfish" => Technique.Swordfish,
				"Finned Swordfish" => Technique.FinnedSwordfish,
				"Sashimi Swordfish" => Technique.SashimiSwordfish,
				"Franken Swordfish" => Technique.FrankenSwordfish,
				"Finned Franken Swordfish" => Technique.FinnedFrankenSwordfish,
				"Sashimi Franken Swordfish" => Technique.SashimiFrankenSwordfish,
				"Mutant Swordfish" => Technique.MutantSwordfish,
				"Finned Mutant Swordfish" => Technique.FinnedMutantSwordfish,
				"Sashimi Mutant Swordfish" => Technique.SashimiMutantSwordfish,
				"Jellyfish" => Technique.Jellyfish,
				"Finned Jellyfish" => Technique.FinnedJellyfish,
				"Sashimi Jellyfish" => Technique.SashimiJellyfish,
				"Franken Jellyfish" => Technique.FrankenJellyfish,
				"Finned Franken Jellyfish" => Technique.FinnedFrankenJellyfish,
				"Sashimi Franken Jellyfish" => Technique.SashimiFrankenJellyfish,
				"Mutant Jellyfish" => Technique.MutantJellyfish,
				"Finned Mutant Jellyfish" => Technique.FinnedMutantJellyfish,
				"Sashimi Mutant Jellyfish" => Technique.SashimiMutantJellyfish,
				"Squirmbag" => Technique.Squirmbag,
				"Finned Squirmbag" => Technique.FinnedSquirmbag,
				"Sashimi Squirmbag" => Technique.SashimiSquirmbag,
				"Franken Squirmbag" => Technique.FrankenSquirmbag,
				"Finned Franken Squirmbag" => Technique.FinnedFrankenSquirmbag,
				"Sashimi Franken Squirmbag" => Technique.SashimiFrankenSquirmbag,
				"Mutant Squirmbag" => Technique.MutantSquirmbag,
				"Finned Mutant Squirmbag" => Technique.FinnedMutantSquirmbag,
				"Sashimi Mutant Squirmbag" => Technique.SashimiMutantSquirmbag,
				"Whale" => Technique.Whale,
				"Finned Whale" => Technique.FinnedWhale,
				"Sashimi Whale" => Technique.SashimiWhale,
				"Franken Whale" => Technique.FrankenWhale,
				"Finned Franken Whale" => Technique.FinnedFrankenWhale,
				"Sashimi Franken Whale" => Technique.SashimiFrankenWhale,
				"Mutant Whale" => Technique.MutantWhale,
				"Finned Mutant Whale" => Technique.FinnedMutantWhale,
				"Sashimi Mutant Whale" => Technique.SashimiMutantWhale,
				"Leviathan" => Technique.Leviathan,
				"Finned Leviathan" => Technique.FinnedLeviathan,
				"Sashimi Leviathan" => Technique.SashimiLeviathan,
				"Franken Leviathan" => Technique.FrankenLeviathan,
				"Finned Franken Leviathan" => Technique.FinnedFrankenLeviathan,
				"Sashimi Franken Leviathan" => Technique.SashimiFrankenLeviathan,
				"Mutant Leviathan" => Technique.MutantLeviathan,
				"Finned Mutant Leviathan" => Technique.FinnedMutantLeviathan,
				"Sashimi Mutant Leviathan" => Technique.SashimiMutantLeviathan,
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
			string exo = Exofins.IsEmpty ? string.Empty : $"f{Exofins.ToString()} ";
			string endo = Endofins.IsEmpty ? string.Empty : $"ef{Endofins.ToString()} ";
			return $@"{Name}: {(Digit + 1).ToString()} in {baseSets}\{coverSets} {exo}{endo}=> {elimStr}";
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
