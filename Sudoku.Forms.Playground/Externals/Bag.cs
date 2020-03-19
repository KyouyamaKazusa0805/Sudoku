using System.Runtime.CompilerServices;

namespace System.Collections.Generic
{
	/// <summary>
	/// <para>
	/// Encapsulates a collection which only allows user add elements
	/// and get them, but cannot remove any elements in this collection
	/// (i.e. Only-in collection).
	/// </para>
	/// </summary>
	/// <typeparam name="T">
	/// The type of each element. The type should not be <see langword="null"/>.
	/// </typeparam>
	public sealed class Bag<T> : IBag<T>
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
		/// Initializes an instance with an element.
		/// </summary>
		/// <param name="element">An element.</param>
		public Bag(T element) => Add(element);

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

		/// <inheritdoc/>
		public void Clear() => _internalList.Clear();

		/// <inheritdoc/>
		public bool Contains(T item) => _internalList.Contains(item);

		/// <inheritdoc/>
		public IEnumerator<T> GetEnumerator() => _internalList.GetEnumerator();

		/// <inheritdoc/>
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

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
