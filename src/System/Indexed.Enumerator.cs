namespace System;

public partial struct Indexed<T>
{
	/// <summary>
	/// Represents an enumerator that can iterate on each element of the sequence, by the specified length.
	/// </summary>
	/// <param name="indexed">The index.</param>
	/// <param name="_length">Indicates the length to be iterated.</param>
	public ref struct Enumerator(Indexed<T> indexed, int _length) : ILocalEnumerator<Enumerator>
	{
		/// <summary>
		/// Indicates the backing field.
		/// </summary>
		private readonly Indexed<T> _indexed = indexed;

		/// <summary>
		/// Indicates the current position.
		/// </summary>
		private int _pos = -1;


		/// <inheritdoc cref="IEnumerator.Current"/>
		public readonly unsafe T Current => _indexed.Pointer[_pos];

		/// <inheritdoc/>
		public readonly unsafe T UnsafePrevious => _indexed.Pointer[_pos - 1];

		/// <inheritdoc/>
		public readonly unsafe T UnsafeNext => _indexed.Pointer[_pos + 1];

		/// <inheritdoc/>
		readonly object? IEnumerator.Current => Current;

		/// <inheritdoc/>
		readonly int ILocalEnumerator<Enumerator>.Pos => _pos;

		/// <inheritdoc/>
		readonly int ILocalEnumerator<Enumerator>.Length => _length;


		/// <inheritdoc/>
		public readonly T this[int index] => _indexed[index];

		/// <inheritdoc/>
		public readonly T this[int index, IndexingMode mode] => _indexed[index, mode];


		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public readonly Enumerator GetEnumerator() => this;

		/// <inheritdoc/>
		public readonly T[] ToArray()
		{
			var list = new List<T>(_length);
			foreach (var element in this)
			{
				list.Add(element);
			}
			return [.. list];
		}

		/// <inheritdoc cref="IEnumerator.MoveNext"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool MoveNext() => ++_pos < _length;

		/// <inheritdoc/>
		readonly void IDisposable.Dispose() { }

		/// <inheritdoc/>
		[DoesNotReturn]
		readonly void IEnumerator.Reset() => throw new NotImplementedException();

		/// <inheritdoc/>
		readonly IEnumerator IEnumerable.GetEnumerator() => ToArray().GetEnumerator();

		/// <inheritdoc/>
		readonly IEnumerator<T> IEnumerable<T>.GetEnumerator() => ((IEnumerable<T>)ToArray()).GetEnumerator();
	}
}
