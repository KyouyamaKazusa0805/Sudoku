using System.Diagnostics;
using System.Linq;

namespace System.Collections.Generic
{
	/// <summary>
	/// Provides extension methods on <see cref="Bag{T}"/> and <see cref="IBag{T}"/>.
	/// </summary>
	/// <seealso cref="Bag{T}"/>
	/// <seealso cref="IBag{T}"/>
	[DebuggerStepThrough]
	public static class BagEx
	{
		/// <summary>
		/// Sort the collection.
		/// </summary>
		/// <typeparam name="T">The type of each element.</typeparam>
		/// <param name="this">(<see langword="this"/> parameter) The collection.</param>
		public static void Sort<T>(this IBag<T> @this) where T : notnull
		{
			var list = @this.ToList();
			list.Sort();
			@this.Clear();
			@this.AddRange(list);
		}

		/// <summary>
		/// Sort the collection using the specified comparison.
		/// </summary>
		/// <typeparam name="T">The type of each element.</typeparam>
		/// <param name="this">(<see langword="this"/> parameter) The collection.</param>
		/// <param name="comparison">The comparison.</param>
		public static void Sort<T>(this IBag<T> @this, Comparison<T> comparison)
			where T : notnull
		{
			var list = @this.ToList();
			list.Sort(comparison);
			@this.Clear();
			@this.AddRange(list);
		}

		/// <summary>
		/// Sort the collection using the specified comparison.
		/// </summary>
		/// <typeparam name="T">The type of each element.</typeparam>
		/// <param name="this">(<see langword="this"/> parameter) The collection.</param>
		/// <param name="comparer">The comparer.</param>
		/// <remarks>
		/// If you want to make the <paramref name="comparer"/> keep <see langword="null"/> value,
		/// please use <see cref="Sort{T}(IBag{T})"/> instead.
		/// </remarks>
		/// <seealso cref="Sort{T}(IBag{T})"/>
		public static void Sort<T>(this IBag<T> @this, IComparer<T>? comparer)
			where T : notnull
		{
			var list = @this.ToList();
			list.Sort(comparer);
			@this.Clear();
			@this.AddRange(list);
		}
	}
}
