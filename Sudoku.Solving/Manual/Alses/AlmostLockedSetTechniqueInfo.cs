using System.Collections.Generic;
using Sudoku.Drawing;

namespace Sudoku.Solving.Manual.Alses
{
	/// <summary>
	/// Provides a usage of almost locked set (ALS) technique.
	/// </summary>
	public abstract class AlmostLockedSetTechniqueInfo : TechniqueInfo
	{
		/// <inheritdoc/>
		protected AlmostLockedSetTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views)
			: base(conclusions, views)
		{
		}
	}
}
