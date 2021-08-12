namespace Sudoku.Solving.Manual;

/// <summary>
/// Provides extension methods on <see cref="StepInfo"/>.
/// </summary>
/// <seealso cref="StepInfo"/>
public static class StepInfoExtensions
{
	/// <summary>
	/// Check whether the current technique information is an ALS technique.
	/// </summary>
	/// <param name="this">The technique instance.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsAlsTechnique(this StepInfo @this) => @this.HasTag(TechniqueTags.Als);

	/// <summary>
	/// Check whether the current technique information is a chaining-ruled technique.
	/// </summary>
	/// <param name="this">The technique instance.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsChainingTechnique(this StepInfo @this) =>
		@this.HasTag(TechniqueTags.Wings | TechniqueTags.ShortChaining | TechniqueTags.LongChaining);

	/// <summary>
	/// Check whether the current technique information is a uniqueness technique.
	/// </summary>
	/// <param name="this">The technique instance.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsUniqueness(this StepInfo @this) => @this.HasTag(TechniqueTags.DeadlyPattern);
}
