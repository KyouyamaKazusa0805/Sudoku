namespace Sudoku.Analytics.Metadata;

/// <summary>
/// Represents an attribute type that describes
/// whether a <see cref="Technique"/> or <see cref="TechniqueGroup"/> instance supports for Siamese rule.
/// </summary>
/// <param name="supportsSiamese">
/// A <see cref="bool"/> value indicating whether a <see cref="Technique"/> or <see cref="TechniqueGroup"/> instance
/// supports for Siamese rule.
/// </param>
/// <seealso cref="TechniqueGroup"/>
[AttributeUsage(AttributeTargets.Field, Inherited = false)]
public sealed partial class IsSiameseSupportedAttribute([PrimaryConstructorParameter] bool supportsSiamese) : Attribute;
