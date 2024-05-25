namespace Sudoku.Strategying;

/// <summary>
/// Provides with extension methods on <see cref="ComparisonOperator"/>.
/// </summary>
/// <seealso cref="ComparisonOperator"/>
public static class ComparisonOperatorExtensions
{
	/// <summary>
	/// Gets the string representation of the operator.
	/// </summary>
	/// <param name="this">The value.</param>
	/// <returns>The string representation.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string GetOperatorString(this ComparisonOperator @this)
		=> @this switch
		{
			ComparisonOperator.Equality => "=",
			ComparisonOperator.Inequality => "<>",
			ComparisonOperator.GreaterThan => ">",
			ComparisonOperator.GreaterThanOrEqual => ">=",
			ComparisonOperator.LessThan => "<",
			ComparisonOperator.LessThanOrEqual => "<="
		};

	/// <summary>
	/// Creates a delegate method that executes the specified rule of comparison.
	/// </summary>
	/// <typeparam name="T">The type of the target.</typeparam>
	/// <param name="this">The comparison.</param>
	/// <returns>A delegate function.</returns>
	/// <exception cref="ArgumentOutOfRangeException">Throws when the argument is out of range (not defined).</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Func<T, T, bool> GetOperator<T>(this ComparisonOperator @this) where T : IComparisonOperators<T, T, bool>
		=> @this switch
		{
			ComparisonOperator.Equality => static (a, b) => a == b,
			ComparisonOperator.Inequality => static (a, b) => a != b,
			ComparisonOperator.GreaterThan => static (a, b) => a > b,
			ComparisonOperator.GreaterThanOrEqual => static (a, b) => a >= b,
			ComparisonOperator.LessThan => static (a, b) => a < b,
			ComparisonOperator.LessThanOrEqual => static (a, b) => a <= b,
			_ => throw new ArgumentOutOfRangeException(nameof(@this))
		};
}
