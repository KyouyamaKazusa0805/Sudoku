using System.Runtime.CompilerServices;

namespace System.Collections.Generic
{
	/// <summary>
	/// Encapsulates the list that notifies the users while adding an element.
	/// </summary>
	/// <typeparam name="T">The type of the element.</typeparam>
	public sealed class NotifyChangedList<T> : IList<T>, IReadOnlyCollection<T>
	{
		/// <summary>
		/// Indicates the inner list.
		/// </summary>
		private readonly IList<T> _innerList = new List<T>();


		/// <inheritdoc/>
		public int Count
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _innerList.Count;
		}

		/// <inheritdoc/>
		public bool IsReadOnly
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _innerList.IsReadOnly;
		}


		/// <inheritdoc/>
		public T this[int index]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _innerList[index];

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set => _innerList[index] = value;
		}


		/// <summary>
		/// Indicates the event triggers when the element is added.
		/// </summary>
		public event EventHandler? ElementAdded;

		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Add(T item)
		{
			_innerList.Add(item);

			ElementAdded?.Invoke(this, EventArgs.Empty);
		}

		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Clear() => _innerList.Clear();

		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Contains(T item) => _innerList.Contains(item);

		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void CopyTo(T[] array, int arrayIndex) => _innerList.CopyTo(array, arrayIndex);

		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IEnumerator<T> GetEnumerator() => _innerList.GetEnumerator();

		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int IndexOf(T item) => _innerList.IndexOf(item);

		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Insert(int index, T item) => _innerList.Insert(index, item);

		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Remove(T item) => _innerList.Remove(item);

		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void RemoveAt(int index) => _innerList.RemoveAt(index);

		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_innerList).GetEnumerator();
	}
}
