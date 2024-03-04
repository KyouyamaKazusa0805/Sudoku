namespace Sudoku.Strategying;

/// <summary>
/// Represents a constraint collection.
/// </summary>
public sealed class ConstraintCollection : List<Constraint>
{
	/// <summary>
	/// For a given <see cref="ConstraintCheckingContext"/>,
	/// determine whether the specified grid and its related analysis result satisfy the current limited constraint rules.
	/// </summary>
	/// <param name="context">The context to be used.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	public bool IsValidFor(scoped ConstraintCheckingContext context)
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

	/// <summary>
	/// Try to get the first <see cref="Constraint"/> satisfying the specified condition,
	/// returning the value of type <typeparamref name="TResult"/> created by the specified method.
	/// </summary>
	/// <typeparam name="TResult">The type of the result.</typeparam>
	/// <param name="match">The match method.</param>
	/// <param name="selector">The selector method.</param>
	/// <param name="defaultValue">The default value.</param>
	/// <returns>The final result.</returns>
	public TResult? FindFirst<TResult>(Func<Constraint, bool> match, Func<Constraint, TResult> selector, TResult? defaultValue = default)
	{
		foreach (var element in this)
		{
			if (match(element))
			{
				return selector(element);
			}
		}
		return defaultValue;
	}

	/// <summary>
	/// Try to get the first <see cref="Constraint"/> satisfying the specified condition,
	/// returning the value of type <typeparamref name="TResult"/> created by the specified method.
	/// </summary>
	/// <typeparam name="TConstraint">The type of constraint.</typeparam>
	/// <typeparam name="TResult">The type of the result.</typeparam>
	/// <param name="matchedSelector">
	/// Indicates the matched selector. The method will be used in both checking and converting operation.
	/// </param>
	/// <param name="defaultValue">
	/// Indicates the default value used. By default the value equals to <see langword="default"/>(<typeparamref name="TResult"/>).
	/// </param>
	/// <returns>The final result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public TResult? FindFirst<TConstraint, TResult>(Func<TConstraint, TResult> matchedSelector, TResult? defaultValue = default)
		where TConstraint : Constraint, new()
		=> FindFirst(static c => c is TConstraint, c => matchedSelector((TConstraint)c), defaultValue);
}
