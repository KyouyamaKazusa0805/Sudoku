using System.Collections.Generic;
using Sudoku.Drawing;

namespace Sudoku.Solving.Manual.Intersections
{
	/// <summary>
	/// Provides a usage of intersection technique.
	/// </summary>
	public abstract class IntersectionTechniqueInfo : TechniqueInfo
	{
		/// <inheritdoc/>
		protected IntersectionTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views)
			: base(conclusions, views)
		{
		}
	}
}
