using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Sudoku.Data;
using Sudoku.DocComments;

namespace Sudoku.Drawing
{
	/// <summary>
	/// Encapsulates a view when displaying the information on forms.
	/// Different with <see cref="View"/>, this data structure can add and remove the items
	/// in the current collection.
	/// </summary>
	/// <param name="CellOffsets">All cells used.</param>
	/// <param name="CandidateOffsets">All candidates used.</param>
	/// <param name="RegionOffsets">All regions used.</param>
	/// <param name="Links">All links used.</param>
	/// <seealso cref="View"/>
	[DebuggerStepThrough]
	public sealed record MutableView(
		ICollection<DrawingInfo>? CellOffsets, ICollection<DrawingInfo>? CandidateOffsets,
		ICollection<DrawingInfo>? RegionOffsets, ICollection<Link>? Links)
	{
		/// <inheritdoc cref="DefaultConstructor"/>
		/// <remarks>
		/// The constructor is equivalent to code '<c>new MutableView(null, null, null, null)</c>'.
		/// </remarks>
		public MutableView()
			: this(new List<DrawingInfo>(), new List<DrawingInfo>(), new List<DrawingInfo>(), new List<Link>())
		{
		}


		/// <summary>
		/// Add the cell into the list.
		/// </summary>
		/// <param name="id">The color ID.</param>
		/// <param name="cell">The cell.</param>
		public void AddCell(int id, int cell) => CellOffsets?.Add(new(id, cell));

		/// <summary>
		/// Add the candidate into the list.
		/// </summary>
		/// <param name="id">The color ID.</param>
		/// <param name="candidate">The cell.</param>
		public void AddCandidate(int id, int candidate) => CandidateOffsets?.Add(new(id, candidate));

		/// <summary>
		/// Add the region into the list.
		/// </summary>
		/// <param name="id">The color ID.</param>
		/// <param name="region">The region.</param>
		public void AddRegion(int id, int region) => RegionOffsets?.Add(new(id, region));

		/// <summary>
		/// Add the link into the list.
		/// </summary>
		/// <param name="inference">The link.</param>
		public void AddLink(Link inference) => Links?.Add(inference);

		/// <summary>
		/// Remove the cell from the list.
		/// </summary>
		/// <param name="cell">The cell.</param>
		public void RemoveCell(int cell) => (CellOffsets as List<DrawingInfo>)?.RemoveAll(p => p.Value == cell);

		/// <summary>
		/// Remove the candidate from the list.
		/// </summary>
		/// <param name="candidate">The candidate.</param>
		public void RemoveCandidate(int candidate) =>
			(CandidateOffsets as List<DrawingInfo>)?.RemoveAll(p => p.Value == candidate);

		/// <summary>
		/// Remove the region from the list.
		/// </summary>
		/// <param name="region">The region.</param>
		public void RemoveRegion(int region) =>
			(RegionOffsets as List<DrawingInfo>)?.RemoveAll(p => p.Value == region);

		/// <summary>
		/// Remove the link from the list.
		/// </summary>
		/// <param name="link">The link.</param>
		public void RemoveLink(Link link) => Links?.Remove(link);

		/// <summary>
		/// Clear all elements.
		/// </summary>
		public void Clear()
		{
			CellOffsets?.Clear();
			CandidateOffsets?.Clear();
			RegionOffsets?.Clear();
			Links?.Clear();
		}

		/// <summary>
		/// Indicates whether the specified list contains the cell.
		/// </summary>
		/// <param name="cell">The cell.</param>
		/// <returns>A <see cref="bool"/> value.</returns>
		public bool ContainsCell(int cell) => CellOffsets?.Any(p => p.Value == cell) ?? false;

		/// <summary>
		/// Indicates whether the specified list contains the candidate.
		/// </summary>
		/// <param name="candidate">The candidate.</param>
		/// <returns>A <see cref="bool"/> value.</returns>
		public bool ContainsCandidate(int candidate) => CandidateOffsets?.Any(p => p.Value == candidate) ?? false;

		/// <summary>
		/// Indicates whether the specified list contains the region.
		/// </summary>
		/// <param name="region">The region.</param>
		/// <returns>A <see cref="bool"/> value.</returns>
		public bool ContainsRegion(int region) => RegionOffsets?.Any(p => p.Value == region) ?? false;

		/// <summary>
		/// Indicates whether the specified list contains the link.
		/// </summary>
		/// <param name="inference">The link.</param>
		/// <returns>A <see cref="bool"/> value.</returns>
		public bool ContainsLink(Link inference) => Links?.Contains(inference) ?? false;
	}
}
