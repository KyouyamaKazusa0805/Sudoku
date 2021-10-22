namespace Sudoku.Diagnostics.CodeGen.Generators;

/// <summary>
/// Defines a source generator that generates the source code for deconstruction methods.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class AutoDeconstruct : ISourceGenerator
{
	/// <summary>
	/// The result collection.
	/// </summary>
	private readonly ICollection<(INamedTypeSymbol Symbol, INamedTypeSymbol Attribute, IEnumerable<AttributeData> AttributesData, Compilation Compilation)> _resultCollection =
		new List<(INamedTypeSymbol, INamedTypeSymbol, IEnumerable<AttributeData>, Compilation)>();


	/// <inheritdoc/>
	public void Execute(GeneratorExecutionContext context)
	{
		foreach (var (typeSymbol, attribute, attributesData, compilation) in _resultCollection)
		{
			var (
				typeName, fullTypeName, namespaceName, genericParameterList, genericParameterListWithoutConstraint,
				typeKind, readOnlyKeyword, inKeyword, nullableAnnotation, _
			) = SymbolOutputInfo.FromSymbol(typeSymbol);

			var possibleMembers = (
				from typeDetail in TypeDetail.GetDetailList(typeSymbol, attribute, false)
				select typeDetail
			).ToArray();
			string methods = string.Join(
				"\r\n\r\n\t",
				from attributeData in attributesData
				let memberArgs = attributeData.ConstructorArguments[0].Values
				select (from memberArg in memberArgs select ((string)memberArg.Value!).Trim()) into members
				where members.All(m => possibleMembers.Any(p => p.Name == m))
				let details = from m in members select possibleMembers.First(p => p.Name == m)
				let deprecatedTypeNames = (
					from detail in details
					let tempTypeName = detail.FullTypeName
					where !KeywordsToBclNames.ContainsKey(tempTypeName)
					let tempSymbol = detail.Symbol
					where compilation.TypeArgumentMarked<ObsoleteAttribute>(tempSymbol)
					select $"'{tempTypeName}'"
				).ToArray()
				let obsoleteAttributeStr = deprecatedTypeNames.Length switch
				{
					0 => string.Empty,
					1 => $"\r\n\t[global::System.Obsolete(\"The method is deprecated because the inner type {deprecatedTypeNames[0]} is deprecated.\", false)]",
					> 1 => $"\r\n\t[global::System.Obsolete(\"The method is deprecated because the inner types {string.Join(", ", deprecatedTypeNames)} are deprecated.\", false)]",
				}
				let paramNames = from paramInfo in details select paramInfo.OutParameterDeclaration
				let paramNamesStr = string.Join(", ", paramNames)
				let assignments =
					from m in members
					let paramName = possibleMembers.First(p => p.Name == m).Name.ToCamelCase()
					select $"{paramName} = {m};"
				let assignmentsStr = string.Join("\r\n\t\t", assignments)
				select $@"/// <summary>
	/// Deconstruct the instance into multiple elements.
	/// </summary>
	[global::System.CodeDom.Compiler.GeneratedCode(""{GetType().FullName}"", ""{VersionValue}"")]
	[global::System.Runtime.CompilerServices.CompilerGenerated]
	[global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]{obsoleteAttributeStr}
	public {readOnlyKeyword}void Deconstruct({paramNamesStr})
	{{
		{assignmentsStr}
	}}"
			);

			context.AddSource(
				typeSymbol.ToFileName(),
				GeneratedFileShortcuts.DeconstructionMethod,
				$@"#nullable enable

namespace {namespaceName};

partial {typeKind}{typeSymbol.Name}{genericParameterList}
{{
	{methods}
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

					var attribute = compilation.GetTypeByMetadataName(typeof(AutoDeconstructAttribute).FullName)!;
					var attributesData =
						from attributeData in typeSymbol.GetAttributes()
						where !attributeData.ConstructorArguments.IsDefaultOrEmpty
						let tempSymbol = attributeData.AttributeClass
						where SymbolEqualityComparer.Default.Equals(tempSymbol, attribute)
						select attributeData;
					if (!attributesData.Any())
					{
						return;
					}

					_resultCollection.Add((typeSymbol, attribute, attributesData, compilation));
				}
			)
		);
}
