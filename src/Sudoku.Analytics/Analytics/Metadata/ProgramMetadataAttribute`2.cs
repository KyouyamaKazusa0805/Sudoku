namespace Sudoku.Analytics.Metadata;

/// <summary>
/// <inheritdoc/>
/// </summary>
/// <typeparam name="TRating"><inheritdoc/></typeparam>
/// <typeparam name="TDifficultyLevel">Indicates the type of the difficulty level enumeration.</typeparam>
public abstract class ProgramMetadataAttribute<TRating, TDifficultyLevel> : ProgramMetadataAttribute<TRating>
	where TRating : unmanaged, INumberBase<TRating>
	where TDifficultyLevel : unmanaged, Enum
{
	/// <summary>
	/// Indicates the difficulty level.
	/// </summary>
	public TDifficultyLevel DifficultyLevel { get; init; }
}
