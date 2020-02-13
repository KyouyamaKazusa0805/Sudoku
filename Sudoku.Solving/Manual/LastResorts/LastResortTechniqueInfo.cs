using System.Collections.Generic;
using Sudoku.Drawing;

namespace Sudoku.Solving.Manual.LastResorts
{
	/// <summary>
	/// Provides a usage of last resort.
	/// </summary>
	public abstract class LastResortTechniqueInfo : TechniqueInfo
	{
		/// <inheritdoc/>
		protected LastResortTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views)
			: base(conclusions, views)
		{
		}
	}
}
