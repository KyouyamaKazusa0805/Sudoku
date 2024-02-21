namespace Sudoku.Strategying.Validation;

/// <summary>
/// Represents a result data that describes why the validation failed.
/// </summary>
/// <param name="IsSuccess">Indicates whether the validation is passed and no error produced.</param>
/// <param name="FailedPropertyName">Indicates the failed property name to be set.</param>
/// <param name="Reason">Indicates the failed reason.</param>
/// <param name="Severity">Indicates the severity of the failure.</param>
/// <completionlist cref="ValidationResult"/>
public abstract record ValidationResult(
	[property: MemberNotNullWhen(false, "FailedPropertyName")] bool IsSuccess,
	string? FailedPropertyName,
	ValidationReason Reason,
	Severity Severity
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
	public static ValidationResult Failed(string failedPropertyName, ValidationReason reason, Severity severity)
		=> new FailedValidationResult(failedPropertyName, reason, severity);
}

/// <summary>
/// Represents a result that is succeeded after executed.
/// </summary>
file sealed record SuccessValidationResult() : ValidationResult(true, null, default, default);

/// <summary>
/// Represents a result why causes the failure.
/// </summary>
/// <param name="FailPropertyName">Indicates the property name that makes the validation failed.</param>
/// <param name="Reason"><inheritdoc cref="ValidationResult" path="/param[@name='Reason']"/></param>
/// <param name="Severity"><inheritdoc cref="ValidationResult" path="/param[@name='Severity']"/></param>
file sealed record FailedValidationResult(string FailPropertyName, ValidationReason Reason, Severity Severity) :
	ValidationResult(false, FailPropertyName, Reason, Severity);
