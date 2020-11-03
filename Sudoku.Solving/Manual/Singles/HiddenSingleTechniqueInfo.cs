using System.Collections.Generic;
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
		public override decimal Difficulty => EnableAndIsLastDigit ? 1.1M : Region < 9 ? 1.2M : 1.5M;

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
			string regionStr = new RegionCollection(Region).ToString();
			int v = Digit + 1;
			return EnableAndIsLastDigit ? $"{Name}: {cellStr} = {v}" : $"{Name}: {cellStr} = {v} in {regionStr}";
		}

		/// <inheritdoc/>
		public override string ToString(CountryCode countryCode)
		{
			switch (countryCode)
			{
				default:
				case CountryCode.EnUs:
				{
					return ToString();
				}
				case CountryCode.ZhCn:
				{
					string cellStr = new GridMap { Cell }.ToString();
					string regionStr = new RegionCollection(Region).ToString();
					int v = Digit + 1;
					return EnableAndIsLastDigit
						? $"{Name}: {cellStr} = {v}"
						: $"{Name}: 在 {regionStr} 里，{cellStr} = {v}";
				}
			}
		}

		/// <inheritdoc/>
		public override string ToFullString(CountryCode countryCode)
		{
			return countryCode == CountryCode.ZhCn ? toChinese() : ToString();
			string toChinese()
			{
				string cellStr = new GridMap { Cell }.ToString();
				string regionStr = new RegionCollection(Region).ToString();
				int v = Digit + 1;
				if (EnableAndIsLastDigit)
				{
					return
						$"{Name}：全盘仅剩下唯一一个需要填入 {v} 的机会，由于只有 {regionStr} 没有填入 {v} 了，" +
						$"所以可以确定 {cellStr} = {v}。";
				}
				else
				{
					return
						$"{Name}：在 {regionStr} 里，只有 {cellStr} 是可以填入 {v} 的地方，" +
						$"所以可以确定 {cellStr} = {v}。";
				}
			}
		}
	}
}
