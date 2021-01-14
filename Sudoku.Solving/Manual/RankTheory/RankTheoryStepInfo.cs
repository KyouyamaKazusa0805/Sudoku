using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Drawing;

namespace Sudoku.Solving.Manual.RankTheory
{
	/// <summary>
	/// Provides a usage of <b>rank theory</b> technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	public abstract record RankTheoryStepInfo(IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views)
		: StepInfo(Conclusions, Views);
}
