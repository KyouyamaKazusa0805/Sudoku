using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Drawing;

namespace Sudoku.Solving.Manual.Wings
{
	/// <summary>
	/// Provides a usage of <b>wing</b> technique.
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
