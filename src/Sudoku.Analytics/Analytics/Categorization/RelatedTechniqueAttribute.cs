namespace Sudoku.Analytics.Categorization;

/// <summary>
/// Represents an attribute type that describes the related technique of a subtype.
/// </summary>
/// <param name="technique">Indicates the technique related.</param>
[AttributeUsage(AttributeTargets.Field, Inherited = false)]
public sealed partial class RelatedTechniqueAttribute([RecordParameter] Technique technique) : Attribute;
