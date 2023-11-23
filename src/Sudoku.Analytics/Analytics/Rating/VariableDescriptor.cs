using System.Linq.Expressions;

namespace Sudoku.Analytics.Rating;

/// <summary>
/// Represents a variable description.
/// </summary>
/// <param name="Variable">The variable expression.</param>
/// <param name="Description">The variable description.</param>
public readonly record struct VariableDescriptor(ParameterExpression Variable, string Description);
