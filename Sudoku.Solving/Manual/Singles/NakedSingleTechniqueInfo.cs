using System.Collections.Generic;
using System.Text;
using Sudoku.Data;
using Sudoku.Drawing;
using Sudoku.Globalization;

namespace Sudoku.Solving.Manual.Singles
{
	/// <summary>
	/// Indicates a usage of <b>naked single</b> technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Cell">The cell.</param>
	/// <param name="Digit">The digit.</param>
	public sealed record NakedSingleTechniqueInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views, int Cell, int Digit)
		: SingleTechniqueInfo(Conclusions, Views, Cell, Digit)
	{
		/// <inheritdoc/>
		public override decimal Difficulty => 2.3M;

		/// <inheritdoc/>
		public override TechniqueCode TechniqueCode => TechniqueCode.NakedSingle;


		/// <inheritdoc/>
		public override string ToString() => $"{Name}: {new GridMap { Cell }} = {Digit + 1}";

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
				int v = Digit + 1;
				return new StringBuilder()
					.Append(Name)
					.Append('：')
					.Append(cellStr)
					.Append(" 仅可以填入数字 ")
					.Append(v)
					.Append("，因为别的情况都可从外部给出的确定值信息所排除，所以可以确定 ")
					.Append(cellStr)
					.Append(" = ")
					.Append(v)
					.Append('。')
					.ToString();
			}
		}
	}
}
