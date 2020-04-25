using System.Collections.Generic;
using Sudoku.Drawing;

namespace Sudoku.Solving.Manual.Alses.Mslses
{
	/// <summary>
	/// Provides a usage of <b>multi-sector locked set</b> (MSLS) technique.
	/// </summary>
	public abstract class MslsTechniqueInfo : AlsTechniqueInfo
	{
		/// <inheritdoc/>
		protected MslsTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views)
			: base(conclusions, views)
		{
		}
	}
}
