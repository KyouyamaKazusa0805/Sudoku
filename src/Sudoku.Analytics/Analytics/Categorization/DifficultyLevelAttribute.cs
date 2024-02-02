namespace Sudoku.Analytics.Categorization;

/// <summary>
/// Provides with an attribute type that represents the target difficulty level for the specified technique.
/// </summary>
/// <param name="level">The difficulty level. This value is used to describe the target difficulty for the specified technique.</param>
[AttributeUsage(AttributeTargets.Field, Inherited = false)]
public sealed partial class DifficultyLevelAttribute([PrimaryCosntructorParameter] DifficultyLevel level) : Attribute;
