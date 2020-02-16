using System.Collections.Generic;
using Sudoku.Drawing;

namespace Sudoku.Solving.Manual.Uniqueness.Rectangles
{
	/// <summary>
	/// Provides a usage of rectangle technique.
	/// </summary>
	public abstract class RectangleTechniqueInfo : UniquenessTechniqueInfo
	{
		/// <summary>
		/// Provides passing data when initializing an instance of derived types.
		/// </summary>
		/// <param name="conclusions">The conclusions.</param>
		/// <param name="views">The views of this solving step.</param>
		/// <param name="detailData">The detail data.</param>
		protected RectangleTechniqueInfo(
			IReadOnlyList<Conclusion> conclusions, IReadOnlyList<View> views,
			IRectangleDetailData detailData)
			: base(conclusions, views) => DetailData = detailData;


		/// <summary>
		/// The detail data of the technique.
		/// </summary>
		public IRectangleDetailData DetailData { get; }


		/// <inheritdoc/>
		public abstract override string ToString();
	}
}
