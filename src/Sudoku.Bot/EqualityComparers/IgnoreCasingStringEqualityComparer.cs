namespace Sudoku.Bot.EqualityComparers;

/// <summary>
/// Compares two strings without casing.
/// </summary>
internal sealed class IgnoreCasingStringEqualityComparer : IEqualityComparer<string>
{
	/// <inheritdoc/>
	public bool Equals(string? x, string? y)
		=> (x, y) switch
		{
			(null, null) => true,
			(not null, not null) => x.Equals(y, StringComparison.OrdinalIgnoreCase),
			_ => false
		};

	/// <inheritdoc/>
	public int GetHashCode(string obj) => obj.GetHashCode();
}
