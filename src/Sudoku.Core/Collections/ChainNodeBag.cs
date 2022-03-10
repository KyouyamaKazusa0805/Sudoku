namespace Sudoku.Collections;

/// <summary>
/// Defines a data structure that stores a set of chain nodes.
/// </summary>
public struct ChainNodeBag
{
	/// <summary>
	/// Indicates the capacity.
	/// </summary>
	private int _capacity;

	/// <summary>
	/// Indicates the inner data structure.
	/// </summary>
	private ChainNode[] _chainNodes;


	/// <summary>
	/// Creates a <see cref="ChainNodeBag"/> instance via the default capacity 16.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ChainNodeBag()
	{
		Count = 0;
		_capacity = 16;
		_chainNodes = new ChainNode[_capacity];
	}


	/// <summary>
	/// Indicates the length of the bag.
	/// </summary>
	public int Count { get; private set; }

	/// <summary>
	/// Gets the chain node at the specified index.
	/// </summary>
	/// <param name="index">The index.</param>
	/// <returns>The chain node at the specified index.</returns>
	public readonly ChainNode this[int index]
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _chainNodes[index];
	}


	/// <summary>
	/// Adds a <see cref="ChainNode"/> into the collection.
	/// </summary>
	/// <param name="next">The next node to be added into.</param>
	public void Add(ChainNode next)
	{
		if (Count == _capacity)
		{
			_capacity <<= 1;
			Array.Resize(ref _chainNodes, _capacity);
		}

		_chainNodes[Count++] = next;
	}
}
