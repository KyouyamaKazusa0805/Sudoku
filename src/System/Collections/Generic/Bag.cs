#undef USE_EQUALITY_COMPARER

namespace System.Collections.Generic;

/// <summary>
/// Defines a data structure that stores the items without any remove operations,
/// except removing the last element.
/// </summary>
/// <remarks>
/// This data structure is nearly same as <see cref="List{T}"/>. However, this collection
/// doesn't contain any removing operations, except the method <see cref="Remove"/>,
/// which means to remove the last element in the collection.
/// </remarks>
/// <typeparam name="T">The type of each element.</typeparam>
/// <seealso cref="List{T}"/>
#if DEBUG
[DebuggerDisplay($$"""{{{nameof(GetDebuggerDisplay)}}(),nq}""")]
#endif
public ref partial struct Bag<T>
{
	/// <summary>
	/// The capacity of the current collection.
	/// </summary>
	private int _capacity;

	/// <summary>
	/// The inner collection.
	/// </summary>
	private T[] _values;


	/// <summary>
	/// Initializes a <see cref="Bag{T}"/> instance via the default capacity 8.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Bag()
	{
		_capacity = 8;
		_values = ArrayPool<T>.Shared.Rent(_capacity);
	}

	/// <summary>
	/// Initializes a <see cref="Bag{T}"/> instance via the specified list of elements to be added.
	/// </summary>
	/// <param name="elements">The list of elements to be added.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Bag(IEnumerable<T> elements) : this() => AddRange(elements);


	/// <summary>
	/// Indicates the number of elements having been stored in the collection.
	/// </summary>
	public int Count { get; private set; } = 0;


	/// <summary>
	/// Gets the element at the specified index.
	/// </summary>
	/// <param name="index">The index whose corresponding element will be fetched.</param>
	/// <returns>The element value.</returns>
	/// <exception cref="IndexOutOfRangeException">Throws when the index is out of range.</exception>
	public readonly T this[int index]
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _values[index >= Count ? throw new IndexOutOfRangeException() : index];
	}


	/// <summary>
	/// Determine whether the collection contains the specified element.
	/// </summary>
	/// <param name="element">The element to get from the collection.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	public readonly bool Contains(T element)
	{
#if !USE_EQUALITY_COMPARER
		nuint size = (nuint)Marshal.SizeOf<T>();
#endif

		for (int i = 0; i < Count; i++)
		{
#if USE_EQUALITY_COMPARER
			if (EqualityComparer<T>.Default.Equals(_values[i], element))
			{
				return true;
			}
#else
			bool areSame = true;
			for (nuint j = 0; j < size; j++)
			{
				byte v1 = Unsafe.As<T, byte>(ref Unsafe.AddByteOffset(ref _values[i], j));
				byte v2 = Unsafe.As<T, byte>(ref Unsafe.AddByteOffset(ref element, j));
				if (v1 != v2)
				{
					areSame = false;
					break;
				}
			}
			if (areSame)
			{
				return true;
			}
#endif
		}

		return false;
	}

	/// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly Enumerator GetEnumerator() => new(_values, Count);

	/// <summary>
	/// Adds the new element into the collection and return the collection added, immutely.
	/// </summary>
	/// <param name="element">The element to be added.</param>
	/// <returns>The newer collection.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly Bag<T> ImmutelyAdd(T element)
	{
		var result = new Bag<T>(_values[0..Count]);
		result.Add(element);

		return result;
	}

	/// <summary>
	/// Try to get the last element in this collection, without any removing operation.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly T Peek() => _values[Count - 1];

	/// <summary>
	/// Gets the final array that is a copy.
	/// </summary>
	/// <returns>The array that is a copy.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly T[] ToArray()
	{
		var result = new T[Count];
		Unsafe.CopyBlock(
			ref Unsafe.As<T, byte>(ref result[0]),
			ref Unsafe.As<T, byte>(ref _values[0]),
			(uint)(Marshal.SizeOf<T>() * Count));

		return result;
	}

	/// <summary>
	/// Add an element into the collection.
	/// </summary>
	/// <param name="element">The element to be added.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Add(T element)
	{
		if (Count >= _capacity)
		{
			_capacity <<= 1;

			var newer = ArrayPool<T>.Shared.Rent(_capacity);
			Unsafe.CopyBlock(
				ref Unsafe.As<T, byte>(ref newer[0]),
				ref Unsafe.As<T, byte>(ref _values[0]),
				(uint)(Marshal.SizeOf<T>() * Count));

			ArrayPool<T>.Shared.Return(_values);

			_values = newer;
		}

		_values[Count++] = element;
	}

	/// <summary>
	/// Adds a serial of elements into the collection, one by one.
	/// </summary>
	/// <param name="elements">The lements to be added.</param>
	public void AddRange(IEnumerable<T> elements)
	{
		foreach (var element in elements)
		{
			Add(element);
		}
	}

	/// <summary>
	/// Removes the last element.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Remove() => Count--;

	/// <summary>
	/// Release the memory or return the rent memory.
	/// </summary>
	public void Dispose()
	{
		ArrayPool<T>.Shared.Return(_values);

		this = default;
	}

#if DEBUG
	/// <summary>
	/// Gets the display view of the type on debugger.
	/// </summary>
	/// <returns>The string representation of the instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private readonly string GetDebuggerDisplay() =>
		$$"""
		Bag { ElementType = {{typeof(T)}}, Count = {{Count}} }
		""";
#endif
}
