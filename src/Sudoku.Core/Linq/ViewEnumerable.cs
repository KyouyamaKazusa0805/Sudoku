using System.Runtime.CompilerServices;
using Sudoku.Rendering;

namespace Sudoku.Linq;

/// <summary>
/// Represents with LINQ methods for <see cref="View"/> instances.
/// </summary>
/// <seealso cref="View"/>
public static class ViewEnumerable
{
	/// <summary>
	/// Filters the view nodes, only returns nodes of type <typeparamref name="T"/>.
	/// </summary>
	/// <typeparam name="T">The type of the node.</typeparam>
	/// <returns>The target collection of element type <typeparamref name="T"/>.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ViewNodeIterator<T> OfType<T>(this View @this) where T : ViewNode => new(@this.GetEnumerator());
}
