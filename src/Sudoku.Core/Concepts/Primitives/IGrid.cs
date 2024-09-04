namespace Sudoku.Concepts.Primitives;

/// <summary>
/// Represents a sudoku grid.
/// </summary>
/// <typeparam name="TSelf"><include file="../../global-doc-comments.xml" path="/g/self-type-constraint"/></typeparam>
public interface IGrid<TSelf> :
	IComparable<TSelf>,
	IComparisonOperators<TSelf, TSelf, bool>,
	IEnumerable<Digit>,
	IEquatable<TSelf>,
	IEqualityOperators<TSelf, TSelf, bool>,
	IFormattable,
	IGridConstants<TSelf>,
	IGridOperations<TSelf>,
	IGridProperties<TSelf>,
	IGridSolvingMembers<TSelf>,
	IMinMaxValue<TSelf>,
	IParsable<TSelf>,
	IReadOnlyCollection<Digit>,
	ISpanFormattable,
	ISpanParsable<TSelf>,
	IToArrayMethod<TSelf, Digit>
	where TSelf : unmanaged, IGrid<TSelf>
{
	/// <summary>
	/// Determines whether the current grid contains any missing candidates.
	/// </summary>
	public abstract bool IsMissingCandidates { get; }

	/// <summary>
	/// Indicates the inner array that stores the masks of the sudoku grid, which stores the in-time sudoku grid inner information.
	/// </summary>
	/// <remarks>
	/// The field uses the mask table of length 81 to indicate the state and all possible candidates
	/// holding for each cell. Each mask uses a <see cref="Mask"/> value, but only uses 11 of 16 bits.
	/// <code>
	/// | 16  15  14  13  12  11  10  9   8   7   6   5   4   3   2   1   0 |
	/// |-------------------|-----------|-----------------------------------|
	/// |    unused bits    | 0 | 0 | 1 | 1 | 1 | 1 | 1 | 1 | 1 | 1 | 1 | 1 |
	/// '-------------------|-----------|-----------------------------------'
	///                      \_________/ \_________________________________/
	///                          (2)                     (1)
	/// </code>
	/// Here the 9 bits in (1) indicate whether each digit is possible candidate in the current cell for each bit respectively,
	/// and the higher 3 bits in (2) indicate the cell state. The possible cell state are:
	/// <list type="table">
	/// <listheader>
	/// <term>State name</term>
	/// <description>Description</description>
	/// </listheader>
	/// <item>
	/// <term>Empty cell (i.e. <see cref="CellState.Empty"/>)</term>
	/// <description>The cell is currently empty, and wait for being filled.</description>
	/// </item>
	/// <item>
	/// <term>Modifiable cell (i.e. <see cref="CellState.Modifiable"/>)</term>
	/// <description>The cell is filled by a digit, but the digit isn't the given by the initial grid.</description>
	/// </item>
	/// <item>
	/// <term>Given cell (i.e. <see cref="CellState.Given"/>)</term>
	/// <description>The cell is filled by a digit, which is given by the initial grid and can't be modified.</description>
	/// </item>
	/// </list>
	/// </remarks>
	/// <seealso cref="CellState"/>
	[UnscopedRef]
	protected abstract ref readonly Mask FirstMaskRef { get; }


	/// <summary>
	/// Indicates the minimum possible grid value that the current type can reach.
	/// </summary>
	/// <remarks>
	/// This value is found out via backtracking algorithm.
	/// </remarks>
	static TSelf IMinMaxValue<TSelf>.MinValue => TSelf.Parse("123456789456789123789123456214365897365897214897214365531642978642978531978531642");

	/// <summary>
	/// Indicates the maximum possible grid value that the current type can reach.
	/// </summary>
	/// <remarks>
	/// This value is found out via backtracking algorithm.
	/// </remarks>
	static TSelf IMinMaxValue<TSelf>.MaxValue => TSelf.Parse("987654321654321987321987654896745213745213896213896745579468132468132579132579468");


	/// <inheritdoc cref="object.Equals(object?)"/>
	public abstract bool Equals([NotNullWhen(true)] object? other);

	/// <summary>
	/// Determines whether the current instance has same mask values with the other object.
	/// </summary>
	/// <param name="other">The other instance.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	public abstract bool Equals(ref readonly TSelf other);

	/// <summary>
	/// Determine whether the digit in the target cell is conflict with a certain cell in the peers of the current cell,
	/// if the digit is filled into the cell.
	/// </summary>
	/// <param name="cell">The cell.</param>
	/// <param name="digit">The digit.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	public abstract bool ConflictWith(Cell cell, Digit digit);

	/// <inheritdoc cref="object.GetHashCode"/>
	public abstract int GetHashCode();

	/// <inheritdoc cref="IComparable{T}.CompareTo(T)"/>
	public abstract int CompareTo(ref readonly TSelf other);

	/// <inheritdoc cref="object.ToString"/>
	public abstract string ToString();

	/// <inheritdoc cref="IFormattable.ToString(string?, IFormatProvider?)"/>
	public virtual string ToString(IFormatProvider? formatProvider) => ToString(null, formatProvider);

	/// <inheritdoc cref="IFormattable.ToString(string?, IFormatProvider?)"/>
	public virtual string ToString(string? format) => ToString(format, null);

	/// <summary>
	/// Serializes this instance to an array, where all digit value will be stored.
	/// </summary>
	/// <returns>
	/// This array. All elements are the raw masks
	/// that are between 0 and <see cref="IGridConstants{TSelf}.MaxCandidatesMask"/> (i.e. 511).
	/// </returns>
	/// <seealso cref="IGridConstants{TSelf}.MaxCandidatesMask"/>
	public abstract Mask[] ToCandidateMaskArray();

	/// <summary>
	/// Try to create a new array of <see cref="Digit"/> instances indicating filling digits inside cells.
	/// </summary>
	/// <returns>An array of <see cref="Digit"/> instances.</returns>
	/// <seealso cref="Digit"/>
	public abstract Digit[] ToDigitsArray();

	/// <inheritdoc/>
	bool IEquatable<TSelf>.Equals(TSelf other) => Equals(in other);

	/// <inheritdoc/>
	int IComparable<TSelf>.CompareTo(TSelf other) => CompareTo(in other);

	/// <inheritdoc/>
	int IReadOnlyCollection<Digit>.Count => 81;

	/// <inheritdoc/>
	Digit[] IToArrayMethod<TSelf, Digit>.ToArray() => ToDigitsArray();

	/// <inheritdoc/>
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


	/// <inheritdoc cref="IParsable{TSelf}.TryParse(string?, IFormatProvider?, out TSelf)"/>
	public static new virtual bool TryParse(string? s, IFormatProvider? formatProvider, out TSelf result)
	{
		try
		{
			result = TSelf.Parse(s, formatProvider);
			return true;
		}
		catch (FormatException)
		{
			result = TSelf.Undefined;
			return false;
		}
	}

	/// <inheritdoc cref="ISpanParsable{TSelf}.TryParse(ReadOnlyCharSequence, IFormatProvider?, out TSelf)"/>
	public static new virtual bool TryParse(ReadOnlyCharSequence s, IFormatProvider? formatProvider, out TSelf result)
	{
		try
		{
			result = TSelf.Parse(s, formatProvider);
			return true;
		}
		catch (FormatException)
		{
			result = TSelf.Undefined;
			return false;
		}
	}

	/// <inheritdoc cref="IParsable{TSelf}.TryParse(string?, IFormatProvider?, out TSelf)"/>
	public static virtual bool TryParse(string? s, out TSelf result) => TSelf.TryParse(s, null, out result);

	/// <inheritdoc cref="ISpanParsable{TSelf}.TryParse(ReadOnlyCharSequence, IFormatProvider?, out TSelf)"/>
	public static virtual bool TryParse(ReadOnlyCharSequence s, out TSelf result) => TSelf.TryParse(s, null, out result);

	/// <summary>
	/// Creates a <typeparamref name="TSelf"/> instance via the specified list of <see cref="Mask"/> values.
	/// </summary>
	/// <param name="values">The values to be created.</param>
	/// <returns>A <typeparamref name="TSelf"/> instance created.</returns>
	public static abstract TSelf Create(ReadOnlySpan<Mask> values);

	/// <inheritdoc cref="IParsable{TSelf}.Parse(string?, IFormatProvider?)"/>
	public static virtual TSelf Parse(string? s) => TSelf.Parse(s, null);

	/// <inheritdoc cref="ISpanParsable{TSelf}.Parse(ReadOnlyCharSequence, IFormatProvider?)"/>
	public static virtual TSelf Parse(ReadOnlyCharSequence s) => TSelf.Parse(s, null);


	/// <inheritdoc cref="IEqualityOperators{TSelf, TOther, TResult}.op_Equality(TSelf, TOther)"/>
	public static abstract bool operator ==(in TSelf left, in TSelf right);

	/// <inheritdoc cref="IEqualityOperators{TSelf, TOther, TResult}.op_Inequality(TSelf, TOther)"/>
	public static abstract bool operator !=(in TSelf left, in TSelf right);

	/// <inheritdoc cref="IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThan(TSelf, TOther)"/>
	public static abstract bool operator >(in TSelf left, in TSelf right);

	/// <inheritdoc cref="IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThanOrEqual(TSelf, TOther)"/>
	public static abstract bool operator >=(in TSelf left, in TSelf right);

	/// <inheritdoc cref="IComparisonOperators{TSelf, TOther, TResult}.op_LessThan(TSelf, TOther)"/>
	public static abstract bool operator <(in TSelf left, in TSelf right);

	/// <inheritdoc cref="IComparisonOperators{TSelf, TOther, TResult}.op_LessThanOrEqual(TSelf, TOther)"/>
	public static abstract bool operator <=(in TSelf left, in TSelf right);

	/// <inheritdoc/>
	static bool IEqualityOperators<TSelf, TSelf, bool>.operator ==(TSelf left, TSelf right) => left == right;

	/// <inheritdoc/>
	static bool IEqualityOperators<TSelf, TSelf, bool>.operator !=(TSelf left, TSelf right) => left != right;

	/// <inheritdoc/>
	static bool IComparisonOperators<TSelf, TSelf, bool>.operator >(TSelf left, TSelf right) => left > right;

	/// <inheritdoc/>
	static bool IComparisonOperators<TSelf, TSelf, bool>.operator <(TSelf left, TSelf right) => left < right;

	/// <inheritdoc/>
	static bool IComparisonOperators<TSelf, TSelf, bool>.operator >=(TSelf left, TSelf right) => left >= right;

	/// <inheritdoc/>
	static bool IComparisonOperators<TSelf, TSelf, bool>.operator <=(TSelf left, TSelf right) => left <= right;


	/// <summary>
	/// Converts the specified array elements into the target <typeparamref name="TSelf"/> instance,
	/// without any value boundary checking.
	/// </summary>
	/// <param name="maskArray">An array of the target mask. The array must be of a valid length.</param>
	public static abstract explicit operator TSelf(Mask[] maskArray);

	/// <summary>
	/// Converts the specified array elements into the target <typeparamref name="TSelf"/> instance, with value boundary checking.
	/// </summary>
	/// <param name="maskArray">
	/// <inheritdoc cref="op_Explicit(Mask[])" path="/param[@name='maskArray']"/>
	/// </param>
	/// <exception cref="ArgumentException">
	/// Throws when at least one element in the mask array is greater than 0b100__111_111_111 (i.e. 2559) or less than 0.
	/// </exception>
	public static abstract explicit operator checked TSelf(Mask[] maskArray);
}
