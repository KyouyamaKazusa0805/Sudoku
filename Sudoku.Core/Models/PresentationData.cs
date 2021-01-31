using System;
using System.Collections.Generic;
using Sudoku.Data;

namespace Sudoku.Models
{
	/// <summary>
	/// This is a data structure that stores the presentation data when drawing onto a picture.
	/// </summary>
	/// <param name="Cells">The cell information.</param>
	/// <param name="Candidates">The candidate information.</param>
	/// <param name="Regions">The region information.</param>
	/// <param name="Links">The link information.</param>
	/// <param name="DirectLines">The direct line information.</param>
	public sealed record PresentationData(
		ICollection<DrawingInfo> Cells, ICollection<DrawingInfo> Candidates, ICollection<DrawingInfo> Regions,
		ICollection<Link> Links, ICollection<(Cells Start, Cells End)> DirectLines)
	{
		/// <summary>
		/// Indicates the event triggered when the cell list is changed.
		/// </summary>
		public event PresentationDataChangedEventHandler<DrawingInfo>? CellsChanged;

		/// <summary>
		/// Indicates the event triggered when the candidate list is changed.
		/// </summary>
		public event PresentationDataChangedEventHandler<DrawingInfo>? CandidatesChanged;

		/// <summary>
		/// Indicates the event triggered when the region list is changed.
		/// </summary>
		public event PresentationDataChangedEventHandler<DrawingInfo>? RegionsChanged;

		/// <summary>
		/// Indicates the event triggered when the link list is changed.
		/// </summary>
		public event PresentationDataChangedEventHandler<Link>? LinksChanged;

		/// <summary>
		/// Indicates the event triggered when the direct line list is changed.
		/// </summary>
		public event PresentationDataChangedEventHandler<(Cells Start, Cells End)>? DirectLinesChanged;


		/// <summary>
		/// Add a new instance into the collection.
		/// </summary>
		/// <typeparam name="T">The type of the value to add into.</typeparam>
		/// <param name="propertyName">The property name.</param>
		/// <param name="value">The value to add into.</param>
		public bool Add<T>(string propertyName, in T value) where T : struct
		{
			switch (propertyName)
			{
				case nameof(Cells) when value is DrawingInfo i:
				{
					Cells.Add(i);
					CellsChanged?.Invoke(Cells);
					return true;
				}
				case nameof(Candidates) when value is DrawingInfo i:
				{
					Candidates.Add(i);
					CandidatesChanged?.Invoke(Candidates);
					return true;
				}
				case nameof(Regions) when value is DrawingInfo i:
				{
					Regions.Add(i);
					RegionsChanged?.Invoke(Regions);
					return true;
				}
				case nameof(Links) when value is Link i:
				{
					Links.Add(i);
					LinksChanged?.Invoke(Links);
					return true;
				}
				case nameof(DirectLines) when value is ValueTuple<Cells, Cells> i:
				{
					DirectLines.Add(i);
					DirectLinesChanged?.Invoke(DirectLines);
					return true;
				}
				default:
				{
					return false;
				}
			}
		}

		/// <summary>
		/// Remove a new instance from the collection.
		/// </summary>
		/// <typeparam name="T">The type of the value to remove.</typeparam>
		/// <param name="propertyName">The property name.</param>
		/// <param name="value">The value to remove.</param>
		public bool Remove<T>(string propertyName, in T value) where T : struct
		{
			switch (propertyName)
			{
				case nameof(Cells) when value is DrawingInfo i:
				{
					Cells.Remove(i);
					CellsChanged?.Invoke(Cells);
					return true;
				}
				case nameof(Candidates) when value is DrawingInfo i:
				{
					Candidates.Remove(i);
					CandidatesChanged?.Invoke(Candidates);
					return true;
				}
				case nameof(Regions) when value is DrawingInfo i:
				{
					Regions.Remove(i);
					RegionsChanged?.Invoke(Regions);
					return true;
				}
				case nameof(Links) when value is Link i:
				{
					Links.Remove(i);
					LinksChanged?.Invoke(Links);
					return true;
				}
				case nameof(DirectLines) when value is ValueTuple<Cells, Cells> i:
				{
					DirectLines.Remove(i);
					DirectLinesChanged?.Invoke(DirectLines);
					return true;
				}
				default:
				{
					return false;
				}
			}
		}
	}
}
