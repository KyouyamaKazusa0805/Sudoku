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
	}
}
