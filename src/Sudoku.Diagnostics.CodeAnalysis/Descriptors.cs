namespace Sudoku.Diagnostics.CodeAnalysis;

/// <summary>
/// Provides with the descriptors.
/// </summary>
internal static class Descriptors
{
	/// <summary>
	/// Indicates the descriptor that reports the lacks of the CRTP-constraint clause on a type parameter.
	/// </summary>
	public static readonly DiagnosticDescriptor Sdc0101 = new(
		id: "SDC0101",
		title: "The type parameter lacks a CRTP-constraint clause",
		messageFormat: "The type parameter lacks a CRTP-constraint clause; you should append the type constraint '{0}' into the whole clause 'where {1} : {0}'",
		category: "Sunnie.Usage",
		defaultSeverity: DiagnosticSeverity.Error,
		isEnabledByDefault: true,
		helpLinkUri: null
	);

	/// <summary>
	/// Indicates the descriptor that reports the invalid type constraint.
	/// </summary>
	public static readonly DiagnosticDescriptor Sdc0102 = new(
		id: "SDC0102",
		title: "The type parameter lacks a CRTP type constraint",
		messageFormat: "The type parameter lacks a CRTP type constraint; you should apply the constraint like: 'where {0}: {1}'",
		category: "Sunnie.Usage",
		defaultSeverity: DiagnosticSeverity.Error,
		isEnabledByDefault: true,
		helpLinkUri: null
	);

	/// <summary>
	/// Indicates the descriptor that reports the reference to a discarded parameter.
	/// </summary>
	public static readonly DiagnosticDescriptor Sdc0201 = new(
		id: "SDC0201",
		title: "The discarded parameter can't be used or referenced unless a 'nameof' expression",
		messageFormat: "The discarded parameter can't be used or referenced unless a 'nameof' expression",
		category: "Sunnie.Usage",
		defaultSeverity: DiagnosticSeverity.Error,
		isEnabledByDefault: true,
		helpLinkUri: null
	);

	/// <summary>
	/// Indicates the descriptor that reports the discarded parameter with any modifiers.
	/// </summary>
	public static readonly DiagnosticDescriptor Sdc0202 = new(
		id: "SDC0202",
		title: "Discard parameter can't be modified",
		messageFormat: "Discard parameter can't be modified",
		category: "Sunnie.Usage",
		defaultSeverity: DiagnosticSeverity.Error,
		isEnabledByDefault: true,
		helpLinkUri: null
	);

	/// <summary>
	/// Indicates the dsecriptor that reports the wrong usages on a discard parameter <see langword="_"/>
	/// that is marked <see cref="IsDiscardAttribute"/>.
	/// </summary>
	public static readonly DiagnosticDescriptor Sdc0203 = new(
		id: "SDC0203",
		title: $"Can't apply '{nameof(IsDiscardAttribute)}' onto a parameter that has already discarded",
		messageFormat: $"Can't apply '{nameof(IsDiscardAttribute)}' onto a parameter that has already discarded",
		category: "Sunnie.Usage",
		defaultSeverity: DiagnosticSeverity.Error,
		isEnabledByDefault: true,
		helpLinkUri: null
	);
}
