using System;
using System.Collections.Generic;
using Sudoku.Data;

namespace Sudoku.Models
{
	/// <summary>
	/// This is a data structure that stores the presentation data when drawing onto a picture.
	/// </summary>
	public sealed class PresentationData
	{
		/// <summary>
		/// The cell information.
		/// </summary>
		public ICollection<DrawingInfo>? Cells { get; set; }

		/// <summary>
		/// The candidate information.
		/// </summary>
		public ICollection<DrawingInfo>? Candidates { get; set; }

		/// <summary>
		/// The region information.
		/// </summary>
		public ICollection<DrawingInfo>? Regions { get; set; }

		/// <summary>
		/// The link information.
		/// </summary>
		public ICollection<Link>? Links { get; set; }

		/// <summary>
		/// The direct line information.
		/// </summary>
		public ICollection<(Cells Start, Cells End)>? DirectLines { get; set; }


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
					(Cells ??= new List<DrawingInfo>()).Add(i);
					CellsChanged?.Invoke(Cells);
					return true;
				}
				case nameof(Candidates) when value is DrawingInfo i:
				{
					(Candidates ??= new List<DrawingInfo>()).Add(i);
					CandidatesChanged?.Invoke(Candidates);
					return true;
				}
				case nameof(Regions) when value is DrawingInfo i:
				{
					(Regions ??= new List<DrawingInfo>()).Add(i);
					RegionsChanged?.Invoke(Regions);
					return true;
				}
				case nameof(Links) when value is Link i:
				{
					(Links ??= new List<Link>()).Add(i);
					LinksChanged?.Invoke(Links);
					return true;
				}
				case nameof(DirectLines) when value is ValueTuple<Cells, Cells> i:
				{
					(DirectLines ??= new List<(Cells, Cells)>()).Add(i);
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
				case nameof(Cells) when value is DrawingInfo i && Cells is not null:
				{
					Cells.Remove(i);
					CellsChanged?.Invoke(Cells);
					return true;
				}
				case nameof(Candidates) when value is DrawingInfo i && Candidates is not null:
				{
					Candidates.Remove(i);
					CandidatesChanged?.Invoke(Candidates);
					return true;
				}
				case nameof(Regions) when value is DrawingInfo i && Regions is not null:
				{
					Regions.Remove(i);
					RegionsChanged?.Invoke(Regions);
					return true;
				}
				case nameof(Links) when value is Link i && Links is not null:
				{
					Links.Remove(i);
					LinksChanged?.Invoke(Links);
					return true;
				}
				case nameof(DirectLines) when value is ValueTuple<Cells, Cells> i && DirectLines is not null:
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
