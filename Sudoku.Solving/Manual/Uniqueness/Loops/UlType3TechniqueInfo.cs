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
	public sealed class UlType3TechniqueInfo : UlTechniqueInfo
	{
		/// <include file='SolvingDocComments.xml' path='comments/constructor[@type="TechniqueInfo"]'/>
		/// <param name="d1">The digit 1.</param>
		/// <param name="d2">The digit 2.</param>
		/// <param name="loop">The loop.</param>
		/// <param name="subsetDigitsMask">The subset digits mask.</param>
		/// <param name="subsetCells">The subset cells.</param>
		public UlType3TechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views, int d1, int d2, GridMap loop,
			short subsetDigitsMask, IReadOnlyList<int> subsetCells)
			: base(conclusions, views, d1, d2, loop) =>
			(SubsetDigitsMask, SubsetCells) = (subsetDigitsMask, subsetCells);


		/// <summary>
		/// Indicates the extra digit mask.
		/// </summary>
		public short SubsetDigitsMask { get; }

		/// <summary>
		/// Indicates the subset cells.
		/// </summary>
		public IReadOnlyList<int> SubsetCells { get; }

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
