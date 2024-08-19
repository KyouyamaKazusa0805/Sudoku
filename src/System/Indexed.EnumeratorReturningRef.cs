namespace System;

public partial struct Indexed<T>
{
	/// <inheritdoc cref="Enumerator"/>
	public ref struct EnumeratorReturningRef(Indexed<T> indexed, int _length) : ILocalEnumerator<EnumeratorReturningRef>
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
		public readonly unsafe ref readonly T Current => ref _indexed.Pointer[_pos];

		/// <summary>
		/// Indicates the next element. <b>This property may cause "out of bound" critical problem.</b>
		/// </summary>
		public readonly unsafe ref readonly T UnsafeNext => ref _indexed.Pointer[_pos + 1];

		/// <summary>
		/// Indicates the previous element. <b>This property may cause "out of bound" critical problem.</b>
		/// </summary>
		public readonly unsafe ref readonly T UnsafePrevious => ref _indexed.Pointer[_pos - 1];

		/// <inheritdoc/>
		readonly object? IEnumerator.Current => Current;

		/// <inheritdoc/>
		readonly int ILocalEnumerator<EnumeratorReturningRef>.Pos => _pos;

		/// <inheritdoc/>
		readonly int ILocalEnumerator<EnumeratorReturningRef>.Length => _length;

		/// <inheritdoc/>
		readonly T IEnumerator<T>.Current => Current;

		/// <inheritdoc/>
		readonly T ILocalEnumerator<EnumeratorReturningRef>.UnsafeNext => UnsafeNext;

		/// <inheritdoc/>
		readonly T ILocalEnumerator<EnumeratorReturningRef>.UnsafePrevious => UnsafePrevious;


		/// <inheritdoc cref="ILocalEnumerator{TSelf}.this[int]"/>
		public readonly ref readonly T this[int index] => ref _indexed[index];

		/// <inheritdoc cref="ILocalEnumerator{TSelf}.this[int, IndexingMode]"/>
		public readonly ref readonly T this[int index, IndexingMode mode] => ref _indexed[index, mode];

		/// <inheritdoc/>
		readonly T ILocalEnumerator<EnumeratorReturningRef>.this[int index] => _indexed[index];

		/// <inheritdoc/>
		readonly T ILocalEnumerator<EnumeratorReturningRef>.this[int index, IndexingMode mode] => _indexed[index, mode];


		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public readonly EnumeratorReturningRef GetEnumerator() => this;

		/// <inheritdoc/>
		public readonly T[] ToArray()
		{
			var list = new List<T>(_length);
			foreach (ref readonly var element in this)
			{
				list.AddRef(in element);
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
