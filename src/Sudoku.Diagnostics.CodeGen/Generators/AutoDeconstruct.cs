namespace Sudoku.Diagnostics.CodeGen.Generators;

/// <summary>
/// Defines a source generator that generates the source code for deconstruction methods.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class AutoDeconstruct : ISourceGenerator
{
	/// <inheritdoc/>
	public void Execute(GeneratorExecutionContext context)
	{
		var compilation = context.Compilation;
		foreach (
			var (
				typeSymbol, attributesData, (
					typeName, fullTypeName, namespaceName, genericParameterList,
					genericParameterListWithoutConstraint, typeKind, readOnlyKeyword,
					inKeyword, nullableAnnotation, _
				), possibleMembers
			) in ((Receiver)context.SyntaxContextReceiver!).Collection
		)
		{
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
					let tempSymbol = detail.TypeSymbol
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
	[global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
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
		context.RegisterForSyntaxNotifications(() => new Receiver(context.CancellationToken));


	/// <summary>
	/// Defines a syntax context receiver.
	/// </summary>
	/// <param name="CancellationToken">The cancellation token to cancel the operation.</param>
	private sealed record Receiver(CancellationToken CancellationToken) : IResultCollectionReceiver<AutoDeconstructInfo>
	{
		/// <inheritdoc/>
		public ICollection<AutoDeconstructInfo> Collection { get; } = new List<AutoDeconstructInfo>();


		/// <inheritdoc/>
		public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
		{
			if (
				context is not
				{
					Node: TypeDeclarationSyntax { AttributeLists.Count: not 0 } n,
					SemanticModel: { Compilation: { } compilation } semanticModel
				}
			)
			{
				return;
			}

			if (semanticModel.GetDeclaredSymbol(n, CancellationToken) is not { } typeSymbol)
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

			Collection.Add(
				(
					typeSymbol,
					attributesData,
					SymbolOutputInfo.FromSymbol(typeSymbol),
					MemberDetail.GetDetailList(typeSymbol, attribute, false)
				)
			);
		}
	}
}
