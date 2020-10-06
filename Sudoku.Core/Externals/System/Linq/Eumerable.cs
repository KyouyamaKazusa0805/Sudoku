using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

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
		/// <typeparam name="TNotNull">The element type.</typeparam>
		/// <typeparam name="TComparable">The comparing type.</typeparam>
		/// <param name="this">The elements to search the minimal one.</param>
		/// <param name="selector">The selector.</param>
		/// <returns>
		/// The result value. If the collection does not have a minimal element,
		/// the result will be the default value, where it will be <see langword="null"/>, which
		/// is decided in the element type.
		/// </returns>
		/// <remarks>
		/// Note that the return value can be <see langword="null"/> if the list cannot be found
		/// the specified element, but this type parameter is named <typeparamref name="TNotNull"/>
		/// because each element cannot be <see langword="null"/> (either value types or non-<see langword="null"/>
		/// reference types).
		/// </remarks>
		public static unsafe TNotNull GetElementByMinSelector<TNotNull, TComparable>(
			this IEnumerable<TNotNull> @this, delegate* managed<TNotNull, TComparable> selector)
			where TNotNull : notnull where TComparable : IComparable<TComparable> =>
			(from element in @this orderby selector(element) select element).FirstOrDefault();

		/// <summary>
		/// Check whether the specified collection is empty (no elements in it).
		/// </summary>
		/// <typeparam name="T">The type of the element.</typeparam>
		/// <param name="this">(<see langword="this"/> parameter) The list.</param>
		/// <returns>A <see cref="bool"/> value indicating that.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool None<T>(this IEnumerable<T> @this) => !@this.Any();

		/// <summary>
		/// Check whether the specified list has only one element.
		/// </summary>
		/// <typeparam name="TNotNull">The type of the element.</typeparam>
		/// <param name="this">(<see langword="this"/> parameter) The list.</param>
		/// <returns>A <see cref="bool"/> result.</returns>
		public static bool HasOnlyOneElement<TNotNull>(this IEnumerable<TNotNull> @this) where TNotNull : notnull
		{
			if (@this.None())
			{
				return false;
			}

			int count = 0;
			using var enumerator = @this.GetEnumerator();
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
