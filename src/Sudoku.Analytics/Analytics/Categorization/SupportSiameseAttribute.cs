namespace Sudoku.Analytics.Categorization;

/// <summary>
/// Represents an attribute type that describes whether a <see cref="TechniqueGroup"/> instance supports for Siamese rule.
/// </summary>
/// <param name="supportSiamese">A <see cref="bool"/> value indicating that.</param>
/// <seealso cref="TechniqueGroup"/>
[AttributeUsage(AttributeTargets.Field, Inherited = false)]
public sealed partial class SupportSiameseAttribute([PrimaryCosntructorParameter] bool supportSiamese) : Attribute;
