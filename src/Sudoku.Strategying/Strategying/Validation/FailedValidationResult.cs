namespace Sudoku.Strategying.Validation;

/// <summary>
/// Represents a result why causes the failure.
/// </summary>
/// <param name="FailPropertyName">Indicates the property name that makes the validation failed.</param>
/// <param name="Reason"><inheritdoc cref="ValidationResult" path="/param[@name='Reason']"/></param>
/// <param name="Severity"><inheritdoc cref="ValidationResult" path="/param[@name='Severity']"/></param>
public sealed record FailedValidationResult(string FailPropertyName, ValidationReason Reason, ValidationSeverity Severity) :
	ValidationResult(false, FailPropertyName, Reason, Severity);
