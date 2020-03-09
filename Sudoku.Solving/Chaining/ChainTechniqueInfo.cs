using System.Collections.Generic;
using Sudoku.Drawing;

namespace Sudoku.Solving.Chaining
{
	/// <summary>
	/// Provides a usage of chain technique.
	/// </summary>
	public abstract class ChainTechniqueInfo : TechniqueInfo
	{
		/// <inheritdoc/>
		protected ChainTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views)
			: base(conclusions, views)
		{
		}
	}
}
