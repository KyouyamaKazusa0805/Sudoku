namespace Sudoku.Collections;

/// <summary>
/// Defines a table that stores the list of <see cref="HashNode"/>s.
/// </summary>
/// <seealso cref="HashNode"/>
public unsafe struct HashNodeTable
{
	/// <summary>
	/// Indicates the maximum capacity.
	/// </summary>
	private const int MaxCapacity = 10000;


	/// <summary>
	/// Defines a list of map with the capacity <see cref="MaxCapacity"/>, of type <see cref="HashNode"/>.
	/// </summary>
	private readonly HashNode[] _map = new HashNode[MaxCapacity];

	/// <summary>
	/// Indicates the start index.
	/// </summary>
	private int _index = 0;


	/// <summary>
	/// Initializes a <see cref="HashNodeTable"/> instance.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public HashNodeTable()
	{
	}


	/// <summary>
	/// Adds the node into the collection.
	/// </summary>
	/// <param name="node">The node to be added.</param>
	/// <exception cref="OutOfMemoryException">Throws when the list is full.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Add(HashNode node)
	{
		if (_index == MaxCapacity)
		{
			throw new OutOfMemoryException();
		}

		_map[_index++] = node;
	}
}
