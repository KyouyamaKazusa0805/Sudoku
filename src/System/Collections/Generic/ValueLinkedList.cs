namespace System.Collections.Generic;

/// <summary>
/// Defines a linked list that is a light-weight data structure,
/// storing <typeparamref name="TUnmanaged"/>-typed elements.
/// </summary>
/// <typeparam name="TUnmanaged">The type of each element.</typeparam>
public unsafe partial struct ValueLinkedList<TUnmanaged> where TUnmanaged : unmanaged
{
	/// <summary>
	/// Initializes a <see cref="ValueLinkedList{T}"/> instance.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ValueLinkedList()
	{
		FirstUnsafe = null;
		Count = 0;
	}

	/// <summary>
	/// Intializes a <see cref="ValueLinkedList{T}"/> instance via the specified reference value
	/// that references to the first element of an enumerable list of elements to be added.
	/// </summary>
	/// <param name="refListOfNodes">
	/// <para>
	/// The read-only reference value that references to the first element of an enumerable list
	/// of elements to be added.
	/// </para>
	/// <para>
	/// <include
	///     file='../../global-doc-comments.xml'
	///     path='g/csharp7/feature[@name="ref-returns"]/target[@name="in-parameter"]'/>
	/// </para>
	/// </param>
	/// <param name="count">The number of the referenced collection.</param>
	/// <remarks>
	/// <para>
	/// <include
	///     file='../../global-doc-comments.xml'
	///     path='g/csharp7/feature[@name="ref-returns"]/target[@name="method"]'/>
	/// </para>
	/// <para>
	/// Due to the limitation of the keyword, all fixable types are allowed using.
	/// e.g.:
	/// <list type="bullet">
	/// <item><typeparamref name="TUnmanaged"/>[]</item>
	/// <item><see cref="ReadOnlySpan{T}"/> of type <typeparamref name="TUnmanaged"/></item>
	/// </list>
	/// </para>
	/// </remarks>
	public ValueLinkedList(in ValueLinkedListNode<TUnmanaged> refListOfNodes, int count) : this()
	{
		fixed (ValueLinkedListNode<TUnmanaged>* p = &refListOfNodes)
		{
			for (int i = 0; i < count; i++)
			{
				Append(p + i);
			}
		}
	}


	/// <summary>
	/// Indicates the number of elements stored.
	/// </summary>
	public int Count { get; private set; }

	/// <summary>
	/// Returns the reference of the first element in the linked list.
	/// </summary>
	public readonly ref readonly ValueLinkedListNode<TUnmanaged> First
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => ref *FirstUnsafe;
	}

	/// <summary>
	/// Returns the reference of the last element in the linked list.
	/// </summary>
	public readonly ref readonly ValueLinkedListNode<TUnmanaged> Last
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get =>
			ref FirstUnsafe == null
				? ref Unsafe.NullRef<ValueLinkedListNode<TUnmanaged>>()
				: ref *FirstUnsafe->_pPrevious;
	}

	/// <summary>
	/// Returns the raw pointer value of the first element in the linked list.
	/// </summary>
	public ValueLinkedListNode<TUnmanaged>* FirstUnsafe { get; private set; }

	/// <summary>
	/// Returns the raw pointer value of the last element in the linked list.
	/// </summary>
	public readonly ValueLinkedListNode<TUnmanaged>* LastUnsafe
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => FirstUnsafe == null ? null : FirstUnsafe->_pPrevious;
	}


	/// <summary>
	/// Determines whether the linked list contains the node whose value is the specified one.
	/// </summary>
	/// <param name="value">The value to be checked and compared.</param>
	/// <param name="equalityComparer">
	/// The custom comparer method that operates and compares two instances of type <typeparamref name="TUnmanaged"/>,
	/// to determine whether they hold same value. If <see langword="null"/>, the default equality comparer
	/// <see cref="EqualityComparer{T}.Default"/> will be used and invoked.
	/// </param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	/// <seealso cref="EqualityComparer{T}.Default"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool Contains(TUnmanaged value, delegate*<TUnmanaged, TUnmanaged, bool> equalityComparer) =>
		Find(value, equalityComparer) != null;

	/// <summary>
	/// Finds the pointer value corresponding to a node whose data is the specified one.
	/// </summary>
	/// <param name="value">The value to be checked and determined.</param>
	/// <param name="equalityComparer">The equality comparer of type <typeparamref name="TUnmanaged"/>.</param>
	/// <returns>The pointer value corresponding to the found node.</returns>
	public readonly ValueLinkedListNode<TUnmanaged>* Find(
		TUnmanaged value, delegate*<TUnmanaged, TUnmanaged, bool> equalityComparer)
	{
		var node = FirstUnsafe;
		if (node != null)
		{
			do
			{
				if (equalityComparer is null && EqualityComparer<TUnmanaged>.Default.Equals(node->Data, value)
					|| equalityComparer(node->Data, value))
				{
					return node;
				}

				node = node->_pNext;
			} while (node != FirstUnsafe);
		}

		return null;
	}

	/// <summary>
	/// Finds the pointer value corresponding to a node whose data is the specified one,
	/// via the reverse searching order.
	/// </summary>
	/// <param name="value">The value to be checked and determined.</param>
	/// <param name="equalityComparer">The equality comparer of type <typeparamref name="TUnmanaged"/>.</param>
	/// <returns>The pointer value corresponding to the found node.</returns>
	public readonly ValueLinkedListNode<TUnmanaged>* FindLast(
		TUnmanaged value, delegate*<TUnmanaged, TUnmanaged, bool> equalityComparer)
	{
		if (FirstUnsafe == null)
		{
			return null;
		}

		var last = FirstUnsafe->_pPrevious;
		var node = LastUnsafe;
		if (node != null)
		{
			do
			{
				if (equalityComparer == null && EqualityComparer<TUnmanaged>.Default.Equals(node->Data, value)
					|| equalityComparer(node->Data, value))
				{
					return node;
				}

				node = node->_pPrevious;
			} while (node != last);
		}

		return null;
	}

	/// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly Enumerator GetEnumerator()
	{
		fixed (ValueLinkedList<TUnmanaged>* pThis = &this)
		{
			return new(pThis);
		}
	}

	/// <inheritdoc cref="object.ToString"/>
	public override readonly string ToString()
	{
		const string separator = ", ";
		var sb = new StringHandler();
		foreach (var value in this)
		{
			sb.Append(value.ToString() ?? "<null>");
			sb.Append(separator);
		}

		sb.RemoveFromEnd(separator.Length);
		return sb.ToStringAndClear();
	}

	/// <summary>
	/// Appends the specified element to the tail of the collection.
	/// </summary>
	/// <param name="node">The value to be appended.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Append(ValueLinkedListNode<TUnmanaged>* node)
	{
		if (FirstUnsafe == null)
		{
			InternalInsertNodeToEmptyList(node);
		}
		else
		{
			InternalInsertNodeBefore(FirstUnsafe, node);
		}
	}

	/// <summary>
	/// Prepends the specified <see cref="ValueLinkedListNode{T}"/> instance into the collection,
	/// replacing the original first node.
	/// </summary>
	/// <param name="node">The node to be prepended.</param>
	public void Prepend(ValueLinkedListNode<TUnmanaged>* node)
	{
		if (FirstUnsafe == null)
		{
			InternalInsertNodeToEmptyList(node);
		}
		else
		{
			InternalInsertNodeBefore(FirstUnsafe, node);
			FirstUnsafe = node;
		}
	}

	/// <summary>
	/// Adds the specified <see cref="ValueLinkedListNode{T}"/> instance after the node.
	/// </summary>
	/// <param name="node">The node to be searched.</param>
	/// <param name="newNode">The node to be added.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void AddAfter(ValueLinkedListNode<TUnmanaged>* node, ValueLinkedListNode<TUnmanaged>* newNode) =>
		InternalInsertNodeBefore(node->_pNext, newNode);

	/// <summary>
	/// Adds the specified <see cref="ValueLinkedListNode{T}"/> instance before the node.
	/// </summary>
	/// <param name="node">The node to be searched.</param>
	/// <param name="newNode">The node to be added.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void AddBefore(ValueLinkedListNode<TUnmanaged>* node, ValueLinkedListNode<TUnmanaged>* newNode)
	{
		InternalInsertNodeBefore(node, newNode);
		if (node == FirstUnsafe)
		{
			FirstUnsafe = newNode;
		}
	}

	/// <summary>
	/// Removes the element from the collection.
	/// </summary>
	/// <param name="value">The value to be checked.</param>
	/// <param name="equalityComparer">
	/// The method that checks whether two instances of type <typeparamref name="TUnmanaged"/> hold same value.
	/// </param>
	public void Remove(TUnmanaged value, delegate*<TUnmanaged, TUnmanaged, bool> equalityComparer)
	{
		var node = Find(value, equalityComparer);
		if (node != null)
		{
			InternalRemoveNode(node);
		}
	}

	/// <summary>
	/// Removes the node from the collection.
	/// </summary>
	/// <param name="node">The node to be checked.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Remove(ValueLinkedListNode<TUnmanaged>* node) => InternalRemoveNode(node);

	/// <summary>
	/// Removes the first node from the collection.
	/// </summary>
	/// <exception cref="InvalidOperationException">Throws when the collection is empty.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void RemoveFirst()
	{
		if (FirstUnsafe == null)
		{
			throw new InvalidOperationException("The collection is empty.");
		}

		InternalRemoveNode(FirstUnsafe);
	}

	/// <summary>
	/// Removes the last node from the collection.
	/// </summary>
	/// <exception cref="InvalidOperationException">Throws when the collection is empty.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void RemoveLast()
	{
		if (FirstUnsafe == null)
		{
			throw new InvalidOperationException("The collection is empty.");
		}

		InternalRemoveNode(FirstUnsafe->_pPrevious);
	}

	/// <summary>
	/// Clear the linked list.
	/// </summary>
	public void Clear()
	{
		for (var current = FirstUnsafe; current != null;)
		{
			var temp = current;
			current = current->_pNext;
			temp->Invalidate();
		}

		FirstUnsafe = null;
		Count = 0;
	}

	/// <summary>
	/// Inserts a node into the empty list. This method is only used if the collection is empty at present.
	/// </summary>
	/// <param name="newNode">The node to be added.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void InternalInsertNodeToEmptyList(ValueLinkedListNode<TUnmanaged>* newNode)
	{
		newNode->_pNext = newNode;
		newNode->_pPrevious = newNode;
		FirstUnsafe = newNode;
		Count++;
	}

	/// <summary>
	/// Inserts a node into the target position which is before the specified node.
	/// </summary>
	/// <param name="node">The node as the relative position to be added before.</param>
	/// <param name="newNode">The node to be added.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void InternalInsertNodeBefore(
		ValueLinkedListNode<TUnmanaged>* node, ValueLinkedListNode<TUnmanaged>* newNode)
	{
		newNode->_pNext = node;
		newNode->_pPrevious = node->_pPrevious;
		node->_pPrevious->_pNext = newNode;
		node->_pPrevious = newNode;
		Count++;
	}

	/// <summary>
	/// Removes the specified node from the current collection.
	/// </summary>
	/// <param name="node">The node to be removed.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void InternalRemoveNode(ValueLinkedListNode<TUnmanaged>* node)
	{
		if (node->_pNext == node)
		{
			FirstUnsafe = null;
		}
		else
		{
			node->_pNext->_pPrevious = node->_pPrevious;
			node->_pPrevious->_pNext = node->_pNext;
			if (FirstUnsafe == node)
			{
				FirstUnsafe = node->_pNext;
			}
		}

		node->Invalidate();
		Count--;
	}
}
