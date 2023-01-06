namespace Sudoku.Solving.Logical.Patterns;

/// <summary>
/// Defines a node used in chaining.
/// </summary>
/// <remarks>
/// This type corresponds to the concept of Sudoku Explainer's logic "<b>Potential</b>".
/// </remarks>
#if DEBUG
[DebuggerDisplay($$"""{{{nameof(DebuggerDisplayString)}},nq}""")]
#endif
internal readonly partial struct ChainNode : IEquatable<ChainNode>, IEqualityOperators<ChainNode, ChainNode, bool>
{
	/// <summary>
	/// The internal mask.
	/// </summary>
	private readonly short _mask;


	/// <summary>
	/// Initializes a <see cref="ChainNode"/> instance via the specified data.
	/// </summary>
	/// <param name="candidate">The candidate.</param>
	/// <param name="isOn">Indicates whether the node is on.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ChainNode(int candidate, bool isOn) : this((byte)(candidate / 9), (byte)(candidate % 9), isOn)
	{
	}

	/// <summary>
	/// <inheritdoc cref="ChainNode(int, bool)" path="/summary"/>
	/// </summary>
	/// <param name="cell">The cell.</param>
	/// <param name="digit">The digit.</param>
	/// <param name="isOn"><inheritdoc cref="ChainNode(int, bool)" path="/param[@name='isOn']"/></param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ChainNode(byte cell, byte digit, bool isOn) => _mask = (short)((isOn ? 1 : 0) << 10 | cell * 9 + digit);

	/// <summary>
	/// <inheritdoc cref="ChainNode(int, bool)" path="/summary"/>
	/// </summary>
	/// <param name="base">The base potential instance.</param>
	/// <param name="isOn"><inheritdoc cref="ChainNode(int, bool)" path="/param[@name='isOn']"/></param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ChainNode(ChainNode @base, bool isOn) : this(@base.Cell, @base.Digit, isOn)
	{
	}


	/// <summary>
	/// Indicates whether the node is on.
	/// </summary>
	public bool IsOn => _mask >= 729;

	/// <summary>
	/// Indicates the cell used.
	/// </summary>
	public byte Cell => (byte)(Candidate / 9);

	/// <summary>
	/// Indicates the digit used.
	/// </summary>
	public byte Digit => (byte)(Candidate % 9);

	/// <summary>
	/// Indicates the candidate.
	/// </summary>
	public int Candidate => _mask & (1 << 10) - 1;

	/// <summary>
	/// Defines an accessor that allows user assigning a singleton parent node into the current data structure on instantiation phase.
	/// </summary>
	public ChainNode SingletonParent
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		init => Parents.Add(value);
	}

	/// <summary>
	/// Indicates all <see cref="ChainNode"/>s in a single chain. This property should only be used in normal AICs.
	/// </summary>
	public ChainNode[] ChainPotentials
	{
		get
		{
			var result = new List<ChainNode>();
			for (var p = this; p.Parents is [var parent]; p = parent)
			{
				result.Add(p);
			}

			return result.ToArray();
		}
	}

	/// <summary>
	/// Gets the chain of all <see cref="ChainNode"/>s from the current <see cref="ChainNode"/> as the target node.
	/// </summary>
	public ChainNode[] FullChainPotentials
	{
		get
		{
			var result = new List<ChainNode>();
			var done = new NodeSet();
			var todo = new List<ChainNode> { this };
			while (todo.Count > 0)
			{
				var next = new List<ChainNode>();
				foreach (var p in todo)
				{
					if (!done.Contains(p))
					{
						done.Add(p);
						result.Add(p);
						next.AddRange(p.Parents);
					}
				}

				todo = next;
			}

			return result.ToArray();
		}
	}

	/// <summary>
	/// Indicates the step detail of the nested chain.
	/// </summary>
	public ChainingStep? NestedChainDetails { get; init; }

	/// <summary>
	/// <para>Indicates the parents of the current instance.</para>
	/// <para>
	/// The result always returns a list of length 1 if the chain is not dynamic.
	/// In addition, if a <see cref="ChainNode"/> instance has no available parent (i.e. the return collection is empty),
	/// it must the head of a chain.
	/// </para>
	/// <para>
	/// If you want to append more parent nodes into the current <see cref="ChainNode"/> instance,
	/// just call <see cref="List{T}.Add(T)"/> to add it using this property: <c>p.Parents.Add(parent);</c>.
	/// </para>
	/// </summary>
	public List<ChainNode> Parents { get; } = new(1);

	/// <summary>
	/// Indicates the candidate string representation.
	/// </summary>
	[DebuggerHidden]
	[GeneratedDisplayName(nameof(Candidate))]
	private string CandidateString => $"{CellsMap[Cell]}({Digit + 1})";

#if DEBUG
	/// <summary>
	/// Indicates the string that is used for display on debugger.
	/// </summary>
	[DebuggerHidden]
	private string DebuggerDisplayString
	{
		get
		{
			const string trueStr = "true", falseStr = "false";

			return $"{CandidateString} is {(IsOn ? trueStr : falseStr)}";
		}
	}
#endif


	[GeneratedDeconstruction]
	public partial void Deconstruct(out int candidate, out bool isOn);

	[GeneratedDeconstruction]
	public partial void Deconstruct(out byte cell, out byte digit, out bool isOn);

	[GeneratedOverriddingMember(GeneratedEqualsBehavior.TypeCheckingAndCallingOverloading)]
	public override partial bool Equals(object? obj);

	/// <inheritdoc cref="IEquatable{T}.Equals(T)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(ChainNode other) => _mask == other._mask;

	[GeneratedOverriddingMember(GeneratedGetHashCodeBehavior.SimpleField, nameof(_mask))]
	public override partial int GetHashCode();

	[GeneratedOverriddingMember(GeneratedToStringBehavior.RecordLike, nameof(CandidateString), nameof(IsOn))]
	public override partial string ToString();


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(ChainNode left, ChainNode right) => left.Equals(right);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(ChainNode left, ChainNode right) => !(left == right);
}
