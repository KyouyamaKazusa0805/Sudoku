namespace Sudoku.Presentation;

/// <summary>
/// Presents a kind to identify the inner presentation data.
/// </summary>
public enum PresentationDataKind : byte
{
	/// <summary>
	/// Indicates the data kind is none.
	/// </summary>
	None,

	/// <summary>
	/// Indicates the data kind is for cells.
	/// </summary>
	Cells,

	/// <summary>
	/// Indicates the data kind is for candidates.
	/// </summary>
	Candidates,

	/// <summary>
	/// Indicates the data kind is for regions.
	/// </summary>
	Regions,

	/// <summary>
	/// Indicates the data kind is for links.
	/// </summary>
	Links,

	/// <summary>
	/// Indicates the data kind is for direct lines.
	/// </summary>
	DirectLines,

	/// <summary>
	/// Indicates the data kind is for unknown value.
	/// </summary>
	UnknownValue
}
