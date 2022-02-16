using static System.Numerics.BitOperations;

namespace Sudoku.UI.Drawing.Shapes;

/// <summary>
/// Defines a custom collection that stores the <see cref="DrawingElement"/>s.
/// </summary>
internal sealed partial class DrawingElementBag :
	IReadOnlyCollection<DrawingElement>,
	IReadOnlyList<DrawingElement>,
	IEnumerable,
	IEnumerable<DrawingElement>
{
	/// <summary>
	/// <para>Defines the inner elements.</para>
	/// <para>
	/// The reference of the current field may be modified if adding a new element into it
	/// but it being already full.
	/// </para>
	/// </summary>
	private DrawingElement[] _elements;

	/// <summary>
	/// Indicates the capacity value.
	/// </summary>
	private uint _capacity;


	/// <summary>
	/// Initializes a <see cref="DrawingElement"/> instance with the default capacity value 16.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public DrawingElementBag() : this(16U)
	{
	}

	/// <summary>
	/// Initializes a <see cref="DrawingElementBag"/> instance
	/// via the specified list of <see cref="DrawingElement"/>s to add into the current collection.
	/// </summary>
	/// <param name="elements">The list of <see cref="DrawingElement"/>s.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public DrawingElementBag(DrawingElement[] elements) : this(RoundUpToPowerOf2((uint)elements.Length)) =>
		Array.Copy(elements, _elements, elements.Length);

	/// <summary>
	/// Initializes a <see cref="DrawingElementBag"/> instance
	/// via the specified list of <see cref="DrawingElement"/>s to add into the current collection.
	/// </summary>
	/// <param name="elements">The list of <see cref="DrawingElement"/>s.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public DrawingElementBag(IEnumerable<DrawingElement> elements) : this(16U) => AddRange(elements);

	/// <summary>
	/// Initializes a <see cref="DrawingElementBag"/> instance via the specified capacity.
	/// </summary>
	/// <param name="capacity">
	/// The capacity of the collection to be initialized. In other words, this parameter indicates
	/// how many elements can be stored into the current collection.
	/// </param>
	/// <remarks>
	/// The argument <paramref name="capacity"/> must be the power of 2;
	/// otherwise, the method will call <see cref="RoundUpToPowerOf2(uint)"/> to make the value
	/// be the power of 2.
	/// </remarks>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private DrawingElementBag(uint capacity) =>
		_elements = new DrawingElement[_capacity = RoundUpToPowerOf2(capacity)];


	/// <summary>
	/// Indicates the capacity of the current collection. The value may be greater than <see cref="Count"/>.
	/// </summary>
	/// <seealso cref="Count"/>
	public int Capacity
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (int)_capacity;
	}

	/// <summary>
	/// Indicates the number of <see cref="DrawingElement"/>s being stored in the current collection.
	/// </summary>
	public int Count { get; private set; }


	/// <summary>
	/// Gets the <see cref="DrawingElement"/> instance at the specified index.
	/// </summary>
	/// <param name="index">The index. Should be less than <see cref="Count"/>.</param>
	/// <returns>The <see cref="DrawingElement"/> element at the specified index.</returns>
	public DrawingElement this[int index]
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => index >= Count ? throw new ArgumentOutOfRangeException(nameof(index)) : _elements[index];
	}


	/// <summary>
	/// Adds the specified element into the current collection.
	/// </summary>
	/// <param name="element">The element to be added.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Add(DrawingElement element)
	{
		EnsureCapacity();

		_elements[Count++] = element;
	}

	/// <summary>
	/// Adds the specified list of elements into the current collection.
	/// </summary>
	/// <param name="elements">The elements to be added.</param>
	public void AddRange(IEnumerable<DrawingElement> elements)
	{
		foreach (var element in elements)
		{
			Add(element);
		}
	}

	/// <summary>
	/// Determine whether the collection has been stored the instance whose value is equal to the specified one.
	/// </summary>
	/// <param name="element">The element to compare.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	public bool Contains<TDrawingElement>([NotNullWhen(true)] TDrawingElement? element) where TDrawingElement : DrawingElement
	{
		foreach (var e in this)
		{
			if (e == element)
			{
				return true;
			}
		}

		return false;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString() => $"{nameof(DrawingElementBag)} {{ {nameof(Count)} = {Count} }}";

	/// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Enumerator GetEnumerator() => new(_elements, Count);

	/// <summary>
	/// Slices the current collection and only gets the specified number of elements from the basic collection,
	/// from the first element.
	/// </summary>
	/// <param name="count">The desired number of elements.</param>
	/// <returns>The list of the collection.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public DrawingElementBag Slice(int count) => new(_elements[..count]);

	/// <summary>
	/// Slices the current collection and only gets the specified number of elements from the basic collection,
	/// at the specified index as the beginning.
	/// </summary>
	/// <param name="start">The desired start index.</param>
	/// <param name="count">The desired number of elements.</param>
	/// <returns>The list of the collection.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public DrawingElementBag Slice(int start, int count) => new(_elements[start..(count - start)]);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<DrawingElement>)this).GetEnumerator();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	IEnumerator<DrawingElement> IEnumerable<DrawingElement>.GetEnumerator() =>
		((IEnumerable<DrawingElement>)_elements[..Count]).GetEnumerator();

	/// <summary>
	/// Ensures the capacity, allowing new element being added into the current collection.
	/// If the collection has been already full, the method will re-allocate the memory to allow
	/// more elements stored into the current collection.
	/// </summary>
	/// <exception cref="OutOfMemoryException">
	/// Throws when the size of the collection is greater than or equals 32767.
	/// </exception>
	private void EnsureCapacity()
	{
		if (_capacity >= short.MaxValue)
		{
			throw new OutOfMemoryException("The collection uses the maximum memory size and no longer can be expanded.");
		}

		if (_capacity <= Count + 1)
		{
			_capacity <<= 1;
			Array.Resize(ref _elements, (int)_capacity);
		}
	}
}
