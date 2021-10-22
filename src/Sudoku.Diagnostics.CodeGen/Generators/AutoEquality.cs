namespace Sudoku.Diagnostics.CodeGen.Generators;

/// <summary>
/// Defines a source generator that generates the source code
/// for equality methods and the equality operator overloading.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class AutoEquality : ISourceGenerator
{
	/// <summary>
	/// The result collection.
	/// </summary>
	private readonly ICollection<(INamedTypeSymbol Symbol, AttributeData AttributeData)> _resultCollection =
		new List<(INamedTypeSymbol, AttributeData)>();


	/// <inheritdoc/>
	public void Execute(GeneratorExecutionContext context)
	{
		foreach (var (typeSymbol, attributeData) in _resultCollection)
		{
			var (
				typeName, fullTypeName, namespaceName, genericParameterList, genericParameterListWithoutConstraint,
				typeKind, readOnlyKeyword, inKeyword, nullableAnnotation, _
			) = SymbolOutputInfo.FromSymbol(typeSymbol);
			string nullCheck = typeSymbol.TypeKind == TypeKind.Class ? "other is not null && " : string.Empty;
			string memberCheck = string.Join(
				" && ",
				from arg in attributeData.ConstructorArguments[0].Values
				select ((string)arg.Value!).Trim() into member
				select $"{member} == other.{member}"
			);

			string objectEqualsMethod = typeSymbol.IsRefLikeType
				? "// This type is a ref struct, so 'bool Equals(object?) is useless."
				: typeSymbol.IsRecord && typeSymbol.TypeKind == TypeKind.Struct
					? "// This type is a record struct, so 'bool Equals(object?) can't be syntheized."
					: $@"/// <inheritdoc cref=""object.Equals(object?)""/>
	[global::System.CodeDom.Compiler.GeneratedCode(""{GetType().FullName}"", ""{VersionValue}"")]
	[global::System.Runtime.CompilerServices.CompilerGenerated]
	[global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
	public override {readOnlyKeyword}bool Equals(object? other) => other is {typeName}{genericParameterList} comparer && Equals(comparer);";

			var memberSymbols = typeSymbol.GetMembers().OfType<IMethodSymbol>();
			string opEquality = memberSymbols.All(static method => method.Name != OperatorNames.Equality)
				? $@"/// <summary>
	/// Determines whether the two instances contain a same value.
	/// </summary>
	/// <param name=""left"">The left-side-operator instance to compare.</param>
	/// <param name=""right"">The right-side-operator instance to compare.</param>
	/// <returns>A <see cref=""bool""/> result.</returns>
	[global::System.CodeDom.Compiler.GeneratedCode(""{GetType().FullName}"", ""{VersionValue}"")]
	[global::System.Runtime.CompilerServices.CompilerGenerated]
	[global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
	public static bool operator ==({inKeyword}{typeName}{genericParameterListWithoutConstraint} left, {inKeyword}{typeName}{genericParameterListWithoutConstraint} right) => left.Equals(right);"
				: "// 'operator ==' does exist in the type.";

			string opInequality = memberSymbols.All(static method => method.Name != OperatorNames.Inequality)
				? $@"/// <summary>
	/// Determines whether the two instances don't contain a same value.
	/// </summary>
	/// <param name=""left"">The left-side-operator instance to compare.</param>
	/// <param name=""right"">The right-side-operator instance to compare.</param>
	/// <returns>A <see cref=""bool""/> result.</returns>
	[global::System.CodeDom.Compiler.GeneratedCode(""{GetType().FullName}"", ""{VersionValue}"")]
	[global::System.Runtime.CompilerServices.CompilerGenerated]
	[global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
	public static bool operator !=({inKeyword}{typeName}{genericParameterListWithoutConstraint} left, {inKeyword}{typeName}{genericParameterListWithoutConstraint} right) => !(left == right);"
				: "// 'operator !=' does exist in the type.";

			context.AddSource(
				typeSymbol.ToFileName(),
				GeneratedFileShortcuts.EqualsMethod,
				$@"#nullable enable

namespace {namespaceName};

partial {typeKind}{typeName}{genericParameterList}
{{
	{objectEqualsMethod}

	/// <summary>
	/// Indicates whether the current object is equal to another object of the same type.
	/// </summary>
	/// <param name=""other"">An object to compare with this object.</param>
	/// <returns>
	/// <see langword=""true""/> if the current object is equal to the other parameter;
	/// otherwise, <see langword=""false""/>.
	/// </returns>
	[global::System.CodeDom.Compiler.GeneratedCode(""{GetType().FullName}"", ""{VersionValue}"")]
	[global::System.Runtime.CompilerServices.CompilerGenerated]
	[global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
	public {readOnlyKeyword}bool Equals({inKeyword}{typeName}{genericParameterListWithoutConstraint}{nullableAnnotation} other) => {nullCheck}{memberCheck};


	{opEquality}

	{opInequality}
}}
"
			);
		}
	}

	/// <inheritdoc/>
	public void Initialize(GeneratorInitializationContext context) =>
		context.RegisterForSyntaxNotifications(
			() => SyntaxContextReceiverCreator.Create(
				(syntaxNode, semanticModel) =>
				{
					if (
						(
							SyntaxNode: syntaxNode,
							SemanticModel: semanticModel,
							Context: context
						) is not (
							SyntaxNode: TypeDeclarationSyntax { AttributeLists.Count: not 0 } n,
							SemanticModel: { Compilation: { } compilation },
							Context: { CancellationToken: var cancellationToken }
						)
					)
					{
						return;
					}

					if (semanticModel.GetDeclaredSymbol(n, cancellationToken) is not { } typeSymbol)
					{
						return;
					}

					var attribute = compilation.GetTypeByMetadataName(typeof(AutoEqualityAttribute).FullName)!;
					var attributesData = typeSymbol.GetAttributes();
					var attributeData = attributesData.FirstOrDefault(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, attribute));
					if (attributeData is not { ConstructorArguments.IsDefaultOrEmpty: false })
					{
						return;
					}

					_resultCollection.Add((typeSymbol, attributeData));
				}
			)
		);
}
