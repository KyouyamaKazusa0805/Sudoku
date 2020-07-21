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
	public sealed class HobiwanFishTechniqueInfo : FishTechniqueInfo
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


		/// <include file='SolvingDocComments.xml' path='comments/constructor[@type="TechniqueInfo"]'/>
		/// <param name="digit">The digit.</param>
		/// <param name="baseSets">The base sets.</param>
		/// <param name="coverSets">The cover sets.</param>
		/// <param name="exofins">The exo-fins.</param>
		/// <param name="endofins">The endo-fins.</param>
		/// <param name="isSashimi">Indicates the sashimi fish.</param>
		public HobiwanFishTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views,
			int digit, IReadOnlyList<int> baseSets, IReadOnlyList<int> coverSets,
			GridMap exofins, GridMap endofins, bool? isSashimi) : base(conclusions, views, digit, baseSets, coverSets) =>
			(ExofinCells, EndofinCells, IsSashimi) = (exofins, endofins, isSashimi);


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
		/// Indicates all exo-fins.
		/// </summary>
		public GridMap ExofinCells { get; }

		/// <summary>
		/// Indicates all endo-fins.
		/// </summary>
		public GridMap EndofinCells { get; }

		/// <inheritdoc/>
		public override decimal Difficulty =>
			BasicDiff[Size] + IsSashimi switch
			{
				false => FinnedDiff[Size],
				true => SashimiDiff[Size],
				null => 0
			} + true switch
			{
				_ when IsBasic() => 0,
				_ when IsFranken() => FrankenShapeDiffExtra[Size],
				_ => MutantShapeDiffExtra[Size],
			};

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel =>
			Size switch
			{
				2 => DifficultyLevel.Fiendish,
				3 => DifficultyLevel.Fiendish,
				4 => DifficultyLevel.Fiendish,
				_ => DifficultyLevel.Nightmare,
			};

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
				_ => throw new NotSupportedException("The current instance does not support this kind of fish.")
			};

		/// <summary>
		/// The internal name.
		/// </summary>
		private string InternalName =>
			$@"{IsSashimi switch
			{
				null => string.Empty,
				true => "Sashimi ",
				false => "Finned "
			}}{true switch
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
			string exo = ExofinCells.IsEmpty ? string.Empty : $"f{new CellCollection(ExofinCells).ToString()} ";
			string endo = EndofinCells.IsEmpty ? string.Empty : $"ef{new CellCollection(EndofinCells).ToString()} ";
			return $@"{Name}: {Digit + 1} in {baseSets}\{coverSets} {exo}{endo}=> {elimStr}";
		}

		/// <summary>
		/// To check whether the specified structure is basic.
		/// </summary>
		/// <returns>A <see cref="bool"/> value.</returns>
		private bool IsBasic()
		{
			return false;

			#region Obsolete code
			//static bool r(int region) => region / 9 == 1;
			//static bool c(int region) => region / 9 == 2;
			//return BaseSets.All(r) && CoverSets.All(c) || BaseSets.All(c) && CoverSets.All(r);
			#endregion
		}

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
