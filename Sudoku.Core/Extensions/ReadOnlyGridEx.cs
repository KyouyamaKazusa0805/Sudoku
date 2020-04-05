using System.Diagnostics;
using System.Runtime.CompilerServices;
using Sudoku.Data;

namespace Sudoku.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="IReadOnlyGrid"/>.
	/// </summary>
	/// <seealso cref="IReadOnlyGrid"/>
	[DebuggerStepThrough]
	public static class ReadOnlyGridEx
	{
		/// <summary>
		/// Convert the current read-only grid to mutable one.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The grid.</param>
		/// <returns>The mutable one.</returns>
		/// <remarks>
		/// This method is only use type conversion, so the return value has a same
		/// reference with this specified argument holds.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Grid ToMutable(this IReadOnlyGrid @this) => (Grid)@this;
	}
}
