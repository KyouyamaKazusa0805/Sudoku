using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;

namespace Sudoku.Solving.Manual.Uniqueness.Loops
{
	/// <summary>
	/// Provides a usage of <b>unique loop type 4</b> technique.
	/// </summary>
	public sealed class UlType4TechniqueInfo : UlTechniqueInfo
	{
		/// <summary>
		/// Initializes an instance with the specified information.
		/// </summary>
		/// <param name="conclusions">All conclusions.</param>
		/// <param name="views">All views.</param>
		/// <param name="d1">The digit 1.</param>
		/// <param name="d2">The digit 2.</param>
		/// <param name="loop">The loop.</param>
		/// <param name="conjugatePair">The conjugate pair.</param>
		public UlType4TechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views, int d1, int d2, GridMap loop,
			ConjugatePair conjugatePair) : base(conclusions, views, d1, d2, loop) =>
			ConjugatePair = conjugatePair;


		/// <summary>
		/// Indicates the conjugate pair.
		/// </summary>
		public ConjugatePair ConjugatePair { get; }

		/// <inheritdoc/>
		public override int Type => 4;


		/// <inheritdoc/>
		public override string ToString()
		{
			string cellsStr = new CellCollection(Loop).ToString();
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			return
				$"{Name}: Digits {Digit1 + 1}, {Digit2 + 1} in cells {cellsStr} " +
				$"with the conjugate pair {ConjugatePair} => {elimStr}";
		}
	}
}
