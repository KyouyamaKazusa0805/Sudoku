using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Extensions;

namespace Sudoku.Solving.Manual.Uniqueness.Square
{
	/// <summary>
	/// Provides a usage of <b>unique square type 4</b> (US) technique.
	/// </summary>
	public sealed class UsType4TechniqueInfo : UsTechniqueInfo
	{
		/// <include file='SolvingDocComments.xml' path='comments/constructor[@type="TechniqueInfo"]'/>
		/// <param name="cells">The cells.</param>
		/// <param name="digitsMask">The digits mask.</param>
		/// <param name="d1">The digit 1.</param>
		/// <param name="d2">The digit 2.</param>
		/// <param name="conjugateRegion">The so-called conjugate region.</param>
		public UsType4TechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views, GridMap cells, short digitsMask,
			int d1, int d2, GridMap conjugateRegion) : base(conclusions, views, cells, digitsMask) =>
			(Digit1, Digit2, ConjugateRegion) = (d1, d2, conjugateRegion);


		/// <summary>
		/// Indicates the digit 1.
		/// </summary>
		public int Digit1 { get; }

		/// <summary>
		/// Indicates the digit 2.
		/// </summary>
		public int Digit2 { get; }

		/// <summary>
		/// Indicates the so-called conjugate region.
		/// </summary>
		public GridMap ConjugateRegion { get; }

		/// <inheritdoc/>
		public override TechniqueCode TechniqueCode => TechniqueCode.UsType4;


		/// <inheritdoc/>
		public override string ToString()
		{
			string digitsStr = new DigitCollection(DigitsMask.GetAllSets()).ToString();
			string cellsStr = new CellCollection(Cells).ToString();
			string conjStr = new CellCollection(ConjugateRegion).ToString();
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			return
				$"{Name}: Digits {digitsStr} in cells {cellsStr} can avoid to form a deadly pattern " +
				$"if and only if the conjugate region {conjStr} cannot set the digit " +
				$"neither {Digit1 + 1} nor {Digit2 + 1} => {elimStr}";
		}
	}
}
