namespace Sudoku.Analytics.Categorization;

/// <summary>
/// Predefined <see cref="TechniqueSet"/> instances.
/// </summary>
/// <seealso cref="TechniqueSet"/>
public static class TechniqueSets
{
	/// <summary>
	/// Indicates the number of techniques supported in this program.
	/// </summary>
	public static int TechniquesCount => All.Count;

	/// <summary>
	/// Indicates all techniques are not included.
	/// </summary>
	public static TechniqueSet None => [];

	/// <summary>
	/// Indicates all <see cref="Technique"/> fields included.
	/// </summary>
	public static TechniqueSet All => [.. Enum.GetValues<Technique>()];

	/// <summary>
	/// Indicates all assignment techniques.
	/// </summary>
	public static TechniqueSet Assignments
		=> [
			Technique.FullHouse, Technique.LastDigit, Technique.HiddenSingleBlock,
			Technique.HiddenSingleRow, Technique.HiddenSingleColumn, Technique.NakedSingle
		];

	/// <summary>
	/// Indicates the techniques that ittoryu path finder will use.
	/// </summary>
	/// <seealso cref="DisorderedIttoryuFinder"/>
	public static TechniqueSet IttoryuTechniques
		=> [Technique.FullHouse, Technique.HiddenSingleBlock, Technique.HiddenSingleRow, Technique.HiddenSingleColumn, Technique.NakedSingle];

	/// <summary>
	/// Indicates the techniques that ittoryu path finder will use, and naked singles are not included.
	/// </summary>
	/// <seealso cref="DisorderedIttoryuFinder"/>
	public static TechniqueSet IttoryuNakedSingleNotIncluded => IttoryuTechniques - Technique.NakedSingle;

	/// <summary>
	/// Indicates the techniques that ittroyu path finder will use,
	/// only containing <see cref="Technique.FullHouse"/> and <see cref="Technique.HiddenSingleBlock"/>.
	/// </summary>
	/// <seealso cref="DisorderedIttoryuFinder"/>
	/// <seealso cref="Technique.FullHouse"/>
	/// <seealso cref="Technique.HiddenSingleBlock"/>
	public static TechniqueSet IttoryuBlockHiddenSingle => [Technique.FullHouse, Technique.HiddenSingleBlock];
}
