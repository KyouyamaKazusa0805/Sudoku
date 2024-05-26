namespace Sudoku.Analytics.Categorization;

/// <summary>
/// Provides with extension methods on <see cref="Technique"/> on generator module.
/// </summary>
/// <seealso cref="Technique"/>
public static class TechniqueGeneratorExtensions
{
	/// <summary>
	/// Indicates the generator types.
	/// </summary>
	private static readonly FrozenDictionary<Technique, Type> GeneratorTypes = new Dictionary<Technique, Type>
	{
		{ Technique.FullHouse, typeof(FullHousePuzzleGenerator) },
		{ Technique.CrosshatchingBlock, typeof(HiddenSinglePuzzleGenerator) },
		{ Technique.HiddenSingleBlock, typeof(HiddenSinglePuzzleGenerator) },
		{ Technique.CrosshatchingRow, typeof(HiddenSinglePuzzleGenerator) },
		{ Technique.HiddenSingleRow, typeof(HiddenSinglePuzzleGenerator) },
		{ Technique.CrosshatchingColumn, typeof(HiddenSinglePuzzleGenerator) },
		{ Technique.HiddenSingleColumn, typeof(HiddenSinglePuzzleGenerator) },
		{ Technique.NakedSingle, typeof(NakedSinglePuzzleGenerator) }
	}.ToFrozenDictionary();


	/// <summary>
	/// Creates a <see cref="TechniqueBasedPuzzleGenerator"/> instance that creates puzzles that uses the specified technique.
	/// </summary>
	/// <param name="this">The <see cref="Technique"/> instance.</param>
	/// <returns>A <see cref="TechniqueBasedPuzzleGenerator"/> instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TechniqueBasedPuzzleGenerator? GetSpecificPuzzleGenerator(this Technique @this)
		=> GeneratorTypes.TryGetValue(@this, out var result)
			? (TechniqueBasedPuzzleGenerator?)Activator.CreateInstance(result)
			: null;
}
