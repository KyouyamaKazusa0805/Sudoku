namespace Sudoku.SourceGeneration.CollectedResults;

/// <summary>
/// The nesting data structure for <see cref="AttachedPropertyCollectedResult"/>.
/// </summary>
/// <seealso cref="AttachedPropertyCollectedResult"/>
internal sealed record AttachedPropertyData(
	string PropertyName,
	ITypeSymbol PropertyType,
	DocumentationCommentData DocumentationCommentData,
	string? DefaultValueGeneratingMemberName,
	DefaultValueGeneratingMemberKind? DefaultValueGeneratingMemberKind,
	object? DefaultValue,
	string? CallbackMethodName,
	bool IsNullable
);
