namespace System.Collections.Generic;

partial struct ValueLinkedList<TUnmanaged>
{
	/// <summary>
	/// Defines the enumerator that can iterate on <see cref="ValueLinkedListNode{T}"/>s.
	/// </summary>
	/// <seealso cref="ValueLinkedListNode{T}"/>
	public unsafe ref partial struct Enumerator
	{
		/// <summary>
		/// Indicates the linked list.
		/// </summary>
		private readonly ValueLinkedList<TUnmanaged>* _list;

		/// <summary>
		/// Indicates the pointer value of the current node being iterated.
		/// </summary>
		private ValueLinkedListNode<TUnmanaged>* _node;

		/// <summary>
		/// Indicates the index.
		/// </summary>
		private int _index;


		/// <summary>
		/// Initializes an <see cref="Enumerator"/> instance via the specified linked list.
		/// </summary>
		/// <param name="list">The list.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal Enumerator(ValueLinkedList<TUnmanaged>* list)
		{
			_list = list;
			_node = list->FirstUnsafe;
			_index = 0;
		}


		/// <inheritdoc cref="IEnumerator.Current"/>
		public TUnmanaged Current { get; private set; } = default;


		/// <inheritdoc cref="IEnumerator.MoveNext"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool MoveNext()
		{
			if (_node == null)
			{
				_index = _list->Count + 1;
				return false;
			}

			++_index;
			Current = _node->Data;
			_node = _node->_pNext;
			if (_node == _list->FirstUnsafe)
			{
				_node = null;
			}

			return true;
		}
	}
}
