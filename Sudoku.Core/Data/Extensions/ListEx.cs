using System.Collections.Generic;
using System.Diagnostics;

namespace Sudoku.Data.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="IList{T}"/>.
	/// </summary>
	/// <seealso cref="IList{T}"/>
	[DebuggerStepThrough]
	public static class ListEx
	{
		/// <summary>
		/// Remove the last element of the specified list.
		/// </summary>
		/// <typeparam name="T">The type of this element.</typeparam>
		/// <param name="this">(<see langword="this"/> parameter) The list.</param>
		public static void RemoveLastElement<T>(this IList<T> @this) =>
			@this.RemoveAt(@this.Count - 1);
	}
}
