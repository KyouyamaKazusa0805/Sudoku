namespace Sudoku.Analytics.Metadata;

/// <summary>
/// Represents an exception that describes for a field in <see cref="Technique"/> is missing for difficulty level attribute.
/// </summary>
/// <param name="memberName">The field name.</param>
/// <seealso cref="Technique"/>
/// <seealso cref="DifficultyLevel"/>
/// <seealso cref="DifficultyLevelAttribute"/>
public sealed partial class MissingDifficultyLevelException([RecordParameter] string memberName) : Exception
{
	/// <inheritdoc/>
	public override string Message => $"The member '{MemberName}' is missing difficulty level attribute.";
}
