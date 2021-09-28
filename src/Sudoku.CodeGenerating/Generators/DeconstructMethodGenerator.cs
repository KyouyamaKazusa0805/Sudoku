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

		Func<ISymbol?, ISymbol?, bool> f = SymbolEqualityComparer.Default.Equals;
		var nameDic = new Dictionary<string, int>();
		var compilation = context.Compilation;
		var attributeSymbol = compilation.GetTypeByMetadataName<AutoDeconstructAttribute>();
		foreach (var type in
			from candidateType in receiver.Candidates
			let model = compilation.GetSemanticModel(candidateType.SyntaxTree)
			select model.GetDeclaredSymbol(candidateType)! into type
			where type.GetAttributes().Any(a => f(a.AttributeClass, attributeSymbol))
			select type)
		{
			type.DeconstructInfo(
				false, out string fullTypeName, out string namespaceName, out string genericParametersList,
				out _, out string typeKind, out string readonlyKeyword, out _
			);
			var possibleArgs = (
				from x in GetMembers(type, false, attributeSymbol)
				select (Info: x, Param: $"out {x.Type} {x.ParameterName}")
			).ToArray();
			string methods = string.Join(
				"\r\n\r\n\t",
				from attributeStr in type.GetAttributeStrings(attributeSymbol)
				where attributeStr is not null
				let tokenStartIndex = attributeStr.IndexOf("({")
				where tokenStartIndex != -1
				select attributeStr.GetMemberValues(tokenStartIndex) into members
				where members is not null && !members.Any(m => possibleArgs.All(pair => pair.Info.Name != m))
				let parameterList = string.Join(
					", ",
					from memberStr in members
					select possibleArgs.First(p => p.Info.Name == memberStr).Param
				)
				let assignments = string.Join(
					"\r\n\t\t",
					from member in members
					let paramName = possibleArgs.First(p => p.Info.Name == member).Info.ParameterName
					select $"{paramName} = {member};"
				)
				select $@"[global::System.CodeDom.Compiler.GeneratedCode(""{GetType().FullName}"", ""{VersionValue}"")]
	[global::System.Runtime.CompilerServices.CompilerGenerated]
	[global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
	public {readonlyKeyword}void Deconstruct({parameterList})
	{{
		{assignments}
	}}"
			);

			context.AddSource(
				type.ToFileName(),
				"d",
				$@"#pragma warning disable 618, 1591

using System;

#nullable enable

namespace {namespaceName};

partial {typeKind}{type.Name}{genericParametersList}
{{
	{methods}
}}
"
			);
		}
	}

	/// <inheritdoc/>
	public void Initialize(GeneratorInitializationContext context) => context.FastRegister<SyntaxReceiver>();

	/// <summary>
	/// Try to get all possible fields or properties in the specified type.
	/// </summary>
	/// <param name="symbol">The specified symbol.</param>
	/// <param name="handleRecursively">
	/// A <see cref="bool"/> value indicating whether the method will handle the type recursively.
	/// </param>
	/// <param name="attributeSymbol">The attribute symbol to check.</param>
	/// <returns>The result list that contains all member symbols.</returns>
	private static IReadOnlyList<(string Type, string ParameterName, string Name, ImmutableArray<AttributeData> Attributes)> GetMembers(INamedTypeSymbol symbol, bool handleRecursively, INamedTypeSymbol? attributeSymbol)
	{
		var result = new List<(string, string, string, ImmutableArray<AttributeData>)>(
			(
				from x in symbol.GetMembers().OfType<IFieldSymbol>()
				select (
					x.Type.ToDisplayString(FormatOptions.PropertyTypeFormat),
					x.Name.ToCamelCase(),
					x.Name,
					x.GetAttributes()
				)
			).Concat(
				from x in symbol.GetMembers().OfType<IPropertySymbol>()
				select (
					x.Type.ToDisplayString(FormatOptions.PropertyTypeFormat),
					x.Name.ToCamelCase(),
					x.Name,
					x.GetAttributes()
				)
			)
		);

		Func<ISymbol?, ISymbol?, bool> f = SymbolEqualityComparer.Default.Equals;
		if (handleRecursively && symbol.BaseType is { } baseType
			&& baseType.GetAttributes().Any(a => f(a.AttributeClass, attributeSymbol)))
		{
			result.AddRange(GetMembers(baseType, true, attributeSymbol));
		}

		return result;
	}
}
