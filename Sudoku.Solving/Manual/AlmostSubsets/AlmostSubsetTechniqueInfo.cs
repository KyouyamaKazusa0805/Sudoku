using System.Collections.Generic;
using Sudoku.Drawing;

namespace Sudoku.Solving.Manual.AlmostSubsets
{
	/// <summary>
	/// Provides a usage of almost subset (ALS) technique.
	/// </summary>
	public abstract class AlmostSubsetTechniqueInfo : TechniqueInfo
	{
		/// <inheritdoc/>
		protected AlmostSubsetTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views)
			: base(conclusions, views)
		{
		}
	}
}
