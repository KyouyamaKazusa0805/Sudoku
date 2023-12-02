using System.Runtime.CompilerServices;
using Sudoku.Concepts;

namespace Sudoku.Runtime.CompilerServices;

/// <summary>
/// Represents a list of extension methods that is used by <see cref="List{T}"/> of <see cref="AlmostLockedSet"/> instances.
/// </summary>
/// <seealso cref="AlmostLockedSet"/>
public static class ListAlmostLockedSetsExtensions
{
	/// <summary>
	/// Try to fetch the span of internal array, cut the trailing zeros.
	/// </summary>
	/// <param name="this">The instance.</param>
	/// <returns>The span of <see cref="AlmostLockedSet"/> instances, having cut the trailing zeros.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ReadOnlySpan<AlmostLockedSet> GetSpan(this List<AlmostLockedSet> @this)
#if NET9_0_OR_GREATER
		=> @this.GetItems().AsSpan()[..@this.Count];
#else
		// Here is a bug to be fixed in feature "Unsafe Accessor".
		// https://github.com/dotnet/runtime/issues/92633
		//
		// UnsafeAccessor throws MissingFieldException on generic field (T) in generic type, where genetic T is:
		//
		//   * class (e.g. string)
		//   * struct that has class as a generic parameter (e.g. ArraySegment<string>)
		//
		// Wait for fixing.
		=> @this.ToArray();
#endif

#if NET9_0_OR_GREATER
	/// <summary>
	/// Try to fetch the internal array.
	/// </summary>
	/// <param name="this">The instance.</param>
	/// <returns>The internal array returned.</returns>
	[UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_items")]
	public static extern ref AlmostLockedSet[] GetItems(this List<AlmostLockedSet> @this);
#endif
}
