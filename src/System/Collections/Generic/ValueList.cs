#pragma warning disable IDE0032, IDE0044

using System.Runtime.InteropServices;

namespace System.Collections.Generic;

/// <summary>
/// Defines a value-type sequence list.
/// </summary>
/// <typeparam name="TUnmanaged">The element type.</typeparam>
public unsafe ref partial struct ValueList<TUnmanaged> where TUnmanaged : unmanaged
{
	/// <summary>
	/// Indicates the length of the list.
	/// </summary>
	private byte _capacity;

	/// <summary>
	/// Indicates the current length.
	/// </summary>
	private byte _length = 0;

	/// <summary>
	/// Indicates the pointer that points to the first element.
	/// </summary>
	private TUnmanaged* _startPtr;


	/// <summary>
	/// Initializes a list that stores the specified number of elements.
	/// </summary>
	/// <param name="capacity">The initial capacity.</param>
	/// <remarks>
	/// If you call this constructor to initialize an instance, please append keyword <see langword="using"/>
	/// to implicitly call the dispose method.
	/// </remarks>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ValueList(byte capacity)
	{
		_startPtr = (TUnmanaged*)NativeMemory.Alloc((nuint)sizeof(TUnmanaged) * capacity);
		_capacity = capacity;
	}

	/// <summary>
	/// Initializes a list that stores the specified elements.
	/// </summary>
	/// <param name="list">
	/// <para>The reference to the first element in a certain array to add into.</para>
	/// <para>
	/// <include
	///     file='../../global-doc-comments.xml'
	///     path='g/csharp7/feature[@name="ref-returns"]/target[@name="in-parameter"]'/>
	/// </para>
	/// </param>
	/// <param name="length">The length of the list.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ValueList(in TUnmanaged list, int length)
	{
		fixed (TUnmanaged* p = &list)
		{
			_startPtr = p;
			_capacity = _length = (byte)length;
		}
	}


	/// <summary>
	/// Indicates the length of the list.
	/// </summary>
	public readonly byte Count
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _length;
	}


	/// <summary>
	/// Gets the element from the current list, or sets the element to the current list,
	/// with the specified index.
	/// </summary>
	/// <param name="index">The index.</param>
	/// <returns>The reference to the element at the specified index.</returns>
	public readonly ref TUnmanaged this[byte index]
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => ref _startPtr[index];
	}


	/// <summary>
	/// Adds the element to the current list.
	/// </summary>
	/// <param name="element">The element.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Add(TUnmanaged element)
	{
		if (_length >= _capacity)
		{
			throw new InvalidOperationException("Cannot add because the collection is full.");
		}

		_startPtr[_length++] = element;
	}

	/// <summary>
	/// Removes the last element from the collection.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Remove() => _length--;

	/// <summary>
	/// Removes all elements in this collection.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Clear() => _length = 0;

	/// <summary>
	/// To dispose the current list.
	/// </summary>
	/// <remarks><i>
	/// This method should be called when the constructor <see cref="ValueList{TUnmanaged}(byte)"/> is called.
	/// </i></remarks>
	/// <seealso cref="ValueList{TUnmanaged}(byte)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Dispose()
	{
		NativeMemory.Free(_startPtr);
		_startPtr = null;
	}

	/// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly Enumerator GetEnumerator() => new(this);
}
