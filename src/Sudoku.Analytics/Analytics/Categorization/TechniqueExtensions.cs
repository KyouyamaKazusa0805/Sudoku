namespace Sudoku.Analytics.Categorization;

/// <summary>
/// Provides with extension methods on <see cref="Technique"/>.
/// </summary>
/// <seealso cref="Technique"/>
public static class TechniqueExtensions
{
	/// <summary>
	/// Try to get the name of the current <see cref="Technique"/>.
	/// </summary>
	/// <param name="this">The <see cref="Technique"/> instance.</param>
	/// <returns>
	/// The name of the current technique.
	/// Return <see langword="null"/> if the current technique does not contain a valid name in resource dictionary.
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string? GetName(this Technique @this) => R[@this.ToString()];

	/// <summary>
	/// Try to get the abbreviation of the current <see cref="Technique"/>.
	/// </summary>
	/// <param name="this">The <see cref="Technique"/> instance.</param>
	/// <returns>The abbreviation of the current technique.</returns>
	/// <remarks>
	/// The routing rule can be described as below:
	/// <list type="number">
	/// <item>
	/// Check whether the field is marked attribute <see cref="AbbreviationAttribute"/>,
	/// and return property value <see cref="AbbreviationAttribute.Abbreviation"/> if marked.
	/// </item>
	/// <item>If 1) returns <see langword="null"/>, then search for resource dictionary, and return the target value if found.</item>
	/// <item>If 2) returns <see langword="null"/>, then check its <see cref="TechniqueGroup"/>, then return its abbreviation if worth.</item>
	/// <item>If 3) returns <see langword="null"/>, this method will return <see langword="null"/>; otherwise, a valid value.</item>
	/// </list>
	/// </remarks>
	/// <seealso cref="AbbreviationAttribute"/>
	/// <seealso cref="TechniqueGroup"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string? GetAbbreviation(this Technique @this)
		=> typeof(Technique).GetField(@this.ToString())!.GetCustomAttribute<AbbreviationAttribute>()?.Abbreviation
		?? R[$"TechniqueAbbr_{@this}"]
		?? @this.GetGroup().GetAbbreviation();

	/// <summary>
	/// Try to get all aliases of the current <see cref="Technique"/>.
	/// </summary>
	/// <param name="this">The <see cref="Technique"/> instance.</param>
	/// <returns>
	/// All possible aliases of the current technique.
	/// If the technique does not contain any aliases, the return value will be <see langword="null"/>.
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string[]? GetAliases(this Technique @this)
		=> R[$"TechniqueAlias_{@this}"]?.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

	/// <summary>
	/// Try to get the group that the current <see cref="Technique"/> belongs to.
	/// </summary>
	/// <param name="this">The <see cref="Technique"/> instance.</param>
	/// <returns>The <see cref="TechniqueGroup"/> value that the current <see cref="Technique"/> belongs to.</returns>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Throws when the specified <see cref="Technique"/> does not belong to any <see cref="TechniqueGroup"/>.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TechniqueGroup GetGroup(this Technique @this)
		=> typeof(Technique).GetField(@this.ToString())?.GetCustomAttribute<TechniqueGroupAttribute>()?.Group
		?? throw new ArgumentOutOfRangeException(nameof(@this));
}
