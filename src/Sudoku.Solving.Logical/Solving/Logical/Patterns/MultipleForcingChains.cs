namespace Sudoku.Solving.Logical.Patterns;

/// <summary>
/// Defines a multiple forcing chains.
/// </summary>
[GeneratedOverloadingOperator(GeneratedOperator.EqualityOperators)]
internal sealed partial class MultipleForcingChains :
	IEnumerable<(byte CellOrDigit, ChainNode Potential)>,
	IEquatable<MultipleForcingChains>,
	IEqualityOperators<MultipleForcingChains, MultipleForcingChains, bool>,
	IReadOnlyCollection<(byte CellOrDigit, ChainNode Potential)>,
	IReadOnlyList<(byte CellOrDigit, ChainNode Potential)>
{
	/// <summary>
	/// Indicates the internal dictionary.
	/// </summary>
	private readonly SortedDictionary<byte, ChainNode> _internalDictionary = new(CellOrDigitComparer.Instance);

	/// <summary>
	/// Indicates the keys.
	/// </summary>
	private readonly List<byte> _keys = new(7);

	/// <summary>
	/// Indicates the values.
	/// </summary>
	private readonly List<ChainNode> _values = new(7);


	/// <summary>
	/// Indicates the number of elements stored in this collection.
	/// </summary>
	public int Count => _internalDictionary.Count;

	/// <summary>
	/// Indicates the cells or digits.
	/// </summary>
	public IReadOnlyList<byte> CellsOrDigits => _keys;

	/// <summary>
	/// Indicates the potentials.
	/// </summary>
	public IReadOnlyList<ChainNode> Potentials => _values;


	/// <summary>
	/// Gets the element at the specified index.
	/// </summary>
	/// <param name="index">The desired index.</param>
	/// <returns>The pair of data.</returns>
	/// <exception cref="IndexOutOfRangeException">Throws when the index is out of range.</exception>
	public (byte CellOrDigit, ChainNode Potential) this[int index]
	{
		get
		{
			var enumerator = _internalDictionary.GetEnumerator();
			var lastIndex = index;
			while (enumerator.MoveNext())
			{
				if (--lastIndex == -1)
				{
					var (a, b) = enumerator.Current;
					return (a, b);
				}
			}

			throw new IndexOutOfRangeException();
		}
	}


	/// <summary>
	/// Adds a pair of data into the collection.
	/// </summary>
	/// <param name="cellOrDigit">The cell or digit.</param>
	/// <param name="potential">The potential.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Add(byte cellOrDigit, ChainNode potential)
	{
		_internalDictionary.Add(cellOrDigit, potential);
		_keys.Add(cellOrDigit);
		_values.Add(potential);
	}

	[GeneratedOverriddingMember(GeneratedEqualsBehavior.AsCastAndCallingOverloading)]
	public override partial bool Equals(object? obj);

	/// <inheritdoc/>
	public bool Equals([NotNullWhen(true)] MultipleForcingChains? other)
	{
		if (other is null)
		{
			return false;
		}

		if (Count != other.Count)
		{
			return false;
		}

		for (var i = 0; i < Count; i++)
		{
			if (this[i] != other[i])
			{
				return false;
			}
		}

		return true;
	}

	/// <inheritdoc cref="object.GetHashCode"/>
	public override int GetHashCode()
	{
		var result = 0;
		foreach (var (key, value) in this)
		{
			result ^= key * (729 << 1) + value.GetHashCode();
		}

		return result;
	}

	/// <inheritdoc/>
	public IEnumerator<(byte CellOrDigit, ChainNode Potential)> GetEnumerator()
	{
		foreach (var (a, b) in this)
		{
			yield return (a, b);
		}
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


	/// <summary>
	/// Implicit cast from <see cref="Dictionary{TKey, TValue}"/> to <see cref="MultipleForcingChains"/>.
	/// </summary>
	/// <param name="dictionary">The dictionary.</param>
	public static implicit operator MultipleForcingChains(Dictionary<byte, ChainNode> dictionary)
	{
		var result = new MultipleForcingChains();
		foreach (var (key, value) in dictionary)
		{
			result.Add(key, value);
		}

		return result;
	}


	/// <summary>
	/// Defines an enumerator that iterates the current collection.
	/// </summary>
	public ref partial struct Enumerator
	{
		/// <summary>
		/// Initializes an <see cref="Enumerator"/> instance via the specified <see cref="MultipleForcingChains"/> instance.
		/// </summary>
		/// <param name="multipleForcingChains">The <see cref="MultipleForcingChains"/> instance.</param>
		[FileAccessOnly]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal Enumerator(MultipleForcingChains multipleForcingChains)
			=> _enumerator = multipleForcingChains._internalDictionary.GetEnumerator();
	}
}

/// <summary>
/// Defines a comparer that compares with inner key.
/// </summary>
file sealed class CellOrDigitComparer : IComparer<byte>
{
	/// <summary>
	/// Indicates the singleton instance.
	/// </summary>
	public static readonly CellOrDigitComparer Instance = new();


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private CellOrDigitComparer()
	{
	}


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int Compare(byte x, byte y) => Sign(x - y);
}
