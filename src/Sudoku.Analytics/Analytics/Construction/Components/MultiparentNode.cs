namespace Sudoku.Analytics.Construction.Components;

/// <summary>
/// Represents a chain node, allowing multiple parents.
/// </summary>
/// <param name="candidate">Indicates the candidate used.</param>
/// <param name="isOn">Indicates whether the node is on.</param>
/// <param name="parents">Indicates all possible parents.</param>
[TypeImpl(TypeImplFlags.AllObjectMethods | TypeImplFlags.EqualityOperators)]
public sealed partial class MultiparentNode(
	[Property, HashCodeMember] Candidate candidate,
	[Property, HashCodeMember] bool isOn,
	[Property] ReadOnlyMemory<MultiparentNode> parents
) :
	ICloneable,
	IParentLinkedNode<MultiparentNode>
{
	/// <summary>
	/// Indicates whether the current node has only one parent node.
	/// </summary>
	public bool IsSingleParent => Parents.Length == 1;

	/// <inheritdoc/>
	public ReadOnlySpan<MultiparentNode> Ancestors
	{
		get
		{
			var result = new List<MultiparentNode>();
			foreach (var element in Parents)
			{
				result.Add(element);
				result.AddRange(element.Parents.Span);
			}
			return result.AsSpan();
		}
	}

	/// <inheritdoc/>
	public MultiparentNode Root
	{
		get
		{
			var result = this;
			var temp = Parents.Span[0];
			while (temp is not null)
			{
				result = temp;
				temp = temp.Parents.Span[0];
			}
			return result;
		}
	}

	/// <inheritdoc/>
	ComponentType IComponent.Type => ComponentType.MultiparentChainNode;

	/// <summary>
	/// Represents the parent of the node. If a node doesn't contain only one parent, <see langword="null"/> will be returned.
	/// </summary>
	MultiparentNode? IParentLinkedNode<MultiparentNode>.Parent => Parents.Span is [var parent] ? parent : null;


	/// <inheritdoc/>
	public bool Equals([NotNullWhen(true)] MultiparentNode? other)
		=> other is not null && Candidate == other.Candidate && IsOn == other.IsOn;

	/// <inheritdoc/>
	public bool IsAncestorOf(MultiparentNode childNode)
	{
		foreach (var node in childNode.Ancestors)
		{
			if (Equals(node))
			{
				return true;
			}
		}
		return false;
	}

	/// <inheritdoc/>
	public string ToString(IFormatProvider? formatProvider)
	{
		var converter = CoordinateConverter.GetInstance(formatProvider);
		return $"{converter.CandidateConverter(Candidate.AsCandidateMap())}: {IsOn}";
	}

	/// <inheritdoc/>
	/// <remarks>
	/// Format description:
	/// <list type="table">
	/// <listheader>
	/// <term>Format character</term>
	/// <description>Description</description>
	/// </listheader>
	/// <item>
	/// <term><c>m</c></term>
	/// <description>The map text. For example, <c>r1c23(4)</c></description>
	/// </item>
	/// <item>
	/// <term><c>S</c> and <c>s</c></term>
	/// <description>
	/// The <see cref="IsOn"/> property value (<see langword="true"/> or <see langword="false"/>).
	/// If the character <c>s</c> is upper-cased, the result text will be upper-cased on initial letter.
	/// </description>
	/// </item>
	/// </list>
	/// For example, format value <c>"m: S"</c> will be replaced with value <c>"r1c23(4): True"</c>.
	/// </remarks>
	public string ToString(string? format, IFormatProvider? formatProvider)
		=> (format ?? $"{Node.MapFormatString}: {Node.IsOnFormatString}")
			.Replace(Node.MapFormatString, Candidate.AsCandidateMap().ToString(formatProvider))
			.Replace(Node.IsOnFormatString, IsOn.ToString().ToLower());

	/// <inheritdoc cref="ICloneable.Clone"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public MultiparentNode Clone() => new(Candidate, IsOn, Parents);

	/// <inheritdoc/>
	object ICloneable.Clone() => Clone();


	/// <inheritdoc/>
	public static MultiparentNode operator >>(MultiparentNode current, MultiparentNode? parent)
		=> new(
			current.Candidate,
			current.IsOn,
			parent is null ? ReadOnlyMemory<MultiparentNode>.Empty : new SingletonArray<MultiparentNode>(parent)
		);

	/// <summary>
	/// Creates a <see cref="MultiparentNode"/> instance, with specified parents appended into the current instance.
	/// </summary>
	/// <param name="current">Indicates the current instance.</param>
	/// <param name="parents">Indciates the parent nodes.</param>
	/// <returns>A <see cref="MultiparentNode"/> instance created.</returns>
	public static MultiparentNode operator >>(MultiparentNode current, ReadOnlyMemory<MultiparentNode> parents)
		=> new(current.Candidate, current.IsOn, parents);
}
