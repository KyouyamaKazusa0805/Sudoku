using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Solving.Utils;

namespace Sudoku.Solving.Manual.Alses
{
	/// <summary>
	/// Provides a usage of <b>sue de coq</b> (SdC) technique.
	/// </summary>
	public sealed class SdcTechniqueInfo : AlsTechniqueInfo
	{
		/// <summary>
		/// Initializes an instance with the specified information.
		/// </summary>
		/// <param name="conclusions">All conclusions.</param>
		/// <param name="views">All views.</param>
		/// <param name="als1Cells">ALS 1 cells.</param>
		/// <param name="als1Digits">ALS 1 digits.</param>
		/// <param name="als2Cells">ALS 2 cells.</param>
		/// <param name="als2Digits">ALS 2 digits.</param>
		/// <param name="interCells">Intersection cells.</param>
		/// <param name="interDigits">Intersection digits.</param>
		public SdcTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views,
			IReadOnlyList<int> als1Cells, IReadOnlyList<int> als1Digits,
			IReadOnlyList<int> als2Cells, IReadOnlyList<int> als2Digits,
			IReadOnlyList<int> interCells, IReadOnlyList<int> interDigits)
			: base(conclusions, views) =>
			(Als1Cells, Als1Digits, Als2Cells, Als2Digits, IntersectionCells, IntersectionDigits) = (als1Cells, als1Digits, als2Cells, als2Digits, interCells, interDigits);


		/// <summary>
		/// Indicates all cells in ALS 1.
		/// </summary>
		public IReadOnlyList<int> Als1Cells { get; }

		/// <summary>
		/// Indicates all cells in ALS 2.
		/// </summary>
		public IReadOnlyList<int> Als2Cells { get; }

		/// <summary>
		/// Indicates all digits in ALS 1.
		/// </summary>
		public IReadOnlyList<int> Als1Digits { get; }

		/// <summary>
		/// Indicates all digits in ALS 2.
		/// </summary>
		public IReadOnlyList<int> Als2Digits { get; }

		/// <summary>
		/// Indicates all intersection cells.
		/// </summary>
		public IReadOnlyList<int> IntersectionCells { get; }

		/// <summary>
		/// Indicates all intersection digits.
		/// </summary>
		public IReadOnlyList<int> IntersectionDigits { get; }

		/// <inheritdoc/>
		public override string Name => "Sue de Coq";

		/// <inheritdoc/>
		public override decimal Difficulty => IntersectionCells.Count == 2 ? 5M : 5.1M;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;


		/// <inheritdoc/>
		public override bool Equals(TechniqueInfo other)
		{
			return other is SdcTechniqueInfo comparer
				&& (new GridMap(Als1Cells) | new GridMap(Als2Cells) | new GridMap(IntersectionCells))
				== (new GridMap(comparer.Als1Cells) | new GridMap(comparer.Als2Cells)
				| new GridMap(comparer.IntersectionCells));
		}

		/// <inheritdoc/>
		public override string ToString()
		{
			string interCells = new CellCollection(IntersectionCells).ToString();
			string digits = new DigitCollection(IntersectionDigits).ToString(null);
			string als1Cells = new CellCollection(Als1Cells).ToString();
			string als1Digits = new DigitCollection(Als1Digits).ToString(null);
			string als2Cells = new CellCollection(Als2Cells).ToString();
			string als2Digits = new DigitCollection(Als2Digits).ToString(null);
			string elimStr = ConclusionCollection.ToString(Conclusions);
			return
				$"{Name}: {interCells}({digits}) - ({als1Cells}({als1Digits}) & {als2Cells}({als2Digits})) => " +
				$"{elimStr}";
		}
	}
}
