using Gathered = System.ValueTuple<
	Microsoft.CodeAnalysis.INamedTypeSymbol, // typeArg
	System.Collections.Generic.IEnumerable<string>, // argStrs
	string?, // customNamespace
	string // typeArgStr
>;
using GatheredArray = System.Collections.Immutable.ImmutableArray<
	System.Linq.IGrouping<
		string,
		System.ValueTuple<
			Microsoft.CodeAnalysis.INamedTypeSymbol, // typeArg
			System.Collections.Generic.IEnumerable<string>, // argStrs
			string?, // customNamespace
			string // typeArgStr
		>
	>
>;

namespace Sudoku.CodeGenerating.Generators;

/// <summary>
/// Provides a generator that generates the deconstruction methods that are extension methods.
/// </summary>
[Generator]
public sealed partial class ExtensionDeconstructMethodGenerator : IIncrementalGenerator, ISyntaxHandling<GatheredArray>
{
	/// <summary>
	/// Indicates whether the current project has any assembly attribute.
	/// </summary>
	private volatile bool _hasAssemblyAttribute;


	/// <inheritdoc/>
	public void Initialize(IncrementalGeneratorInitializationContext context) =>
		context.RegisterImplementationSourceOutput(
			context.SyntaxProvider.CreateSyntaxProvider(WithAttributes, Transform),
			SourceProduce
		);

	/// <inheritdoc cref="ISyntaxHandling{TStruct}.WithAttributes(SyntaxNode, CancellationToken)"/>
	private bool WithAttributes(SyntaxNode n, CancellationToken _) =>
		n is CompilationUnitSyntax { AttributeLists.Count: not 0, Members.Count: 0 }
		&& !_hasAssemblyAttribute
		&& (_hasAssemblyAttribute = true);

	/// <inheritdoc cref="ISyntaxHandling{TStruct}.Transform(GeneratorSyntaxContext, CancellationToken)"/>
	private GatheredArray? Transform(GeneratorSyntaxContext gsc, CancellationToken _)
	{
		if (gsc is not { SemanticModel: { Compilation: var compilation } semanticModel })
		{
			return null;
		}

		var listOfPossibleResults = new List<Gathered>();
		var attributeSymbol = compilation
			.GetTypeByMetadataName(typeof(AutoDeconstructExtensionAttribute<>).FullName)!
			.ConstructUnboundGenericType();
		var attributesData = compilation.Assembly.GetAttributes();
		foreach (var attributeData in attributesData)
		{
			if (
				attributeData is not
				{
					AttributeClass:
					{
						IsGenericType: true,
						TypeArguments: { IsDefaultOrEmpty: false } typeArgs
					} a,
					ConstructorArguments: { IsDefaultOrEmpty: false } ctorArgs
				}
			)
			{
				continue;
			}

			var unboundAttribute = a.ConstructUnboundGenericType();
			if (!SymbolEqualityComparer.Default.Equals(unboundAttribute, attributeSymbol))
			{
				continue;
			}

			if (typeArgs[0] is not INamedTypeSymbol typeArg)
			{
				continue;
			}

			string typeArgStr = typeArg.ToDisplayString(TypeFormats.FullName);
			var argStrs = from arg in ctorArgs[0].Values select ((string)arg.Value!).Trim();
			string? customNamespace = attributeData.TryGetNamedArgument(
				nameof(AutoDeconstructExtensionAttribute<object>.Namespace),
				out var na
			) ? ((string)na.Value!).Trim() : null;

			listOfPossibleResults.Add((typeArg, argStrs, customNamespace, typeArgStr));
		}

		/*naked-value-tuple-name*/
		return (from item in listOfPossibleResults group item by item.Item4).ToImmutableArray();
	}

	/// <inheritdoc cref="ISyntaxHandling{TStruct}.SourceProduce(SourceProductionContext, TStruct?)"/>
	private void SourceProduce(SourceProductionContext spc, GatheredArray? gatheredArray)
	{
		if (!gatheredArray.HasValue)
		{
			return;
		}

		foreach (var groupedResult in gatheredArray)
		{
			var (typeArg, argStrs, n, _) = groupedResult.First();
			string namespaceResult = n ?? typeArg.ContainingNamespace.ToDisplayString();
			string typeResult = typeArg.Name;

			var deconstrcutionMethodsStr = new List<string>();
			foreach (var (typeArgument, arguments, namedArgumentNamespace, _) in groupedResult)
			{
				typeArgument.DeconstructInfo(
					false, out _, out _, out _, out string genericParameterListWithoutConstraint,
					out _, out _, out _
				);
				string fullTypeNameWithoutConstraint = typeArgument.ToDisplayString(TypeFormats.FullNameWithConstraints);
				string constraint = fullTypeNameWithoutConstraint.IndexOf("where") is var index and not -1
					? fullTypeNameWithoutConstraint.Substring(index)
					: string.Empty;
				string inModifier = typeArgument.TypeKind == TypeKind.Struct ? "in " : string.Empty;
				string parameterList = string.Join(
					", ",
					from member in arguments
					let memberFound = typeArgument.GetAllMembers().FirstOrDefault(m => m.Name == member)
					where memberFound is not null
					let memberType = memberFound.GetMemberType()
					where memberType is not null
					select $@"out {memberType} {member.ToCamelCase()}"
				);
				string assignments = string.Join(
					"\r\n\t\t",
					from member in arguments select $"{member.ToCamelCase()} = @this.{member};"
				);

				deconstrcutionMethodsStr.Add($@"/// <summary>
	/// Deconstruct the instance to multiple elements.
	/// </summary>
	[global::System.CodeDom.Compiler.GeneratedCode(""{GetType().FullName}"", ""{VersionValue}"")]
	[global::System.Runtime.CompilerServices.CompilerGenerated]
	[global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
	public static void Deconstruct{genericParameterListWithoutConstraint}(this {inModifier}{fullTypeNameWithoutConstraint} @this, {parameterList}){constraint}
	{{
		{assignments}
	}}");
			}

			string deconstructionMethodsCode = string.Join("\r\n\r\n\t", deconstrcutionMethodsStr);
			string sourceCode = $@"{Configuration.GetNullableEnableString(GetType())}

namespace {namespaceResult};

/// <summary>
/// Provides the extension methods on this type.
/// </summary>
public static class {typeResult}_DeconstructionMethods
{{
	{deconstructionMethodsCode}
}}
";
			spc.AddSource(typeArg.ToFileName(), GeneratedFileShortcuts.ExtensionDeconstructionMethod, sourceCode);
		}
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	bool ISyntaxHandling<GatheredArray>.WithAttributes(SyntaxNode n, CancellationToken c) =>
		WithAttributes(n, c);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	GatheredArray? ISyntaxHandling<GatheredArray>.Transform(GeneratorSyntaxContext gsc, CancellationToken c) =>
		Transform(gsc, c);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	void ISyntaxHandling<GatheredArray>.SourceProduce(SourceProductionContext spc, GatheredArray? gathered) =>
		SourceProduce(spc, gathered);
}
