using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Drawing;

namespace Sudoku.Solving.Manual.LastResorts
{
	/// <summary>
	/// Provides a usage of <b>last resort</b> technique.
	/// </summary>
	public abstract class LastResortTechniqueInfo : TechniqueInfo
	{
		/// <inheritdoc/>
		protected LastResortTechniqueInfo(IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views)
			: base(conclusions, views)
		{
		}
	}
}
