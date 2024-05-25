namespace Sudoku.Analytics.Categorization;

/// <summary>
/// Provides with extension methods on <see cref="TechniqueGroup"/>.
/// </summary>
/// <seealso cref="TechniqueGroup"/>
public static class TechniqueGroupExtensions
{
	/// <summary>
	/// Represents <see langword="typeof"/>(<see cref="TechniqueGroup"/>).
	/// </summary>
	private static readonly Type TypeOfTechniqueGroup = typeof(TechniqueGroup);


	/// <summary>
	/// Indicates whether the technique group supports for Siamese rule.
	/// </summary>
	/// <param name="this">The <see cref="TechniqueGroup"/> instance.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool SupportsSiamese(this TechniqueGroup @this)
		=> TypeOfTechniqueGroup.GetField(@this.ToString())!.GetCustomAttribute<TechniqueMetadataAttribute>()?.SupportsSiamese ?? false;

	/// <summary>
	/// Try to get shortened name of the current <see cref="TechniqueGroup"/> instance. If the group has an abbreviation,
	/// return its abbreviation; otherwise, its full name.
	/// </summary>
	/// <param name="this">The <see cref="TechniqueGroup"/> instance.</param>
	/// <returns>The shortened name.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string GetShortenedName(this TechniqueGroup @this) => @this.GetShortenedName(CultureInfo.CurrentUICulture);

	/// <summary>
	/// Try to get shortened name of the current <see cref="TechniqueGroup"/> instance. If the group has an abbreviation,
	/// return its abbreviation; otherwise, its full name.
	/// </summary>
	/// <param name="this">The <see cref="TechniqueGroup"/> instance.</param>
	/// <param name="cultureInfo">The culture information instance.</param>
	/// <returns>The shortened name.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string GetShortenedName(this TechniqueGroup @this, CultureInfo? cultureInfo)
		=> @this.GetAbbreviation() is { } abbr ? abbr : @this.GetName(cultureInfo ?? CultureInfo.CurrentUICulture);

	/// <summary>
	/// Try to get name of the current <see cref="TechniqueGroup"/> instance, with the specified culture information.
	/// </summary>
	/// <param name="this">The <see cref="TechniqueGroup"/> instance.</param>
	/// <param name="formatProvider">The culture information instance.</param>
	/// <returns>The name.</returns>
	/// <exception cref="ResourceNotFoundException">Throws when the specified group does not contain a name.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string GetName(this TechniqueGroup @this, IFormatProvider? formatProvider)
		=> ResourceDictionary.Get($"{nameof(TechniqueGroup)}_{@this}", formatProvider as CultureInfo ?? CultureInfo.CurrentUICulture);

	/// <summary>
	/// Try to get abbreviation of the current <see cref="TechniqueGroup"/> instance.
	/// </summary>
	/// <param name="this">The <see cref="TechniqueGroup"/> instance.</param>
	/// <returns>The abbreviation.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string? GetAbbreviation(this TechniqueGroup @this)
		=> TypeOfTechniqueGroup.GetField(@this.ToString())!.GetCustomAttribute<TechniqueMetadataAttribute>()?.Abbreviation;

	/// <summary>
	/// Try to get all possible <see cref="Technique"/> fields belonging to the current group.
	/// </summary>
	/// <param name="this">The group to be checked.</param>
	/// <param name="filter">Indicates the filter.</param>
	/// <returns>A <see cref="TechniqueSet"/> instance that contains all <see cref="Technique"/> fields belonging to the current group.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TechniqueSet GetTechniques(this TechniqueGroup @this, Func<Technique, bool>? filter = null)
		=> filter is null
			? TechniqueSet.TechniqueRelationGroups[@this]
			: from technique in TechniqueSet.TechniqueRelationGroups[@this] where filter(technique) select technique;
}
