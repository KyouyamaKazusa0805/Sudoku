using System.Collections.Generic;
using System.Text;
using Sudoku.Constants;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Globalization;
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
					"Row" => TechniqueCode.HiddenSingleRow,
					"Column" => TechniqueCode.HiddenSingleColumn,
					"Block" => TechniqueCode.HiddenSingleBlock,
					_ => throw Throwings.ImpossibleCase
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
					.Append('：')
					.Append(cellStr)
					.Append(" = ")
					.Append(v)
					.ToString(),
				_ =>
					new StringBuilder()
					.Append(Name)
					.Append('：')
					.Append(cellStr)
					.Append(" = ")
					.Append(v)
					.Append(" in ")
					.Append(new RegionCollection(Region).ToString())
					.ToString()
			};
		}

		/// <inheritdoc/>
		public override string ToString(CountryCode countryCode)
		{
			switch (countryCode)
			{
				case CountryCode.ZhCn:
				{
					string cellStr = new GridMap { Cell }.ToString();
					int v = Digit + 1;
					return EnableAndIsLastDigit switch
					{
						true =>
							new StringBuilder()
							.Append(Name)
							.Append('：')
							.Append(cellStr)
							.Append(" = ")
							.Append(v)
							.ToString(),
						_ =>
							new StringBuilder()
							.Append(Name)
							.Append(": 在 ")
							.Append(new RegionCollection(Region).ToString())
							.Append(" 里，")
							.Append(cellStr)
							.Append(" = ")
							.Append(v)
							.ToString()
					};
				}
				default:
				{
					return ToString();
				}
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
				string cellStr = new GridMap { Cell }.ToString();
				string regionStr = new RegionCollection(Region).ToString();
				int v = Digit + 1;
				return EnableAndIsLastDigit switch
				{
					true =>
						new StringBuilder()
						.Append(Name)
						.Append("：全盘仅剩下唯一一个需要填入 ")
						.Append(v)
						.Append(" 的机会，由于只有 ")
						.Append(regionStr)
						.Append(" 没有填入 ")
						.Append(v)
						.Append(" 了，所以可以确定 ")
						.Append(cellStr)
						.Append(" = ")
						.Append(v)
						.Append('。')
						.ToString(),
					_ =>
						new StringBuilder()
						.Append(Name)
						.Append("：在 ")
						.Append(regionStr)
						.Append(" 里，只有 ")
						.Append(cellStr)
						.Append(" 是可以填入 ")
						.Append(v)
						.Append(" 的地方，所以可以确定 ")
						.Append(cellStr)
						.Append(" = ")
						.Append(v)
						.Append('。')
						.ToString()
				};
			}
		}
	}
}
