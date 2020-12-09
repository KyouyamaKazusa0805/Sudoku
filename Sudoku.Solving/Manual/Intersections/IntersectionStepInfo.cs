using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Drawing;

namespace Sudoku.Solving.Manual.Intersections
{
	/// <summary>
	/// Provides a usage of <b>intersection</b> technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	public abstract record IntersectionStepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views) : StepInfo(Conclusions, Views);
}
