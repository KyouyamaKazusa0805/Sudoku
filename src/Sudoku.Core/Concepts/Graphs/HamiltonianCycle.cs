namespace Sudoku.Concepts.Graphs;

/// <summary>
/// Represents the concept "<see href="https://en.wikipedia.org/wiki/Hamiltonian_path">Hamiltonian Path</see>"
/// that will be applied to a <see cref="CellGraph"/> instance.
/// </summary>
/// <param name="cells">Indicates the cells.</param>
/// <seealso cref="CellGraph"/>
[StructLayout(LayoutKind.Auto)]
[TypeImpl(TypeImplFlags.Object_Equals | TypeImplFlags.EqualityOperators)]
public readonly partial struct HamiltonianCycle([Field] Cell[] cells) :
	IEnumerable<Cell>,
	IEquatable<HamiltonianCycle>,
	IEqualityOperators<HamiltonianCycle, HamiltonianCycle, bool>,
	IReadOnlyList<Cell>
{
	/// <summary>
	/// Indicates the number of cells used.
	/// </summary>
	public int Length => _cells.Length;

	/// <summary>
	/// Indicates the sequence of cells in order.
	/// </summary>
	public ReadOnlySpan<Cell> Cells => _cells;

	/// <inheritdoc/>
	int IReadOnlyCollection<Cell>.Count => Length;


	/// <summary>
	/// Returns the cell at the specified index.
	/// </summary>
	/// <param name="index">The desired index.</param>
	/// <returns>The cell at the specified index.</returns>
	public Cell this[int index] => _cells[index];


	/// <inheritdoc/>
	public bool Equals(HamiltonianCycle other) => Equals(other, HamiltonianCycleComparison.Default);

	/// <summary>
	/// Determine whether two <see cref="HamiltonianCycle"/> instances are considered as equal under the specified comparison rule.
	/// </summary>
	/// <param name="other">Indicates the other object to be compared.</param>
	/// <param name="comparison">The comparison rule to be used.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Throws when the argument <paramref name="comparison"/> is not defined.
	/// </exception>
	public bool Equals(HamiltonianCycle other, HamiltonianCycleComparison comparison)
		=> comparison switch
		{
			HamiltonianCycleComparison.Default => _cells.SequenceEqual(other._cells),
			HamiltonianCycleComparison.IgnoreDirection
				=> _cells.SequenceEqual(other._cells) || _cells.Reverse().SequenceEqual(other._cells),
			_ => throw new ArgumentOutOfRangeException(nameof(comparison))
		};

	/// <inheritdoc/>
	public override int GetHashCode() => GetHashCode(HamiltonianCycleComparison.Default);

	/// <summary>
	/// Returns the hash code of the current instance, using the specified comparison rule.
	/// </summary>
	/// <param name="comparison">The comparison rule.</param>
	/// <returns>An <see cref="int"/> value indicating the result.</returns>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Throws when the argument <paramref name="comparison"/> is not defined.
	/// </exception>
	public int GetHashCode(HamiltonianCycleComparison comparison)
	{
		switch (comparison)
		{
			case HamiltonianCycleComparison.IgnoreDirection:
			{
				var hashCode = new HashCode();
				var cellsSorted = _cells[..];
				Array.Sort(cellsSorted, static (left, right) => string.Compare(left.ToString(), right.ToString()));

				foreach (var cell in cellsSorted)
				{
					hashCode.Add(cell);
				}
				return hashCode.ToHashCode();
			}
			case HamiltonianCycleComparison.Default:
			{
				var hashCode = new HashCode();
				foreach (var cell in _cells)
				{
					hashCode.Add(cell);
				}
				return hashCode.ToHashCode();
			}
			default:
			{
				throw new ArgumentOutOfRangeException(nameof(comparison));
			}
		}
	}

	/// <inheritdoc cref="object.ToString"/>
	public override string ToString() => ToString(null);

	/// <summary>
	/// Returns a string that represents the current object, using the specified coordinate converter object to format cells.
	/// </summary>
	/// <param name="converter">
	/// Indicates the converter. If <see langword="null"/> is assigned, RxCy notation will be adopted.
	/// </param>
	/// <returns>A <see cref="string"/> that represents the current object.</returns>
	public string ToString(CoordinateConverter? converter)
	{
		converter ??= new RxCyConverter();

		const string separator = " -> ";
		var sb = new StringBuilder();
		foreach (var cell in _cells)
		{
			sb.Append(converter.CellConverter(cell.AsCellMap()));
			sb.Append(separator);
		}
		return sb.RemoveFrom(^separator.Length).ToString();
	}

	/// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
	public AnonymousSpanEnumerator<Cell> GetEnumerator() => new(_cells);

	/// <inheritdoc/>
	IEnumerator IEnumerable.GetEnumerator() => _cells.GetEnumerator();

	/// <inheritdoc/>
	IEnumerator<Cell> IEnumerable<Cell>.GetEnumerator() => _cells.AsEnumerable().GetEnumerator();
}
