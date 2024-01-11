namespace Sudoku.Analytics.Categorization;

/// <summary>
/// Represents an attribute type that describes for a group that the marked technique belongs to.
/// </summary>
/// <param name="group">Indicates the group that the current technique belong to.</param>
[AttributeUsage(AttributeTargets.Field, Inherited = false)]
public sealed partial class TechniqueGroupAttribute([RecordParameter] TechniqueGroup group) : Attribute;
