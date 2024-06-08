namespace Sudoku.Drawing;

/// <summary>
/// Provides with a data structure that displays a view for basic information.
/// </summary>
[TypeImpl(TypeImplFlag.Object_Equals | TypeImplFlag.EqualityOperators)]
public sealed partial class View :
	HashSet<ViewNode>,
	IEquatable<View>,
	IEqualityOperators<View, View, bool>,
	IFirstLastMethod<View, ViewNode>,
	IOfTypeMethod<View, ViewNode>,
	ISelectMethod<View, ViewNode>,
	IWhereMethod<View, ViewNode>
{
	/// <summary>
	/// Adds a list of <see cref="ViewNode"/>s into the collection.
	/// </summary>
	/// <param name="nodes">A list of <see cref="ViewNode"/> instance.</param>
	public void AddRange<T>(scoped ReadOnlySpan<T> nodes) where T : ViewNode
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
	public bool Equals([NotNullWhen(true)] View? other)
	{
		if (other is null)
		{
			return false;
		}

		if (Count != other.Count)
		{
			return false;
		}

		foreach (var element in this)
		{
			if (!other.Contains(element))
			{
				return false;
			}
		}
		foreach (var element in other)
		{
			if (!Contains(element))
			{
				return false;
			}
		}
		return true;
	}

	/// <inheritdoc/>
	public override int GetHashCode()
	{
		var result = new HashCode();
		var i = 0;
		foreach (var element in this)
		{
			if ((i++ & 1) != 0)
			{
				result.Add(element.GetHashCode() * 17 & 333);
			}
			else
			{
				result.Add(element.GetHashCode() * 29 & 666);
			}
		}
		return result.ToHashCode();
	}

	/// <summary>
	/// Creates a new <see cref="View"/> instance with same values as the current instance, with independency.
	/// </summary>
	/// <returns>A new <see cref="View"/> instance with same values as the current instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public View Clone() => Count == 0 ? [] : [.. from node in this select node.Clone()];

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
	public static View operator &(View left, View right)
	{
		var result = new View();
		foreach (var element in left)
		{
			if (right.Contains(element))
			{
				result.Add(element);
			}
		}
		foreach (var element in right)
		{
			if (left.Contains(element))
			{
				result.Add(element);
			}
		}
		return result;
	}

	/// <summary>
	/// Merges two <see cref="View"/> instances into one <see cref="View"/>.
	/// </summary>
	/// <param name="left">Indicates the left-side <see cref="View"/> instance.</param>
	/// <param name="right">Indicates the right-side <see cref="View"/> instance.</param>
	/// <returns>A <see cref="View"/> result merged.</returns>
	public static View operator |(View left, View right)
	{
		var result = new View();
		foreach (var element in left)
		{
			result.Add(element);
		}
		foreach (var element in right)
		{
			result.Add(element);
		}
		return result;
	}

	/// <summary>
	/// Creates a <see cref="View"/> instance, whose elements is from two <see cref="View"/> collections
	/// <paramref name="left"/> and <paramref name="right"/>, with only one-side containing this element.
	/// </summary>
	/// <param name="left">The left-side <see cref="View"/> instance.</param>
	/// <param name="right">The right-side <see cref="View"/> instance.</param>
	/// <returns>A <see cref="View"/> result created.</returns>
	public static View operator ^(View left, View right)
	{
		var result = new View();
		foreach (var element in left)
		{
			if (!right.Contains(element))
			{
				result.Add(element);
			}
		}
		foreach (var element in right)
		{
			if (!left.Contains(element))
			{
				result.Add(element);
			}
		}
		return result;
	}
}
