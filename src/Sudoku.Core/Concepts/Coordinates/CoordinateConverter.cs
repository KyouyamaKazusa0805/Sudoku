namespace Sudoku.Concepts.Coordinates;

/// <summary>
/// Represents an option provider for coordinates.
/// </summary>
/// <param name="DefaultSeparator"><inheritdoc/></param>
/// <param name="DigitsSeparator"><inheritdoc/></param>
/// <param name="CurrentCulture"><inheritdoc/></param>
/// <remarks>
/// You can use types <see cref="RxCyConverter"/>, <seealso cref="K9Converter"/>, <see cref="LiteralCoordinateConverter"/>
/// and <see cref="ExcelCoordinateConverter"/>.
/// They are the derived types of the current type.
/// </remarks>
/// <seealso cref="RxCyConverter"/>
/// <seealso cref="K9Converter"/>
/// <seealso cref="LiteralCoordinateConverter"/>
/// <seealso cref="ExcelCoordinateConverter"/>
public abstract record CoordinateConverter(string DefaultSeparator = ", ", string? DigitsSeparator = null, CultureInfo? CurrentCulture = null) :
	GenericConceptConverter(DefaultSeparator, DigitsSeparator, CurrentCulture)
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

	/// <summary>
	/// Indicates the target culture.
	/// </summary>
	private protected CultureInfo TargetCurrentCulture => CurrentCulture ?? CultureInfo.CurrentUICulture;


	/// <summary>
	/// Indicates the <see cref="CoordinateConverter"/> instance for the invariant culture,
	/// meaning it ignores which culture your device will use.
	/// </summary>
	public static CoordinateConverter InvariantCultureConverter => new RxCyConverter();


	/// <summary>
	/// Try to get a <see cref="CoordinateConverter"/> instance from the specified culture.
	/// </summary>
	/// <param name="culture">The culture.</param>
	/// <returns>The <see cref="CoordinateConverter"/> instance from the specified culture.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CoordinateConverter GetConverter(CultureInfo culture)
		=> culture switch
		{
			{ Name: ['Z' or 'z', 'H' or 'h', ..] } => new K9Converter(true, CurrentCulture: culture),
			_ => new RxCyConverter(true, true, CurrentCulture: culture)
		};
}
