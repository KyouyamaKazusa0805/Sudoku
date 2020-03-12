using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace System.Collections.Generic
{
	/// <summary>
	/// Provides a basic only-in collection.
	/// </summary>
	/// <typeparam name="T">
	/// The type of each element. The type should not be <see langword="null"/>.
	/// </typeparam>
	public interface IBag<T> : IReadOnlyCollection<T> where T : notnull
	{
		/// <summary>
		/// To get an element with the specified index.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <returns>The element.</returns>
		[NotNull]
		T this[int index] { get; }


		/// <summary>
		/// Add the element into the collection.
		/// </summary>
		/// <param name="item">The element.</param>
		void Add([NotNull] T item);

		/// <summary>
		/// To clear all elements.
		/// </summary>
		void Clear();

		/// <summary>
		/// Indicates whether the collection contains the specified item.
		/// </summary>
		/// <param name="item">The element.</param>
		/// <returns>A <see cref="bool"/> value indicating that.</returns>
		bool Contains([NotNull] T item);

		/// <summary>
		/// Adds an object into the end of the <see cref="IBag{T}"/>
		/// when the specified list does not contain the specified element.
		/// </summary>
		/// <param name="item">The item to add.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AddIfDoesNotContain(T item)
		{
			if (!Contains(item))
			{
				Add(item);
			}
		}

		/// <summary>
		/// Adds a series of elements to the <see cref="IBag{T}"/>.
		/// </summary>
		/// <param name="items">The elements to add.</param>
		public void AddRange(IEnumerable<T> items)
		{
			foreach (var item in items)
			{
				Add(item);
			}
		}
	}
}
