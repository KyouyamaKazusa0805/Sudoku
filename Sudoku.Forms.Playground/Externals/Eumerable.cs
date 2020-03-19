using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace System.Linq
{
	/// <summary>
	/// Provides a set of static methods for querying objects that implement
	/// <see cref="IEnumerable"/> and <see cref="IEnumerable{T}"/>.
	/// </summary>
	/// <remarks>
	/// This class has the same function and status with <see cref="Enumerable"/>.
	/// </remarks>
	/// <seealso cref="IEnumerable"/>
	/// <seealso cref="IEnumerable{T}"/>
	/// <seealso cref="Enumerable"/>
	public static class EnumerableEx
	{
		/// <summary>
		/// Get the element whose selection is the minimal one.
		/// </summary>
		/// <typeparam name="TElement">The element type.</typeparam>
		/// <typeparam name="TComparable">The comparing type.</typeparam>
		/// <param name="this">The elements to search the minimal one.</param>
		/// <param name="selector">The selector.</param>
		/// <returns>
		/// The result value. If the collection does not have a minimal element,
		/// the result will be the default value, where it will be <see langword="null"/>, which
		/// is decided in the element type.
		/// </returns>
		public static TElement GetElementByMinSelector<TElement, TComparable>(
			this IEnumerable<TElement> @this, Func<TElement, IComparable<TComparable>> selector) =>
			(from element in @this orderby selector(element) select element).FirstOrDefault();

		/// <summary>
		/// Check whether the specified list has only one element.
		/// </summary>
		/// <typeparam name="TElement">The type of the element.</typeparam>
		/// <param name="this">(<see langword="this"/> parameter) The list.</param>
		/// <returns>A <see cref="bool"/> result.</returns>
		public static bool HasOnlyOneElement<TElement>(this IEnumerable<TElement> @this)
		{
			if (!@this.Any())
			{
				// An empty list.
				return false;
			}

			int count = 0;
			var enumerator = @this.GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (++count >= 2)
				{
					return false;
				}
			}

			return true;
		}
	}
}
