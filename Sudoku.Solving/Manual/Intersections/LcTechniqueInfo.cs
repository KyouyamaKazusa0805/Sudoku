using System.Collections.Generic;
using System.Text;
using Sudoku.Constants;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Globalization;
using Sudoku.Windows;

namespace Sudoku.Solving.Manual.Intersections
{
	/// <summary>
	/// Provides a usage of <b>locked candidates</b> (LC) technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Digit">The digit.</param>
	/// <param name="BaseSet">The base region.</param>
	/// <param name="CoverSet">The cover region.</param>
	public sealed record LcTechniqueInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views, int Digit, int BaseSet, int CoverSet)
		: IntersectionTechniqueInfo(Conclusions, Views)
	{
		/// <inheritdoc/>
		public override decimal Difficulty => BaseSet < 9 ? 2.6M : 2.8M;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Moderate;

		/// <inheritdoc/>
		public override TechniqueCode TechniqueCode =>
			BaseSet < 9 ? TechniqueCode.Pointing : TechniqueCode.Claiming;


		/// <inheritdoc/>
		public override string ToString()
		{
			using var elims = new ConclusionCollection(Conclusions);
			return new StringBuilder()
				.Append(Name)
				.Append(Resources.GetValue("Colon"))
				.Append(Resources.GetValue("Space"))
				.Append(Digit + 1)
				.Append(Resources.GetValue("_LcSimple1"))
				.Append(new RegionCollection(BaseSet).ToString())
				.Append(Resources.GetValue("Backslash"))
				.Append(new RegionCollection(CoverSet).ToString())
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
				_ => ToString()
			};

			string toChinese()
			{
				using var elims = new ConclusionCollection(Conclusions);
				return new StringBuilder()
					.Append(Name)
					.Append(Resources.GetValue("Colon"))
					.Append(Resources.GetValue("_LcSimple1"))
					.Append(new RegionCollection(BaseSet).ToString())
					.Append(Resources.GetValue("Backslash"))
					.Append(new RegionCollection(CoverSet).ToString())
					.Append(Resources.GetValue("_LcSimple2"))
					.Append(Digit + 1)
					.Append(Resources.GetValue("_LcSimple3"))
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
				_ => ToString()
			};

			string toChinese()
			{
				using var elims = new ConclusionCollection(Conclusions);
				string regionChineseName = Resources.GetValue(Processings.GetLabel(CoverSet).ToString());
				int digit = Digit + 1;
#if OBSOLETE
				return
					$"{Name}：可以发现数字 {digit} 只出现在 {new RegionCollection(BaseSet).ToString()}" +
					$" 的固定几格上，而同时我们又可以发现，此时它们恰好处于同一个{regionChineseName}" +
					$"（即 {new RegionCollection(CoverSet).ToString()}），因此我们可以知道，" +
					$"当前{regionChineseName}里的其它单元格就不能再填入 {digit} 了，" +
					$"所以我们可以得到 {elims.ToString()}。";
#else
				return new StringBuilder()
					.Append(Name)
					.Append(Resources.GetValue("Colon"))
					.Append(Resources.GetValue("Space"))
					.Append(Resources.GetValue("_Lc1"))
					.Append(digit)
					.Append(Resources.GetValue("_Lc2"))
					.Append(new RegionCollection(BaseSet).ToString())
					.Append(Resources.GetValue("_Lc3"))
					.Append(regionChineseName)
					.Append(Resources.GetValue("_Lc4"))
					.Append(new RegionCollection(CoverSet).ToString())
					.Append(Resources.GetValue("_Lc5"))
					.Append(regionChineseName)
					.Append(Resources.GetValue("_Lc6"))
					.Append(digit)
					.Append(Resources.GetValue("_Lc7"))
					.Append(elims.ToString())
					.Append(Resources.GetValue("Period"))
					.ToString();
#endif
			}
		}
	}
}
