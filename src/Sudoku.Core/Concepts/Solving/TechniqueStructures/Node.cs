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
	/// <para>Indicates whether the current node is a grouped node, which means it is not a sole candidate node.</para>
	/// <para>
	/// This property only checks for the type of the node.
	/// If the <see cref="Type"/> property doesn't hold the value <see cref="NodeType.Sole"/>,
	/// this method will return <see langword="true"/>.
	/// </para>
	/// </summary>
	/// <seealso cref="Type"/>
	/// <seealso cref="NodeType"/>
	public bool IsGroupedNode
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Type != NodeType.Sole;
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

	/// <inheritdoc cref="IEquatable{T}.Equals(T)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(scoped in Node other) => Cells == other.Cells && Digit == other.Digit;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int GetHashCode() => HashCode.Combine(Cells, Digit);

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
	public Conclusion[] PotentialConclusionsWith(scoped in Grid grid, scoped in Node node)
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
			case NodeType.AlmostUniqueRectangle:
			{
				// It's very difficult to describe about how to eliminate eliminations for AUR nodes in loop.
				// Please see the sketch:
				//
				//                      Case 1 |  Case 2
				//                             |
				// -ab                 -ab -ab | -ab
				// -ab                 -ab -ab | -ab
				//  ab abc    Cases     ab  ab |  ab   c
				// --------  ======>  ---------|----------
				// abd  ab               d  ab |  ab  ab
				//     -ab                 -ab | -ab -ab
				//     -ab                 -ab | -ab -ab
				using scoped var result = new ValueList<Conclusion>(8);
				int thisDigit = Digit;
				int otherDigit = node.Digit;
				short urDigits = (short)(grid.GetDigitsUnion(Cells) & ~(1 << thisDigit | 1 << otherDigit));
				foreach (int digit in urDigits)
				{
					foreach (int cell in !Cells & !node.Cells & grid.CandidatesMap[digit])
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
	bool IEquatable<Node>.Equals(Node other) => Equals(other);


	/// <inheritdoc cref="IEqualityOperators{TSelf, TOther}.operator =="/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(scoped in Node left, scoped in Node right) => left.Equals(right);

	/// <inheritdoc cref="IEqualityOperators{TSelf, TOther}.operator !="/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(scoped in Node left, scoped in Node right) => !(left == right);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool IEqualityOperators<Node, Node>.operator ==(Node left, Node right) => left == right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool IEqualityOperators<Node, Node>.operator !=(Node left, Node right) => left != right;
}
