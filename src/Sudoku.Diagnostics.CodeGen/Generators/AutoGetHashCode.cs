namespace Sudoku.Diagnostics.CodeGen.Generators;

/// <summary>
/// Defines a source generator that generates the source code for getting hash code.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class AutoGetHashCode : ISourceGenerator
{
	/// <summary>
	/// The result node collection.
	/// </summary>
	private readonly ICollection<(INamedTypeSymbol Symbol, AttributeData AttributeData)> _resultCollection =
		new List<(INamedTypeSymbol, AttributeData)>();


	/// <inheritdoc/>
	public void Execute(GeneratorExecutionContext context)
	{
		foreach (var (typeSymbol, attributeData) in _resultCollection)
		{
			var (
				typeName, _, namespaceName, genericParameterList, _, typeKind, readOnlyKeyword, _, _, _
			) = SymbolOutputInfo.FromSymbol(typeSymbol);

			string hashCodeStr = string.Join(
				" ^ ",
				from member in attributeData.ConstructorArguments[0].Values
				let memberValue = ((string)member.Value!).Trim()
				select $"{memberValue}.GetHashCode()"
			);

			context.AddSource(
				typeSymbol.ToFileName(),
				GeneratedFileShortcuts.GetHashCode,
				$@"#nullable enable

namespace {namespaceName};

partial {typeKind}{typeSymbol.Name}{genericParameterList}
{{
	/// <inheritdoc cref=""object.GetHashCode""/>
	[global::System.CodeDom.Compiler.GeneratedCode(""{GetType().FullName}"", ""{VersionValue}"")]
	[global::System.Runtime.CompilerServices.CompilerGenerated]
	[global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
	public override {readOnlyKeyword}int GetHashCode() => {hashCodeStr};
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

					var attribute = compilation.GetTypeByMetadataName(typeof(AutoGetHashCodeAttribute).FullName)!;
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
