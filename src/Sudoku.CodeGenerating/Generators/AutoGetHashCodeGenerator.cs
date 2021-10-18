namespace Sudoku.CodeGenerating.Generators;

/// <summary>
/// Indicates the generator that generates the code that overrides <see cref="object.GetHashCode"/>.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class AutoGetHashCodeGenerator : IIncrementalGenerator
{
	/// <inheritdoc/>
	public void Initialize(IncrementalGeneratorInitializationContext context) =>
		context.RegisterImplementationSourceOutput(
			context.SyntaxProvider.CreateSyntaxProvider(WithAttributes, Transform),
			SourceProduce
		);

	private bool WithAttributes(SyntaxNode n, CancellationToken _) =>
		n is TypeDeclarationSyntax { AttributeLists.Count: not 0 };

	private (INamedTypeSymbol, AttributeData)? Transform(GeneratorSyntaxContext gsc, CancellationToken c)
	{
		if (
			gsc is not
			{
				Node: var originalNode,
				SemanticModel: { Compilation: var compilation } semanticModel
			}
		)
		{
			return null;
		}

		if (originalNode is not TypeDeclarationSyntax { AttributeLists.Count: not 0 } node)
		{
			return null;
		}

		if (semanticModel.GetDeclaredSymbol(node, c) is not { } typeSymbol)
		{
			return null;
		}

		var attribute = compilation.GetTypeByMetadataName(typeof(AutoHashCodeAttribute).FullName)!;
		var attributeData = typeSymbol.FirstAttributeData(attribute);
		if (attributeData is not { ConstructorArguments.IsDefaultOrEmpty: false })
		{
			return null;
		}

		return (typeSymbol, attributeData);
	}

	private void SourceProduce(SourceProductionContext spc, (INamedTypeSymbol, AttributeData)? pair)
	{
		if (pair is not (var typeSymbol, var attributeData))
		{
			return;
		}

		typeSymbol.DeconstructInfo(
			true, out string fullTypeName, out string namespaceName, out string genericParametersList,
			out _, out string typeKind, out string readonlyKeyword, out _
		);

		string hashCodeStr = string.Join(
			" ^ ",
			from member in attributeData.ConstructorArguments[0].Values
			let memberValue = ((string)member.Value!).Trim()
			select $"{memberValue}.GetHashCode()"
		);

		spc.AddSource(
			typeSymbol.ToFileName(),
			GeneratedFileShortcuts.GetHashCode,
			$@"#nullable enable

namespace {namespaceName};

partial {typeKind}{typeSymbol.Name}{genericParametersList}
{{
	/// <inheritdoc cref=""object.GetHashCode""/>
	[global::System.CodeDom.Compiler.GeneratedCode(""{GetType().FullName}"", ""{VersionValue}"")]
	[global::System.Runtime.CompilerServices.CompilerGenerated]
	[global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
	public override {readonlyKeyword}int GetHashCode() => {hashCodeStr};
}}
"
		);
	}
}
