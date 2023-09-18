using Sudoku.Algorithm.Ittoryu;

namespace Sudoku.Analytics.Categorization;

/// <summary>
/// Predefined <see cref="TechniqueSet"/> instances.
/// </summary>
/// <seealso cref="TechniqueSet"/>
public static class TechniqueSets
{
	/// <summary>
	/// Indicates all <see cref="Technique"/> fields included.
	/// </summary>
	public static TechniqueSet All => [.. Enum.GetValues<Technique>()];

	/// <summary>
	/// Indicates the techniques that ittoryu path finder will use.
	/// </summary>
	/// <seealso cref="IttoryuPathFinder"/>
	public static TechniqueSet IttoryuTechniques
		=> [Technique.FullHouse, Technique.HiddenSingleBlock, Technique.HiddenSingleRow, Technique.HiddenSingleColumn, Technique.NakedSingle];

	/// <summary>
	/// Indicates the techniques that ittoryu path finder will use, and naked singles are not included.
	/// </summary>
	/// <seealso cref="IttoryuPathFinder"/>
	public static TechniqueSet IttoryuNakedSingleNotIncluded => IttoryuTechniques - Technique.NakedSingle;

	/// <summary>
	/// Indicates the techniques that ittroyu path finder will use,
	/// only containing <see cref="Technique.FullHouse"/> and <see cref="Technique.HiddenSingleBlock"/>.
	/// </summary>
	/// <seealso cref="IttoryuPathFinder"/>
	/// <seealso cref="Technique.FullHouse"/>
	/// <seealso cref="Technique.HiddenSingleBlock"/>
	public static TechniqueSet IttoryuBlockHiddenSingle => [Technique.FullHouse, Technique.HiddenSingleBlock];
}
