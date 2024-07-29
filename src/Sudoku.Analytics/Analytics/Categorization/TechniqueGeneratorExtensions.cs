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
		{ Technique.FullHouse, typeof(FullHouseGenerator) },
		{ Technique.CrosshatchingBlock, typeof(HiddenSingleGenerator) },
		{ Technique.HiddenSingleBlock, typeof(HiddenSingleGenerator) },
		{ Technique.CrosshatchingRow, typeof(HiddenSingleGenerator) },
		{ Technique.HiddenSingleRow, typeof(HiddenSingleGenerator) },
		{ Technique.CrosshatchingColumn, typeof(HiddenSingleGenerator) },
		{ Technique.HiddenSingleColumn, typeof(HiddenSingleGenerator) },
		{ Technique.NakedSingle, typeof(NakedSingleGenerator) }
	}.ToFrozenDictionary();


	/// <summary>
	/// Creates an <see cref="PrimaryGenerator"/> instance that creates puzzles that uses the specified technique.
	/// </summary>
	/// <param name="this">The <see cref="Technique"/> instance.</param>
	/// <returns>An <see cref="PrimaryGenerator"/> instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static PrimaryGenerator? GetSpecificPuzzleGenerator(this Technique @this)
		=> GeneratorTypes.TryGetValue(@this, out var result) ? (PrimaryGenerator?)Activator.CreateInstance(result) : null;
}
