using System.Reflection;

namespace Sudoku.Analytics.Categorization;

/// <summary>
/// Provides with extension methods on <see cref="TechniqueGroup"/>.
/// </summary>
/// <seealso cref="TechniqueGroup"/>
public static class TechniqueGroupExtensions
{
	/// <summary>
	/// Try to get abbreviation of the current <see cref="TechniqueGroup"/> instance.
	/// </summary>
	/// <param name="this">The <see cref="TechniqueGroup"/> instance.</param>
	/// <returns>The abbreviation.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string? GetAbbreviation(this TechniqueGroup @this)
		=> typeof(TechniqueGroup).GetField(@this.ToString())!.GetCustomAttribute<AbbreviationAttribute>()?.Abbreviation;
}
