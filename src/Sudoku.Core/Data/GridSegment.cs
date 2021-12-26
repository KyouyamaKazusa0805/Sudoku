namespace Sudoku.Data;

/// <summary>
/// Provides a segment that holds a list of candidate states for a certain set of cells.
/// </summary>
public readonly unsafe struct GridSegment
: IEqualityOperators<GridSegment, GridSegment>
, IEquatable<GridSegment>
, IValueEquatable<GridSegment>
{
	/// <summary>
	/// Indicates the list of masks used.
	/// </summary>
	private readonly ushort[] _values;

	/// <summary>
	/// Indicates the code that represents for the grid.
	/// </summary>
	private readonly string _gridCode;


	/// <summary>
	/// Initializes a <see cref="GridSegment"/> instance via a <see cref="Grid"/> as the base mask information,
	/// and a <see cref="Cells"/> collection to hold the details which cells will be used.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <param name="pattern">Indicates the cells used.</param>
	public GridSegment(in Grid grid, in Cells pattern)
	{
		_gridCode = grid.ToString("#");
		ushort[] values = new ushort[pattern.Count];
		fixed (short* pGrid = grid)
		{
			int i = 0;
			foreach (int cell in pattern)
			{
				values[i++] = (ushort)(cell << 9 | pGrid[cell] & 511);
			}
		}

		_values = values;
	}


	/// <summary>
	/// Indicates which cells the segment covers.
	/// </summary>
	public Cells Cells
	{
		get
		{
			var result = Cells.Empty;
			foreach (ushort value in _values)
			{
				result.AddAnyway(value >> 9);
			}

			return result;
		}
	}


	/// <summary>
	/// Gets the mask that is located to the specified index.
	/// </summary>
	/// <param name="index">The index value to get.</param>
	/// <param name="mode">
	/// The mode you want to find that is applied to the index as the behavior of this indexer.
	/// The default value is <see cref="GridSegmentIndexerMode.ByCellIndex"/>.
	/// </param>
	/// <returns>
	/// The pair as the result. The pair contains the cell and the mask of the current candidate.
	/// </returns>
	/// <exception cref="IndexOutOfRangeException">
	/// Throws when the <paramref name="index"/> is out of range via the specified indexing mode.
	/// </exception>
	/// <exception cref="InvalidOperationException">
	/// Throws when the collection doesn't contain the mask at the specified index
	/// via the specified indexing mode.
	/// </exception>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Throws when the argument <paramref name="mode"/> is invalid.
	/// The argument <paramref name="mode"/> must be <see cref="GridSegmentIndexerMode.ByCellIndex"/>
	/// or <see cref="GridSegmentIndexerMode.ByArrayIndex"/>.
	/// </exception>
	/// <seealso cref="GridSegmentIndexerMode.ByCellIndex"/>
	public (int Cell, short CandidatesMask) this[int index, GridSegmentIndexerMode mode = 0]
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => mode switch
		{
			GridSegmentIndexerMode.ByCellIndex => index is < 0 or >= 81
				? throw new IndexOutOfRangeException()
				: Array.FindIndex(_values, mask => mask >> 9 == index) is var z and not -1 && _values[z] is var mask
					? (mask >> 9, (short)(mask & 511))
					: throw new InvalidOperationException(
						"The collection doesn't contain the specified element at the desired index value."
					),
			GridSegmentIndexerMode.ByArrayIndex when _values[index] is var mask => (mask >> 9, (short)(mask & 511)),
			_ => throw new ArgumentOutOfRangeException(nameof(mode))
		};
	}


	/// <summary>
	/// Returns the reference of the mask that is the first position of the segment to iterate.
	/// </summary>
	/// <returns>The reference of the mask.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public ref readonly ushort GetPinnableReference() => ref _values[0];

	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] object? obj) => base.Equals(obj);

	/// <inheritdoc/>
	public bool Equals(GridSegment other)
	{
		if (_gridCode != other._gridCode)
		{
			return false;
		}

		int pLength = _values.Length, qLength = other._values.Length;
		if (pLength != qLength)
		{
			return false;
		}

		fixed (ushort* p = _values, q = other._values)
		{
			var sp = new Span<ushort>(_values);

			for (int i = 0; i < pLength; i++)
			{
				if (p[i] != q[i])
				{
					return false;
				}
			}
		}

		return true;
	}

	/// <inheritdoc/>
	public override int GetHashCode()
	{
		var @base = new HashCode();
		foreach (ushort mask in _values)
		{
			@base.Add(mask);
		}

		return @base.ToHashCode() ^ _gridCode.GetHashCode();
	}

	/// <inheritdoc cref="object.ToString"/>
	public override string ToString() => $"{{ {nameof(GridSegment)} ({_gridCode} at {Cells}) }}";

	/// <inheritdoc/>
	bool IValueEquatable<GridSegment>.Equals(in GridSegment other) => Equals(other);


	/// <inheritdoc cref="IEqualityOperators{TSelf, TOther}.operator =="/>
	public static bool operator ==(GridSegment left, GridSegment right) => left.Equals(right);

	/// <inheritdoc cref="IEqualityOperators{TSelf, TOther}.operator !="/>
	public static bool operator !=(GridSegment left, GridSegment right) => !(left == right);
}
