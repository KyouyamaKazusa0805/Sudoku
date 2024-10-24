namespace Sudoku.Analytics.Construction.Components;

/// <summary>
/// Represents a whip node.
/// </summary>
/// <param name="assignment">Indicates the assignment conclusion.</param>
/// <param name="grid">The currently grid state.</param>
/// <param name="parent">Indicates the parent node.</param>
[StructLayout(LayoutKind.Auto)]
[TypeImpl(TypeImplFlags.AllObjectMethods | TypeImplFlags.Equatable | TypeImplFlags.EqualityOperators)]
public sealed partial class WhipNode(
	[Property, HashCodeMember] WhipAssignment assignment,
	[Field, HashCodeMember] ref readonly Grid grid,
	[Property] WhipNode? parent
) :
	IEquatable<WhipNode>,
	IFormattable,
	IEqualityOperators<WhipNode, WhipNode, bool>,
	IShiftOperators<WhipNode, WhipNode, WhipNode>
{
	/// <summary>
	/// Initializes a <see cref="WhipNode"/> instance via the candidate set and its corresponding grid.
	/// </summary>
	/// <param name="candidate">The assignment.</param>
	/// <param name="grid">The grid.</param>
	public WhipNode(Candidate candidate, ref readonly Grid grid) : this(new(candidate, Technique.None), in grid, null)
	{
	}

	/// <summary>
	/// Initializes a <see cref="WhipNode"/> instance via the candidate set and its corresponding grid.
	/// </summary>
	/// <param name="assignment">The assignment.</param>
	/// <param name="grid">The grid.</param>
	public WhipNode(WhipAssignment assignment, ref readonly Grid grid) : this(assignment, in grid, null)
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
	[EquatableMember]
	public ref Grid Grid => ref _grid;

	[EquatableMember]
	private WhipAssignment AssignmentEntry => Assignment;


	/// <inheritdoc cref="IFormattable.ToString(string?, IFormatProvider?)"/>
	public string ToString(IFormatProvider? formatProvider)
	{
		var converter = CoordinateConverter.GetInstance(formatProvider);
		var parentString = Parent?.ToString(converter) ?? "<null>";
		return $$"""{{nameof(WhipNode)}} { {{nameof(Assignment)}} = {{Assignment}}, {{nameof(Parent)}} = {{parentString}} }""";
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
	public static WhipNode operator >>(WhipNode current, WhipNode? parent) => new(current.Assignment, in current.Grid, parent);

	/// <inheritdoc/>
	static WhipNode IShiftOperators<WhipNode, WhipNode, WhipNode>.operator >>>(WhipNode value, WhipNode shiftAmount)
		=> value >> shiftAmount;
}
