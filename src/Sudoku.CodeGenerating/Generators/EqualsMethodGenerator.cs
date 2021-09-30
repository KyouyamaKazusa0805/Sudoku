namespace Sudoku.CodeGenerating.Generators;

/// <summary>
/// Indicates the generator that generates the methods about the equality checking. The methods below
/// will be generated:
/// <list type="bullet">
/// <item><c>bool Equals(object? obj)</c></item>
/// <item><c>bool Equals(T comparer)</c></item>
/// <item><c>bool ==(T left, T right)</c></item>
/// <item><c>bool !=(T left, T right)</c></item>
/// </list>
/// </summary>
/// <remarks>
/// Please note that if the type is a <see langword="ref struct"/>, the first one won't be generated
/// because this method is useless in the by-ref-like types.
/// </remarks>
[Generator]
public sealed partial class EqualsMethodGenerator : ISourceGenerator
{
	/// <inheritdoc/>
	public void Execute(GeneratorExecutionContext context)
	{
		var receiver = (SyntaxReceiver)context.SyntaxReceiver!;
		var compilation = context.Compilation;
		var attributeSymbol = compilation.GetTypeByMetadataName(typeof(AutoEqualityAttribute).FullName);

		foreach (var (typeSymbol, attributeData) in
			from type in receiver.Candidates
			select compilation.GetSemanticModel(type.SyntaxTree).GetDeclaredSymbol(type)! into typeSymbol
			let attributesData = typeSymbol.GetAttributes()
			let attributeData = attributesData.FirstOrDefault(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, attributeSymbol))
			where attributeData is { ConstructorArguments.IsDefaultOrEmpty: false }
			select (typeSymbol, attributeData))
		{
			typeSymbol.DeconstructInfo(
				false, out string fullTypeName, out string namespaceName, out string genericParametersList,
				out string genericParametersListWithoutConstraint, out string typeKind,
				out string readonlyKeyword, out _
			);
			string inKeyword = typeSymbol.TypeKind == TypeKind.Struct ? "in " : string.Empty;
			string nullableAnnotation = typeSymbol.TypeKind == TypeKind.Class ? "?" : string.Empty;
			string nullCheck = typeSymbol.TypeKind == TypeKind.Class ? "other is not null && " : string.Empty;
			string memberCheck = string.Join(
				" && ",
				from arg in attributeData.ConstructorArguments[0].Values
				select ((string)arg.Value!).Trim() into member
				select $"{member} == other.{member}"
			);

			string typeName = typeSymbol.Name;
			string objectEqualsMethod = typeSymbol.IsRefLikeType
				? "// This type is a ref struct, so 'bool Equals(object?) is useless."
				: typeSymbol.IsRecord && typeSymbol.TypeKind == TypeKind.Struct
				? "// This type is a record struct, so 'bool Equals(object?) can't be syntheized."
				: $@"[global::System.CodeDom.Compiler.GeneratedCode(""{GetType().FullName}"", ""{VersionValue}"")]
	[global::System.Runtime.CompilerServices.CompilerGenerated]
	[global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
	public override {readonlyKeyword}bool Equals(object? other) => other is {typeName}{genericParametersList} comparer && Equals(comparer);";

			string specifyEqualsMethod = $@"[global::System.CodeDom.Compiler.GeneratedCode(""{GetType().FullName}"", ""0.3"")]
	[global::System.Runtime.CompilerServices.CompilerGenerated]
	[global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
	public {readonlyKeyword}bool Equals({inKeyword}{typeName}{genericParametersListWithoutConstraint}{nullableAnnotation} other) => {nullCheck}{memberCheck};";

			var memberSymbols = typeSymbol.GetMembers().OfType<IMethodSymbol>();
			string opEquality = isOp(memberSymbols, OperatorNames.Equality)
				? $@"[global::System.CodeDom.Compiler.GeneratedCode(""{GetType().FullName}"", ""{VersionValue}"")]
	[global::System.Runtime.CompilerServices.CompilerGenerated]
	[global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
	public static bool operator ==({inKeyword}{typeName}{genericParametersListWithoutConstraint} left, {inKeyword}{typeName}{genericParametersListWithoutConstraint} right) => left.Equals(right);"
				: "// 'operator ==' does exist in the type.";

			string opInequality = isOp(memberSymbols, OperatorNames.Inequality)
				? $@"[global::System.CodeDom.Compiler.GeneratedCode(""{GetType().FullName}"", ""{VersionValue}"")]
	[global::System.Runtime.CompilerServices.CompilerGenerated]
	[global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
	public static bool operator !=({inKeyword}{typeName}{genericParametersListWithoutConstraint} left, {inKeyword}{typeName}{genericParametersListWithoutConstraint} right) => !(left == right);"
				: "// 'operator !=' does exist in the type.";

			context.AddSource(
				typeSymbol.ToFileName(),
				GeneratedFileShortcuts.EqualsMethod,
				$@"#pragma warning disable CS1591

#nullable enable

namespace {namespaceName};

partial {typeKind}{typeName}{genericParametersList}
{{
	{objectEqualsMethod}

	{specifyEqualsMethod}


	{opEquality}

	{opInequality}
}}
"
			);


			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			static bool isOp(IEnumerable<IMethodSymbol> methods, string operatorName) =>
				methods.All(method => method.Name != operatorName);
		}
	}

	/// <inheritdoc/>
	public void Initialize(GeneratorInitializationContext context) => context.FastRegister<SyntaxReceiver>();
}
