using System.Runtime.CompilerServices;

namespace System.Collections.Generic
{
	/// <summary>
	/// Provides a basic only-in collection.
	/// </summary>
	/// <typeparam name="T">
	/// The type of each element. The type should not be <see langword="null"/>.
	/// </typeparam>
	public interface IBag<T> : IReadOnlyCollection<T>
	{
		/// <summary>
		/// To get an element with the specified index.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <returns>The element.</returns>
		T this[int index] { get; }


		/// <summary>
		/// Add the element into the collection.
		/// </summary>
		/// <param name="item">The element.</param>
		void Add(T item);

		/// <summary>
		/// To clear all elements.
		/// </summary>
		void Clear();

		/// <summary>
		/// Indicates whether the collection contains the specified item.
		/// </summary>
		/// <param name="item">The element.</param>
		/// <returns>A <see cref="bool"/> value indicating that.</returns>
		bool Contains(T item);
	}
}
