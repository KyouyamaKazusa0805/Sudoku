using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Drawing;
using Sudoku.Globalization;

namespace Sudoku.Solving.Manual.Singles
{
	/// <summary>
	/// Indicates a usage of <b>full house</b> technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Cell">The cell.</param>
	/// <param name="Digit">The digit.</param>
	public sealed record FullHouseTechniqueInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views, int Cell, int Digit)
		: SingleTechniqueInfo(Conclusions, Views, Cell, Digit)
	{
		/// <inheritdoc/>
		public override decimal Difficulty => 1.0M;

		/// <inheritdoc/>
		public override TechniqueCode TechniqueCode => TechniqueCode.FullHouse;


		/// <inheritdoc/>
		public override string ToString() => $"{Name}: {new GridMap { Cell }} = {Digit + 1}";

		/// <inheritdoc/>
		public override string ToFullString(CountryCode countryCode)
		{
			return countryCode == CountryCode.ZhCn ? toChinese() : ToString();
			string toChinese()
			{
				string cellStr = new GridMap { Cell }.ToString();
				int digit = Digit + 1;
				return $"{Name}：{cellStr} 是当前区域唯一一个空格，所以可以确定是数字 {digit}，即 {cellStr} = {digit}。";
			}
		}
	}
}
