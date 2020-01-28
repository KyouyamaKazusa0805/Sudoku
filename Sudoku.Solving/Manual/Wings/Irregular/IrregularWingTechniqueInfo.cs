using System.Collections.Generic;
using Sudoku.Drawing;

namespace Sudoku.Solving.Manual.Wings.Irregular
{
	/// <summary>
	/// Encapsulates a usage of irregular wing technique.
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
