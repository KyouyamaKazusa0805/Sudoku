namespace Sudoku.Diagnostics.CodeGen;

/// <summary>
/// Provides with the shared <see cref="DiagnosticDescriptor"/> instances.
/// </summary>
internal static class DiagnosticDescriptors
{
	public static readonly DiagnosticDescriptor SCA0001 = new(
		id: nameof(SCA0001),
		title: "Ref structs lacks of the keyword 'partial'",
		messageFormat: "Ref structs lacks of the keyword 'partial'",
		category: "SourceGen",
		defaultSeverity: DiagnosticSeverity.Error,
		isEnabledByDefault: true,
		helpLinkUri: "https://sunnieshine.github.io/Sudoku/code-analysis/sca0001");

	public static readonly DiagnosticDescriptor SCA0002 = new(
		id: nameof(SCA0002),
		title: "The type lacks of the keyword 'partial'",
		messageFormat: "The type lacks of the keyword 'partial'",
		category: "SourceGen",
		defaultSeverity: DiagnosticSeverity.Error,
		isEnabledByDefault: true,
		helpLinkUri: "https://sunnieshine.github.io/Sudoku/code-analysis/sca0002");

	public static readonly DiagnosticDescriptor SCA0003 = new(
		id: nameof(SCA0003),
		title: "The struct type has already had a parameterless constructor",
		messageFormat: "The struct type has already had a parameterless constructor; please remove it",
		category: "SourceGen",
		defaultSeverity: DiagnosticSeverity.Error,
		isEnabledByDefault: true,
		helpLinkUri: "https://sunnieshine.github.io/Sudoku/code-analysis/sca0003");

	public static readonly DiagnosticDescriptor SCA0004 = new(
		id: nameof(SCA0004),
		title: "You cannot set both 'Message' and 'SuggestedMemberName' to null value",
		messageFormat: "You cannot set both 'Message' and 'SuggestedMemberName' to null value",
		category: "SourceGen",
		defaultSeverity: DiagnosticSeverity.Warning,
		isEnabledByDefault: true,
		helpLinkUri: "https://sunnieshine.github.io/Sudoku/code-analysis/sca0004");

	public static readonly DiagnosticDescriptor SCA0005 = new(
		id: nameof(SCA0005),
		title: "You cannot set both 'Message' or 'SuggestedMemberName' to a not-null value",
		messageFormat: "You cannot set both 'Message' or 'SuggestedMemberName' to a not-null value",
		category: "SourceGen",
		defaultSeverity: DiagnosticSeverity.Warning,
		isEnabledByDefault: true,
		helpLinkUri: "https://sunnieshine.github.io/Sudoku/code-analysis/sca0005");

	public static readonly DiagnosticDescriptor SCA0006 = new(
		id: nameof(SCA0006),
		title: "The attribute type can only work for top-levelled types",
		messageFormat: "The attribute type can only work for top-levelled types",
		category: "SourceGen",
		defaultSeverity: DiagnosticSeverity.Warning,
		isEnabledByDefault: true,
		helpLinkUri: "https://sunnieshine.github.io/Sudoku/code-analysis/sca0006");

	public static readonly DiagnosticDescriptor SCA0007 = new(
		id: nameof(SCA0007),
		title: "The attribute requires at least one element in the 'params' array argument",
		messageFormat: "The attribute requires at least one element in the 'params' array argument",
		category: "SourceGen",
		defaultSeverity: DiagnosticSeverity.Warning,
		isEnabledByDefault: true,
		helpLinkUri: "https://sunnieshine.github.io/Sudoku/code-analysis/sca0007");

	public static readonly DiagnosticDescriptor SCA0008 = new(
		id: nameof(SCA0008),
		title: "The argument is mismatched",
		messageFormat: "The argument is mismatched; the argument will be ignored",
		category: "SourceGen",
		defaultSeverity: DiagnosticSeverity.Warning,
		isEnabledByDefault: true,
		helpLinkUri: "https://sunnieshine.github.io/Sudoku/code-analysis/sca0008");

	public static readonly DiagnosticDescriptor SCA0009 = new(
		id: nameof(SCA0009),
		title: "The argument cannot point to a real member",
		messageFormat: "The argument cannot point to a real member, which means the argument name is not a valid member name; the argument will be ignored",
		category: "SourceGen",
		defaultSeverity: DiagnosticSeverity.Warning,
		isEnabledByDefault: true,
		helpLinkUri: "https://sunnieshine.github.io/Sudoku/code-analysis/sca0009");

	public static readonly DiagnosticDescriptor SCA0010 = new(
		id: nameof(SCA0010),
		title: "The corresponding property type cannot be pointer one",
		messageFormat: "The corresponding property type cannot be pointer one; the argument will be ignored",
		category: "SourceGen",
		defaultSeverity: DiagnosticSeverity.Warning,
		isEnabledByDefault: true,
		helpLinkUri: "https://sunnieshine.github.io/Sudoku/code-analysis/sca0010");

	public static readonly DiagnosticDescriptor SCA0011 = new(
		id: nameof(SCA0011),
		title: "Method 'ToString' has already been declared by user",
		messageFormat: "Method 'ToString' has already been declared by user; please remove it",
		category: "SourceGen",
		defaultSeverity: DiagnosticSeverity.Warning,
		isEnabledByDefault: true,
		helpLinkUri: "https://sunnieshine.github.io/Sudoku/code-analysis/sca0011");
}
