namespace Sudoku.Runtime.MeasuringServices;

/// <summary>
/// Represents a list of cells that form a undirected graph.
/// </summary>
/// <seealso href="https://en.wikipedia.org/wiki/Component_(graph_theory)">Component (Graph Theory)</seealso>
[TypeImpl(TypeImplFlag.Object_Equals | TypeImplFlag.Object_GetHashCode | TypeImplFlag.EqualityOperators)]
public readonly partial struct UndirectedCellGraph() :
	IBitwiseOperators<UndirectedCellGraph, UndirectedCellGraph, UndirectedCellGraph>,
	IEquatable<UndirectedCellGraph>,
	IEqualityOperators<UndirectedCellGraph, UndirectedCellGraph, bool>,
	IReadOnlySet<Cell>,
	IFormattable
{
	/// <summary>
	/// Indicates the default empty graph without any cells.
	/// </summary>
	public static readonly UndirectedCellGraph Empty = new();


	/// <summary>
	/// Indicates the backing field.
	/// </summary>
	[HashCodeMember]
	private readonly CellMap _map;

	/// <summary>
	/// Indicates the directly-connected cells.
	/// </summary>
	private readonly Dictionary<Cell, CellMap> _directlyConnectedCellsDictionary = [];


	/// <summary>
	/// Initializes a <see cref="UndirectedCellGraph"/> instance.
	/// </summary>
	/// <param name="cells">Indicates the cells used.</param>
	public UndirectedCellGraph(ref readonly CellMap cells) : this()
	{
		_map = cells;

		_directlyConnectedCellsDictionary = new(cells.Count);
		foreach (var cell in cells)
		{
			_directlyConnectedCellsDictionary.Add(cell, cells & PeersMap[cell]);
		}
	}


	/// <summary>
	/// Indicates whether the graph is connected, i.e. all nodes in the graph is a part of the whole graph without being separated.
	/// </summary>
	public bool IsConnected => Components is [var onlyComponent] && this == onlyComponent;

	/// <summary>
	/// Indicates whether the current graph is an empty graph, i.e. there's no cells inside the current graph.
	/// </summary>
	public bool IsEmpty => this == Empty;

	/// <summary>
	/// Indicates the vertices count, i.e. the number of cells in the graph.
	/// </summary>
	public int VerticesCount => _map.Count;

	/// <summary>
	/// Indicates the internal map.
	/// </summary>
	[UnscopedRef]
	public ref readonly CellMap Map => ref _map;

	/// <summary>
	/// Indicates all possible connected components of the graph.
	/// </summary>
	public ReadOnlySpan<UndirectedCellGraph> Components
	{
		get
		{
			var lastCells = _map;
			var result = new List<UndirectedCellGraph>();
			while (lastCells)
			{
				foreach (var cell in lastCells.Offsets)
				{
					var queue = new Queue<Cell>();
					queue.Enqueue(cell);

					var currentGraph = cell.AsCellMap();
					while (queue.Count != 0)
					{
						var currentCell = queue.Dequeue();
						var connectedCells = _directlyConnectedCellsDictionary[currentCell];
						foreach (var peerCell in connectedCells)
						{
							if (!currentGraph.Contains(peerCell))
							{
								queue.Enqueue(peerCell);
							}
						}
						currentGraph |= connectedCells;
					}

					lastCells &= ~currentGraph;
					result.Add(new(in currentGraph));
					break;
				}
			}
			return result.AsReadOnlySpan();
		}
	}

	/// <inheritdoc/>
	int IReadOnlyCollection<Cell>.Count => VerticesCount;


	/// <summary>
	/// Try to get all cells whose degree is the specified value.
	/// </summary>
	/// <param name="degree">The degree. The value must be between 0 and 20.</param>
	/// <returns>All cells whose degree is equal to the specified value.</returns>
	public UndirectedCellGraph this[[ConstantExpected(Min = 0, Max = 20)] int degree]
	{
		get
		{
			var result = CellMap.Empty;
			foreach (var cell in _map)
			{
				if (GetDegreeOf(cell) == degree)
				{
					result.Add(cell);
				}
			}
			return new(in result);
		}
	}


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(UndirectedCellGraph other) => _map == other._map;

	/// <summary>
	/// Indicates whether the current graph is superset of the specified graph,
	/// i.e. all nodes in <paramref name="other"/> belong to the current graph.
	/// </summary>
	/// <param name="other">The other graph to be checked.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool IsSupersetOf(UndirectedCellGraph other) => (_map & other._map) == other._map;

	/// <summary>
	/// Indicates whether the current graph is subset of the specified graph,
	/// i.e. all nodes in the current instance belong to the graph <paramref name="other"/>.
	/// </summary>
	/// <param name="other">The other graph to be checked.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool IsSubsetOf(UndirectedCellGraph other) => (other._map & _map) == _map;

	/// <summary>
	/// Try to get the degree of the specified cell.
	/// </summary>
	/// <param name="cell">The desired cell.</param>
	/// <returns>An <see cref="int"/> indicating that. If the cell isn't in the current graph, -1 will be returned.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int GetDegreeOf(Cell cell) => _map.Contains(cell) ? _directlyConnectedCellsDictionary[cell].Count : -1;

	/// <inheritdoc cref="object.ToString"/>
	public override string ToString() => ToString(null);

	/// <inheritdoc cref="IFormattable.ToString(string?, IFormatProvider?)"/>
	public string ToString(IFormatProvider? formatProvider)
	{
		var converter = CoordinateConverter.GetInstance(formatProvider);
		var sb = new StringBuilder();
		foreach (var kvp in _directlyConnectedCellsDictionary)
		{
			sb.AppendLine($"{converter.CellConverter(in kvp.Key.AsCellMap())}: {converter.CellConverter(in kvp.ValueRef())}");
		}
		return sb.ToString();
	}

	/// <summary>
	/// Try to get connected cells for the specified cell. 
	/// </summary>
	/// <param name="cell">Indicates the cell to be checked.</param>
	/// <returns>All cells that connect the current cell.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public CellMap GetConnectedCells(Cell cell) => _directlyConnectedCellsDictionary[cell];

	/// <summary>
	/// Creates an enumerator type that can iterate on each cell in the collection.
	/// </summary>
	/// <returns>An enumerator instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[SuppressMessage("Style", "IDE0305:Simplify collection initialization", Justification = "<Pending>")]
	public Enumerator GetEnumerator() => new(_map.ToArray());

	/// <summary>
	/// Try to get a <see cref="UndirectedCellGraph"/> that contains the specified cell.
	/// </summary>
	/// <param name="cell">The desired cell.</param>
	/// <param name="depths">A list of <see cref="UndirectedCellGraphDepth"/> values.</param>
	/// <returns>A <see cref="UndirectedCellGraph"/> instance.</returns>
	public UndirectedCellGraph GetComponentOf(Cell cell, out ReadOnlySpan<UndirectedCellGraphDepth> depths)
	{
		var startDepthInstance = new UndirectedCellGraphDepth(0, cell);
		var queue = new Queue<UndirectedCellGraphDepth>();
		var depthValues = new List<UndirectedCellGraphDepth>();
		queue.Enqueue(startDepthInstance);
		depthValues.Add(startDepthInstance);

		var currentGraph = cell.AsCellMap();
		while (queue.Count != 0)
		{
			var (currentDepth, currentCell) = queue.Dequeue();
			var connectedCells = _directlyConnectedCellsDictionary[currentCell];
			foreach (var peerCell in connectedCells)
			{
				if (!currentGraph.Contains(peerCell))
				{
					var depth = new UndirectedCellGraphDepth(currentDepth + 1, peerCell);
					queue.Enqueue(depth);
					depthValues.Add(depth);
				}
			}
			currentGraph |= connectedCells;
		}

		depths = depthValues.AsReadOnlySpan();
		return new(in currentGraph);
	}

	/// <inheritdoc/>
	public bool Contains(Cell item) => _map.Contains(item);

	/// <inheritdoc/>
	bool IReadOnlySet<Cell>.IsProperSubsetOf(IEnumerable<Cell> other)
	{
		var map = (CellMap)([.. other]);
		return _map != map && (map & _map) == _map;
	}

	/// <inheritdoc/>
	bool IReadOnlySet<Cell>.IsProperSupersetOf(IEnumerable<Cell> other)
	{
		var map = (CellMap)([.. other]);
		return _map != map && (_map & map) == map;
	}

	/// <inheritdoc/>
	bool IReadOnlySet<Cell>.IsSubsetOf(IEnumerable<Cell> other) => ([.. other] & _map) == _map;

	/// <inheritdoc/>
	bool IReadOnlySet<Cell>.IsSupersetOf(IEnumerable<Cell> other)
	{
		var map = (CellMap)([.. other]);
		return (_map & map) == map;
	}

	/// <inheritdoc/>
	bool IReadOnlySet<Cell>.Overlaps(IEnumerable<Cell> other) => !!(_map & [.. other]);

	/// <inheritdoc/>
	bool IReadOnlySet<Cell>.SetEquals(IEnumerable<Cell> other) => _map == [.. other];

	/// <inheritdoc/>
	string IFormattable.ToString(string? format, IFormatProvider? formatProvider) => ToString(formatProvider);

	/// <inheritdoc/>
	IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<Cell>)this).GetEnumerator();

	/// <inheritdoc/>
	IEnumerator<Cell> IEnumerable<Cell>.GetEnumerator() => ((IEnumerable<Cell>)[.. _map]).GetEnumerator();


	/// <inheritdoc cref="IBitwiseOperators{TSelf, TOther, TResult}.op_OnesComplement(TSelf)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static UndirectedCellGraph operator ~(UndirectedCellGraph value) => new(~value._map);

	/// <inheritdoc cref="IBitwiseOperators{TSelf, TOther, TResult}.op_BitwiseAnd(TSelf, TOther)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static UndirectedCellGraph operator &(UndirectedCellGraph left, UndirectedCellGraph right) => new(left._map & right._map);

	/// <inheritdoc cref="IBitwiseOperators{TSelf, TOther, TResult}.op_BitwiseOr(TSelf, TOther)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static UndirectedCellGraph operator |(UndirectedCellGraph left, UndirectedCellGraph right) => new(left._map | right._map);

	/// <inheritdoc cref="IBitwiseOperators{TSelf, TOther, TResult}.op_ExclusiveOr(TSelf, TOther)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static UndirectedCellGraph operator ^(UndirectedCellGraph left, UndirectedCellGraph right) => new(left._map ^ right._map);
}
