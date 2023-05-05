namespace Sudoku.Diagnostics.CodeGen;

/// <summary>
/// The nesting data structure for <see cref="DependencyPropertyCollectedResult"/>.
/// </summary>
/// <seealso cref="DependencyPropertyCollectedResult"/>
internal sealed record DependencyPropertyData(
	string PropertyName,
	ITypeSymbol PropertyType,
	DocumentationCommentData DocumentationCommentData,
	string? DefaultValueGeneratingMemberName,
	DefaultValueGeneratingMemberKind? DefaultValueGeneratingMemberKind,
	object? DefaultValue,
	string? CallbackMethodName,
	bool IsNullable,
	Accessibility Accessibility,
	string[]? MembersNotNullWhenReturnsTrue
);
