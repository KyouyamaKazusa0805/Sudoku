using Sudoku.Data.Extensions;

namespace System.Collections.Generic
{
	/// <summary>
	/// Encapsulates a collection which only allows user add elements
	/// and get them, but cannot remove one or more elements.
	/// </summary>
	/// <typeparam name="T">
	/// The type of each element. The type should not be <see langword="null"/>.
	/// </typeparam>
	public sealed class Bag<T> : IBag<T> where T : notnull
	{
		/// <summary>
		/// The internal list.
		/// </summary>
		private readonly IList<T> _internalList = new List<T>();


		/// <summary>
		/// Initializes a default <see cref="Bag{T}"/>.
		/// </summary>
		public Bag() { }

		/// <summary>
		/// Initializes an instance with the specified elements.
		/// </summary>
		/// <param name="elements">The elements.</param>
		public Bag(IEnumerable<T> elements) => AddRange(elements);


		/// <inheritdoc/>
		public int Count => _internalList.Count;


		/// <inheritdoc/>
		public T this[int index] => _internalList[index];


		/// <inheritdoc/>
		public void Add(T item) => _internalList.Add(item);

		/// <summary>
		/// Add a serial of elements.
		/// </summary>
		/// <param name="items">A serial of elements.</param>
		public void AddRange(IEnumerable<T> items) => _internalList.AddRange(items);

		/// <inheritdoc/>
		public void Clear() => _internalList.Clear();

		/// <inheritdoc/>
		public bool Contains(T item) => _internalList.Contains(item);

		/// <inheritdoc/>
		public IEnumerator<T> GetEnumerator() => _internalList.GetEnumerator();

		/// <inheritdoc/>
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
