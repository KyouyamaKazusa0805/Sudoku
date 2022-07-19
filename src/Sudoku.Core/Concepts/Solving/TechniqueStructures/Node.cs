namespace Sudoku.Concepts.Solving.TechniqueStructures;

/// <summary>
/// Defines a chain node.
/// </summary>
public readonly struct Node : IEquatable<Node>, IEqualityOperators<Node, Node>
{
	/// <summary>
	/// Initializes a <see cref="Node"/> instance via the basic data.
	/// </summary>
	/// <param name="nodeType">The node type.</param>
	/// <param name="digit">The digit used.</param>
	/// <param name="cell">The cell used.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Node(NodeType nodeType, byte digit, int cell) : this(nodeType, digit, Cells.Empty + cell)
	{
	}

	/// <summary>
	/// Initializes a <see cref="Node"/> instance via the basic data.
	/// </summary>
	/// <param name="nodeType">The node type.</param>
	/// <param name="digit">The digit used.</param>
	/// <param name="cells">The cells used.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Node(NodeType nodeType, byte digit, scoped in Cells cells)
		=> (Digit, Type, Cells, FullCells) = (digit, nodeType, cells, cells);

	/// <summary>
	/// Initializes a <see cref="Node"/> instance via the basic data.
	/// </summary>
	/// <param name="nodeType">The node type.</param>
	/// <param name="digit">The digit used.</param>
	/// <param name="cells">The cells used.</param>
	/// <param name="extraCells">The extra cells.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Node(NodeType nodeType, byte digit, scoped in Cells cells, scoped in Cells extraCells) :
		this(nodeType, digit, cells) => FullCells = cells | extraCells;


	/// <summary>
	/// Indicates the digit used.
	/// </summary>
	public byte Digit { get; }

	/// <summary>
	/// Indicates whether the current node is a grouped node, which means it uses more than 1 cell.
	/// </summary>
	public bool IsGroupedNode
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Cells.Count >= 2;
	}

	/// <summary>
	/// Indicates the cells used.
	/// </summary>
	public Cells Cells { get; }

	/// <summary>
	/// Indicates the full cells.
	/// </summary>
	public Cells FullCells { get; }

	/// <summary>
	/// Indicates the type of the node.
	/// </summary>
	public NodeType Type { get; }


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] object? obj) => obj is Node comparer && Equals(comparer);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals([NotNullWhen(true)] Node other)
		=> FullCells == other.FullCells && Digit == other.Digit && Type == other.Type;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int GetHashCode() => HashCode.Combine(Cells, Digit, Type);

	/// <summary>
	/// Gets the simplified string value that only displays the important information.
	/// </summary>
	/// <returns>The string value.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string ToSimpleString() => $"{Cells}({Digit + 1})";

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString()
	{
		string nodeTypeName = Type.GetName() ?? "<Unnamed>";
		return $"{nodeTypeName}: {ToSimpleString()}";
	}

	/// <summary>
	/// Indicates the potential conclusions that cannot be directly applied, but can be applied when a loop is formed.
	/// </summary>
	/// <param name="grid">The base sudoku grid that provides with candidates' distribution.</param>
	/// <param name="node">The node that connects with the current node.</param>
	/// <returns>The possible conclusions found. In default, the property returns an empty array.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Conclusion[] PotentialConclusionsWith(scoped in Grid grid, Node node)
	{
		switch (Type)
		{
			case NodeType.AlmostLockedSets:
			{
				using scoped var result = new ValueList<Conclusion>(50);
				foreach (int digit in (short)(grid.GetDigitsUnion(FullCells) & ~(1 << Digit | 1 << node.Digit)))
				{
					foreach (int cell in !(FullCells & grid.CandidatesMap[digit]))
					{
						if (grid.Exists(cell, digit) is true)
						{
							result.Add(new(ConclusionType.Elimination, cell, digit));
						}
					}
				}

				return result.ToArray();
			}
			default:
			{
				return Array.Empty<Conclusion>();
			}
		}
	}


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(Node left, Node right) => left.Equals(right);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(Node left, Node right) => !(left == right);
}
