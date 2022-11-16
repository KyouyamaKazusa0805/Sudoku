namespace Sudoku.Solving.Logical.Patterns;

/// <summary>
/// Defines a chain node.
/// </summary>
[IsLargeStruct]
public readonly partial struct Node : IEquatable<Node>, IEqualityOperators<Node, Node, bool>, ITechniquePattern<Node>
{
	/// <summary>
	/// Initializes a <see cref="Node"/> instance via the basic data.
	/// </summary>
	/// <param name="digit">The digit used.</param>
	/// <param name="cell">The cell used.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Node(byte digit, int cell) : this(digit, CellsMap[cell])
	{
	}

	/// <summary>
	/// Initializes a <see cref="Node"/> instance via the basic data.
	/// </summary>
	/// <param name="digit">The digit used.</param>
	/// <param name="cells">The cells used.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Node(byte digit, scoped in CellMap cells) => (Digit, Cells, FullCells) = (digit, cells, cells);

	/// <summary>
	/// Initializes a <see cref="Node"/> instance via the basic data.
	/// </summary>
	/// <param name="digit">The digit used.</param>
	/// <param name="cells">The cells used.</param>
	/// <param name="extraCells">The extra cells.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Node(byte digit, scoped in CellMap cells, scoped in CellMap extraCells) : this(digit, cells) => FullCells = cells | extraCells;


	/// <summary>
	/// Indicates the digit used.
	/// </summary>
	public byte Digit { get; }

	/// <summary>
	/// Indicates whether the current node is a grouped node, which means it is not a sole candidate node.
	/// </summary>
	public bool IsGrouped
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Cells.Count != 1;
	}

	/// <summary>
	/// Indicates whether the node is advanced node (ALS, AHS, AUR nodes, etc.).
	/// </summary>
	public bool IsAdvanced
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => false;
	}

	/// <summary>
	/// Indicates the cells used.
	/// </summary>
	public CellMap Cells { get; }

	/// <summary>
	/// Indicates the full cells.
	/// </summary>
	public CellMap FullCells { get; }

	/// <inheritdoc/>
	CellMap ITechniquePattern<Node>.Map => FullCells;


	[GeneratedOverriddingMember(GeneratedEqualsBehavior.TypeCheckingAndCallingOverloading)]
	public override partial bool Equals(object? obj);

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
	public override string ToString() => ToSimpleString();

	/// <summary>
	/// Indicates the potential conclusions that cannot be directly applied, but can be applied when a loop is formed.
	/// </summary>
	/// <param name="grid">The base sudoku grid that provides with candidates' distribution.</param>
	/// <param name="node">The node that connects with the current node.</param>
	/// <returns>The possible conclusions found. In default, the property returns an empty array.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Conclusion[] PotentialConclusionsWith(scoped in Grid grid, scoped in Node node)
	{
#if false
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
				// --------  ======>  ---------+----------
				// abd  ab               d  ab |  ab  ab
				//     -ab                 -ab | -ab -ab
				//     -ab                 -ab | -ab -ab
				using scoped var result = new ValueList<Conclusion>(8);
				int thisDigit = Digit;
				int otherDigit = node.Digit;
				short urDigits = (short)(grid.GetDigitsUnion(Cells) & ~(1 << thisDigit | 1 << otherDigit));
				foreach (int digit in urDigits)
				{
					foreach (int cell in +Cells & +node.Cells & grid.CandidatesMap[digit])
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
#else
		return Array.Empty<Conclusion>();
#endif
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	bool IEquatable<Node>.Equals(Node other) => Equals(other);


	/// <inheritdoc cref="IEqualityOperators{TSelf, TOther, TResult}.op_Equality(TSelf, TOther)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(scoped in Node left, scoped in Node right) => left.Equals(right);

	/// <inheritdoc cref="IEqualityOperators{TSelf, TOther, TResult}.op_Inequality(TSelf, TOther)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(scoped in Node left, scoped in Node right) => !(left == right);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool IEqualityOperators<Node, Node, bool>.operator ==(Node left, Node right) => left == right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool IEqualityOperators<Node, Node, bool>.operator !=(Node left, Node right) => left != right;
}
