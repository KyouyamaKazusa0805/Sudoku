using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace System.Linq
{
	/// <summary>
	/// Provides a set of static methods for querying objects that implement
	/// <see cref="IEnumerable"/> and <see cref="IEnumerable{T}"/>.
	/// </summary>
	/// <remarks>This class has the same function and status with <see cref="Enumerable"/>.</remarks>
	/// <seealso cref="IEnumerable"/>
	/// <seealso cref="IEnumerable{T}"/>
	/// <seealso cref="Enumerable"/>
	public static class EnumerableExtensions
	{
		/// <summary>
		/// Check whether the specified list has only one element.
		/// </summary>
		/// <typeparam name="T">The type of the element.</typeparam>
		/// <param name="this">The list.</param>
		/// <returns>A <see cref="bool"/> result.</returns>
		public static bool HasOnlyOneElement<T>(this IEnumerable<T> @this)
		{
			if (!@this.Any())
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

		/// <summary>
		/// Check whether the list contains the element that is in the specified array.
		/// </summary>
		/// <typeparam name="TEquatable">The type of the element to check.</typeparam>
		/// <param name="this">The list.</param>
		/// <param name="elements">
		/// The array that contains the target elements.
		/// </param>
		/// <returns>A <see cref="bool"/> result indicating that.</returns>
		public static bool Contains<TEquatable>(this IEnumerable<TEquatable> @this, params TEquatable[] elements)
			where TEquatable : IEquatable<TEquatable>
		{
			if (elements.Length == 1)
			{
				return Enumerable.Contains(@this, elements[0]);
			}

			foreach (var elementToCompare in @this)
			{
				foreach (var element in elements)
				{
					if (elementToCompare.Equals(element))
					{
						return true;
					}
				}
			}

			return false;
		}

		/// <summary>
		/// Check whether the collection contains any elements that match the specified type.
		/// </summary>
		/// <typeparam name="T">The type to check.</typeparam>
		/// <param name="this">The list.</param>
		/// <returns>A <see cref="bool"/> result indicating that.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool ContainsType<T>(this IEnumerable<T> @this) => @this.OfType<T>().Any();

		/// <summary>
		/// Get the index of the whole list, whose corresponding element is satisfy the specified condition.
		/// </summary>
		/// <typeparam name="T">The type of each element.</typeparam>
		/// <param name="this">The list.</param>
		/// <param name="predicate">The condition to check.</param>
		/// <returns>
		/// The index of the element satisfied the condition. If the list can't find that element,
		/// return -1 as the result.
		/// </returns>
		public static int IndexOf<T>(this IEnumerable<T> @this, Predicate<T> predicate)
		{
			if (!@this.Any())
			{
				return -1;
			}

			int index = 0;
			foreach (var element in @this)
			{
				if (predicate(element))
				{
					return index;
				}

				index++;
			}

			return -1;
		}
	}
}
