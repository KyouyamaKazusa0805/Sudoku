using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace System.Extensions
{
	/// <summary>
	/// Provides a set of static methods for querying objects that implement
	/// <see cref="IEnumerable"/> and <see cref="IEnumerable{T}"/>.
	/// </summary>
	/// <remarks>This class has the same function and status with <see cref="Enumerable"/>.</remarks>
	/// <seealso cref="IEnumerable"/>
	/// <seealso cref="IEnumerable{T}"/>
	/// <seealso cref="Enumerable"/>
	public static class EnumerableEx
	{
		/// <summary>
		/// Check whether the specified list has only one element.
		/// </summary>
		/// <typeparam name="T">The type of the element.</typeparam>
		/// <param name="this">(<see langword="this"/> parameter) The list.</param>
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
		/// Get the index of the whole list, whose corresponding element is satisfy the specified condition.
		/// </summary>
		/// <typeparam name="T">The type of each element.</typeparam>
		/// <param name="this">(<see langword="this"/> parameter) The list.</param>
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
