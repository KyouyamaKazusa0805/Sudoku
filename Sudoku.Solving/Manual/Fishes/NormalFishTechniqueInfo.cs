using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sudoku.Constants;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Extensions;
using Sudoku.Globalization;
using Sudoku.Windows;
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
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views, int Digit,
		IReadOnlyList<int> BaseSets, IReadOnlyList<int> CoverSets, IReadOnlyList<int>? Fins, bool? IsSashimi)
		: FishTechniqueInfo(Conclusions, Views, Digit, BaseSets, CoverSets)
	{
		/// <inheritdoc/>
		public override decimal Difficulty =>
			Size switch { 2 => 3.2M, 3 => 3.8M, 4 => 5.2M, _ => throw Throwings.ImpossibleCase }
			+ IsSashimi switch
			{
				null => 0,
				true => Size switch { 2 => .3M, 3 => .3M, 4 => .4M, _ => throw Throwings.ImpossibleCase },
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
				null => string.Empty,
				true => "Sashimi ",
				false => "Finned "
			}}{FishNames[Size]}";


		/// <inheritdoc/>
		public override string ToString()
		{
			using var elims = new ConclusionCollection(Conclusions);
			return new StringBuilder()
				.Append(Name)
				.Append(Resources.GetValue("Colon"))
				.Append(Resources.GetValue("Space"))
				.Append(Digit + 1)
				.Append(Resources.GetValue("_NormalFishSimple1"))
				.Append(new RegionCollection(BaseSets).ToString())
				.Append(Resources.GetValue("Backslash"))
				.Append(new RegionCollection(CoverSets).ToString())
				.Append(Fins is { Count: not 0 } ? $" f{new GridMap(Fins)}" : string.Empty)
				.Append(Resources.GetValue("GoesTo"))
				.Append(elims.ToString())
				.ToString();
		}

		/// <inheritdoc/>
		public override string ToString(CountryCode countryCode)
		{
			return countryCode switch
			{
				CountryCode.ZhCn => toChinese(),
				_ => base.ToString(countryCode)
			};

			string toChinese()
			{
				using var elims = new ConclusionCollection(Conclusions);
				return new StringBuilder()
					.Append(Name)
					.Append(Resources.GetValue("Colon"))
					.Append(Resources.GetValue("_NormalFishSimple1"))
					.Append(Digit + 1)
					.Append(Resources.GetValue("_NormalFishSimple2"))
					.Append(new RegionCollection(BaseSets).ToString())
					.Append(Resources.GetValue("Backslash"))
					.Append(new RegionCollection(CoverSets).ToString())
					.Append(Fins is { Count: not 0 } ? $" f{new GridMap(Fins)}" : string.Empty)
					.Append(Resources.GetValue("GoesTo"))
					.Append(elims.ToString())
					.ToString();
			}
		}

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
				var cells = Views[0].Cells!;
				int digit = Digit + 1, regionsCount = BaseSets.Count;
				using var elims = new ConclusionCollection(Conclusions);
				static bool finChecking(in DrawingInfo info) => info is { Id: 0 };
				var sb = new StringBuilder()
					.Append(Name)
					.Append(Resources.GetValue("Colon"))
					.Append("首先我们可以确定，数字 ")
					.Append(digit)
					.Append(" 在 ")
					.Append(new RegionCollection(BaseSets).ToString())
					.Append(" 里能填入的位置一共有 ")
					.Append(cells.Count(&finChecking))
					.Append(" 格（")
					.Append(cells)
					.Append("）。数独规则要求一个区域（行、列、宫）只能填入一次数字 ")
					.Append(digit)
					.Append("，因此，")
					.Append(regionsCount)
					.Append(" 个区域就要填入")
					.Append(regionsCount)
					.Append(" 个数字 ")
					.Append(digit)
					.AppendLine(Resources.GetValue("Period"))
					.AppendLine();

				return Fins switch
				{
					null or { Count: 0 } when BaseSets[0] is >= 9 and < 18 is var isRowDirection =>
						sb
						.Append("我们刚才的这些单元格可以用一个长方形框起来，这意味着格子是行列交叉对应起来的。")
						.Append("现在我们尝试随意往里面填入数字 ")
						.Append(digit)
						.Append("。但由于这个长方形的约束，格子的行和列上都不能再填入这个数字，")
						.AppendLine("因此我们使用这一点可以随意完成其中一种合适的填法。")
						.AppendLine()
						.Append("事实上，所有的填法都能够映射到一个相同的结论：由于结构是以")
						.Append(Resources.GetValue(isRowDirection ? "Row" : "Column"))
						.Append("作为假设，所以结构可覆盖到的这些")
						.Append(Resources.GetValue(isRowDirection ? "Column" : "Row"))
						.Append("上都能保证出现一次数字 ")
						.Append(digit)
						.AppendLine("，位于刚才所谓的长方形上。")
						.AppendLine()
						.Append("因此，这些")
						.Append(Resources.GetValue(isRowDirection ? "Column" : "Row"))
						.Append("上的其余单元格就不能填入数字 ")
						.Append(digit)
						.Append(" 了，因此把它们都删除掉，是安全的，所以结论就是 ")
						.Append(elims.ToString())
						.Append(Resources.GetValue("Period"))
						.ToString(),
					_ =>
						sb
						.Append("我们刚才的这些单元格可以用一个长方形框起来，这意味着格子是行列交叉对应起来的，不过 ")
						.Append(new GridMap(Fins))
						.Append(" 是例外。如果我们直接忽略它们的话，逻辑将退化为一个普通的鱼，就可以确定删除的位置了；")
						.Append("但由于现在出现了这些位置会影响到整体结构的推理，那么我们此时假设为真后，可以得到的是，")
						.AppendLine("这些位置可以通过简单的、直接的排除法得到删数的位置。")
						.AppendLine()
						.Append("逻辑需要保证完整性，所以两种不同的情况都能排除到的位置，则一定是题目的结论。")
						.Append("所以，通过逻辑推导，可以得到的是 ")
						.Append(elims.ToString())
						.Append(" 即是题目的结论。")
						.ToString()
				};
			}
		}
	}
}
