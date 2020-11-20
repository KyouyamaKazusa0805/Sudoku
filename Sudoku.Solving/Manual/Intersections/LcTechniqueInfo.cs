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
		public override string ToString() =>
			new StringBuilder()
				.Append(Name)
				.Append(Resources.GetValue("Colon"))
				.Append(Resources.GetValue("Space"))
				.Append(Digit + 1)
				.Append(Resources.GetValue("_LcSimple1"))
				.Append(new RegionCollection(BaseSet).ToString())
				.Append(Resources.GetValue("Backslash"))
				.Append(new RegionCollection(CoverSet).ToString())
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
					.Append(Resources.GetValue("_LcSimple1"))
					.Append(new RegionCollection(BaseSet).ToString())
					.Append(Resources.GetValue("Backslash"))
					.Append(new RegionCollection(CoverSet).ToString())
					.Append(Resources.GetValue("_LcSimple2"))
					.Append(Digit + 1)
					.Append(Resources.GetValue("_LcSimple3"))
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

			string toChinese()
			{
				string regionChineseName = Resources.GetValue(Processings.GetLabel(CoverSet).ToString());
				int digit = Digit + 1;
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
					.Append(new ConclusionCollection(Conclusions).ToString())
					.Append(Resources.GetValue("Period"))
					.ToString();
			}
		}
	}
}
