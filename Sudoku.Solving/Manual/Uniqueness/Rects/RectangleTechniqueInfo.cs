using System.Collections.Generic;
using Sudoku.Drawing;

namespace Sudoku.Solving.Manual.Uniqueness.Rects
{
	/// <summary>
	/// Provides a usage of <b>rectangle</b> technique.
	/// </summary>
	public abstract class RectangleTechniqueInfo : UniquenessTechniqueInfo
	{
		/// <inheritdoc/>
		public RectangleTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views)
			: base(conclusions, views)
		{
		}
	}
}
