using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace System.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="IList{T}"/>.
	/// </summary>
	/// <seealso cref="IList{T}"/>
	public static class ListEx
	{
		/// <summary>
		/// Remove duplicate items in the specified list.
		/// </summary>
		/// <typeparam name="T">The type of each elements.</typeparam>
		/// <param name="this">(<see langword="this"/> parameter) The list.</param>
		/// <returns>Returns the reference of the argument <paramref name="this"/>.</returns>
		public static List<T> RemoveDuplicateItems<T>(this List<T> @this) where T : IEquatable<T>
		{
			var set = new Set<T>(@this);
			@this.Clear();
			@this.AddRange(set);

			return @this;
		}

		/// <summary>
		/// Remove duplicate items in the specified list.
		/// </summary>
		/// <typeparam name="T">The type of each elements.</typeparam>
		/// <param name="this">(<see langword="this"/> parameter) The list.</param>
		/// <returns>Returns the reference of the argument <paramref name="this"/>.</returns>
		public static IList<T> RemoveDuplicateItems<T>(this IList<T> @this) where T : IEquatable<T>
		{
			var set = new Set<T>(@this);
			@this.Clear();
			@this.AddRange(set);

			return @this;
		}

		/// <summary>
		/// Remove the last element of the specified list, which is equivalent to code:
		/// <code>
		/// list.RemoveAt(list.Count - 1);
		/// </code>
		/// or
		/// <code>
		/// list.RemoveAt(^1); // Call extension method 'RemoveAt'.
		/// </code>
		/// </summary>
		/// <typeparam name="T">The type of each element.</typeparam>
		/// <param name="this">(<see langword="this"/> parameter) The list.</param>
		/// <seealso cref="RemoveAt{T}(IList{T}, in Index)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void RemoveLastElement<T>(this IList<T?> @this) => @this.RemoveAt(@this.Count - 1);

		/// <summary>
		/// Remove at the element in the specified index.
		/// </summary>
		/// <typeparam name="T">The type of each element.</typeparam>
		/// <param name="this">(<see langword="this"/> parameter) The list.</param>
		/// <param name="index">(<see langword="in"/> parameter) The index to remove.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void RemoveAt<T>(this IList<T?> @this, in Index index) =>
			@this.RemoveAt(index.GetOffset(@this.Count));

		/// <summary>
		/// Try to convert the current list to a <see cref="IReadOnlyList{T}"/>.
		/// </summary>
		/// <typeparam name="T">The type of each element.</typeparam>
		/// <param name="this">(<see langword="this"/> parameter) The list.</param>
		/// <returns>The result of the conversion.</returns>
		/// <exception cref="InvalidCastException">
		/// Throws when the specified list is neither <see cref="List{T}"/> nor <typeparamref name="T"/>[].
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IReadOnlyList<T> AsReadOnlyList<T>(this IList<T> @this) => @this switch
		{
			List<T> list => list,
			T[] array => array,
			_ =>
				throw new InvalidCastException(
					$"Can't convert {nameof(@this)} to {typeof(IReadOnlyList<>).Name} " +
					$"because the element isn't a normal {typeof(List<>).Name}.")
		};
	}
}
