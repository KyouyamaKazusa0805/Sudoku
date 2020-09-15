using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Extensions;
using static Sudoku.Solving.Constants.Processings;

namespace Sudoku.Solving.Manual.Uniqueness.Loops
{
	/// <summary>
	/// Provides a usage of <b>unique loop type 3</b> technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Digit1">The digit 1.</param>
	/// <param name="Digit2">The digit 2.</param>
	/// <param name="Loop">The loop.</param>
	/// <param name="SubsetDigitsMask">The subset digits mask.</param>
	/// <param name="SubsetCells">The subset cells.</param>
	public sealed record UlType3TechniqueInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views, int Digit1, int Digit2, GridMap Loop,
		short SubsetDigitsMask, IReadOnlyList<int> SubsetCells)
		: UlTechniqueInfo(Conclusions, Views, Digit1, Digit2, Loop)
	{
		/// <inheritdoc/>
		public override int Type => 3;


		/// <inheritdoc/>
		public override string ToString()
		{
			string cellsStr = new CellCollection(Loop).ToString();
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			string subsetName = SubsetNames[SubsetCells.Count + 1];
			string digitsStr = new DigitCollection(SubsetDigitsMask.GetAllSets()).ToString();
			string subsetCellsStr = new CellCollection(SubsetCells).ToString();
			return
				$"{Name}: Digits {Digit1 + 1}, {Digit2 + 1} in cells {cellsStr} " +
				$"with the naked {subsetName} with extra digits {digitsStr} " +
				$"in cells {subsetCellsStr} => {elimStr}";
		}
	}
}
