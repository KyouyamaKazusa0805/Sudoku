using System.Runtime.CompilerServices;

namespace Sudoku.Text.Coordinate;

/// <summary>
/// Represents an option provider for coordinates.
/// </summary>
/// <remarks>
/// You can use types <see cref="RxCyConverter"/>, <seealso cref="K9Converter"/> and <see cref="LiteralCoordinateConverter"/>.
/// They are the derived types of the current type.
/// </remarks>
/// <seealso cref="RxCyConverter"/>
/// <seealso cref="K9Converter"/>
/// <seealso cref="LiteralCoordinateConverter"/>
public abstract record CoordinateConverter
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
	/// <exception cref="NotSupportedException">Throws when the argument <paramref name="conceptNotation"/> is out of range.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CoordinateConverter Create(ConceptNotationBased conceptNotation)
		=> conceptNotation switch
		{
			ConceptNotationBased.LiteralBased => new LiteralCoordinateConverter(),
			ConceptNotationBased.RxCyBased => new RxCyConverter(),
			ConceptNotationBased.K9Based => new K9Converter(),
			_ => throw new NotSupportedException("The current value is not supported.")
		};
}
