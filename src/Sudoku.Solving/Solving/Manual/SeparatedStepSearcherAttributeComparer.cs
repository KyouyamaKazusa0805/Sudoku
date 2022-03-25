namespace Sudoku.Solving.Manual;

/// <summary>
/// Defines a comparer instance that compares two instances of type <see cref="SeparatedStepSearcherAttribute"/>.
/// </summary>
/// <seealso cref="SeparatedStepSearcherAttribute"/>
internal sealed class SeparatedStepSearcherAttributeComparer : IComparer<SeparatedStepSearcherAttribute>
{
	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int Compare(SeparatedStepSearcherAttribute? x, SeparatedStepSearcherAttribute? y) =>
		(x, y) switch
		{
			(null, null) => 0, // Same.
			(not null, not null) => ((IComparable<SeparatedStepSearcherAttribute>)x).CompareTo(y),
			_ => throw new InvalidOperationException($"The method requires both arguments {nameof(x)} and {nameof(y)} are null, or not null.")
		};
}
