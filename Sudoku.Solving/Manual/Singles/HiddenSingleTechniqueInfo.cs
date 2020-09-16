using System.Collections.Generic;
using Sudoku.Constants;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
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
		int Region, bool EnableAndIsLastDigit)
		: SingleTechniqueInfo(Conclusions, Views, Cell, Digit)
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
	}
}
