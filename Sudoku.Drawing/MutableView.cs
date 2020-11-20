using System.Collections.Generic;
using System.Linq;
using Sudoku.Data;
using Sudoku.DocComments;
using Sudoku.Extensions;

namespace Sudoku.Drawing
{
	/// <summary>
	/// Encapsulates a view when displaying the information on forms.
	/// Different with <see cref="View"/>, this data structure can add and remove the items
	/// in the current collection.
	/// </summary>
	/// <param name="Cells">All cells used.</param>
	/// <param name="Candidates">All candidates used.</param>
	/// <param name="Regions">All regions used.</param>
	/// <param name="Links">All links used.</param>
	/// <param name="DirectLines">All direct lines.</param>
	/// <seealso cref="View"/>
	public sealed record MutableView(
		ICollection<DrawingInfo>? Cells, ICollection<DrawingInfo>? Candidates,
		ICollection<DrawingInfo>? Regions, ICollection<Link>? Links,
		ICollection<(GridMap Start, GridMap End)>? DirectLines)
	{
		/// <inheritdoc cref="DefaultConstructor"/>
		public MutableView()
			: this(
				new List<DrawingInfo>(), new List<DrawingInfo>(), new List<DrawingInfo>(), new List<Link>(),
				new List<(GridMap, GridMap)>())
		{
		}


		/// <summary>
		/// Add a cell into the list.
		/// </summary>
		/// <param name="id">The color ID.</param>
		/// <param name="cell">The cell.</param>
		public void AddCell(long id, int cell) => Cells?.Add(new(id, cell));

		/// <summary>
		/// Add a candidate into the list.
		/// </summary>
		/// <param name="id">The color ID.</param>
		/// <param name="candidate">The cell.</param>
		public void AddCandidate(long id, int candidate) => Candidates?.Add(new(id, candidate));

		/// <summary>
		/// Add a region into the list.
		/// </summary>
		/// <param name="id">The color ID.</param>
		/// <param name="region">The region.</param>
		public void AddRegion(long id, int region) => Regions?.Add(new(id, region));

		/// <summary>
		/// Add a link into the list.
		/// </summary>
		/// <param name="inference">The link.</param>
		public void AddLink(in Link inference) => Links?.Add(inference);

		/// <summary>
		/// Add a direct link into the list.
		/// </summary>
		/// <param name="start">(<see langword="in"/> parameter) The start map.</param>
		/// <param name="end">(<see langword="in"/> parameter) The end map.</param>
		public void AddDirectLine(in GridMap start, in GridMap end) => DirectLines?.Add((start, end));

		/// <summary>
		/// Remove the cell from the list.
		/// </summary>
		/// <param name="cell">The cell.</param>
		public void RemoveCell(int cell) => (Cells as List<DrawingInfo>)?.RemoveAll(p => p.Value == cell);

		/// <summary>
		/// Remove the candidate from the list.
		/// </summary>
		/// <param name="candidate">The candidate.</param>
		public void RemoveCandidate(int candidate) =>
			(Candidates as List<DrawingInfo>)?.RemoveAll(p => p.Value == candidate);

		/// <summary>
		/// Remove the region from the list.
		/// </summary>
		/// <param name="region">The region.</param>
		public void RemoveRegion(int region) =>
			(Regions as List<DrawingInfo>)?.RemoveAll(p => p.Value == region);

		/// <summary>
		/// Remove the link from the list.
		/// </summary>
		/// <param name="link">(<see langword="in"/> parameter) The link.</param>
		public void RemoveLink(in Link link) => Links?.Remove(link);

		/// <summary>
		/// Remove the direct link from the list.
		/// </summary>
		/// <param name="start">(<see langword="in"/> parameter) The start map.</param>
		/// <param name="end">(<see langword="in"/> parameter) The end map.</param>
		public void RemoveDirectLine(in GridMap start, in GridMap end) => DirectLines?.Remove((start, end));

		/// <summary>
		/// Clear all elements.
		/// </summary>
		public void Clear()
		{
			Cells?.Clear();
			Candidates?.Clear();
			Regions?.Clear();
			Links?.Clear();
			DirectLines?.Clear();
		}

		/// <summary>
		/// Indicates whether the list contains the specified cell.
		/// </summary>
		/// <param name="cell">The cell.</param>
		/// <returns>A <see cref="bool"/> value.</returns>
		public bool ContainsCell(int cell)
		{
			unsafe
			{
				static bool internalChecking(DrawingInfo p, in int cell) => p.Value == cell;
				return Cells?.Any(&internalChecking, cell) ?? false;
			}
		}

		/// <summary>
		/// Indicates whether the list contains the specified candidate.
		/// </summary>
		/// <param name="candidate">The candidate.</param>
		/// <returns>A <see cref="bool"/> value.</returns>
		public bool ContainsCandidate(int candidate)
		{
			unsafe
			{
				static bool internalChecking(DrawingInfo p, in int candidate) => p.Value == candidate;
				return Candidates?.Any(&internalChecking, candidate) ?? false;
			}
		}

		/// <summary>
		/// Indicates whether the list contains the specified region.
		/// </summary>
		/// <param name="region">The region.</param>
		/// <returns>A <see cref="bool"/> value.</returns>
		public bool ContainsRegion(int region)
		{
			unsafe
			{
				static bool internalChecking(DrawingInfo p, in int region) => p.Value == region;
				return Regions?.Any(&internalChecking, region) ?? false;
			}
		}

		/// <summary>
		/// Indicates whether the list contains the specified link.
		/// </summary>
		/// <param name="inference">(<see langword="in"/> parameter) The link.</param>
		/// <returns>A <see cref="bool"/> value.</returns>
		public bool ContainsLink(in Link inference) => Links?.Contains(inference) ?? false;

		/// <summary>
		/// Indicates whether the list contains the specified direct line.
		/// </summary>
		/// <param name="start">(<see langword="in"/> parameter) The start map.</param>
		/// <param name="end">(<see langword="in"/> parameter) The end map.</param>
		/// <returns>A <see cref="bool"/> value.</returns>
		public bool ContainsDirectLine(in GridMap start, in GridMap end) =>
			DirectLines?.Contains((start, end)) ?? false;
	}
}
