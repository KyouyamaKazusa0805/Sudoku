using System.Collections.Generic;
using Sudoku.Data;

namespace Sudoku.Drawing
{
	/// <summary>
	/// Encapsulates a view when displaying the information on forms.
	/// </summary>
	public sealed record View
	{
		/// <summary>
		/// Indicates all cells used.
		/// </summary>
		public IReadOnlyList<DrawingInfo>? Cells { get; init; }

		/// <summary>
		/// Indicates all candidates used.
		/// </summary>
		public IReadOnlyList<DrawingInfo>? Candidates { get; init; }

		/// <summary>
		/// Indicates all regions used.
		/// </summary>
		public IReadOnlyList<DrawingInfo>? Regions { get; init; }

		/// <summary>
		/// Indicates all links used.
		/// </summary>
		public IReadOnlyList<Link>? Links { get; init; }

		/// <summary>
		/// Indicates all direct lines.
		/// </summary>
		public IReadOnlyList<(Cells Start, Cells End)>? DirectLines { get; init; }
	}
}
