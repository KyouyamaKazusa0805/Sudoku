using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Drawing;

namespace Sudoku.Solving.Manual.Uniqueness
{
	/// <summary>
	/// Provides a usage of <b>uniqueness</b> technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	public abstract record UniquenessStepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views) : StepInfo(Conclusions, Views);
}
