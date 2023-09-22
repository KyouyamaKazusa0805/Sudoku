namespace Sudoku.Text.Coordinate;

/// <summary>
/// Represents an option provider for coordinates.
/// </summary>
/// <param name="DefaultSeparator">
/// <para>Indicates the default separator. The value will be inserted into two non-digit-kind instances.</para>
/// <para>The value is <c>", "</c> by default.</para>
/// </param>
/// <param name="DigitsSeparator">
/// <para>Indicates the digits separator.</para>
/// <para>The value is <see langword="null"/> by default, meaning no separators will be inserted between 2 digits.</para>
/// </param>
/// <remarks>
/// You can use types <see cref="RxCyConverter"/>, <seealso cref="K9Converter"/>, <see cref="LiteralCoordinateConverter"/>
/// and <see cref="ExcelCoordinateConverter"/>.
/// They are the derived types of the current type.
/// </remarks>
/// <seealso cref="RxCyConverter"/>
/// <seealso cref="K9Converter"/>
/// <seealso cref="LiteralCoordinateConverter"/>
/// <seealso cref="ExcelCoordinateConverter"/>
public abstract record CoordinateConverter(string DefaultSeparator = ", ", string? DigitsSeparator = null)
{
	/// <summary>
	/// The converter method that creates a <see cref="string"/> via the specified list of cells.
	/// </summary>
	public abstract CellNotationConverter CellConverter { get; }

	/// <summary>
	/// The converter method that creates a <see cref="string"/> via the specified list of candidates.
	/// </summary>
	public abstract CandidateNotationConverter CandidateConverter { get; }

	/// <summary>
	/// The converter method that creates a <see cref="string"/> via the specified list of houses.
	/// </summary>
	public abstract HouseNotationConverter HouseConverter { get; }

	/// <summary>
	/// The converter method that creates a <see cref="string"/> via the specified list of conclusions.
	/// </summary>
	public abstract ConclusionNotationConverter ConclusionConverter { get; }

	/// <summary>
	/// The converter method that creates a <see cref="string"/> via the specified list of digits.
	/// </summary>
	public abstract DigitNotationConverter DigitConverter { get; }

	/// <summary>
	/// The converter method that creates a <see cref="string"/> via the specified information for an intersection.
	/// </summary>
	public abstract IntersectionNotationConverter IntersectionConverter { get; }

	/// <summary>
	/// The converter method that creates a <see cref="string"/> via the specified list of chute.
	/// </summary>
	public abstract ChuteNotationConverter ChuteConverter { get; }

	/// <summary>
	/// The converter method that creates a <see cref="string"/> via the specified conjugate.
	/// </summary>
	public abstract ConjugateNotationConverter ConjugateConverter { get; }
}
