using System.Runtime.CompilerServices;

namespace Sudoku.Text.Coordinate;

/// <summary>
/// Represents an option provider for coordinates.
/// </summary>
/// <param name="DefaultSeparator">
/// <para>Indicates the default separator. The value will be inserted into two non-digit-kind instances.</para>
/// <para>The value is <c>", "</c> by default.</para>
/// </param>
/// <param name="DigitsSeprarator">
/// <para>Indicates the digits separator.</para>
/// <para>The value is <see langword="null"/> by default, meaning no separators will be inserted between 2 digits.</para>
/// </param>
/// <remarks>
/// You can use types <see cref="RxCyConverter"/>, <seealso cref="K9Converter"/> and <see cref="LiteralCoordinateConverter"/>.
/// They are the derived types of the current type.
/// </remarks>
/// <seealso cref="RxCyConverter"/>
/// <seealso cref="K9Converter"/>
/// <seealso cref="LiteralCoordinateConverter"/>
public abstract record CoordinateConverter(string DefaultSeparator = ", ", string? DigitsSeprarator = null)
{
	/// <summary>
	/// The converter method that creates a <see cref="string"/> via the specified list of cells.
	/// </summary>
	public abstract CellNotationConverter CellNotationConverter { get; }

	/// <summary>
	/// The converter method that creates a <see cref="string"/> via the specified list of candidates.
	/// </summary>
	public abstract CandidateNotationConverter CandidateNotationConverter { get; }

	/// <summary>
	/// The converter method that creates a <see cref="string"/> via the specified list of houses.
	/// </summary>
	public abstract HouseNotationConverter HouseNotationConverter { get; }

	/// <summary>
	/// The converter method that creates a <see cref="string"/> via the specified list of conclusions.
	/// </summary>
	public abstract ConclusionNotationConverter ConclusionNotationConverter { get; }

	/// <summary>
	/// The converter method that creates a <see cref="string"/> via the specified list of digits.
	/// </summary>
	public abstract DigitNotationConverter DigitNotationConverter { get; }

	/// <summary>
	/// The converter method that creates a <see cref="string"/> via the specified information for an intersection.
	/// </summary>
	public abstract IntersectionNotationConverter IntersectionNotationConverter { get; }

	/// <summary>
	/// The converter method that creates a <see cref="string"/> via the specified list of chute.
	/// </summary>
	public abstract ChuteNotationConverter ChuteNotationConverter { get; }

	/// <summary>
	/// The converter method that creates a <see cref="string"/> via the specified conjugate.
	/// </summary>
	public abstract ConjugateNotationConverter ConjugateNotationConverter { get; }


	/// <summary>
	/// Creates a <see cref="CoordinateConverter"/> instance via the specified concept notation.
	/// </summary>
	/// <param name="conceptNotation">The field to represent with a kind of concept notation.</param>
	/// <returns>A <see cref="CoordinateConverter"/> instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CoordinateConverter Create(ConceptNotationBased conceptNotation)
		=> conceptNotation switch
		{
			ConceptNotationBased.RxCyBased => new RxCyConverter(),
			ConceptNotationBased.K9Based => new K9Converter(),
			_ => new LiteralCoordinateConverter()
		};
}
