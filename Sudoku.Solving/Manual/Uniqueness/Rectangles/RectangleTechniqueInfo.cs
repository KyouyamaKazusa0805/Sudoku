using System.Collections.Generic;
using Sudoku.Drawing;

namespace Sudoku.Solving.Manual.Uniqueness.Rectangles
{
	/// <summary>
	/// Provides a usage of rectangle technique.
	/// </summary>
	public abstract class RectangleTechniqueInfo : UniquenessTechniqueInfo
	{
		/// <inheritdoc/>
		protected RectangleTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views)
			: base(conclusions, views)
		{
		}


		/// <summary>
		/// The detail data of the technique.
		/// </summary>
		public abstract IRectangleDetailData DetailData { get; }


		/// <inheritdoc/>
		public abstract override string ToString();
	}
}
