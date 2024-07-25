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
		{ Technique.FullHouse, typeof(FullHousePrimaryGenerator) },
		{ Technique.CrosshatchingBlock, typeof(HiddenSinglePrimaryGenerator) },
		{ Technique.HiddenSingleBlock, typeof(HiddenSinglePrimaryGenerator) },
		{ Technique.CrosshatchingRow, typeof(HiddenSinglePrimaryGenerator) },
		{ Technique.HiddenSingleRow, typeof(HiddenSinglePrimaryGenerator) },
		{ Technique.CrosshatchingColumn, typeof(HiddenSinglePrimaryGenerator) },
		{ Technique.HiddenSingleColumn, typeof(HiddenSinglePrimaryGenerator) },
		{ Technique.NakedSingle, typeof(NakedSinglePrimaryGenerator) }
	}.ToFrozenDictionary();


	/// <summary>
	/// Creates an <see cref="IPrimaryGenerator"/> instance that creates puzzles that uses the specified technique.
	/// </summary>
	/// <param name="this">The <see cref="Technique"/> instance.</param>
	/// <returns>An <see cref="IPrimaryGenerator"/> instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static IPrimaryGenerator? GetSpecificPuzzleGenerator(this Technique @this)
		=> GeneratorTypes.TryGetValue(@this, out var result) ? (IPrimaryGenerator?)Activator.CreateInstance(result) : null;
}
