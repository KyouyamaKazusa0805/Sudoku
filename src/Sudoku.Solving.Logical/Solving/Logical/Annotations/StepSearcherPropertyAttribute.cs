namespace Sudoku.Solving.Logical.Annotations;

/// <summary>
/// Defines an attribute that can be applied to a property in step searcher interface type,
/// indicating the property is represented as a setting property, that can interact
/// with the <see cref="IAnalyzer{TSolver, TSolverResult}"/> type.
/// </summary>
/// <seealso cref="IAnalyzer{TSolver, TSolverResult}"/>
[AttributeUsage(AttributeTargets.Property, Inherited = false)]
public sealed class StepSearcherPropertyAttribute : Attribute;
