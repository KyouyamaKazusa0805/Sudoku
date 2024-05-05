namespace Sudoku.Measuring;

/// <summary>
/// Represents a delegate type that calculates the difficulty rating using the specified operation,
/// with specified arguments.
/// </summary>
/// <param name="arguments">The arguments to be passed.</param>
/// <returns>An <see cref="int"/> value indicating the result difficulty rating.</returns>
public delegate int ParameterizedFormula(params object?[]? arguments);
