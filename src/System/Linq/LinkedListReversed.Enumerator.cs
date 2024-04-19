namespace System.Linq;

public partial struct LinkedListReversed<T>
{
	/// <summary>
	/// Indicates the enumerator type of the <see cref="LinkedListReversed{T}"/> instance.
	/// </summary>
	public ref struct Enumerator(LinkedList<T> baseList)
	{
		/// <summary>
		/// Indicates the last node.
		/// </summary>
		private LinkedListNode<T>? _node = baseList.Last;


		/// <inheritdoc cref="IEnumerator.Current"/>
		public readonly ref T Current => ref _node!.ValueRef;


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
	}
}
