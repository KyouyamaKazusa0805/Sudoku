namespace Sudoku.Analytics.Categorization;

/// <summary>
/// Represents an attribute type that describes for a group that the marked technique belongs to.
/// </summary>
/// <param name="group"><inheritdoc cref="Group" path="/summary"/></param>
[AttributeUsage(AttributeTargets.Field, Inherited = false)]
public sealed class TechniqueGroupAttribute(TechniqueGroup group) : Attribute
{
	/// <summary>
	/// Indicates the group that the current technique belong to.
	/// </summary>
	public TechniqueGroup Group { get; } = group;
}
