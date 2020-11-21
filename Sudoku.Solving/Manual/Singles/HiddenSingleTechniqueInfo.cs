using System.Collections.Generic;
using System.Text;
using Sudoku.Constants;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Globalization;
using Sudoku.Windows;
using static Sudoku.Constants.Processings;

namespace Sudoku.Solving.Manual.Singles
{
	/// <summary>
	/// Indicates a using of <b>hidden single</b> technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Cell">The cell.</param>
	/// <param name="Digit">The digit.</param>
	/// <param name="Region">The region.</param>
	/// <param name="EnableAndIsLastDigit">Indicates whether the current technique is a last digit.</param>
	public sealed record HiddenSingleTechniqueInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views, int Cell, int Digit,
		int Region, bool EnableAndIsLastDigit) : SingleTechniqueInfo(Conclusions, Views, Cell, Digit)
	{
		/// <inheritdoc/>
		public override decimal Difficulty =>
			EnableAndIsLastDigit switch { true => 1.1M, _ => Region switch { < 9 => 1.2M, _ => 1.5M } };

		/// <inheritdoc/>
		public override TechniqueCode TechniqueCode =>
			EnableAndIsLastDigit switch
			{
				true => TechniqueCode.LastDigit,
				_ => GetLabel(Region) switch
				{
					RegionLabel.Row => TechniqueCode.HiddenSingleRow,
					RegionLabel.Column => TechniqueCode.HiddenSingleColumn,
					RegionLabel.Block => TechniqueCode.HiddenSingleBlock
				}
			};


		/// <inheritdoc/>
		public override string ToString()
		{
			string cellStr = new GridMap { Cell }.ToString();
			int v = Digit + 1;
			return EnableAndIsLastDigit switch
			{
				true =>
					new StringBuilder()
					.Append(Name)
					.Append(Resources.GetValue("Colon"))
					.Append(Resources.GetValue("Space"))
					.Append(cellStr)
					.Append(Resources.GetValue("Equals"))
					.Append(v)
					.ToString(),
				_ =>
					new StringBuilder()
					.Append(Name)
					.Append(Resources.GetValue("Colon"))
					.Append(Resources.GetValue("Space"))
					.Append(cellStr)
					.Append(Resources.GetValue("Equals"))
					.Append(v)
					.Append(Resources.GetValue("_HiddenSingleSimple1"))
					.Append(new RegionCollection(Region).ToString())
					.ToString()
			};
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
				string cellStr = new GridMap { Cell }.ToString();
				int v = Digit + 1;
				return EnableAndIsLastDigit switch
				{
					true =>
						new StringBuilder()
						.Append(Name)
						.Append(Resources.GetValue("Colon"))
						.Append(cellStr)
						.Append(Resources.GetValue("Equals"))
						.Append(v)
						.ToString(),
					_ =>
						new StringBuilder()
						.Append(Name)
						.Append(Resources.GetValue("_HiddenSingleSimple1"))
						.Append(new RegionCollection(Region).ToString())
						.Append(Resources.GetValue("_HiddenSingleSimple2"))
						.Append(cellStr)
						.Append(Resources.GetValue("Equals"))
						.Append(v)
						.ToString()
				};
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

			string toChinese()
			{
				string cellStr = new GridMap { Cell }.ToString();
				string regionStr = new RegionCollection(Region).ToString();
				int v = Digit + 1;
				return EnableAndIsLastDigit switch
				{
					true =>
						new StringBuilder()
						.Append(Name)
						.Append(Resources.GetValue("_LastDigit1"))
						.Append(v)
						.Append(Resources.GetValue("_LastDigit2"))
						.Append(regionStr)
						.Append(Resources.GetValue("_LastDigit3"))
						.Append(v)
						.Append(Resources.GetValue("_LastDigit4"))
						.Append(cellStr)
						.Append(Resources.GetValue("Equals"))
						.Append(v)
						.Append(Resources.GetValue("Period"))
						.ToString(),
					_ =>
						new StringBuilder()
						.Append(Name)
						.Append(Resources.GetValue("_HiddenSingle1"))
						.Append(regionStr)
						.Append(Resources.GetValue("_HiddenSingle2"))
						.Append(cellStr)
						.Append(Resources.GetValue("_HiddenSingle3"))
						.Append(v)
						.Append(Resources.GetValue("_HiddenSingle4"))
						.Append(cellStr)
						.Append(Resources.GetValue("Equals"))
						.Append(v)
						.Append(Resources.GetValue("Period"))
						.ToString()
				};
			}
		}
	}
}
