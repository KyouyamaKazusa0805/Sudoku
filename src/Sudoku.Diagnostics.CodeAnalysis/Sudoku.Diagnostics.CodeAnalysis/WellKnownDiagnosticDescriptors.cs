namespace Sudoku.Diagnostics.CodeAnalysis;

/// <summary>
/// Represents with the well-known diagnostic descriptors.
/// </summary>
internal static class WellKnownDiagnosticDescriptors
{
	public static readonly DiagnosticDescriptor SCA0001 =
		new(
			nameof(SCA0001),
			"Special type missing",
			"Special type missing: '{0}'",
			CategoryKind.CodeAnalysis,
			DiagnosticSeverity.Error,
			true,
			"Special type is missing; please check the code analysis project and change the namespace."
		);

	public static readonly DiagnosticDescriptor SCA0101 =
		new(
			nameof(SCA0101),
			"Don't initialize large structure",
			"Don't initialize large structure",
			CategoryKind.Usage,
			DiagnosticSeverity.Warning,
			true,
			"Don't initialize large structure; please use expected fields or 'Create' methods instead if available."
		);
}
