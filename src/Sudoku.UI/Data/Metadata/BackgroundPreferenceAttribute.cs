namespace Sudoku.UI.Data.Metadata;

/// <summary>
/// Defines an attribute that can be applied to a preference item property,
/// indicating the property is not exposed outside for users' usages.
/// </summary>
[AttributeUsage(AttributeTargets.Property, Inherited = false)]
public sealed class BackgroundPreferenceAttribute : Attribute
{
}
