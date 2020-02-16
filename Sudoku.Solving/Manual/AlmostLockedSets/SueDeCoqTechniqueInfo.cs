using System.Collections.Generic;
using Sudoku.Drawing;
using Sudoku.Solving.Utils;

namespace Sudoku.Solving.Manual.AlmostLockedSets
{
	/// <summary>
	/// Provides a usage of sue de coq (SdC) technique.
	/// </summary>
	public sealed class SueDeCoqTechniqueInfo : AlmostLockedSetTechniqueInfo
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
		/// <param name="allDigits">All digits.</param>
		public SueDeCoqTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views,
			IReadOnlyList<int> als1Cells, IReadOnlyList<int> als1Digits,
			IReadOnlyList<int> als2Cells, IReadOnlyList<int> als2Digits,
			IReadOnlyList<int> interCells, IReadOnlyList<int> allDigits)
			: base(conclusions, views)
		{
			Als1Cells = als1Cells;
			Als1Digits = als1Digits;
			Als2Cells = als2Cells;
			Als2Digits = als2Digits;
			IntersectionCells = interCells;
			AllDigits = allDigits;
		}


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
		/// Indicates all digits used.
		/// </summary>
		public IReadOnlyList<int> AllDigits { get; }

		/// <inheritdoc/>
		public override string Name => "Sue de Coq";

		/// <inheritdoc/>
		public override decimal Difficulty => IntersectionCells.Count == 2 ? 5.0m : 5.1m;

		/// <inheritdoc/>
		public override DifficultyLevel DifficultyLevel => DifficultyLevel.VeryHard;


		/// <inheritdoc/>
		public override string ToString()
		{
			string interCells = CellCollection.ToString(IntersectionCells);
			string digits = DigitCollection.ToSimpleString(AllDigits);
			string elimStr = ConclusionCollection.ToString(Conclusions);
			string als1Cells = CellCollection.ToString(Als1Cells);
			string als1Digits = DigitCollection.ToSimpleString(Als1Digits);
			string als2Cells = CellCollection.ToString(Als2Cells);
			string als2Digits = DigitCollection.ToSimpleString(Als2Digits);
			return $"{Name}: {interCells}({digits}) - ({als1Cells}({als1Digits}) and {als2Cells}({als2Digits})) => {elimStr}";
		}
	}
}
