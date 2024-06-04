namespace Sudoku.Analytics.Metadata;

/// <summary>
/// Represents an exception that describes for a field in <see cref="Technique"/> is missing for difficulty level attribute.
/// </summary>
/// <param name="memberName">The field name.</param>
/// <seealso cref="Technique"/>
/// <seealso cref="DifficultyLevel"/>
/// <seealso cref="TechniqueMetadataAttribute"/>
public sealed class MissingDifficultyLevelException(string memberName) : Exception
{
	/// <inheritdoc/>
	public override string Message => string.Format(ResourceDictionary.Get("Message_MissingDifficultyLevelException"), memberName);
}
