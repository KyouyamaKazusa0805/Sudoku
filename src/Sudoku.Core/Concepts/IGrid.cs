namespace Sudoku.Concepts;

/// <summary>
/// Represents a sudoku grid.
/// </summary>
/// <typeparam name="TSelf">The type of itself.</typeparam>
public interface IGrid<TSelf> :
	IComparable<TSelf>,
	IComparisonOperators<TSelf, TSelf, bool>,
	IEnumerable<Digit>,
	IEquatable<TSelf>,
	IEqualityOperators<TSelf, TSelf, bool>,
	IFormattable,
	IMinMaxValue<TSelf>,
	IParsable<TSelf>,
	IReadOnlyCollection<Digit>,
	ISpanFormattable,
	ISpanParsable<TSelf>,
	IToArrayMethod<TSelf, Digit>
	where TSelf : unmanaged, IGrid<TSelf>
{
	/// <summary>
	/// Indicates whether the grid is <see cref="Empty"/>, which means the grid holds totally same value with <see cref="Empty"/>.
	/// </summary>
	/// <seealso cref="Empty"/>
	public virtual bool IsEmpty => (TSelf)this == TSelf.Empty;

	/// <summary>
	/// Indicates whether the grid is <see cref="Undefined"/>, which means the grid holds totally same value with <see cref="Undefined"/>.
	/// </summary>
	/// <seealso cref="Undefined"/>
	public virtual bool IsUndefined => (TSelf)this == TSelf.Undefined;

	/// <summary>
	/// Indicates the reference to the first mask.
	/// </summary>
	[UnscopedRef]
	protected abstract ref readonly Mask FirstMaskRef { get; }


	/// <summary>
	/// Represents a string value that describes a <typeparamref name="TSelf"/> instance can be parsed into <see cref="Empty"/>.
	/// </summary>
	/// <seealso cref="Empty"/>
	public static abstract ref readonly string EmptyString { get; }

	/// <summary>
	/// Represents an empty instance, with valid data to build logic.
	/// </summary>
	public static abstract ref readonly TSelf Empty { get; }

	/// <summary>
	/// Represents an empty instance, equivalent to <see langword="default"/>(<typeparamref name="TSelf"/>).
	/// </summary>
	public static abstract ref readonly TSelf Undefined { get; }

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



	/// <summary>
	/// Gets the mask at the specified position.
	/// </summary>
	/// <param name="cell">The desired cell index.</param>
	/// <returns>The reference to the mask.</returns>
	[UnscopedRef]
	public abstract ref Mask this[Cell cell] { get; }


	/// <inheritdoc cref="object.Equals(object?)"/>
	public abstract bool Equals([NotNullWhen(true)] object? other);

	/// <summary>
	/// Determines whether the current instance has same mask values with the other object.
	/// </summary>
	/// <param name="other">The other instance.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	public abstract bool Equals(ref readonly TSelf other);

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

	/// <inheritdoc cref="IToArrayMethod{TSelf, TSource}.ToArray"/>
	public new abstract Digit[] ToArray();

	/// <inheritdoc/>
	bool IEquatable<TSelf>.Equals(TSelf other) => Equals(in other);

	/// <inheritdoc/>
	int IComparable<TSelf>.CompareTo(TSelf other) => CompareTo(in other);

	/// <inheritdoc/>
	int IReadOnlyCollection<Digit>.Count => 81;

	/// <inheritdoc/>
	Digit[] IToArrayMethod<TSelf, Digit>.ToArray() => ToArray();

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

	/// <inheritdoc cref="ISpanParsable{TSelf}.TryParse(ReadOnlySpan{char}, IFormatProvider?, out TSelf)"/>
	public static new virtual bool TryParse(ReadOnlySpan<char> s, IFormatProvider? formatProvider, out TSelf result)
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
	public static virtual bool TryPase(string? s, out TSelf result) => TSelf.TryParse(s, null, out result);

	/// <inheritdoc cref="ISpanParsable{TSelf}.TryParse(ReadOnlySpan{char}, IFormatProvider?, out TSelf)"/>
	public static virtual bool TryPase(ReadOnlySpan<char> s, out TSelf result) => TSelf.TryParse(s, null, out result);

	/// <summary>
	/// Creates a <typeparamref name="TSelf"/> instance via the specified list of <see cref="Mask"/> values.
	/// </summary>
	/// <param name="values">The values to be created.</param>
	/// <returns>A <typeparamref name="TSelf"/> instance created.</returns>
	public static abstract TSelf Create(ReadOnlySpan<Mask> values);

	/// <inheritdoc cref="IParsable{TSelf}.Parse(string?, IFormatProvider?)"/>
	public static virtual TSelf Parse(string? s) => TSelf.Parse(s, null);

	/// <inheritdoc cref="ISpanParsable{TSelf}.Parse(ReadOnlySpan{char}, IFormatProvider?)"/>
	public static virtual TSelf Parse(ReadOnlySpan<char> s) => TSelf.Parse(s, null);


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
