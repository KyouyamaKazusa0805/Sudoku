namespace System.Collections.Generic;

/// <summary>
/// Defines a node that is used for a linked list.
/// </summary>
/// <typeparam name="TUnmanaged">The type of the element.</typeparam>
public unsafe struct ValueLinkedListNode<TUnmanaged> where TUnmanaged : unmanaged
{
	/// <summary>
	/// Indicates the pointer value of the next node.
	/// </summary>
	/// <remarks>
	/// Although the field is modified as <see langword="internal"/>, the field can only be used
	/// as <see langword="private"/> one, or called by the type <see cref="ValueLinkedList{T}"/>.
	/// </remarks>
	/// <seealso cref="ValueLinkedList{T}"/>
	internal ValueLinkedListNode<TUnmanaged>* _pNext = null;

	/// <summary>
	/// Indicates the pointer value of the previous node.
	/// </summary>
	/// <remarks>
	/// Although the field is modified as <see langword="internal"/>, the field can only be used
	/// as <see langword="private"/> one, or called by the type <see cref="ValueLinkedList{T}"/>.
	/// </remarks>
	/// <seealso cref="ValueLinkedList{T}"/>
	internal ValueLinkedListNode<TUnmanaged>* _pPrevious = null;


	/// <summary>
	/// Initializes a <see cref="ValueLinkedListNode{T}"/> instance via the specified data.
	/// </summary>
	/// <param name="data">The data.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ValueLinkedListNode(TUnmanaged data) => Data = data;

	/// <summary>
	/// Initializes a <see cref="ValueLinkedListNode{T}"/> instance via the specified data and the pointer value.
	/// </summary>
	/// <param name="data">The data.</param>
	/// <param name="pPrevious">The pointer instance that points to the previous node.</param>
	/// <param name="pNext">The pointer instance that points to the next node.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ValueLinkedListNode(
		TUnmanaged data, ValueLinkedListNode<TUnmanaged>* pPrevious, ValueLinkedListNode<TUnmanaged>* pNext)
	{
		Data = data;
		_pPrevious = pPrevious;
		_pNext = pNext;
	}


	/// <summary>
	/// Indicates the data.
	/// </summary>
	public TUnmanaged Data { get; }

	/// <summary>
	/// Returns the reference of the previous node.
	/// </summary>
	public readonly ref readonly ValueLinkedListNode<TUnmanaged> Previous
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => ref *_pPrevious;
	}

	/// <summary>
	/// Returns the reference of the next node.
	/// </summary>
	public readonly ref readonly ValueLinkedListNode<TUnmanaged> Next
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => ref *_pNext;
	}

	/// <summary>
	/// Returns the raw pointer value that points to the previous node.
	/// </summary>
	public readonly ValueLinkedListNode<TUnmanaged>* PreviousUnsafe
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _pPrevious;
	}

	/// <summary>
	/// Returns the raw pointer value that points to the next node.
	/// </summary>
	public readonly ValueLinkedListNode<TUnmanaged>* NextUnsafe
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _pNext;
	}


	/// <summary>
	/// Invalidate the node, to revert the pointers to <see langword="null"/>.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Invalidate()
	{
		_pNext = null;
		_pPrevious = null;
	}
}
