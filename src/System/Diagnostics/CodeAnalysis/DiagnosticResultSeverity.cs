namespace System.Diagnostics.CodeAnalysis;

/// <summary>
/// Defines a severity that contains different level of the diagnostic results.
/// </summary>
[Closed]
public enum DiagnosticResultSeverity : byte
{
	/// <summary>
	/// Indicates the diagnotic severity is none.
	/// </summary>
	None,

	/// <summary>
	/// Indicates the diagnotic severity is hidden.
	/// </summary>
	Hidden,

	/// <summary>
	/// Indicates the diagnotic severity is information.
	/// </summary>
	Info,

	/// <summary>
	/// Indicates the diagnotic severity is warning.
	/// </summary>
	Warning,

	/// <summary>
	/// Indicates the diagnotic severity is error.
	/// </summary>
	Error
}