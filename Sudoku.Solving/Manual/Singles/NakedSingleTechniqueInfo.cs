using System.Collections.Generic;
using System.Text;
using Sudoku.Data;
using Sudoku.Drawing;
using Sudoku.Globalization;
using Sudoku.Windows;

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
		public override string ToString() =>
			new StringBuilder()
			.Append(Name)
			.Append(Resources.GetValue("Colon"))
			.Append(Resources.GetValue("Space"))
			.Append(new GridMap { Cell })
			.Append(Resources.GetValue("Equals"))
			.Append(Digit + 1)
			.ToString();

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
					.Append(Resources.GetValue("Colon"))
					.Append(cellStr)
					.Append(Resources.GetValue("_NakedSingle1"))
					.Append(v)
					.Append(Resources.GetValue("_NakedSingle2"))
					.Append(cellStr)
					.Append(Resources.GetValue("Equals"))
					.Append(v)
					.Append(Resources.GetValue("Period"))
					.ToString();
			}
		}
	}
}
