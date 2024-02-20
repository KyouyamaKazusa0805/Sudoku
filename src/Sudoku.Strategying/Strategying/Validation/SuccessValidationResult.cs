namespace Sudoku.Strategying.Validation;

/// <summary>
/// Represents a result that is succeeded after executed.
/// </summary>
public sealed record SuccessValidationResult() : ValidationResult(true, null, default, default);
