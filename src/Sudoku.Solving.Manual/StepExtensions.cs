namespace Sudoku.Solving.Manual;

/// <summary>
/// Provides with extension methods on <see cref="IStep"/> and <see cref="Step"/>.
/// </summary>
/// <seealso cref="IStep"/>
/// <seealso cref="Step"/>
public static class StepExtensions
{
	/// <summary>
	/// Indicates whether the corresponding technique of the current step is an Almost Locked Sets
	/// (ALS in abbreviation).
	/// </summary>
	/// <typeparam name="TStep">The type of the step.</typeparam>
	/// <param name="this">The <see cref="IStep"/> instance.</param>
	/// <returns>A <see cref="bool"/> value indicating that.</returns>
	public static bool IsAlmostLockedSets<TStep>(this TStep @this) where TStep : class, IStep
		=> @this.HasTag(TechniqueTags.Als);

	/// <summary>
	/// Indicates whether the corresponding technique of the current step is a chain. The chain techiques are:
	/// <list type="bullet">
	/// <item>
	/// Wings
	/// <list type="bullet">
	/// <item>
	/// Regular wings (XY-Wing, XYZ-Wing, WXYZ-Wing, etc.)
	/// </item>
	/// <item>
	/// Irregular wings (W-Wing, M-Wing, Split-Wing, Local-Wing, Hybrid-Wing)
	/// </item>
	/// </list>
	/// </item>
	/// <item>
	/// Short chains
	/// <list type="bullet">
	/// <item>
	/// Two strong links (Skyscraper, Two-string kite, Turbot fish)
	/// </item>
	/// <item>
	/// ALS chaining-like techniques
	/// <list type="bullet">
	/// <item>ALS-XZ</item>
	/// <item>ALS-XY-Wing</item>
	/// <item>ALS-W-Wing</item>
	/// </list>
	/// </item>
	/// <item>Empty rectangle</item>
	/// </list>
	/// </item>
	/// <!--
	/// <item>
	/// Long chains
	/// <list type="bullet">
	/// <item>Forcing chains</item>
	/// <item>Dynamic forcing chains</item>
	/// </list>
	/// </item>
	/// -->
	/// </list>
	/// </summary>
	/// <typeparam name="TStep">The type of the step.</typeparam>
	/// <param name="this">The <see cref="IStep"/> instance.</param>
	/// <returns>A <see cref="bool"/> value indicating that.</returns>
	public static bool IsChaining<TStep>(this TStep @this) where TStep : class, IStep
		=> @this.HasTag(TechniqueTags.Wings | TechniqueTags.ShortChaining | TechniqueTags.LongChaining);

	/// <summary>
	/// Indicates whether the corresponding technique of the current step is a deadly pattern.
	/// The deadly pattern techniques are:
	/// <list type="bullet">
	/// <item>
	/// Bi-value patterns
	/// <list type="bullet">
	/// <item>Unique rectangle (i.e. Uniqueness test)</item>
	/// <item>Unique loop</item>
	/// <item>Bi-value universal grave</item>
	/// </list>
	/// </item>
	/// <item>
	/// Multi-value patterns
	/// <list type="bullet">
	/// <item>Extended rectangle</item>
	/// <item>Unique square</item>
	/// <item>Unique polygon (Borescoper's deadly pattern as its alias)</item>
	/// <item>Qiu's deadly pattern</item>
	/// </list>
	/// </item>
	/// <!--
	/// <item>
	/// Other deadly patterns
	/// <list type="bullet">
	/// <item>Reverse bi-value universal grave</item>
	/// </list>
	/// </item>
	/// -->
	/// </list>
	/// </summary>
	/// <typeparam name="TStep">The type of the step.</typeparam>
	/// <param name="this">The <see cref="IStep"/> instance.</param>
	/// <returns>A <see cref="bool"/> value indicating that.</returns>
	public static bool IsDeadlyPattern<TStep>(this TStep @this) where TStep : class, IStep
		=> @this.HasTag(TechniqueTags.DeadlyPattern);
}
