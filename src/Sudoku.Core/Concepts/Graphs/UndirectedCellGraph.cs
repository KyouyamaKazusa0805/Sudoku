namespace Sudoku.Concepts.Graphs;

/// <summary>
/// <para>Represents a list of cells that form a undirected graph.</para>
/// <para>
/// Please visit <see href="https://en.wikipedia.org/wiki/Component_(graph_theory)">this link</see>
/// to learn more information about concept Component.
/// </para>
/// </summary>
/// <seealso href="https://en.wikipedia.org/wiki/Component_(graph_theory)">Wikipedia - Component (Graph Theory)</seealso>
[CollectionBuilder(typeof(UndirectedCellGraph), nameof(Create))]
public readonly partial struct UndirectedCellGraph() : IFormattable, IReadOnlyCollection<Cell>
{
	/// <summary>
	/// Indicates the default empty graph without any cells.
	/// </summary>
	public static readonly UndirectedCellGraph Empty = new();


	/// <summary>
	/// Indicates the cells used.
	/// </summary>
	private readonly CellMap _cells;

	/// <summary>
	/// Indicates invalid cells. Such cells may not be covered from the grid.
	/// <see cref="GetComponentOf(Cell, out ReadOnlySpan{UndirectedCellGraphDepth})"/> will ignore them.
	/// </summary>
	/// <seealso cref="GetComponentOf(Cell, out ReadOnlySpan{UndirectedCellGraphDepth})"/>
	private readonly CellMap _invalidCells;

	/// <summary>
	/// Indicates the directly-connected cells.
	/// </summary>
	private readonly Dictionary<Cell, CellMap> _directlyConnectedCellsDictionary = [];


	/// <summary>
	/// Initializes an <see cref="UndirectedCellGraph"/> instance.
	/// </summary>
	/// <param name="cells">Indicates the cells used.</param>
	public UndirectedCellGraph(ref readonly CellMap cells) : this()
	{
		_cells = cells;

		_directlyConnectedCellsDictionary = new(cells.Count);
		foreach (var cell in cells)
		{
			_directlyConnectedCellsDictionary.Add(cell, cells & PeersMap[cell]);
		}
	}

	/// <summary>
	/// Initializes an <see cref="UndirectedCellGraph"/> instance.
	/// </summary>
	/// <param name="cells">Indicates the cells used.</param>
	/// <param name="invalidCells">Indicates invalid cells.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private UndirectedCellGraph(ref readonly CellMap cells, ref readonly CellMap invalidCells) : this(in cells)
		=> _invalidCells = invalidCells;


	/// <summary>
	/// Indicates whether the graph is connected, i.e. all nodes in the graph is a part of the whole graph without being separated.
	/// </summary>
	public bool IsConnected => Components is [var onlyComponent] && _cells == onlyComponent._cells;

	/// <summary>
	/// Indicates whether the current graph is an empty graph, i.e. there's no cells inside the current graph.
	/// </summary>
	public bool IsEmpty => _cells.Count == 0;

	/// <summary>
	/// Indicates the vertices count, i.e. the number of cells in the graph.
	/// </summary>
	public int VerticesCount => _cells.Count;

	/// <summary>
	/// Indicates the internal map.
	/// </summary>
	[UnscopedRef]
	public ref readonly CellMap Map => ref _cells;

	/// <summary>
	/// Indicates all possible connected components of the graph.
	/// </summary>
	public ReadOnlySpan<UndirectedCellGraph> Components
	{
		get
		{
			var lastCells = _cells;
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
	/// <param name="degree">The degree. The value must be between 0 and 3.</param>
	/// <returns>All cells whose degree is equal to the specified value.</returns>
	public UndirectedCellGraph this[[ConstantExpected(Min = 0, Max = 3)] int degree]
	{
		get
		{
			var result = CellMap.Empty;
			foreach (var cell in _cells)
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
	public bool Equals(UndirectedCellGraph other) => _cells == other._cells;

	/// <summary>
	/// Try to get the degree of the specified cell.
	/// </summary>
	/// <param name="cell">The desired cell.</param>
	/// <returns>An <see cref="int"/> indicating that. If the cell isn't in the current graph, -1 will be returned.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int GetDegreeOf(Cell cell) => _cells.Contains(cell) ? _directlyConnectedCellsDictionary[cell].Count : -1;

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
	public Enumerator GetEnumerator() => new(_cells.ToArray());

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
			var coveredCells = CellMap.Empty;
			foreach (var peerCell in connectedCells)
			{
				if (currentGraph.Contains(peerCell) || _invalidCells.Contains(peerCell))
				{
					continue;
				}

				var matchHouse = (currentCell.AsCellMap() + peerCell).FirstSharedHouse;
				if (HousesMap[matchHouse] & _invalidCells)
				{
					continue;
				}

				var depth = new UndirectedCellGraphDepth(currentDepth + 1, peerCell);
				queue.Enqueue(depth);
				depthValues.Add(depth);
				coveredCells.Add(peerCell);
			}
			currentGraph |= coveredCells;
		}

		depths = depthValues.AsReadOnlySpan();
		return new(in currentGraph);
	}

	/// <inheritdoc/>
	public bool Contains(Cell item) => _cells.Contains(item);

	/// <inheritdoc/>
	string IFormattable.ToString(string? format, IFormatProvider? formatProvider) => ToString(formatProvider);

	/// <inheritdoc/>
	IEnumerator IEnumerable.GetEnumerator() => _cells.ToArray().GetEnumerator();

	/// <inheritdoc/>
	IEnumerator<Cell> IEnumerable<Cell>.GetEnumerator() => _cells.ToArray().AsEnumerable().GetEnumerator();


	/// <summary>
	/// Creates an <see cref="UndirectedCellGraph"/> instance via the specified cells.
	/// </summary>
	/// <param name="cells">The cells.</param>
	/// <returns>An <see cref="UndirectedCellGraph"/> instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static UndirectedCellGraph Create(ref readonly CellMap cells) => new(in cells);

	/// <inheritdoc cref="Create(ref readonly CellMap)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static UndirectedCellGraph Create(scoped ReadOnlySpan<Cell> cells) => Create(cells.AsCellMap());

	/// <summary>
	/// Initializes an <see cref="UndirectedCellGraph"/> instance via a list of cells, checking conjugate pairs.
	/// </summary>
	/// <param name="grid">The grid to be used.</param>
	/// <param name="digit">The digit to be used.</param>
	/// <param name="cells">The cells to be used.</param>
	/// <returns>An <see cref="UndirectedCellGraph"/> instance.</returns>
	public static UndirectedCellGraph CreateFromConjugatePair(ref readonly Grid grid, Digit digit, ref readonly CellMap cells)
	{
		var globalCells = grid.CandidatesMap[digit];
		var invalidCells = CellMap.Empty;
		foreach (var house in globalCells.Houses)
		{
			var tempCells = globalCells & HousesMap[house];
			if (tempCells != (cells & HousesMap[house]))
			{
				invalidCells |= tempCells & ~(cells & HousesMap[house]);
			}
		}
		return new(in cells, in invalidCells);
	}
}
