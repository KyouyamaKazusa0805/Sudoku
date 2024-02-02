namespace Sudoku.Analytics.Metadata;

/// <summary>
/// Reprsents an exception thrown when a field in <see cref="Technique"/> is missing for technique group attribute.
/// </summary>
/// <param name="memberName">Indicates the field name.</param>
/// <seealso cref="Technique"/>
/// <seealso cref="TechniqueGroup"/>
/// <seealso cref="TechniqueGroupAttribute"/>
public sealed partial class MissingTechniqueGroupException([PrimaryCosntructorParameter] string memberName) : Exception
{
	/// <inheritdoc/>
	public override string Message => $"The member '{MemberName}' is missing technique group attribute.";
}
