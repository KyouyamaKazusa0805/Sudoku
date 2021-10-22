namespace Sudoku.Diagnostics.CodeGen.Generators;

/// <summary>
/// Defines a source generator that generates the source code for formatting methods.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class AutoFormattable : ISourceGenerator
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

			context.AddSource(
				typeSymbol.ToFileName(),
				GeneratedFileShortcuts.FormattedMethods,
				$@"#nullable enable

namespace {namespaceName};

partial {typeKind}{typeSymbol.Name}{genericParameterList}
{{
	/// <inheritdoc cref=""object.ToString""/>
	[global::System.CodeDom.Compiler.GeneratedCode(""{GetType().FullName}"", ""{VersionValue}"")]
	[global::System.Runtime.CompilerServices.CompilerGenerated]
	[global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
	public override {readOnlyKeyword}string ToString() => ToString(null, null);

	/// <summary>
	/// Returns a string that represents the current object with the specified format string.
	/// </summary>
	/// <param name=""format"">
	/// The format. If available, the parameter can be <see langword=""null""/>.
	/// </param>
	/// <returns>The string result.</returns>
	[global::System.CodeDom.Compiler.GeneratedCode(""{GetType().FullName}"", ""{VersionValue}"")]
	[global::System.Runtime.CompilerServices.CompilerGenerated]
	[global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
	public {readOnlyKeyword}string ToString(string? format) => ToString(format, null);
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

					var attribute = compilation.GetTypeByMetadataName(typeof(AutoFormattableAttribute).FullName)!;
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
