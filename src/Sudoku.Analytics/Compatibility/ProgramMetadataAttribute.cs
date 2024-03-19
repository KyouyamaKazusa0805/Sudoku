namespace Sudoku.Compatibility;

/// <summary>
/// Represents program compatibility attribute type.
/// </summary>
/// <typeparam name="TRating">
/// Indicates the type of the rating value. The value must be a number (e.g. a <see cref="double"/> or an <see cref="int"/>).
/// </typeparam>
/// <typeparam name="TDifficultyLevel">Indicates the type of the difficulty level enumeration.</typeparam>
public abstract class ProgramMetadataAttribute<TRating, TDifficultyLevel> : Attribute
	where TRating : unmanaged, INumberBase<TRating>
	where TDifficultyLevel : unmanaged, Enum
{
	/// <summary>
	/// Represents the aliased names for the current technique defined in the current program.
	/// </summary>
	[DisallowNull]
	public string[]? Aliases { get; init; }

	/// <summary>
	/// Indicates the type of the rating value.
	/// </summary>
	public TRating Rating { get; init; }

	/// <summary>
	/// Indicates the rating value that is defined in original program.
	/// </summary>
	/// <remarks>
	/// The value of this property is an array of 1 or 2 elements.
	/// If two, the first one is the minimal possible rating value of the technique;
	/// and the second one is the maximum possible rating value.
	/// </remarks>
	[DisallowNull]
	public TRating[]? RatingValueOriginal { get; init; }

	/// <summary>
	/// Indicates the rating value that is defined in advanced concept.
	/// </summary>
	/// <remarks><inheritdoc cref="RatingValueOriginal" path="/remarks"/></remarks>
	[DisallowNull]
	public TRating[]? RatingValueAdvanced { get; init; }

	/// <summary>
	/// Indicates the difficulty level.
	/// </summary>
	public TDifficultyLevel DifficultyLevel { get; init; }
}
