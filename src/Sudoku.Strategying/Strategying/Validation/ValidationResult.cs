namespace Sudoku.Strategying.Validation;

/// <summary>
/// Represents a result data that describes why the validation failed.
/// </summary>
/// <param name="Success">Indicates whether the validation is passed and no error produced.</param>
/// <param name="FailedPropertyName">Indicates the failed property name to be set.</param>
/// <param name="Reason">Indicates the failed reason.</param>
/// <param name="Severity">Indicates the severity of the failure.</param>
public abstract record ValidationResult(
	[property: MemberNotNullWhen(false, "FailedPropertyName")] bool Success,
	string? FailedPropertyName,
	ValidationReason Reason,
	ValidationSeverity Severity
)
{
	/// <summary>
	/// Represents a <see cref="ValidationResult"/> that describes for a successful message.
	/// </summary>
	public static ValidationResult Successful => new SuccessValidationResult();


	/// <summary>
	/// Represents a <see cref="ValidationResult"/> that describes for a failed message.
	/// </summary>
	/// <param name="failedPropertyName">Indicates the name of the failed property.</param>
	/// <param name="reason">Indicates the reason why raises the failure.</param>
	/// <param name="severity">Indicates the severity of the failure.</param>
	/// <returns>The <see cref="ValidationResult"/> instance.</returns>
	public static ValidationResult Failed(string failedPropertyName, ValidationReason reason, ValidationSeverity severity)
		=> new FailedValidationResult(failedPropertyName, reason, severity);
}
