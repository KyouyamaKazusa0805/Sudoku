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
		/// The result value. If the collection doesn't have a minimal element,
		/// the result will be the default value, where it will be <see langword="null"/>, which
		/// is decided in the element type.
		/// </returns>
		/// <remarks>
		/// Note that the return value can be <see langword="null"/> if the list can't be found
		/// the specified element, but this type parameter is named <typeparamref name="TNotNull"/>
		/// because each element can't be <see langword="null"/> (either value types or non-<see langword="null"/>
		/// reference types).
		/// </remarks>
		public static unsafe TNotNull? GetElementByMinSelector<TNotNull, TComparable>(
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
		public static bool None<T>(this IEnumerable<T?> @this) => !@this.Any();

		/// <summary>
		/// Determines whether all elements of a sequence satisfy a condition
		/// specified as <paramref name="selector"/>.
		/// </summary>
		/// <typeparam name="T">The type of each element.</typeparam>
		/// <param name="this">(<see langword="this"/> parameter) The list.</param>
		/// <param name="selector">The selector, specified a function pointer.</param>
		/// <returns>The result indicating whether all values satisfy the condition.</returns>
		public static unsafe bool All<T>(this IEnumerable<T> @this, delegate* managed<T, bool> selector)
		{
			foreach (var element in @this)
			{
				if (!selector(element))
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Determines whether all elements of a sequence satisfy a condition
		/// specified as <paramref name="selector"/>. In addition, the method allows you
		/// pass another <see langword="in"/> parameter to participate in checking.
		/// </summary>
		/// <typeparam name="TElement">The type of each element.</typeparam>
		/// <typeparam name="TOther">The type of the another value to participate in checking.</typeparam>
		/// <param name="this">(<see langword="in"/> parameter) The list.</param>
		/// <param name="selector">The selector.</param>
		/// <param name="value">The value to participate in checking.</param>
		/// <returns>The result indicating whether all values satisfy the condition.</returns>
		public static unsafe bool All<TElement, TOther>(this IEnumerable<TElement> @this,
			delegate* managed<TElement, in TOther, bool> selector, in TOther value) where TOther : struct
		{
			foreach (var element in @this)
			{
				if (!selector(element, value))
				{
					return false;
				}
			}

			return true;
		}

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
