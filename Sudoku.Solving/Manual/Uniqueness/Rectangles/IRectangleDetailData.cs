using System.Collections.Generic;

namespace Sudoku.Solving.Manual.Uniqueness.Rectangles
{
	/// <summary>
	/// Provides a rectangle detail data model.
	/// </summary>
	public interface IRectangleDetailData
	{
		/// <summary>
		/// Indicates the type of the rectangle.
		/// </summary>
		int Type { get; }

		/// <summary>
		/// Indicates all cells that forms a UR or an AR.
		/// </summary>
		IReadOnlyList<int> Cells { get; }

		/// <summary>
		/// Indicates all digits that forms a UR or an AR.
		/// </summary>
		IReadOnlyList<int> Digits { get; }


		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		/// <returns>A string that represents the current object.</returns>
		string ToString();
	}
}
