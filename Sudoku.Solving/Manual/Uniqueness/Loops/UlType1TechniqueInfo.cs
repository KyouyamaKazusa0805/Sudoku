using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;

namespace Sudoku.Solving.Manual.Uniqueness.Loops
{
	/// <summary>
	/// Provides a usage of <b>unique loop type 1</b> technique.
	/// </summary>
	public sealed class UlType1TechniqueInfo : UlTechniqueInfo
	{
		/// <inheritdoc/>
		public UlType1TechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views, int d1, int d2, GridMap loop)
			: base(conclusions, views, d1, d2, loop)
		{
		}


		/// <inheritdoc/>
		public override int Type => 1;


		/// <inheritdoc/>
		public override string ToString()
		{
			string cellsStr = new CellCollection(Loop).ToString();
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			return $"{Name}: Digits {Digit1 + 1}, {Digit2 + 1} in cells {cellsStr} => {elimStr}";
		}
	}
}
