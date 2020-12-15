using System;
using System.Collections.Generic;
using System.Extensions;
using System.Linq;
using System.Text;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Globalization;
using Sudoku.Windows;
using static Sudoku.Solving.Constants.Processings;
#if DOUBLE_LAYERED_ASSUMPTION
using static Sudoku.Constants.Processings;
using static Sudoku.Solving.TechniqueSearcher;
#endif

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
	public sealed record NormalFishStepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views, int Digit,
		IReadOnlyList<int> BaseSets, IReadOnlyList<int> CoverSets, in Cells Fins, bool? IsSashimi)
		: FishStepInfo(Conclusions, Views, Digit, BaseSets, CoverSets)
#if DOUBLE_LAYERED_ASSUMPTION
		, IHasParentNodeInfo
#endif
	{
		/// <inheritdoc/>
		public override decimal Difficulty => BaseDifficulty + SashimiExtraDifficulty;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;

		/// <inheritdoc/>
		public override TechniqueCode TechniqueCode =>
			Enum.Parse<TechniqueCode>(InternalName.Replace(" ", string.Empty).Replace("-", string.Empty));

		/// <summary>
		/// Indicates the base difficulty.
		/// </summary>
		private decimal BaseDifficulty => Size switch { 2 => 3.2M, 3 => 3.8M, 4 => 5.2M };

		/// <summary>
		/// Indicates the extra difficulty on sashimi judgement.
		/// </summary>
		private decimal SashimiExtraDifficulty =>
			IsSashimi switch
			{
				null => 0,
				true => Size switch { 2 => .3M, 3 => .3M, 4 => .4M },
				false => .2M
			};

		/// <summary>
		/// Indicates the internal name.
		/// </summary>
		private string InternalName =>
			$"{IsSashimi switch { true => "Sashimi ", false => "Finned ", _ => string.Empty }}{FishNames[Size]}";


		/// <inheritdoc/>
		public override string ToString() =>
			new StringBuilder()
				.Append(Name)
				.Append(Resources.GetValue("Colon"))
				.Append(Resources.GetValue("Space"))
				.Append(Digit + 1)
				.Append(Resources.GetValue("_NormalFishSimple1"))
				.Append(new RegionCollection(BaseSets).ToString())
				.Append(Resources.GetValue("Backslash"))
				.Append(new RegionCollection(CoverSets).ToString())
				.Append(Fins.IsEmpty ? string.Empty : $" f{Fins}")
				.Append(Resources.GetValue("GoesTo"))
				.Append(new ConclusionCollection(Conclusions).ToString())
				.ToString();

		/// <inheritdoc/>
		public override string ToString(CountryCode countryCode) =>
			countryCode switch
			{
				CountryCode.ZhCn =>
					new StringBuilder()
					.Append(Name)
					.Append(Resources.GetValue("Colon"))
					.Append(Resources.GetValue("_NormalFishSimple1"))
					.Append(Digit + 1)
					.Append(Resources.GetValue("_NormalFishSimple2"))
					.Append(new RegionCollection(BaseSets).ToString())
					.Append(Resources.GetValue("Backslash"))
					.Append(new RegionCollection(CoverSets).ToString())
					.Append(Fins.IsEmpty ? string.Empty : $" f{Fins}")
					.Append(Resources.GetValue("GoesTo"))
					.Append(new ConclusionCollection(Conclusions).ToString())
					.ToString(),
				_ => base.ToString(countryCode)
			};

		/// <inheritdoc/>
		public override string ToFullString(CountryCode countryCode)
		{
			return countryCode switch
			{
				CountryCode.ZhCn => toChinese(),
				_ => base.ToFullString(countryCode)
			};

			unsafe string toChinese()
			{
				var candidates = Views[0].Candidates!;
				int digit = Digit + 1, regionsCount = BaseSets.Count;
				static bool finChecking(in DrawingInfo info) => info is { Id: 0 };
				var elims = new ConclusionCollection(Conclusions);
				var sb = new StringBuilder()
					.Append(Name)
					.Append(Resources.GetValue("Colon"))
					.Append(Resources.GetValue("_NormalFish1"))
					.Append(digit)
					.Append(Resources.GetValue("_NormalFish2"))
					.Append(new RegionCollection(BaseSets).ToString())
					.Append(Resources.GetValue("_NormalFish3"))
					.Append(candidates.Count(&finChecking))
					.Append(Resources.GetValue("_NormalFish4"))
					.Append(new Candidates(from candidate in candidates select candidate.Value).ToString())
					.Append(Resources.GetValue("_NormalFish5"))
					.Append(digit)
					.Append(Resources.GetValue("_NormalFish6"))
					.Append(regionsCount)
					.Append(Resources.GetValue("_NormalFish7"))
					.Append(regionsCount)
					.Append(Resources.GetValue("_NormalFish8"))
					.Append(digit)
					.AppendLine(Resources.GetValue("Period"))
					.AppendLine();

				return Fins switch
				{
					{ IsEmpty: true } when BaseSets[0] is >= 9 and < 18 is var isRowDirection =>
						sb
						.Append(Resources.GetValue("_NormalFish9"))
						.Append(digit)
						.AppendLine(Resources.GetValue("_NormalFish10"))
						.AppendLine()
						.Append(Resources.GetValue("_NormalFish11"))
						.Append(Resources.GetValue(isRowDirection ? "Row" : "Column"))
						.Append(Resources.GetValue("_NormalFish12"))
						.Append(Resources.GetValue(isRowDirection ? "Column" : "Row"))
						.Append(Resources.GetValue("_NormalFish13"))
						.Append(digit)
						.AppendLine(Resources.GetValue("_NormalFish14"))
						.AppendLine()
						.Append(Resources.GetValue("_NormalFish15"))
						.Append(Resources.GetValue(isRowDirection ? "Column" : "Row"))
						.Append(Resources.GetValue("_NormalFish16"))
						.Append(digit)
						.Append(Resources.GetValue("_NormalFish17"))
						.Append(elims.ToString())
						.Append(Resources.GetValue("Period"))
						.ToString(),
					_ =>
						sb
						.Append(Resources.GetValue("_NormalFish18"))
						.Append(Fins.ToString())
						.AppendLine(Resources.GetValue("_NormalFish19"))
						.AppendLine()
						.Append(Resources.GetValue("_NormalFish20"))
						.Append(elims.ToString())
						.Append(Resources.GetValue("_NormalFish21"))
						.ToString()
				};
			}
		}

#if DOUBLE_LAYERED_ASSUMPTION
		/// <inheritdoc/>
		IEnumerable<Node> IHasParentNodeInfo.GetRuleParents(in SudokuGrid initialGrid, in SudokuGrid currentGrid)
		{
			var result = new List<Node>();
			foreach (int baseSet in BaseSets)
			{
				foreach (int cell in RegionMaps[baseSet] & EmptyMap)
				{
					short mask = currentGrid.GetCandidateMask(cell);
					short initialMask = initialGrid.GetCandidateMask(cell);
					if ((initialMask >> Digit & 1) != 0 && (mask >> Digit & 1) == 0)
					{
						bool isInCoverSet = false;
						foreach (int coverSet in CoverSets)
						{
							foreach (int otherCell in RegionMaps[coverSet] & EmptyMap)
							{
								if (otherCell == cell)
								{
									isInCoverSet = true;
									break;
								}
							}
						}
						if (!isInCoverSet)
						{
							result.Add(new(cell, Digit, false));
						}
					}
				}
			}

			return result;
		}
#endif
	}
}
