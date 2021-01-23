using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Drawing;
using Sudoku.Resources;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.Uniqueness.Bugs
{
	/// <summary>
	/// Provides a usage of <b>BUG + n</b> technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Candidates">All candidates used.</param>
	public sealed record BugMultipleStepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views, IReadOnlyList<int> Candidates)
		: BugStepInfo(Conclusions, Views)
	{
		/// <summary>
		/// The table of extra difficulty values.
		/// </summary>
		private static readonly decimal[] ExtraDifficulty =
		{
			.1M, .2M, .2M, .3M, .3M, .3M, .4M, .4M, .4M, .4M,
			.5M, .5M, .5M, .5M, .5M, .6M, .6M, .6M
		};


		/// <inheritdoc/>
		public override string Name => $"{TextResources.GetValue("Bug")} + {Candidates.Count.ToString()}";

		/// <inheritdoc/>
		public override decimal Difficulty => base.Difficulty + .1M + ExtraDifficulty[Candidates.Count - 1];

		/// <inheritdoc/>
		public override Technique TechniqueCode => Technique.BugMultiple;


		/// <inheritdoc/>
		public override string ToString()
		{
			string candsStr = new Cells(Candidates).ToString();
			string elimStr = new ConclusionCollection(Conclusions).ToString();
			return $"{Name}: True candidates: {candsStr} => {elimStr}";
		}
	}
}
