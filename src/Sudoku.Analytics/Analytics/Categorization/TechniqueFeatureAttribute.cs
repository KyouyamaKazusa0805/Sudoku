using System.SourceGeneration;

namespace Sudoku.Analytics.Categorization;

/// <summary>
/// Represents a type that is used by a field in <see cref="Technique"/> type, indicating the runtime feature to be described.
/// </summary>
/// <seealso cref="Technique"/>
[AttributeUsage(AttributeTargets.Field, Inherited = false)]
public sealed partial class TechniqueFeatureAttribute([Data] TechniqueFeature features) : Attribute;
