namespace SudokuStudio.Interaction;

/// <summary>
/// Represents a format flag.
/// </summary>
[Flags]
internal enum SudokuFormatFlags
{
	/// <summary>
	/// Indicates the default format.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	None = 0,

	/// <summary>
	/// Indicates the initial grid format.
	/// </summary>
	InitialFormat = 1,

	/// <summary>
	/// Indicates the current grid format.
	/// </summary>
	CurrentFormat = 1 << 1,

	/// <summary>
	/// Indicates the current grid format , treating all modifiable values as given ones.
	/// </summary>
	CurrentFormatIgnoringValueKind = 1 << 2,

	/// <summary>
	/// Indicates the Hodoku grid format.
	/// </summary>
	HodokuCompatibleFormat = 1 << 3,

	/// <summary>
	/// Indicates the multiple-line grid format.
	/// </summary>
	MultipleGridFormat = 1 << 4,

	/// <summary>
	/// Indicates the pencilmark format.
	/// </summary>
	PencilMarkFormat = 1 << 5,

	/// <summary>
	/// Indicates the sukaku format.
	/// </summary>
	SukakuFormat = 1 << 6,

	/// <summary>
	/// Indicates the excel format.
	/// </summary>
	ExcelFormat = 1 << 7,

	/// <summary>
	/// Indicates the open-sudoku format.
	/// </summary>
	OpenSudokuFormat = 1 << 8
}
