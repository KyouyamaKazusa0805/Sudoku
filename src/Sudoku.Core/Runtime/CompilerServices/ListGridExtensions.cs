using System.Diagnostics;
using System.Runtime.CompilerServices;
using Sudoku.Concepts;

namespace Sudoku.Runtime.CompilerServices;

/// <summary>
/// Represents a list of extension methods operating with <see cref="List{T}"/> of <see cref="Grid"/>.
/// </summary>
/// <seealso cref="List{T}"/>
/// <seealso cref="Grid"/>
public static class ListGridExtensions
{
	/// <summary>
	/// Adds the given object to the end of this list.
	/// </summary>
	/// <param name="this">The list.</param>
	/// <param name="grid">The item to be added.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void AddRef(this List<Grid> @this, scoped ref readonly Grid grid)
	{
		GetVersion(@this)++;
		scoped var array = @this.GetItems().AsSpan();
		var size = GetSize(@this);
		if ((uint)size < (uint)array.Length)
		{
			GetSize(@this)++;
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
		foreach (ref readonly var cells in collection)
		{
			@this.AddRef(in cells);
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
	[UnsafeAccessor(UnsafeAccessorKind.Method)]
	private static extern int GetNewCapacity(List<Grid> @this, int capacity);

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
}
