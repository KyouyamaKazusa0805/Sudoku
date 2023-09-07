using Sudoku.SourceGeneration.Handlers;

namespace Sudoku.SourceGeneration.CollectedResults;

/// <summary>
/// Indicates the data collected via <see cref="PrimaryConstructorHandler"/>.
/// </summary>
/// <seealso cref="PrimaryConstructorHandler"/>
internal sealed record PrimaryConstructorCollectedResult(
	string ParameterName,
	TypeKind TypeKind,
	RefKind RefKind,
	ScopedKind ScopedKind,
	NullableAnnotation NullableAnnotation,
	ITypeSymbol ParameterType,
	INamedTypeSymbol TypeSymbol,
	bool IsReadOnly,
	string Namesapce,
	string Type,
	bool IsRecord,
	AttributeData[] AttributesData,
	string? Comment
);
