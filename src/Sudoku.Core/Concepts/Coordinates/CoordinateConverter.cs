namespace Sudoku.Concepts.Coordinates;

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
/// <param name="CurrentCulture">
/// <para>Indicates the current culture.</para>
/// <para>The value is <see langword="null"/> by default, meaning the converter uses invariant culture to output some string text.</para>
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
public abstract record CoordinateConverter(string DefaultSeparator = ", ", string? DigitsSeparator = null, CultureInfo? CurrentCulture = null) : IFormatProvider
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
	public abstract Func<HouseMask, string> HouseConverter { get; }

	/// <summary>
	/// The converter method that creates a <see cref="string"/> via the specified list of conclusions.
	/// </summary>
	public abstract Func<ReadOnlySpan<Conclusion>, string> ConclusionConverter { get; }

	/// <summary>
	/// The converter method that creates a <see cref="string"/> via the specified list of digits.
	/// </summary>
	public abstract Func<Mask, string> DigitConverter { get; }

	/// <summary>
	/// The converter method that creates a <see cref="string"/> via the specified information for an intersection.
	/// </summary>
	public abstract Func<ReadOnlySpan<Miniline>, string> IntersectionConverter { get; }

	/// <summary>
	/// The converter method that creates a <see cref="string"/> via the specified list of chute.
	/// </summary>
	public abstract Func<ReadOnlySpan<Chute>, string> ChuteConverter { get; }

	/// <summary>
	/// The converter method that creates a <see cref="string"/> via the specified conjugate.
	/// </summary>
	public abstract Func<ReadOnlySpan<Conjugate>, string> ConjugateConverter { get; }

	/// <summary>
	/// Indicates the target culture.
	/// </summary>
	private protected CultureInfo TargetCurrentCulture => CurrentCulture ?? CultureInfo.CurrentUICulture;


	/// <summary>
	/// Indicates the <see cref="CoordinateConverter"/> instance for the invariant culture,
	/// meaning it ignores which culture your device will use.
	/// </summary>
	public static CoordinateConverter InvariantCultureConverter => new RxCyConverter();


	/// <inheritdoc/>
	[return: NotNullIfNotNull(nameof(formatType))]
	public abstract object? GetFormat(Type? formatType);


	/// <summary>
	/// Try to get a <see cref="CoordinateConverter"/> instance from the specified format provider.
	/// </summary>
	/// <param name="formatProvider">The format provider instance.</param>
	/// <returns>A <see cref="CoordinateConverter"/> instance as the final result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CoordinateConverter GetConverter(IFormatProvider? formatProvider)
		=> formatProvider switch
		{
			CultureInfo c => c switch
			{
				{ Name: var name } when name.StartsWith("zh", StringComparison.OrdinalIgnoreCase)
					=> new K9Converter(true, CurrentCulture: c),
				_
					=> new RxCyConverter(true, true, CurrentCulture: c)
			},
			CoordinateConverter c => c,
			_ => InvariantCultureConverter
		};
}
