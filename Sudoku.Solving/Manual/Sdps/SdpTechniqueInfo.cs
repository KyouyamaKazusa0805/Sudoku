using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Drawing;

namespace Sudoku.Solving.Manual.Sdps
{
	/// <summary>
	/// Provides a usage of <b>single-digit pattern</b> (SDP) technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Digit">The digit used.</param>
	public abstract record SdpTechniqueInfo(IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views, int Digit)
		: TechniqueInfo(Conclusions, Views);
}
