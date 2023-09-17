using System.Reflection;
using System.Runtime.CompilerServices;

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

	/// <summary>
	/// Try to get all possible <see cref="Technique"/> fields belonging to the current group.
	/// </summary>
	/// <param name="this">The group to be checked.</param>
	/// <returns>A <see cref="TechniqueSet"/> instance that contains all <see cref="Technique"/> fields belonging to the current group.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TechniqueSet GetTechniques(this TechniqueGroup @this) => TechniqueSet.TechniqueRelationGroups[@this];
}
