using System.Linq.Expressions;

namespace Sudoku.Analytics.Rating;

/// <summary>
/// Represents a factor that describes for a difficulty factor describing how difficult that users can locate a technique pattern.
/// </summary>
/// <param name="FactorName">Indicates the factor name.</param>
/// <param name="MeasureExpression">Indicates the formula that measures the difficulty of the current factor.</param>
/// <param name="VariableDescriptions">The variable descriptions.</param>
public readonly record struct LocatingDifficultyFactor(string FactorName, Expression MeasureExpression, VariableDescriptor[]? VariableDescriptions);
