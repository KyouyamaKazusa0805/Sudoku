using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Drawing;

namespace Sudoku.Solving.Manual.Alses.Mslses
{
	/// <summary>
	/// Provides a usage of <b>multi-sector locked set</b> (MSLS) technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	public abstract record MslsStepInfo(IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views)
		: AlsStepInfo(Conclusions, Views);
}
