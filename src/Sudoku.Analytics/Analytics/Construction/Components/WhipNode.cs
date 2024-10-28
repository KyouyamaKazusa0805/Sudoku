namespace Sudoku.Analytics.Construction.Components;

/// <summary>
/// Represents a whip node.
/// </summary>
/// <param name="assignment">Indicates the assignment conclusion.</param>
/// <param name="availableAssignments">Indicates the available assignments.</param>
/// <param name="parent">Indicates the parent node.</param>
[StructLayout(LayoutKind.Auto)]
[TypeImpl(TypeImplFlags.AllObjectMethods | TypeImplFlags.Equatable | TypeImplFlags.EqualityOperators)]
public sealed partial class WhipNode(
	[Property, HashCodeMember] NormalWhipAssignment assignment,
	[Property] ReadOnlyMemory<NormalWhipAssignment> availableAssignments,
	[Property] WhipNode? parent = null
) : IParentLinkedNode<WhipNode>
{
	/// <summary>
	/// Initializes a <see cref="WhipNode"/> with no next assignments.
	/// </summary>
	/// <param name="assignment">Indicates assignments.</param>
	public WhipNode(NormalWhipAssignment assignment) : this(assignment, ReadOnlyMemory<NormalWhipAssignment>.Empty)
	{
	}


	/// <inheritdoc/>
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

	[EquatableMember]
	private NormalWhipAssignment AssignmentEntry => Assignment;


	/// <inheritdoc/>
	public string ToString(IFormatProvider? formatProvider)
	{
		var converter = CoordinateConverter.GetInstance(formatProvider);
		var parentString = Parent is { Assignment.Candidate: var candidate }
			? converter.CandidateConverter(candidate.AsCandidateMap())
			: "<null>";
		return $$"""{{nameof(WhipNode)}} { {{nameof(Assignment)}} = {{Assignment}}, {{nameof(Parent)}} = {{parentString}} }""";
	}


	/// <summary>
	/// Creates a <see cref="WhipNode"/> instance with parent node.
	/// </summary>
	/// <param name="current">The current node.</param>
	/// <param name="parent">The parent node.</param>
	/// <returns>The new node created.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static WhipNode operator >>(WhipNode current, WhipNode? parent)
		=> new(current.Assignment, current.AvailableAssignments, parent);
}
