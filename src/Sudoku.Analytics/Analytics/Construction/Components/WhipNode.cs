namespace Sudoku.Analytics.Construction.Components;

/// <summary>
/// Represents a whip node.
/// </summary>
/// <param name="candidate">Indicates the candidates currently set.</param>
/// <param name="grid">The currently grid state.</param>
/// <param name="parent">Indicates the parent node.</param>
[StructLayout(LayoutKind.Auto)]
[TypeImpl(TypeImplFlags.AllObjectMethods | TypeImplFlags.Equatable | TypeImplFlags.EqualityOperators)]
public sealed partial class WhipNode(
	[Property, HashCodeMember] Candidate candidate,
	ref readonly Grid grid,
	[Property] WhipNode? parent
) :
	IEquatable<WhipNode>,
	IFormattable,
	IEqualityOperators<WhipNode, WhipNode, bool>,
	IShiftOperators<WhipNode, WhipNode, WhipNode>
{
	/// <summary>
	/// Indicates the backing grid.
	/// </summary>
	private Grid _grid = grid;


	/// <summary>
	/// Initializes a <see cref="WhipNode"/> instance via the candidate set and its corresponding grid.
	/// </summary>
	/// <param name="candidate">The candidate.</param>
	/// <param name="grid">The grid.</param>
	public WhipNode(Candidate candidate, ref readonly Grid grid) : this(candidate, in grid, null)
	{
	}


	/// <summary>
	/// Indicates the root node.
	/// </summary>
	public WhipNode Root
	{
		get
		{
			var (result, p) = (this, Parent);
			while (p is not null)
			{
				_ = (result = p, p = p.Parent);
			}
			return result;
		}
	}

	/// <summary>
	/// Indicates the current grid.
	/// </summary>
	public ref Grid Grid => ref _grid;

	[EquatableMember]
	private Candidate CandidateEntry => Candidate;


	/// <inheritdoc cref="IFormattable.ToString(string?, IFormatProvider?)"/>
	public string ToString(IFormatProvider? formatProvider)
	{
		var converter = CoordinateConverter.GetInstance(formatProvider);
		var candidateString = converter.CandidateConverter([Candidate]);
		var parentCandidateString = Parent is null ? "<null>" : converter.CandidateConverter([Parent.Candidate]);
		return $$"""{{nameof(WhipNode)}} { {{nameof(Candidate)}} = {{candidateString}}, {{nameof(Parent)}} = {{parentCandidateString}} }""";
	}

	/// <inheritdoc/>
	string IFormattable.ToString(string? format, IFormatProvider? formatProvider) => ToString(formatProvider);


	/// <inheritdoc cref="op_RightShift(WhipNode, WhipNode?)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static WhipNode operator <<(WhipNode? parent, WhipNode current) => current >> parent;

	/// <summary>
	/// Creates a <see cref="WhipNode"/> instance with parent node.
	/// </summary>
	/// <param name="current">The current node.</param>
	/// <param name="parent">The parent node.</param>
	/// <returns>The new node created.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static WhipNode operator >>(WhipNode current, WhipNode? parent) => new(current.Candidate, in current.Grid, parent);

	/// <inheritdoc/>
	static WhipNode IShiftOperators<WhipNode, WhipNode, WhipNode>.operator >>>(WhipNode value, WhipNode shiftAmount)
		=> value >> shiftAmount;
}
