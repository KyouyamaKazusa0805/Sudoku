#pragma warning disable CS1591

namespace Sudoku.Drawing;

/// <summary>
/// Represents a drawable item.
/// </summary>
[StructLayout(LayoutKind.Explicit)]
[TypeImpl(TypeImplFlags.Object_Equals | TypeImplFlags.EqualityOperators)]
public readonly partial struct DrawableItem : IEquatable<DrawableItem>, IEqualityOperators<DrawableItem, DrawableItem, bool>
{
	/// <summary>
	/// The backing field of <see cref="Map"/>.
	/// </summary>
	[FieldOffset(0)]
	private readonly CandidateMap _map;

	/// <summary>
	/// Indicates the real kind used.
	/// </summary>
	[FieldOffset(96)]
	private readonly int _kind;


	private DrawableItem(ViewNode node)
	{
		Node = node;
		_kind = 0;
	}

	private DrawableItem(ref readonly CandidateMap map)
	{
		_map = map;
		_kind = 1;
	}

	private DrawableItem(Conclusion conclusion)
	{
		Conclusion = conclusion;
		_kind = 2;
	}


	/// <summary>
	/// Indicates the conclusion.
	/// </summary>
	/// <exception cref="InvalidOperationException">Throws when type mismatched.</exception>
	[field: FieldOffset(0)]
	public Conclusion Conclusion => _kind != 2 ? throw new InvalidOperationException() : field;

	/// <summary>
	/// Indicates the map.
	/// </summary>
	/// <exception cref="InvalidOperationException">Throws when type mismatched.</exception>
	[UnscopedRef]
	public ref readonly CandidateMap Map
	{
		get
		{
			if (_kind != 1)
			{
				throw new InvalidOperationException();
			}
			return ref _map;
		}
	}

	/// <summary>
	/// Indicates the node.
	/// </summary>
	/// <exception cref="InvalidOperationException">Throws when type mismatched.</exception>
	[field: FieldOffset(0)]
	[field: MaybeNull]
	public ViewNode Node => _kind != 0 ? throw new InvalidOperationException() : field;


	/// <inheritdoc cref="IEquatable{T}.Equals(T)"/>
	public bool Equals(ref readonly DrawableItem other)
		=> _kind == other._kind
		&& _kind switch { 0 => Node == other.Node, 1 => Map == other.Map, 2 => Conclusion == other.Conclusion };

	/// <inheritdoc/>
	public override int GetHashCode()
		=> _kind switch
		{
			0 => HashCode.Combine(0, Node),
			1 => HashCode.Combine(1, _map),
			2 => HashCode.Combine(2, Conclusion)
		};

	/// <inheritdoc cref="object.ToString"/>
	public override string ToString()
		=> _kind switch
		{
			0 => Node.ToString(),
			1 => _map.ToString(),
			2 => Conclusion.ToString()
		};

	/// <inheritdoc/>
	bool IEquatable<DrawableItem>.Equals(DrawableItem other) => Equals(in other);


	public static implicit operator DrawableItem(ViewNode node) => new(node);

	public static implicit operator DrawableItem(in CandidateMap map) => new(in map);

	public static implicit operator DrawableItem(Conclusion conclusion) => new(conclusion);

	public static explicit operator ViewNode?(DrawableItem value) => value._kind != 0 ? null : value.Node;

	public static explicit operator checked ViewNode(in DrawableItem value) => value.Node;

	public static explicit operator CandidateMap(in DrawableItem value) => value._kind != 1 ? default : value._map;

	public static explicit operator checked CandidateMap(in DrawableItem value) => value.Map;

	public static explicit operator Conclusion(in DrawableItem value) => value._kind != 2 ? default : value.Conclusion;

	public static explicit operator checked Conclusion(in DrawableItem value) => value.Conclusion;
}
