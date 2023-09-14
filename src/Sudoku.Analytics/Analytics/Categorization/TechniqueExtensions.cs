using System.Reflection;
using System.Runtime.CompilerServices;
using Sudoku.Analytics.Strings;
using Sudoku.Strings;
using static Sudoku.Analytics.Strings.StringsAccessor;

namespace Sudoku.Analytics.Categorization;

/// <summary>
/// Provides with extension methods on <see cref="Technique"/>.
/// </summary>
/// <seealso cref="Technique"/>
public static class TechniqueExtensions
{
	/// <summary>
	/// Indicates the bound technique type instance.
	/// </summary>
	private static readonly Type TypeOfTechnique = typeof(Technique);


	/// <summary>
	/// Try to get the name of the current <see cref="Technique"/>.
	/// </summary>
	/// <param name="this">The <see cref="Technique"/> instance.</param>
	/// <returns>The name of the current technique.</returns>
	/// <exception cref="ResourceNotFoundException">Throws when the target name is not found in resource dictionary.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string GetName(this Technique @this)
		=> GetString(@this.ToString())
		?? throw new ResourceNotFoundException(@this.ToString(), typeof(TechniqueExtensions).Assembly);

	/// <summary>
	/// Try to get the English name of the current <see cref="Technique"/>.
	/// </summary>
	/// <param name="this">The <see cref="Technique"/> instance.</param>
	/// <returns>The name of the current technique.</returns>
	/// <exception cref="ResourceNotFoundException">Throws when the target name is not found in resource dictionary.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string? GetEnglishName(this Technique @this)
		=> Resources.ResourceManager.GetString(@this.ToString(), new(1033))
		?? throw new ResourceNotFoundException(@this.ToString(), typeof(TechniqueExtensions).Assembly);

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
		=> TypeOfTechnique.GetField(@this.ToString())!.GetCustomAttribute<AbbreviationAttribute>()?.Abbreviation
		?? GetString($"TechniqueAbbr_{@this}")
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
	public static string[]? GetAliases(this Technique @this) => GetString($"TechniqueAlias_{@this}")?.SplitBy([';']);

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
		=> TypeOfTechnique.GetField(@this.ToString())?.GetCustomAttribute<TechniqueGroupAttribute>()?.Group
		?? throw new ArgumentOutOfRangeException(nameof(@this));

	/// <summary>
	/// Try to get its static difficulty level for the specified technique.
	/// </summary>
	/// <param name="this">The <see cref="Technique"/> instance.</param>
	/// <returns>
	/// The difficulty level that the current technique belongs to.
	/// If it doesn't contain a static difficulty level value, <see cref="DifficultyLevel.Unknown"/> will be returned.
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static DifficultyLevel GetDifficultyLevel(this Technique @this)
	{
		var fi = TypeOfTechnique.GetField(@this.ToString())!;
		return (fi.GetCustomAttribute<TechniqueFeatureAttribute>(), fi.GetCustomAttribute<DifficultyLevelAttribute>()) switch
		{
			({ Features: var feature }, _) when feature.Flags(TechniqueFeature.NotImplemented) => DifficultyLevel.Unknown,
			(_, { Level: var level }) => level,
			_ => throw new InvalidOperationException("The state is invalid for the current technique field.")
		};
	}

	/// <summary>
	/// Try to get features for the current <see cref="Technique"/>.
	/// </summary>
	/// <param name="this">The <see cref="Technique"/> instance.</param>
	/// <returns>All found features for the current <see cref="Technique"/> instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TechniqueFeature GetFeature(this Technique @this)
		=> TypeOfTechnique.GetField(@this.ToString())?.GetCustomAttribute<TechniqueFeatureAttribute>()?.Features ?? 0;
}
