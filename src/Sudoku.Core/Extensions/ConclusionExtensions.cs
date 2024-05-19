namespace Sudoku.Analytics;

/// <summary>
/// Provides with extension methods on <see cref="Conclusion"/>.
/// </summary>
/// <seealso cref="Conclusion"/>
public static class ConclusionExtensions
{
	/// <summary>
	/// Converts the <see cref="Conclusion"/> array into a <see cref="ConclusionSet"/> instance.
	/// </summary>
	/// <param name="this">The conclusion array.</param>
	/// <returns>A <see cref="ConclusionSet"/> result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ConclusionSet AsSet(this Conclusion[] @this) => [.. @this];
}
