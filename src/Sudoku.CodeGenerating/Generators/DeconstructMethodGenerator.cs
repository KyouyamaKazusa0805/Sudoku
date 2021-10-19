namespace Sudoku.CodeGenerating.Generators;

/// <summary>
/// Provides a generator that generates the deconstruction methods.
/// </summary>
[Generator]
public sealed partial class DeconstructMethodGenerator : ISourceGenerator
{
	/// <inheritdoc/>
	public void Execute(GeneratorExecutionContext context)
	{
		var receiver = (SyntaxReceiver)context.SyntaxReceiver!;
		var compilation = context.Compilation;
		var attributeSymbol = compilation.GetTypeByMetadataName(typeof(AutoDeconstructAttribute).FullName)!;

		foreach (var (typeSymbol, attributesData) in
			from type in receiver.Candidates
			let model = compilation.GetSemanticModel(type.SyntaxTree)
			select model.GetDeclaredSymbol(type)! into typeSymbol
			let attributesData =
				from attributeData in typeSymbol.GetAttributes()
				where !attributeData.ConstructorArguments.IsDefaultOrEmpty
				let tempSymbol = attributeData.AttributeClass
				where SymbolEqualityComparer.Default.Equals(tempSymbol, attributeSymbol)
				select attributeData
			where attributesData.Any()
			select (typeSymbol, attributesData))
		{
			typeSymbol.DeconstructInfo(
				false, out string fullTypeName, out string namespaceName, out string genericParametersList,
				out _, out string typeKind, out string readonlyKeyword, out _
			);
			var possibleMembers = (
				from typeDetail in TypeDetail.GetDetailList(typeSymbol, attributeSymbol, false)
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
					1 => $@"
	[global::System.Obsolete(""The method is deprecated because the inner type {deprecatedTypeNames[0]} is deprecated."", false)]",
					> 1 => $@"
	[global::System.Obsolete(""The method is deprecated because the inner types {string.Join(", ", deprecatedTypeNames)} are deprecated."", false)]",
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
	public {readonlyKeyword}void Deconstruct({paramNamesStr})
	{{
		{assignmentsStr}
	}}"
			);

			context.AddSource(
				typeSymbol.ToFileName(),
				GeneratedFileShortcuts.DeconstructionMethod,
				$@"using System;

#nullable enable

namespace {namespaceName};

partial {typeKind}{typeSymbol.Name}{genericParametersList}
{{
	{methods}
}}
"
			);
		}
	}

	/// <inheritdoc/>
	public void Initialize(GeneratorInitializationContext context) =>
		context.RegisterForSyntaxNotifications(static () => new SyntaxReceiver());
}
