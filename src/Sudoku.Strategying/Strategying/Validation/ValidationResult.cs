namespace Sudoku.Strategying.Validation;

/// <summary>
/// Represents a result data that describes why the validation failed.
/// </summary>
/// <param name="Success">Indicates whether the validation is passed and no error produced.</param>
/// <param name="FailPropertyNames">Indicates the failed property names to be set.</param>
/// <param name="FailedReason">Indicates the failed reason.</param>
/// <param name="FailedSeverity">Indicates the severity of the failure.</param>
public abstract record ValidationResult(
	[property: MemberNotNullWhen(false, "FailPropertyNames")] bool Success,
	string? FailPropertyNames,
	ValidationFailedReason FailedReason,
	ValidationFailedSeverity FailedSeverity
);
