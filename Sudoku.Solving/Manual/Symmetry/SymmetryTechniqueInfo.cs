using System.Collections.Generic;
using Sudoku.Drawing;

namespace Sudoku.Solving.Manual.Symmetry
{
	/// <summary>
	/// Provides a usage of symmetry technique.
	/// </summary>
	public abstract class SymmetryTechniqueInfo : TechniqueInfo
	{
		/// <inheritdoc/>
		protected SymmetryTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views)
			: base(conclusions, views)
		{
		}
	}
}
