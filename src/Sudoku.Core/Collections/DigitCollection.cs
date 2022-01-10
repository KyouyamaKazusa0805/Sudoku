namespace Sudoku.Collections;

/// <summary>
/// Indicates a collection that contains the several digits.
/// </summary>
public readonly ref partial struct DigitCollection
{
	/// <summary>
	/// Indicates the inner mask.
	/// </summary>
	private readonly short _mask;


	/// <summary>
	/// Initializes the collection using a mask.
	/// </summary>
	/// <param name="mask">The mask.</param>
	public DigitCollection(short mask) => _mask = mask;

	/// <summary>
	/// Initializes an instance with the specified digits.
	/// </summary>
	/// <param name="digits">The digits.</param>
	public DigitCollection(in ReadOnlySpan<int> digits) : this()
	{
		foreach (int digit in digits)
		{
			_mask |= (short)(1 << digit);
		}
	}

	/// <summary>
	/// Initializes an instance with the specified digits.
	/// </summary>
	/// <param name="digits">The digits.</param>
	public DigitCollection(IEnumerable<int> digits) : this()
	{
		foreach (int digit in digits)
		{
			_mask |= (short)(1 << digit);
		}
	}


	/// <summary>
	/// Get the number of digits in the collection.
	/// </summary>
	public int Count => PopCount((uint)_mask);


	/// <summary>
	/// Indicates whether the specified collection contains the digit.
	/// </summary>
	/// <param name="digit">The digit.</param>
	/// <returns>A <see cref="bool"/> value.</returns>
	public bool Contains(int digit) => (_mask >> digit & 1) != 0;

	/// <inheritdoc/>
	public override string ToString() => ToString(", ");

	/// <summary>
	/// Returns a string that represents the current object with the specified format string.
	/// </summary>
	/// <param name="format">The format. If available, the parameter can be <see langword="null"/>.</param>
	/// <returns>The string result.</returns>
	public string ToString(string? format)
	{
		if (_mask == 0)
		{
			return "{ }";
		}

		if ((_mask & _mask - 1) == 0)
		{
			return (TrailingZeroCount(_mask) + 1).ToString();
		}

		string separator = format ?? string.Empty;
		var sb = new StringHandler(initialCapacity: 9);
		foreach (int digit in _mask)
		{
			sb.Append(digit + 1);
			sb.Append(separator);
		}

		sb.RemoveFromEnd(separator.Length);
		return sb.ToStringAndClear();
	}

	/// <summary>
	/// Get the enumerator of the current instance in order to use <see langword="foreach"/> loop.
	/// </summary>
	/// <returns>The enumerator instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ReadOnlySpan<int>.Enumerator GetEnumerator() => _mask.GetEnumerator();


	/// <summary>
	/// Make all <see langword="true"/> bits to be set <see langword="false"/>,
	/// and make all <see langword="false"/> bits to be set <see langword="true"/>.
	/// </summary>
	/// <param name="collection">The instance to negate.</param>
	/// <returns>The negative result.</returns>
	public static DigitCollection operator ~(DigitCollection collection) => new((short)~collection._mask);

	/// <summary>
	/// Apply the intersection from two <see cref="DigitCollection"/>s.
	/// </summary>
	/// <param name="left">The left instance.</param>
	/// <param name="right">The right instance.</param>
	/// <returns>The result collection.</returns>
	public static DigitCollection operator &(DigitCollection left, DigitCollection right) =>
		new((short)(left._mask & right._mask));

	/// <summary>
	/// Apply the union of two <see cref="DigitCollection"/>s.
	/// </summary>
	/// <param name="left">The left instance.</param>
	/// <param name="right">The right instance.</param>
	/// <returns>The result collection.</returns>
	public static DigitCollection operator |(DigitCollection left, DigitCollection right) =>
		new((short)(left._mask | right._mask));

	/// <summary>
	/// Apply the exclusive union of two <see cref="DigitCollection"/>s.
	/// </summary>
	/// <param name="left">The left instance.</param>
	/// <param name="right">The right instance.</param>
	/// <returns>The result collection.</returns>
	public static DigitCollection operator ^(DigitCollection left, DigitCollection right) =>
		new((short)(left._mask ^ right._mask));
}
