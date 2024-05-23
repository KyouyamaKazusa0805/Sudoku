namespace Sudoku.Concepts;

/// <summary>
/// Represents a list of extension methods operating with <see cref="List{T}"/> of <see cref="Grid"/>.
/// </summary>
/// <seealso cref="List{T}"/>
/// <seealso cref="Grid"/>
#if NET9_0_OR_GREATER
[Obsolete("This type is replaced with generic version, which starts supporting from .NET 9 preview 4.", false)]
#endif
public static class ListGridExtensions
{
#if !NET9_0_OR_GREATER
	/// <summary>
	/// Adds the given object to the end of this list.
	/// </summary>
	/// <param name="this">The list.</param>
	/// <param name="grid">The item to be added.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void AddRef(this List<Grid> @this, scoped ref readonly Grid grid)
	{
		GetVersion(@this)++;
		var array = @this.GetItems().AsSpan();
		var size = GetSize(@this);
		if ((uint)size < (uint)array.Length)
		{
			GetSize(@this) = size + 1;
			array[size] = grid;
		}
		else
		{
			@this.AddWithResize(in grid);
		}
	}

	/// <inheritdoc cref="List{T}.AddRange(IEnumerable{T})"/>
	public static void AddRangeRef(this List<Grid> @this, scoped ReadOnlySpan<Grid> collection)
	{
		foreach (ref readonly var grid in collection)
		{
			@this.AddRef(in grid);
		}
	}

	/// <summary>
	/// Add an item and resize the <see cref="List{T}"/> of <see cref="Grid"/>.
	/// </summary>
	/// <param name="this">The list.</param>
	/// <param name="grid">The grid to be added.</param>
	private static void AddWithResize(this List<Grid> @this, scoped ref readonly Grid grid)
	{
		Debug.Assert(GetSize(@this) == @this.GetItems().Length);

		var size = GetSize(@this);
		@this.Capacity = GetNewCapacity(@this, size + 1);
		GetSize(@this) = size + 1;
		@this.GetItems()[size] = grid;
	}

	/// <summary>
	/// Try to get a new capacity value by the desired capacity to be set.
	/// </summary>
	/// <param name="this">The collection.</param>
	/// <param name="capacity">The desired capacity to be set.</param>
	/// <returns>The result value to be set.</returns>
	private static int GetNewCapacity(List<Grid> @this, int capacity)
	{
		Debug.Assert(@this.GetItems().Length < capacity);

		var newCapacity = @this.GetItems().Length == 0 ? 4 : 2 * @this.GetItems().Length;

		// Allow the list to grow to maximum possible capacity (~2G elements) before encountering overflow.
		// Note that this check works even when _items.Length overflowed thanks to the (uint) cast
		if ((uint)newCapacity > Array.MaxLength)
		{
			newCapacity = Array.MaxLength;
		}

		// If the computed capacity is still less than specified, set to the original argument.
		// Capacities exceeding Array.MaxLength will be surfaced as OutOfMemoryException by Array.Resize.
		return Math.Min(newCapacity, capacity);
	}

	/// <summary>
	/// Try to fetch the internal field <c>_size</c> in type <see cref="List{T}"/>.
	/// </summary>
	/// <param name="this">The list.</param>
	/// <returns>The reference to the internal field.</returns>
	[UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_size")]
	private static extern ref int GetSize(List<Grid> @this);

	/// <summary>
	/// Try to fetch the internal field <c>_version</c> in type <see cref="List{T}"/>.
	/// </summary>
	/// <param name="this">The list.</param>
	/// <returns>The reference to the internal field.</returns>
	[UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_version")]
	private static extern ref int GetVersion(List<Grid> @this);

	/// <summary>
	/// Try to fetch the internal reference to the first element of type <see cref="Grid"/> in a <see cref="List{T}"/>.
	/// </summary>
	/// <param name="this">The list of grids.</param>
	/// <returns>The reference to the first element of type <see cref="Grid"/>.</returns>
	/// <remarks><b><i>
	/// Please note that this method will return the reference to the internal field,
	/// but this doesn't mean you can use its reference and re-assign it.
	/// </i></b></remarks>
	[UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_items")]
	private static extern ref Grid[] GetItems(this List<Grid> @this);
#endif
}
