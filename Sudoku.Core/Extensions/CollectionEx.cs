using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Sudoku.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="ICollection{T}"/> and <see cref="IReadOnlyCollection{T}"/>.
	/// </summary>
	/// <seealso cref="ICollection{T}"/>
	/// <seealso cref="IReadOnlyCollection{T}"/>
	public static class CollectionEx
	{
		/// <summary>
		/// Adds the elements of the specified collection to the end of the
		/// <see cref="ICollection{T}"/>.
		/// </summary>
		/// <typeparam name="TNotNull">
		/// The type of each element. Should be not <see langword="null"/>.
		/// </typeparam>
		/// <param name="this">(<see langword="this"/> parameter) The collection.</param>
		/// <param name="values">
		/// (<see langword="params"/> parameter) The values you want to add to the end of the collection.
		/// </param>
		public static void AddRange<TNotNull>(this ICollection<TNotNull> @this, params TNotNull[] values)
			where TNotNull : notnull => @this.AddRange(values.AsEnumerable());

		/// <summary>
		/// Adds the elements of the specified collection to the end of the
		/// <see cref="ICollection{T}"/>.
		/// </summary>
		/// <typeparam name="TNotNull">
		/// The type of each element. Should be not <see langword="null"/>.
		/// </typeparam>
		/// <param name="this">(<see langword="this"/> parameter) The collection.</param>
		/// <param name="values">
		/// The values you want to add to the end of the collection.
		/// </param>
		public static void AddRange<TNotNull>(this ICollection<TNotNull> @this, IEnumerable<TNotNull> values)
			where TNotNull : notnull
		{
			foreach (var value in values)
			{
				@this.Add(value);
			}
		}

		/// <summary>
		/// Adds the elements of the specified collection to the end of the
		/// <see cref="ICollection{T}"/>.
		/// </summary>
		/// <typeparam name="TNotNull">
		/// The type of each element. Should be not <see langword="null"/>.
		/// </typeparam>
		/// <param name="this">(<see langword="this"/> parameter) The collection.</param>
		/// <param name="values">
		/// The values you want to add to the end of the collection.
		/// </param>
		/// <param name="verifyDuplicate">
		/// Indicates whether the method should check duplicating values first.
		/// If so, the value won't add (do nothing).
		/// </param>
		public static void AddRange<TNotNull>(
			this ICollection<TNotNull> @this, IEnumerable<TNotNull> values, bool verifyDuplicate)
			where TNotNull : notnull
		{
			foreach (var value in values)
			{
				if (verifyDuplicate)
				{
					@this.AddIfDoesNotContain(value);
				}
				else
				{
					@this.Add(value);
				}
			}
		}

		/// <summary>
		/// Adds an object to the end of the <see cref="ICollection{T}"/> when
		/// the specified list doesn't contain the specified element.
		/// </summary>
		/// <typeparam name="TNotNull">
		/// The type of all elements. Should be not <see langword="null"/>.
		/// </typeparam>
		/// <param name="this">(<see langword="this"/> parameter) The list.</param>
		/// <param name="item">The item to add.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void AddIfDoesNotContain<TNotNull>(this ICollection<TNotNull> @this, TNotNull item)
			where TNotNull : notnull
		{
			if (!@this.Contains(item))
			{
				@this.Add(item);
			}
		}

		/// <summary>
		/// Check whether two <see cref="ICollection{T}"/>s are equal.
		/// </summary>
		/// <typeparam name="TNotNull">The type of each element.</typeparam>
		/// <param name="this">(<see langword="this"/> parameter) The collection.</param>
		/// <param name="other">Another collection.</param>
		/// <returns>The <see cref="bool"/> value.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static unsafe bool CollectionEquals<TNotNull>(
			this ICollection<TNotNull> @this, ICollection<TNotNull> other) where TNotNull : notnull
		{
			return @this.Count == other.Count && @this.All(&internalEquals, other);

			static bool internalEquals(TNotNull element, in ICollection<TNotNull> other) =>
				other.Contains(element);
		}

		/// <summary>
		/// Get a result collection from a may-be-<see langword="null"/> collection.
		/// If the collection is <see langword="null"/>, the method will return an empty array
		/// of type <typeparamref name="TNotNull"/>.
		/// </summary>
		/// <typeparam name="TNotNull">
		/// The type of each element. The element should be not <see langword="null"/>.
		/// </typeparam>
		/// <param name="this">(<see langword="this"/> parameter) The collection.</param>
		/// <returns>The return collection that can't be <see langword="null"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<TNotNull> NullableCollection<TNotNull>(this IEnumerable<TNotNull>? @this)
			where TNotNull : notnull => @this ?? Array.Empty<TNotNull>();
	}
}
