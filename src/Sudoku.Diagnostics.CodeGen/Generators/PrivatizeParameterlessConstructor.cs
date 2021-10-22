namespace Sudoku.Diagnostics.CodeGen.Generators;

/// <summary>
/// Provides a source generator that automatically generates the source code for disability of the
/// usage of the parameterless constructor outside the type range.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class PrivatizeParameterlessConstructor : ISourceGenerator
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
			var (_, _, namespaceName, _, _, _, _, _, _, _) = SymbolOutputInfo.FromSymbol(typeSymbol, true);
			if (typeSymbol is not { TypeArguments.IsDefaultOrEmpty: true, Name: var typeName })
			{
				continue;
			}

			context.AddSource(
				typeSymbol.ToFileName(),
				GeneratedFileShortcuts.PrivateParameterlessConstructor,
				$@"#nullable enable

namespace {namespaceName};

partial class {typeName}
{{
	/// <summary>
	/// <para>Indicates the parameterless constructor.</para>
	/// <para><i>This constructor can't be used anyway.</i></para>
	/// </summary>
	/// <exception cref=""NotSupportedException"">Always throws.</exception>
	[global::System.CodeDom.Compiler.GeneratedCode(""{GetType().FullName}"", ""{VersionValue}"")]
	[global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER || (NETCOREAPP3_0 || NETCOREAPP3_1)
	[global::System.Obsolete(""You can't call or invoke this constructor anyway."", true, DiagnosticId = ""BAN"")]
#else
	[global::System.Obsolete(""You can't call or invoke this constructor anyway."", true)]
#endif
	[global::System.Runtime.CompilerServices.CompilerGenerated]
	private {typeName}() => throw new NotSupportedException();
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

					var attribute = compilation.GetTypeByMetadataName(typeof(PrivatizeParameterlessConstructorAttribute).FullName)!;
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
