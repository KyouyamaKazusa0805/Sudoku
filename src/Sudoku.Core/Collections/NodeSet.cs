using System.ComponentModel;
using static System.Algorithm.Sorting;

namespace Sudoku.Collections;

/// <summary>
/// Defines a set that stores the chain nodes.
/// </summary>
public partial struct NodeSet :
	IDefaultable<NodeSet>,
	IEnumerable,
	IEquatable<NodeSet>
#if FEATURE_GENERIC_MATH
	,
	IEqualityOperators<NodeSet, NodeSet>
#if FEATURE_GENERIC_MATH_IN_ARG
	,
	IValueBitwiseAndOperators<NodeSet, NodeSet, NodeSet>,
	IValueBitwiseOrOperators<NodeSet, NodeSet, NodeSet>,
	IValueEqualityOperators<NodeSet, NodeSet>
#endif
#endif
{
	/// <summary>
	/// Indicates the uninitalized instance that is used for checking whether the collection
	/// hasn't been initialized.
	/// </summary>
	public static readonly NodeSet Uninitialized;


	/// <summary>
	/// Indicates the capacity of the collection.
	/// </summary>
	private int _capacity;

	/// <summary>
	/// Indicates the inner data structure.
	/// </summary>
	private Node[] _chainNodes;


	/// <summary>
	/// Initializes a <see cref="NodeSet"/> instance, with the default capacity 16.
	/// </summary>
	/// <remarks>
	/// <include
	///     file='../../global-doc-comments.xml'
	///     path='g/csharp9/feature[@name="parameterless-struct-constructor"]/target[@name="constructor"]' />
	/// </remarks>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public NodeSet()
	{
		_chainNodes = new Node[16];
		_capacity = 16;
	}

	/// <summary>
	/// Initializes a <see cref="NodeSet"/> instance via the set of chain nodes.
	/// </summary>
	/// <param name="nodes">The chain nodes.</param>
	public NodeSet(Node[] nodes)
	{
		_capacity = nodes.Length;
		_chainNodes = new Node[_capacity];
		for (int i = 1, length = nodes.Length; i < length; i++)
		{
			int s = i - 1;
			bool exists = false;
			for (int j = s; j < i; j++)
			{
				if (_chainNodes[j].Mask == nodes[i].Mask)
				{
					exists = true;
				}
			}
			if (!exists)
			{
				_chainNodes[Count++] = _chainNodes[s];
			}
		}
	}

	/// <summary>
	/// The copy constructor that allows copying the elements from the specified collection
	/// to the current collection.
	/// </summary>
	/// <param name="another">The another collection.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public NodeSet(in NodeSet another)
	{
		_capacity = another._capacity;
		Count = another.Count;
		_chainNodes = new Node[_capacity];
		Buffer.BlockCopy(_chainNodes, 0, another._chainNodes, 0, Count);
	}


	/// <summary>
	/// Indicates whether the current instance has been uninitialized.
	/// </summary>
	/// <remarks>
	/// Here we use a simple way to check. C# 10 allows us using parameterless constructor
	/// to intialize a <see langword="struct"/> to change the default behavior.
	/// However, we can also use <see langword="default"/> expressions to initialize a default instance,
	/// whose fields of reference type will be initialized by 0 (or <see langword="null"/> literally).
	/// The only way to check whether the set is uninitialized is to use this property
	/// to check whether the reference-typed property is <see langword="null"/>.
	/// </remarks>
	public readonly bool IsUninitialized
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _chainNodes is null;
	}

	/// <summary>
	/// Indicates the number of elements in this collection.
	/// </summary>
	public int Count { get; private set; } = 0;

	/// <summary>
	/// Indicates the top element in this collection stored.
	/// If you remove the top element, you can call the method <see cref="Remove"/>,
	/// and then the current element will be removed at first.
	/// </summary>
	public readonly Node Top
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => this[0];
	}

	/// <summary>
	/// Indicates the bottom element in this collection stored.
	/// The element is the earliest element added into the current collection.
	/// </summary>
	public readonly Node Bottom
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => this[^1];
	}

	/// <inheritdoc/>
	readonly bool IDefaultable<NodeSet>.IsDefault => IsUninitialized;

	/// <inheritdoc/>
	static NodeSet IDefaultable<NodeSet>.Default => Uninitialized;


	/// <summary>
	/// Get the chain node at the specified index.
	/// </summary>
	/// <param name="index">The desired index.</param>
	/// <returns>The result chain node as reference.</returns>
	public readonly Node this[int index]
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _chainNodes[index];
	}

	/// <summary>
	/// <para>Get the reference of the chain node that equals to the specified chain node.</para>
	/// <para>
	/// If the collection doesn't contain such node, the return value will be null reference.
	/// Therefore, if you want to check the existence of the node, you should call the method
	/// <see cref="Unsafe.IsNullRef{T}(ref T)"/> firstly. If the return value is <see langword="true"/>,
	/// you can use the inner properties safely.
	/// </para>
	/// </summary>
	/// <param name="node">The node to check the equality.</param>
	/// <returns>The reference to the chain.</returns>
	/// <seealso cref="Unsafe.IsNullRef{T}(ref T)"/>
	public readonly ref Node this[Node node]
	{
		get
		{
			for (int i = 0; i < Count; i++)
			{
				ref var currentNode = ref _chainNodes[i];
				if (currentNode.Mask == node.Mask)
				{
					return ref currentNode;
				}
			}

			return ref Unsafe.NullRef<Node>();
		}
	}


	/// <summary>
	/// Remove the node at the specified index.
	/// </summary>
	/// <param name="index">The desired index.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void RemoveAt(Index index) => RemoveAt(index.GetOffset(Count));

	/// <summary>
	/// Removes the last element. If the collection has already been empty,
	/// an <see cref="InvalidOperationException"/> instance will be created and thrown.
	/// </summary>
	/// <exception cref="InvalidOperationException">Throws when the list has already been empty.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Node Remove() =>
		Count > 0 ? _chainNodes[--Count] : throw new InvalidOperationException("The list has already been empty.");

	/// <summary>
	/// To sort the collection.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe void Sort()
	{
		_chainNodes.Sort(&cmp, 0, Count - 1);


		static int cmp(Node l, Node r) => l.Mask > r.Mask ? 1 : l.Mask < r.Mask ? -1 : 0;
	}

	/// <summary>
	/// Determine whether the collection contains the specified chain node,
	/// whose mask exists in this collection.
	/// </summary>
	/// <param name="node">The node to check.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	public readonly bool Contains(Node node)
	{
		for (int i = 0; i < Count; i++)
		{
			if (_chainNodes[i].Mask == node.Mask)
			{
				return true;
			}
		}

		return false;
	}

	/// <inheritdoc cref="object.Equals(object?)"/>
	public override bool Equals([NotNullWhen(true)] object? obj) =>
		obj is NodeSet comparer && Equals(comparer);

	/// <inheritdoc/>
	public readonly bool Equals(NodeSet other)
	{
		if (Count != other.Count)
		{
			return false;
		}

		for (int i = 0; i < Count; i++)
		{
			if (this[i] != other[i])
			{
				return false;
			}
		}

		return true;
	}

	/// <inheritdoc/>
	public override readonly int GetHashCode()
	{
		var resultHash = new HashCode();
		foreach (var node in this)
		{
			resultHash.Add(node);
		}

		return resultHash.ToHashCode();
	}

	/// <summary>
	/// Adds the specified node into the collection. If the specified node exists in the collection,
	/// the method will do nothing but return <see langword="false"/>; otherwise, add it
	/// into the collection and then return <see langword="true"/>.
	/// </summary>
	/// <param name="node">The node.</param>
	/// <returns>
	/// A <see cref="bool"/> value indicating whether the operation is successful. Details:
	/// <list type="table">
	/// <listheader>
	/// <term>Return value</term>
	/// <description>Description</description>
	/// </listheader>
	/// <item>
	/// <term><see langword="true"/></term>
	/// <description>
	/// If the current collection contains the specified element to add.
	/// </description>
	/// </item>
	/// <item>
	/// <term><see langword="false"/></term>
	/// <description>Otherwise.</description>
	/// </item>
	/// </list>
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Add(Node node)
	{
		if (Contains(node))
		{
			return false;
		}

		if (Count >= _capacity)
		{
			Grow();
		}

		_chainNodes[Count++] = node;
		return true;
	}

	/// <summary>
	/// Remove the node at the specified index.
	/// </summary>
	/// <param name="index">The desired index.</param>
	public Node RemoveAt(int index)
	{
		if (index >= Count)
		{
			throw new ArgumentOutOfRangeException(nameof(index));
		}

		var result = _chainNodes[Count--];
		if (index < Count)
		{
			Array.Copy(_chainNodes, index + 1, _chainNodes, index, Count - index);
		}

		return result;
	}

	/// <summary>
	/// Gets the pinnable reference that is the reference to the first element in this collection,
	/// in order to use <see langword="fixed"/> statement on <see cref="NodeSet"/> instances.
	/// </summary>
	/// <returns>The reference to the first element in this collection.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public readonly ref readonly Node GetPinnableReference() => ref _chainNodes[..Count][0];

	/// <summary>
	/// Gets the array of <see cref="Node"/>s.
	/// </summary>
	/// <returns>The array of <see cref="Node"/>s.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly Node[] ToArray() => _chainNodes[..Count];

	/// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly Enumerator GetEnumerator() => new(_chainNodes, Count);

	/// <inheritdoc cref="IEnumerable.GetEnumerator"/>
	/// <exception cref="NotSupportedException">Always throws.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	readonly IEnumerator IEnumerable.GetEnumerator() =>
		throw new NotSupportedException("The current type has an enumerator of ref struct type.");

	/// <summary>
	/// Grow the collection to make the capacity be the 2-time value than before.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void Grow()
	{
		_capacity <<= 1;
		Array.Resize(ref _chainNodes, _capacity);
	}


	/// <summary>
	/// Merges two different <see cref="NodeSet"/>s into one, and checks and stores once that
	/// both <see cref="NodeSet"/>s store the node.
	/// </summary>
	/// <param name="left">The left-side collection to merge.</param>
	/// <param name="right">The right-side collection to merge.</param>
	/// <returns>The merged result.</returns>
	public static NodeSet operator &(in NodeSet left, in NodeSet right)
	{
		var result = new NodeSet();
		foreach (var leftNode in left)
		{
			if (right.Contains(leftNode))
			{
				result.Add(leftNode);
			}
		}

		return result;
	}

	/// <summary>
	/// Merges two different <see cref="NodeSet"/> into one. Same nodes will be stored only once.
	/// </summary>
	/// <param name="left">The left-side collection to merge.</param>
	/// <param name="right">The right-side collection to merge.</param>
	/// <returns>The merged result.</returns>
	public static NodeSet operator |(in NodeSet left, in NodeSet right)
	{
		var result = new NodeSet(left);
		foreach (var node in right)
		{
			result.Add(node);
		}

		return result;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(in NodeSet left, in NodeSet right) => left.Equals(right);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(in NodeSet left, in NodeSet right) => !(left == right);

#if FEATURE_GENERIC_MATH
	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool IEqualityOperators<NodeSet, NodeSet>.operator ==(NodeSet left, NodeSet right) => left == right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool IEqualityOperators<NodeSet, NodeSet>.operator !=(NodeSet left, NodeSet right) => left != right;

#if FEATURE_GENERIC_MATH_IN_ARG
	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static NodeSet IValueBitwiseAndOperators<NodeSet, NodeSet, NodeSet>.operator &(in NodeSet left, NodeSet right) =>
		left & right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static NodeSet IValueBitwiseAndOperators<NodeSet, NodeSet, NodeSet>.operator &(NodeSet left, in NodeSet right) =>
		left & right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static NodeSet IValueBitwiseOrOperators<NodeSet, NodeSet, NodeSet>.operator |(in NodeSet left, NodeSet right) =>
		left | right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static NodeSet IValueBitwiseOrOperators<NodeSet, NodeSet, NodeSet>.operator |(NodeSet left, in NodeSet right) =>
		left | right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool IValueEqualityOperators<NodeSet, NodeSet>.operator ==(in NodeSet left, NodeSet right) =>
		left == right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool IValueEqualityOperators<NodeSet, NodeSet>.operator ==(NodeSet left, in NodeSet right) =>
		left == right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool IValueEqualityOperators<NodeSet, NodeSet>.operator !=(in NodeSet left, NodeSet right) =>
		left != right;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool IValueEqualityOperators<NodeSet, NodeSet>.operator !=(NodeSet left, in NodeSet right) =>
		left != right;
#endif
#endif
}
