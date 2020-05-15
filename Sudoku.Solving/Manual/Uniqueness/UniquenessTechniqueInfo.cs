using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Drawing;

namespace Sudoku.Solving.Manual.Uniqueness
{
	/// <summary>
	/// Provides a usage of <b>uniqueness</b> technique.
	/// </summary>
	public abstract class UniquenessTechniqueInfo : TechniqueInfo
	{
		/// <inheritdoc/>
		protected UniquenessTechniqueInfo(IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views)
			: base(conclusions, views)
		{
		}
	}
}
