namespace Sudoku.Concepts;

/// <summary>
/// Represents a type of notation to describe a coordinate in sudoku.
/// </summary>
public enum CoordinateType
{
	/// <summary>
	/// Idnicates the notation is based on literally notation.
	/// </summary>
	[CoordinateConverter<LiteralCoordinateConverter>]
	Literal,

	/// <summary>
	/// Indicates the notation is based on <b>RxCy</b> notation.
	/// </summary>
	[CoordinateConverter<RxCyConverter>]
	[CoordinateParser<RxCyParser>]
	RxCy,

	/// <summary>
	/// Indicates the notation is based on <b>K9</b> notation.
	/// </summary>
	[CoordinateConverter<K9Converter>]
	[CoordinateParser<K9Parser>]
	K9,

	/// <summary>
	/// Indicates the notation is based on <b>Excel</b> notation.
	/// </summary>
	[CoordinateConverter<ExcelCoordinateConverter>]
	Excel
}
