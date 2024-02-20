namespace Sudoku.Strategying.Validation;

/// <summary>
/// Represents a result why causes the failure.
/// </summary>
/// <param name="FailPropertyName">Indicates the property name that makes the validation failed.</param>
/// <param name="Reason"><inheritdoc/></param>
/// <param name="Severity"><inheritdoc/></param>
public sealed record FailedValidationResult(string FailPropertyName, ValidationFailedReason Reason, ValidationFailedSeverity Severity) :
	ValidationResult(false, FailPropertyName, Reason, Severity);
