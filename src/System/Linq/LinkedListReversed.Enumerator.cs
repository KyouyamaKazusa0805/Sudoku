namespace System.Linq;

public partial struct LinkedListReversed<T>
{
	/// <summary>
	/// Indicates the enumerator type of the <see cref="LinkedListReversed{T}"/> instance.
	/// </summary>
	public ref struct Enumerator(LinkedList<T> _baseList) : IEnumerator<T>
	{
		/// <summary>
		/// Indicates the last node.
		/// </summary>
		private LinkedListNode<T>? _node = _baseList.Last;


		/// <inheritdoc cref="IEnumerator.Current"/>
		public readonly ref T Current => ref _node!.ValueRef;

		/// <inheritdoc/>
		readonly object? IEnumerator.Current => _node!.Value;

		/// <inheritdoc/>
		readonly T IEnumerator<T>.Current => _node!.Value;


		/// <inheritdoc cref="IEnumerator.MoveNext"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool MoveNext()
		{
			if (_node is null)
			{
				return false;
			}

			_node = _node.Previous;
			return true;
		}

		/// <inheritdoc/>
		readonly void IDisposable.Dispose() { }

		/// <inheritdoc/>
		[DoesNotReturn]
		readonly void IEnumerator.Reset() => throw new NotImplementedException();
	}
}
