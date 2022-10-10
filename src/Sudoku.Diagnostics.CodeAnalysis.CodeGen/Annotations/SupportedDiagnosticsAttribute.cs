namespace Sudoku.Diagnostics.CodeGen.Annotations;

/// <summary>
/// Indicates an attribute type that can be applied to a code analyzer type, telling the source generators
/// that the analyzer supports the specified diagnostics.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class SupportedDiagnosticsAttribute : Attribute
{
	/// <summary>
	/// Initializes a <see cref="SupportedDiagnosticsAttribute"/> instance via the specified supported diagnostic IDs.
	/// </summary>
	/// <param name="supportedDiagnosticIds">Supported diagnostic IDs.</param>
	public SupportedDiagnosticsAttribute(params string[] supportedDiagnosticIds) => SupportedDiagnosticIds = supportedDiagnosticIds;


	/// <summary>
	/// Indicates the supported diagnostic IDs.
	/// </summary>
	public string[] SupportedDiagnosticIds { get; }

	/// <summary>
	/// The custom property names. The default value is <see langword="null"/>.
	/// </summary>
	public string[]? CustomPropertyNames { get; init; }
}
