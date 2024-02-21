namespace Sudoku.Strategying.Validation;

/// <summary>
/// Represents a result information that describes the confliction.
/// </summary>
/// <param name="IsSuccess">Indicates the confliction result descibes for "success" status, without any confliction.</param>
/// <param name="FailedConstraint">Indicates the failed constraint.</param>
/// <param name="Type">Indicates the confliction type appeared.</param>
/// <param name="Severity">Indicates the severity of the failure.</param>
/// <completionlist cref="ConflictionResult"/>
public abstract record ConflictionResult(
	[property: MemberNotNullWhen(true, "FailedConstraint")] bool IsSuccess,
	Constraint? FailedConstraint,
	ConflictionType Type,
	Severity Severity
)
{
	/// <summary>
	/// Indictes the successful message.
	/// </summary>
	public static ConflictionResult Successful => new NoConflictionResult();


	/// <summary>
	/// Indicates the failed message.
	/// </summary>
	/// <param name="constraint">The constraint.</param>
	/// <param name="type">The type.</param>
	/// <param name="severity">The severity.</param>
	/// <returns>A <see cref="ConflictionResult"/> instance.</returns>
	public static ConflictionResult Failed(Constraint constraint, ConflictionType type, Severity severity)
		=> new ContainsConflictionResult(constraint, type, severity);
}

/// <summary>
/// Indicates there is no confliction.
/// </summary>
file sealed record NoConflictionResult() : ConflictionResult(true, null, default, default);

/// <summary>
/// Indicates there is any confliction appears.
/// </summary>
/// <param name="Constraint">The constraint that creates this error.</param>
/// <param name="Type">The type of the confliction.</param>
/// <param name="Severity">Indicates the severity of this failure.</param>
file sealed record ContainsConflictionResult(Constraint Constraint, ConflictionType Type, Severity Severity) :
	ConflictionResult(false, Constraint, Type, Severity);
