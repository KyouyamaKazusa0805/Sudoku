namespace Sudoku.Drawing;

/// <summary>
/// Provides with a data structure that displays a view for basic information.
/// </summary>
[TypeImpl(TypeImplFlag.Object_Equals | TypeImplFlag.EqualityOperators)]
public sealed partial class View :
	HashSet<ViewNode>,
	IEquatable<View>,
	IExceptMethod<View, ViewNode>,
	IEqualityOperators<View, View, bool>,
	IFirstLastMethod<View, ViewNode>,
	IOfTypeMethod<View, ViewNode>,
	ISelectMethod<View, ViewNode>,
	IWhereMethod<View, ViewNode>
{
	/// <summary>
	/// Indicates the backing comparer.
	/// </summary>
	private static IEqualityComparer<View>? _backingComparer;


	/// <summary>
	/// Indicates an empty <see cref="View"/> instance. You can use this property to create a new instance.
	/// </summary>
	public static View Empty => [];

	/// <summary>
	/// Represents a <see cref="IEqualityComparer"/> of <see cref="View"/> type that can compares equality of a whole set
	/// with specified equality comparison rules on each element of type <see cref="ViewNode"/>.
	/// </summary>
	/// <seealso cref="IEqualityComparer{T}"/>
	/// <seealso cref="ViewNode"/>
	public static IEqualityComparer<View> SetComparer => _backingComparer ??= CreateSetComparer();


	/// <summary>
	/// Adds a list of <see cref="ViewNode"/>s into the collection.
	/// </summary>
	/// <param name="nodes">A list of <see cref="ViewNode"/> instance.</param>
	public void AddRange<TViewNode>(ReadOnlySpan<TViewNode> nodes) where TViewNode : ViewNode
	{
		foreach (var node in nodes)
		{
			Add(node);
		}
	}

	/// <summary>
	/// Try to find the candidate whose cell is specified one.
	/// </summary>
	/// <param name="cell">The cell to be found.</param>
	/// <returns>The found node; or <see langword="null"/> if none found.</returns>
	public CellViewNode? FindCell(Cell cell)
	{
		foreach (var node in this.OfType<CellViewNode>())
		{
			if (node.Cell == cell)
			{
				return node;
			}
		}
		return null;
	}

	/// <summary>
	/// Try to find the candidate whose candidate is specified one.
	/// </summary>
	/// <param name="candidate">The candidate to be found.</param>
	/// <returns>The found node; or <see langword="null"/> if none found.</returns>
	public CandidateViewNode? FindCandidate(Candidate candidate)
	{
		foreach (var node in this.OfType<CandidateViewNode>())
		{
			if (node.Candidate == candidate)
			{
				return node;
			}
		}
		return null;
	}

	/// <summary>
	/// Determines whether the specified <see cref="View"/> stores several <see cref="BabaGroupViewNode"/>s,
	/// and at least one of it overlaps the specified cell.
	/// </summary>
	/// <param name="cell">The cell.</param>
	/// <returns>A <see cref="bool"/> value indicating whether being overlapped.</returns>
	public bool UnknownOverlaps(Cell cell)
	{
		foreach (var babaGroupNode in this.OfType<BabaGroupViewNode>())
		{
			if (babaGroupNode.Cell == cell)
			{
				return true;
			}
		}
		return false;
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals([NotNullWhen(true)] View? other) => SetComparer.Equals(this, other);

	/// <inheritdoc/>
	public override int GetHashCode() => SetComparer.GetHashCode(this);

	/// <inheritdoc cref="IExceptMethod{TSelf, TSource}.Except(IEnumerable{TSource})"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public View ExceptWith(View other)
	{
		var result = ShallowClone();
		result.ExceptWith((IEnumerable<ViewNode>)other);
		return result;
	}

	/// <summary>
	/// Creates a new <see cref="View"/> instance with same values as the current instance, with independency.
	/// </summary>
	/// <returns>A new <see cref="View"/> instance with same values as the current instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public View Clone() => Count == 0 ? Empty : [.. from node in this select node.Clone()];

	/// <summary>
	/// Creates a new <see cref="View"/> instance whose contents are all come from the current instance,
	/// with reference cloned.
	/// </summary>
	/// <returns>
	/// A new <see cref="View"/> instance with same values as the current instance, with reference cloned.
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public View ShallowClone() => Count == 0 ? Empty : [.. this];

	/// <summary>
	/// Try to convert this collection as a <see cref="ReadOnlySpan{T}"/> instance.
	/// </summary>
	/// <returns>A <see cref="ReadOnlySpan{T}"/> instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ReadOnlySpan<ViewNode> AsSpan() => from node in this select node;

	/// <inheritdoc/>
	ViewNode IFirstLastMethod<View, ViewNode>.First() => this.First();

	/// <inheritdoc/>
	ViewNode IFirstLastMethod<View, ViewNode>.First(Func<ViewNode, bool> predicate) => this.First(predicate);

	/// <inheritdoc/>
	ViewNode? IFirstLastMethod<View, ViewNode>.FirstOrDefault() => this.FirstOrDefault();

	/// <inheritdoc/>
	ViewNode IFirstLastMethod<View, ViewNode>.FirstOrDefault(ViewNode defaultValue) => this.FirstOrDefault() ?? defaultValue;

	/// <inheritdoc/>
	ViewNode? IFirstLastMethod<View, ViewNode>.FirstOrDefault(Func<ViewNode, bool> predicate) => this.FirstOrDefault(predicate);

	/// <inheritdoc/>
	ViewNode IFirstLastMethod<View, ViewNode>.FirstOrDefault(Func<ViewNode, bool> predicate, ViewNode defaultValue)
		=> this.FirstOrDefault(predicate) ?? defaultValue;

	/// <inheritdoc/>
	IEnumerable<ViewNode> IWhereMethod<View, ViewNode>.Where(Func<ViewNode, bool> predicate) => this.Where(predicate).ToArray();

	/// <inheritdoc/>
	IEnumerable<ViewNode> IExceptMethod<View, ViewNode>.Except(IEnumerable<ViewNode> second) => ExceptWith([.. second]);

	/// <inheritdoc/>
	IEnumerable<ViewNode> IExceptMethod<View, ViewNode>.Except(IEnumerable<ViewNode> second, IEqualityComparer<ViewNode>? comparer)
		=> ((IExceptMethod<View, ViewNode>)this).Except(second);

	/// <inheritdoc/>
	IEnumerable<TResult> ISelectMethod<View, ViewNode>.Select<TResult>(Func<ViewNode, TResult> selector)
		=> this.Select(selector).ToArray();

	/// <inheritdoc/>
	IEnumerable<TResult> IOfTypeMethod<View, ViewNode>.OfType<TResult>() => this.OfType<TResult>().ToArray();


	/// <summary>
	/// Creates a <see cref="View"/> whose elements contains both <paramref name="left"/> and <paramref name="right"/>.
	/// </summary>
	/// <param name="left">The left-side <see cref="View"/> instance.</param>
	/// <param name="right">The right-side <see cref="View"/> instance.</param>
	/// <returns>A <see cref="View"/> result created.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static View operator &(View left, View right)
	{
		var result = left.ShallowClone();
		result.IntersectWith(right);
		return result;
	}

	/// <summary>
	/// Merges two <see cref="View"/> instances into one <see cref="View"/>.
	/// </summary>
	/// <param name="left">Indicates the left-side <see cref="View"/> instance.</param>
	/// <param name="right">Indicates the right-side <see cref="View"/> instance.</param>
	/// <returns>A <see cref="View"/> result merged.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static View operator |(View left, View right)
	{
		var result = left.ShallowClone();
		result.UnionWith(right);
		return result;
	}

	/// <summary>
	/// Creates a <see cref="View"/> instance, whose elements is from two <see cref="View"/> collections
	/// <paramref name="left"/> and <paramref name="right"/>, with only one-side containing this element.
	/// </summary>
	/// <param name="left">The left-side <see cref="View"/> instance.</param>
	/// <param name="right">The right-side <see cref="View"/> instance.</param>
	/// <returns>A <see cref="View"/> result created.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static View operator ^(View left, View right)
	{
		var result = left.ShallowClone();
		result.SymmetricExceptWith(right);
		return result;
	}
}
