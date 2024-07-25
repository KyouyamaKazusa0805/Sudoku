namespace Sudoku.Runtime.CoordinateServices;

/// <summary>
/// Represents for a parser instance that parses a <see cref="string"/> text,
/// converting into a valid instance that can be represented as a sudoku concept.
/// </summary>
public abstract record CoordinateParser : IFormatProvider
{
	/// <summary>
	/// The not supported information for property implemented.
	/// </summary>
	protected const string DeprecatedInfo_NotSupported = "This property is not implemented due to the lack of K9 notation rules.";


	/// <summary>
	/// The parser method that can creates a <see cref="CellMap"/> via the specified text to be parsed.
	/// </summary>
	/// <seealso cref="CellMap"/>
	public abstract Func<string, CellMap> CellParser { get; }

	/// <summary>
	/// The parser method that can creates a <see cref="CandidateMap"/> via the specified text to be parsed.
	/// </summary>
	/// <seealso cref="CandidateMap"/>
	public abstract Func<string, CandidateMap> CandidateParser { get; }

	/// <summary>
	/// The parser method that can creates a <see cref="HouseMask"/> via the specified text to be parsed.
	/// </summary>
	/// <seealso cref="HouseMask"/>
	public abstract Func<string, HouseMask> HouseParser { get; }

	/// <summary>
	/// The parser method that can creates a <see cref="Conclusion"/> list via the specified text to be parsed.
	/// </summary>
	/// <seealso cref="Conclusion"/>
	public abstract Func<string, ConclusionSet> ConclusionParser { get; }

	/// <summary>
	/// The parser method that can creates a <see cref="Mask"/> via the specified text to be parsed.
	/// </summary>
	/// <seealso cref="Mask"/>
	public abstract Func<string, Mask> DigitParser { get; }

	/// <summary>
	/// The parser method that can creates a list of pairs of <see cref="MinilineBase"/> and <see cref="MinilineResult"/>
	/// via the specified text to be parsed.
	/// </summary>
	/// <seealso cref="MinilineBase"/>
	/// <seealso cref="MinilineResult"/>
	public abstract Func<string, ReadOnlySpan<Miniline>> IntersectionParser { get; }

	/// <summary>
	/// The parser method that can creates a <see cref="Chute"/> list via the specified text to be parsed.
	/// </summary>
	/// <seealso cref="Chute"/>
	public abstract Func<string, ReadOnlySpan<Chute>> ChuteParser { get; }

	/// <summary>
	/// The parser method that can creates a <see cref="Conjugate"/> list via the specified text to be parsed.
	/// </summary>
	/// <seealso cref="Conjugate"/>
	public abstract Func<string, ReadOnlySpan<Conjugate>> ConjugateParser { get; }


	/// <summary>
	/// Indicates the <see cref="CoordinateParser"/> instance for the invariant culture,
	/// meaning it ignores which culture your device will use.
	/// </summary>
	public static CoordinateParser InvariantCultureParser => new RxCyParser();


	/// <inheritdoc/>
	[return: NotNullIfNotNull(nameof(formatType))]
	public abstract object? GetFormat(Type? formatType);


	/// <summary>
	/// Try to get a <see cref="CoordinateParser"/> instance from the specified culture.
	/// </summary>
	/// <param name="formatProvider">The format provider instance.</param>
	/// <returns>The <see cref="CoordinateParser"/> instance from the specified culture.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CoordinateParser GetParser(IFormatProvider? formatProvider)
		=> formatProvider switch
		{
			CultureInfo { Name: var name } when name.StartsWith("zh", StringComparison.OrdinalIgnoreCase) => new K9Parser(),
			CoordinateParser c => c,
			_ => InvariantCultureParser
		};
}
