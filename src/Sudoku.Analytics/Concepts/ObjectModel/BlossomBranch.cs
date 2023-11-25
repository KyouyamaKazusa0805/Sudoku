namespace Sudoku.Concepts.ObjectModel;

/// <summary>
/// Represents for a blossom branch.
/// </summary>
public sealed class BlossomBranch : Dictionary<Digit, AlmostLockedSet>
{
	/// <summary>
	/// Transforms the current collection into another representation, using the specified function to transform.
	/// </summary>
	/// <typeparam name="TResult">The type of the results.</typeparam>
	/// <param name="selector">The selector to tranform elements.</param>
	/// <returns>The results.</returns>
	public ReadOnlySpan<TResult> Select<TResult>(Func<(Digit Digit, AlmostLockedSet AlsPattern), TResult> selector)
	{
		var result = new TResult[Count];
		var i = 0;
		foreach (var (key, value) in this)
		{
			result[i++] = selector((key, value));
		}

		return result;
	}
}
