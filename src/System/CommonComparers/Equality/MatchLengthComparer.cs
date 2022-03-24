namespace System.CommonComparers.Equality;

/// <summary>
/// Indicates the inner equality comparer to determine the equality of length
/// of 2 <see cref="Match"/>es to compare.
/// </summary>
public sealed class MatchLengthComparer : IEqualityComparer<Match>
{
	/// <inheritdoc/>
	public bool Equals(Match? x, Match? y) => (x?.Value.Length ?? -1) == (y?.Value.Length ?? -1);

	/// <inheritdoc/>
	public int GetHashCode([DisallowNull] Match? obj) => obj?.Value.Length ?? -1;
}
