using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;

namespace Sudoku.Solving.Manual.Uniqueness.Qiu
{
	/// <summary>
	/// Provides a usage of <b>Qiu's deadly pattern locked type</b> (QDP) technique.
	/// </summary>
	public sealed class QdpLockedTypeTechniqueInfo : QdpTechniqueInfo
	{
		/// <include file='SolvingDocComments.xml' path='comments/constructor[@type="TechniqueInfo"]'/>
		/// <param name="pattern">The pattern.</param>
		/// <param name="candidates">The candidates.</param>
		public QdpLockedTypeTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views, Pattern pattern,
			IReadOnlyList<int> candidates) : base(conclusions, views, pattern) => Candidates = candidates;


		/// <summary>
		/// Indicates the candidates.
		/// </summary>
		public IReadOnlyList<int> Candidates { get; }

		/// <inheritdoc/>
		public override decimal Difficulty => base.Difficulty + .2M;

		/// <inheritdoc/>
		public override TechniqueCode TechniqueCode => TechniqueCode.LockedQdp;


		/// <inheritdoc/>
		public override string ToString()
		{
			string patternStr = new CellCollection(Pattern.FullMap).ToString();
			string candStr = new CandidateCollection(Candidates).ToString();
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			string quantifier = Candidates.Count switch { 1 => string.Empty, 2 => " both", _ => " all" };
			string number = Candidates.Count == 1 ? "the" : $"{Candidates.Count}";
			string singularOrPlural = Candidates.Count == 1 ? " candidate" : " candidates";
			string beVerb = Candidates.Count == 1 ? "is" : "are";
			return
				$"{Name}: Cells {patternStr} will be a deadly pattern " +
				$"if{quantifier}{number} {singularOrPlural} {candStr} {beVerb} false => {elimStr}";
		}
	}
}
