using System.Collections.Generic;
using Sudoku.Drawing;

namespace Sudoku.Solving.Manual.Uniqueness
{
	/// <summary>
	/// Provides a usage of uniqueness technique.
	/// </summary>
	public abstract class UniquenessTechniqueInfo : TechniqueInfo
	{
		/// <inheritdoc/>
		protected UniquenessTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views)
			: base(conclusions, views)
		{
		}
	}
}
