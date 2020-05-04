using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Drawing;

namespace Sudoku.Solving.Manual.Alses
{
	/// <summary>
	/// Provides a usage of <b>almost locked set</b> (ALS) technique.
	/// </summary>
	public abstract class AlsTechniqueInfo : TechniqueInfo
	{
		/// <inheritdoc/>
		protected AlsTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views)
			: base(conclusions, views)
		{
		}
	}
}
