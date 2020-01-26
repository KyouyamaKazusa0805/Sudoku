using System.Collections.Generic;
using Sudoku.Drawing;

namespace Sudoku.Solving.Manual.Wings
{
	/// <summary>
	/// Provides a usage of wing technique.
	/// </summary>
	public abstract class WingTechniqueInfo : TechniqueInfo
	{
		/// <inheritdoc/>
		protected WingTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views)
			: base(conclusions, views)
		{
		}
	}
}
