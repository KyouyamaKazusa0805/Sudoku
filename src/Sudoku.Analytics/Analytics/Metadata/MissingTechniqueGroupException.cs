namespace Sudoku.Analytics.Metadata;

/// <summary>
/// Reprsents an exception thrown when a field in <see cref="Technique"/> is missing for technique group attribute.
/// </summary>
/// <param name="_memberName">Indicates the field name.</param>
/// <seealso cref="Technique"/>
/// <seealso cref="TechniqueGroup"/>
/// <seealso cref="TechniqueMetadataAttribute"/>
public sealed class MissingTechniqueGroupException(string _memberName) : Exception
{
	/// <inheritdoc/>
	public override string Message => string.Format(SR.Get("Message_MissingTechniqueGroupException"), _memberName);
}
