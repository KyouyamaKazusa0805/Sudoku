namespace Sudoku.Runtime.CompilerServices;

/// <summary>
/// Represents a list of extension methods operating with <see cref="List{T}"/>
/// of <see cref="IBitStatusMap{TSelf, TElement, TEnumerator}"/>.
/// </summary>
/// <seealso cref="List{T}"/>
/// <seealso cref="IBitStatusMap{TSelf, TElement, TEnumerator}"/>
public static class ListBitStatusMapExtensions
{
	/// <summary>
	/// Adds the given object to the end of this list.
	/// </summary>
	/// <param name="this">The list.</param>
	/// <param name="cells">The <see cref="CellMap"/> to be added.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void AddRef(this List<CellMap> @this, ref readonly CellMap cells)
	{
		GetVersion(@this)++;
		var array = @this.GetItems().AsSpan();
		var size = GetSize(@this);
		if ((uint)size < (uint)array.Length)
		{
			GetSize(@this)++;
			array[size] = cells;
		}
		else
		{
			@this.AddWithResize(in cells);
		}
	}

	/// <summary>
	/// Adds the given object to the end of this list.
	/// </summary>
	/// <param name="this">The list.</param>
	/// <param name="candidates">The <see cref="CandidateMap"/> to be added.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void AddRef(this List<CandidateMap> @this, ref readonly CandidateMap candidates)
	{
		GetVersion(@this)++;
		var array = @this.GetItems().AsSpan();
		var size = GetSize(@this);
		if ((uint)size < (uint)array.Length)
		{
			GetSize(@this)++;
			array[size] = candidates;
		}
		else
		{
			@this.AddWithResize(in candidates);
		}
	}

	/// <inheritdoc cref="List{T}.AddRange(IEnumerable{T})"/>
	public static void AddRangeRef(this List<CellMap> @this, params ReadOnlySpan<CellMap> collection)
	{
		foreach (ref readonly var cells in collection)
		{
			@this.AddRef(in cells);
		}
	}

	/// <inheritdoc cref="List{T}.AddRange(IEnumerable{T})"/>
	public static void AddRangeRef(this List<CandidateMap> @this, params ReadOnlySpan<CandidateMap> collection)
	{
		foreach (ref readonly var cells in collection)
		{
			@this.AddRef(in cells);
		}
	}

	/// <summary>
	/// Add an item and resize the <see cref="List{T}"/> of <see cref="CellMap"/>.
	/// </summary>
	/// <param name="this">The list.</param>
	/// <param name="cells">The <see cref="CellMap"/> to be added.</param>
	private static void AddWithResize(this List<CellMap> @this, ref readonly CellMap cells)
	{
		Debug.Assert(GetSize(@this) == @this.GetItems().Length);
		var size = GetSize(@this);
		@this.Capacity = GetNewCapacity(@this, size + 1);
		GetSize(@this) = size + 1;
		@this.GetItems()[size] = cells;
	}

	/// <summary>
	/// Add an item and resize the <see cref="List{T}"/> of <see cref="CandidateMap"/>.
	/// </summary>
	/// <param name="this">The list.</param>
	/// <param name="candidates">The <see cref="CandidateMap"/> to be added.</param>
	private static void AddWithResize(this List<CandidateMap> @this, ref readonly CandidateMap candidates)
	{
		Debug.Assert(GetSize(@this) == @this.GetItems().Length);
		var size = GetSize(@this);
		@this.Capacity = GetNewCapacity(@this, size + 1);
		GetSize(@this) = size + 1;
		@this.GetItems()[size] = candidates;
	}

	/// <summary>
	/// Try to get a new capacity value by the desired capacity to be set.
	/// </summary>
	/// <param name="this">The collection.</param>
	/// <param name="capacity">The desired capacity to be set.</param>
	/// <returns>The result value to be set.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int GetNewCapacity(List<CellMap> @this, int capacity)
	{
		Debug.Assert(@this.GetItems().Length < capacity);
		return @this.GetItems().Length == 0 ? 4 : @this.GetItems().Length << 1;
	}

	/// <inheritdoc cref="GetNewCapacity(List{CellMap}, int)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int GetNewCapacity(List<CandidateMap> @this, int capacity)
	{
		Debug.Assert(@this.GetItems().Length < capacity);
		return @this.GetItems().Length == 0 ? 4 : @this.GetItems().Length << 1;
	}

	/// <summary>
	/// Try to fetch the internal field <c>_size</c> in type <see cref="List{T}"/>.
	/// </summary>
	/// <param name="this">The list.</param>
	/// <returns>The reference to the internal field.</returns>
	[UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_size")]
	private static extern ref int GetSize(List<CellMap> @this);

	/// <inheritdoc cref="GetSize(List{CellMap})"/>
	[UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_size")]
	private static extern ref int GetSize(List<CandidateMap> @this);

	/// <summary>
	/// Try to fetch the internal field <c>_version</c> in type <see cref="List{T}"/>.
	/// </summary>
	/// <param name="this">The list.</param>
	/// <returns>The reference to the internal field.</returns>
	[UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_version")]
	private static extern ref int GetVersion(List<CellMap> @this);

	/// <inheritdoc cref="GetVersion(List{CellMap})"/>
	[UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_version")]
	private static extern ref int GetVersion(List<CandidateMap> @this);

	/// <summary>
	/// Try to fetch the internal reference to the first element of type <see cref="CellMap"/> in a <see cref="List{T}"/>.
	/// </summary>
	/// <param name="this">The list of <see cref="CellMap"/> instances.</param>
	/// <returns>The reference to the first element of type <see cref="CellMap"/>.</returns>
	/// <remarks><b><i>
	/// Please note that this method will return the reference to the internal field,
	/// but this doesn't mean you can use its reference and re-assign it.
	/// </i></b></remarks>
	[UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_items")]
	private static extern ref CellMap[] GetItems(this List<CellMap> @this);

	/// <summary>
	/// Try to fetch the internal reference to the first element of type <see cref="CandidateMap"/> in a <see cref="List{T}"/>.
	/// </summary>
	/// <param name="this">The list of <see cref="CandidateMap"/> instances.</param>
	/// <returns>The reference to the first element of type <see cref="CandidateMap"/>.</returns>
	/// <remarks><b><i>
	/// Please note that this method will return the reference to the internal field,
	/// but this doesn't mean you can use its reference and re-assign it.
	/// </i></b></remarks>
	[UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_items")]
	private static extern ref CandidateMap[] GetItems(this List<CandidateMap> @this);
}
