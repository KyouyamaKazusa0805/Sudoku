using System.Collections.Generic;
using System.Text;
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
	/// <param name="IsFranken">Indicates whether the current structure is a Franken fish.</param>
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
	public sealed partial record ComplexFishStepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views, int Digit,
		IReadOnlyList<int> BaseSets, IReadOnlyList<int> CoverSets, in Cells Exofins,
		in Cells Endofins, bool IsFranken, bool? IsSashimi
	) : FishStepInfo(Conclusions, Views, Digit, BaseSets, CoverSets)
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
		public override decimal Difficulty => BaseDifficulty + SashimiExtraDifficulty + ShapeExtraDifficulty;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel =>
			Size is 2 or 3 or 4 ? DifficultyLevel.Fiendish : DifficultyLevel.Nightmare;

		/// <inheritdoc/>
		public override Technique TechniqueCode =>
			TechniqueStrings.GetComplexFishTechniqueCodeFromName(InternalName);

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
			IsFranken ? FrankenShapeDiffExtra[Size] : MutantShapeDiffExtra[Size];

		/// <summary>
		/// The internal name.
		/// </summary>
		private string InternalName
		{
			get
			{
				var sb = new ValueStringBuilder(stackalloc char[50]);
				sb.Append(FinModifier == FinModifiers.Normal ? string.Empty : $"{FinModifier.ToString()} ");
				sb.Append(ShapeModifier == ShapeModifiers.Basic ? string.Empty : $"{ShapeModifier.ToString()} ");
				sb.Append(FishNames[Size]);

				return sb.ToString();
			}
		}

		/// <summary>
		/// Indicates the fin modifier.
		/// </summary>
		private FinModifiers FinModifier => IsSashimi switch
		{
			true => FinModifiers.Sashimi,
			false => FinModifiers.Finned,
			_ => FinModifiers.Normal
		};

		/// <summary>
		/// The shape modifier.
		/// </summary>
		private ShapeModifiers ShapeModifier => IsFranken ? ShapeModifiers.Franken : ShapeModifiers.Mutant;


		/// <inheritdoc/>
		public bool Equals(ComplexFishStepInfo? other)
		{
			if (other is null)
			{
				return false;
			}

			if (Digit != other.Digit)
			{
				return false;
			}

			if (new RegionCollection(BaseSets) != new RegionCollection(other.BaseSets)
				|| new RegionCollection(CoverSets) != new RegionCollection(other.CoverSets))
			{
				return false;
			}

			if (Exofins != other.Exofins || Endofins != other.Endofins)
			{
				return false;
			}

			return true;
		}

		/// <inheritdoc/>
		public override int GetHashCode()
		{
			int result = Digit << 17 & 0xABC0DEF;
			result ^= new RegionCollection(BaseSets).GetHashCode();
			result ^= new RegionCollection(CoverSets).GetHashCode();
			result ^= Exofins.GetHashCode();
			result ^= Endofins.GetHashCode();

			return result;
		}

		/// <inheritdoc/>
		public override string ToString()
		{
			string digitStr = (Digit + 1).ToString();
			string baseSets = new RegionCollection(BaseSets).ToString();
			string coverSets = new RegionCollection(CoverSets).ToString();
			string exo = Exofins.IsEmpty ? string.Empty : $"f{Exofins.ToString()} ";
			string endo = Endofins.IsEmpty ? string.Empty : $"ef{Endofins.ToString()} ";
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			return $@"{Name}: {digitStr} in {baseSets}\{coverSets} {exo}{endo}=> {elimStr}";
		}
	}
}
