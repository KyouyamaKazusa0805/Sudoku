using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Drawing;

namespace Sudoku.Solving.Manual.Wings.Irregular
{
	/// <summary>
	/// Encapsulates a usage of <b>irregular wing</b> technique.
	/// </summary>
	public abstract class IrregularWingTechniqueInfo : TechniqueInfo
	{
		/// <inheritdoc/>
		protected IrregularWingTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views)
			: base(conclusions, views)
		{
		}
	}
}
