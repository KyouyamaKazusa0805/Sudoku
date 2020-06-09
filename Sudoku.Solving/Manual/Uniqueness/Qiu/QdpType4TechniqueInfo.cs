using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;

namespace Sudoku.Solving.Manual.Uniqueness.Qiu
{
	/// <summary>
	/// Provides a usage of <b>Qiu's deadly pattern type 4</b> (QDP) technique.
	/// </summary>
	public sealed class QdpType4TechniqueInfo : QdpTechniqueInfo
	{
		/// <include file='SolvingDocComments.xml' path='comments/constructor[@type="TechniqueInfo"]'/>
		/// <param name="pattern">The pattern.</param>
		/// <param name="conjugatePair">The conjugate pair.</param>
		public QdpType4TechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views, Pattern pattern,
			ConjugatePair conjugatePair) : base(conclusions, views, pattern) => ConjugatePair = conjugatePair;


		/// <summary>
		/// Indicates the conjugate pair.
		/// </summary>
		public ConjugatePair ConjugatePair { get; }

		/// <inheritdoc/>
		public override decimal Difficulty => base.Difficulty + .2M;

		/// <inheritdoc/>
		public override TechniqueCode TechniqueCode => TechniqueCode.QdpType4;


		/// <inheritdoc/>
		public override string ToString()
		{
			string patternStr = new CellCollection(Pattern.FullMap).ToString();
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			return
				$"{Name}: Cells {patternStr} will be a deadly pattern " +
				$"if another digit in either cells lying on the conjugate pair {ConjugatePair} is true => {elimStr}";
		}
	}
}
