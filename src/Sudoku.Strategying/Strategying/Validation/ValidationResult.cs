namespace Sudoku.Strategying.Validation;

/// <summary>
/// Represents a result data that describes why the validation failed.
/// </summary>
/// <param name="Success">Indicates whether the validation is passed and no error produced.</param>
/// <param name="FailedPropertyNames">Indicates the failed property names to be set.</param>
/// <param name="Reason">Indicates the failed reason.</param>
/// <param name="Severity">Indicates the severity of the failure.</param>
public abstract record ValidationResult(
	[property: MemberNotNullWhen(false, "FailedPropertyNames")] bool Success,
	string? FailedPropertyNames,
	ValidationReason Reason,
	ValidationSeverity Severity
);
