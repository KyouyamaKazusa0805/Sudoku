namespace Sudoku.Concepts.Solving;

/// <summary>
/// Provides extension methods on <see cref="ConclusionType"/>.
/// </summary>
/// <seealso cref="ConclusionType"/>
public static class ConclusionTypeExtensions
{
	/// <summary>
	/// Gets the notation of the conclusion type.
	/// </summary>
	/// <param name="this">The conclusion type.</param>
	/// <returns>The notation of the conclusion type.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string GetNotation(this ConclusionType @this) =>
		typeof(ConclusionType).GetField(@this.ToString())!.GetCustomAttribute<EnumFieldNameAttribute>()!.Name;
}
