using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;

namespace Sudoku.Solving.Manual.Uniqueness.Qiu
{
	/// <summary>
	/// Provides a usage of <b>Qiu's deadly pattern type 1</b> (QDP) technique.
	/// </summary>
	public sealed class QdpType1TechniqueInfo : QdpTechniqueInfo
	{
		/// <include file='SolvingDocComments.xml' path='comments/constructor[@type="TechniqueInfo"]'/>
		/// <param name="pattern">The pattern.</param>
		/// <param name="candidate">The candidate.</param>
		public QdpType1TechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views, Pattern pattern, int candidate)
			: base(conclusions, views, pattern) => Candidate = candidate;


		/// <summary>
		/// Indicates the candidate.
		/// </summary>
		public int Candidate { get; }

		/// <inheritdoc/>
		public override TechniqueCode TechniqueCode => TechniqueCode.QdpType1;


		/// <inheritdoc/>
		public override string ToString()
		{
			string candStr = new CandidateCollection(Candidate).ToString();
			string patternStr = new CellCollection(Pattern.FullMap).ToString();
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			return
				$"{Name}: Cells will be a deadly pattern {patternStr} " +
				$"if {candStr} is false => {elimStr}";
		}
	}
}
