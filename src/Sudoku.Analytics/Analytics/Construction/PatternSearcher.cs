namespace Sudoku.Analytics.Construction;

/// <summary>
/// Represents a pattern searcher type.
/// </summary>
/// <typeparam name="TPattern">The type of pattern.</typeparam>
public abstract class PatternSearcher<TPattern> : IConstructible<PatternSearcherType> where TPattern : Pattern
{
	/// <inheritdoc/>
	public abstract PatternSearcherType Type { get; }

	/// <inheritdoc/>
	DataStructureType IDataStructure.Type => DataStructureType.None;

	/// <inheritdoc/>
	DataStructureBase IDataStructure.Base => DataStructureBase.None;


	/// <summary>
	/// Try to search patterns and return them.
	/// </summary>
	/// <param name="grid">The grid to be checked.</param>
	/// <returns>The found patterns.</returns>
	public abstract ReadOnlySpan<TPattern> Search(ref readonly Grid grid);
}
