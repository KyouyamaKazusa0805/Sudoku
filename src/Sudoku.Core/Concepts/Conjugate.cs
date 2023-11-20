using System.Numerics;
using System.Runtime.CompilerServices;
using System.SourceGeneration;
using Sudoku.Concepts.Converters;
using Sudoku.Concepts.Parsers;
using Sudoku.Concepts.Primitive;
using static Sudoku.SolutionWideReadOnlyFields;

namespace Sudoku.Concepts;

/// <summary>
/// Represents a <see href="http://sudopedia.enjoysudoku.com/Conjugate_pair.html">conjugate pair</see>.
/// </summary>
/// <remarks>
/// A <b>Conjugate pair</b> is a pair of two candidates, in the same house where all cells has only
/// two position can fill this candidate.
/// </remarks>
/// <param name="mask">Indicates the target mask.</param>
[Equals]
[GetHashCode]
[EqualityOperators]
public readonly partial struct Conjugate([Data(DataMemberKinds.Field)] ConjugateMask mask) :
	IEquatable<Conjugate>,
	IEqualityOperators<Conjugate, Conjugate, bool>,
	ICoordinateObject<Conjugate>
{
	/// <summary>
	/// Initializes a <see cref="Conjugate"/> instance with from and to cell offset and a digit.
	/// </summary>
	/// <param name="from">The from cell.</param>
	/// <param name="to">The to cell.</param>
	/// <param name="digit">The digit.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Conjugate(Cell from, Cell to, Digit digit) : this(digit << 20 | from << 10 | to)
	{
	}

	/// <summary>
	/// Initializes a <see cref="Conjugate"/> instance with the map and the digit.
	/// The map should contains two cells, the first one is the start one, and the second one is the end one.
	/// </summary>
	/// <param name="map">The map.</param>
	/// <param name="digit">The digit.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Conjugate(scoped ref readonly CellMap map, Digit digit) : this(map[0], map[1], digit)
	{
	}


	/// <summary>
	/// Indicates the "from" cell, i.e. the base cell that starts the conjugate pair.
	/// </summary>
	public Cell From => _mask & 1023;

	/// <summary>
	/// Indicates the "to" cell, i.e. the target cell that ends the conjugate pair.
	/// </summary>
	public Cell To => _mask >> 10 & 1023;

	/// <summary>
	/// Indicates the digit used.
	/// </summary>
	[HashCodeMember]
	public Digit Digit => _mask >> 20 & 15;

	/// <summary>
	/// Indicates the target line of the two cells lie in.
	/// </summary>
	public House Line => Map.CoveredLine;

	/// <summary>
	/// Indicates the house that the current conjugate pair lies in.
	/// </summary>
	public HouseMask Houses => Map.CoveredHouses;

	/// <summary>
	/// Indicates the cells (the "from" cell and "to" cell).
	/// </summary>
	[HashCodeMember]
	public CellMap Map => [From, To];


	/// <include file="../../global-doc-comments.xml" path="g/csharp7/feature[@name='deconstruction-method']/target[@name='method']"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Deconstruct(out Candidate fromCand, out Candidate toCand) => (fromCand, toCand) = (From * 9 + Digit, To * 9 + Digit);

	/// <inheritdoc cref="IEquatable{T}.Equals(T)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(Conjugate other) => Map == other.Map && Digit == other.Digit;

	/// <inheritdoc cref="object.ToString"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString() => $"{CellsMap[From]} == {CellsMap[To]}({Digit + 1})";

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string ToString(CoordinateConverter converter) => converter.ConjugateConverter([this]);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Conjugate ParseExact(string str, CoordinateParser parser)
		=> parser.ConjuagteParser(str) is [var result] ? result : throw new FormatException("Multiple conjuagte pair values found.");
}
