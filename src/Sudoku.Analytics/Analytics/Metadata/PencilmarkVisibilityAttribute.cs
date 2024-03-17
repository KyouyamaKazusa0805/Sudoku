namespace Sudoku.Analytics.Metadata;

/// <summary>
/// Indicates the visibility mode for pencilmarks that will be used in puzzle-solving for a technique.
/// </summary>
/// <param name="visibilityModes">The supported visibility modes.</param>
[AttributeUsage(AttributeTargets.Field, Inherited = false)]
public sealed partial class PencilmarkVisibilityAttribute([PrimaryConstructorParameter] PencilmarkVisibility visibilityModes) : Attribute;
