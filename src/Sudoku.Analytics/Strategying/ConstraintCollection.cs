namespace Sudoku.Strategying;

/// <summary>
/// Represents a constraint collection.
/// </summary>
public sealed class ConstraintCollection :
	List<Constraint>,
	IAdditionOperators<ConstraintCollection, Constraint?, ConstraintCollection>,
	ISubtractionOperators<ConstraintCollection, Constraint?, ConstraintCollection>
{
	/// <inheritdoc cref="List{T}()"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ConstraintCollection() : base()
	{
	}

	/// <inheritdoc cref="List{T}(int)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ConstraintCollection(int capacity) : base(capacity)
	{
	}

	/// <summary>
	/// Create a new instance and copy elements.
	/// </summary>
	/// <param name="other">The other collection.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ConstraintCollection(ConstraintCollection other) : base(other)
	{
	}


	/// <summary>
	/// Determine whether the collection contains an element of the specified type.
	/// </summary>
	/// <typeparam name="T">The type of the constraint to be checked.</typeparam>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	public bool Has<T>() where T : Constraint
	{
		foreach (var element in this)
		{
			if (element is T)
			{
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// For a given <see cref="ConstraintCheckingContext"/>,
	/// determine whether the specified grid and its related analysis result satisfy the current limited constraint rules.
	/// </summary>
	/// <param name="context">The context to be used.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	public bool IsValidFor(ConstraintCheckingContext context)
	{
		foreach (var constraint in this)
		{
			if (!constraint.Check(context))
			{
				return false;
			}
		}
		return true;
	}

	/// <inheritdoc cref="Enumerable.FirstOrDefault{TSource}(IEnumerable{TSource})"/>
	public Constraint? FirstOrDefault(Func<Constraint, bool> match)
	{
		foreach (var element in this)
		{
			if (match(element))
			{
				return element;
			}
		}
		return null;
	}

	/// <summary>
	/// Filter the collection, only reserving constraints of type <typeparamref name="TConstraint"/>.
	/// </summary>
	/// <typeparam name="TConstraint">The type of the target constraint to be reserved.</typeparam>
	/// <returns>A new collection that only contains <typeparamref name="TConstraint"/> instances.</returns>
	public ReadOnlySpan<TConstraint> OfType<TConstraint>() where TConstraint : Constraint
	{
		var result = new List<TConstraint>();
		foreach (var element in this)
		{
			if (element is TConstraint constraint)
			{
				result.Add(constraint);
			}
		}
		return result.AsReadOnlySpan();
	}

	/// <summary>
	/// Filters the collection, only reserving constraints satisfying the specified condition.
	/// </summary>
	/// <param name="predicate">The condition to be satisfied.</param>
	/// <returns>A new collection that only contains constraints satisfying the specified condition.</returns>
	public ConstraintCollection Where(Func<Constraint, bool> predicate)
	{
		var result = new ConstraintCollection();
		foreach (var element in this)
		{
			if (predicate(element))
			{
				result.Add(element);
			}
		}
		return result;
	}

	/// <summary>
	/// Projects each element in this collection into a new form.
	/// </summary>
	/// <typeparam name="TResult">The type of each result element.</typeparam>
	/// <param name="selector">The selector method to convert the value into <typeparamref name="TResult"/> instance.</param>
	/// <returns>A list of <typeparamref name="TResult"/> instances.</returns>
	public ReadOnlySpan<TResult> Select<TResult>(Func<Constraint, TResult> selector)
	{
		var result = new List<TResult>(Count);
		foreach (var element in this)
		{
			result.Add(selector(element));
		}
		return result.AsReadOnlySpan();
	}

	/// <inheritdoc cref="List{T}.Slice(int, int)"/>
	public new ConstraintCollection Slice(int start, int count)
	{
		var result = new ConstraintCollection(count);
		for (var i = start; i < start + count; i++)
		{
			result.Add(this[i]);
		}
		return result;
	}


	/// <inheritdoc/>
	public static ConstraintCollection operator +(ConstraintCollection left, Constraint? right)
	{
		if (right is null)
		{
			return new(left);
		}

		var result = new ConstraintCollection(left.Count + 1);
		foreach (var element in left)
		{
			if (element == right)
			{
				return new(left);
			}
		}

		result.Add(right);
		return result;
	}

	/// <inheritdoc/>
	public static ConstraintCollection operator -(ConstraintCollection left, Constraint? right)
	{
		if (right is null)
		{
			return new(left);
		}

		var result = new ConstraintCollection(left.Count - 1);
		foreach (var element in left)
		{
#if false
			if (!ReferenceEquals(element, right))
#else
			if (element != right)
#endif
			{
				result.Add(element);
			}
		}
		return result;
	}
}
