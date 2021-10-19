using Gathered = System.ValueTuple<
	Microsoft.CodeAnalysis.INamedTypeSymbol, // typeSymbol
	Microsoft.CodeAnalysis.INamedTypeSymbol, // attributeSymbol
	System.Collections.Generic.IEnumerable<Microsoft.CodeAnalysis.AttributeData>, // attributesData
	Microsoft.CodeAnalysis.Compilation // compilation
>;

namespace Sudoku.CodeGenerating.Generators;

/// <summary>
/// Indicates the source generator that generates the source code for deconstruction methods.
/// </summary>
/// <remarks>
/// <para>
/// A <b>deconstruction method</b> is a method that allows an instance can use value-tuple syntax to
/// be separated into multiple instances, for example:
/// <code><![CDATA[
/// class T
/// {
///     public void Deconstruct(out int a, out double b, out string? c)
///     {
///         a = something;
///         b = something;
///         c = something;
///     }
/// }
/// ]]></code>
/// Then you can use the syntax to deconstruct an instance of type <c>T</c>:
/// <code><![CDATA[
/// var t = new T(arguments);
/// var (a, b, c) = t; // Calls 't.Deconstruct'.
/// ]]></code>
/// </para>
/// <para>
/// Deconstruct methods can be also used on pattern matching:
/// <code><![CDATA[
/// if (t is (a: 10, b: _, c: "Hello"))
/// {
///     // Do something.
/// }
/// ]]></code>
/// </para>
/// </remarks>
[Generator(LanguageNames.CSharp)]
[EmitNullableEnable]
public sealed class DeconstructMethodGenerator : IIncrementalGenerator, ISyntaxHandling<Gathered>
{
	/// <inheritdoc/>
	public void Initialize(IncrementalGeneratorInitializationContext context) =>
		context.RegisterImplementationSourceOutput(
			context.SyntaxProvider.CreateSyntaxProvider(WithAttributes, Transform),
			SourceProduce
		);

	/// <inheritdoc cref="ISyntaxHandling{TStruct}.WithAttributes(SyntaxNode, CancellationToken)"/>
	private bool WithAttributes(SyntaxNode n, /*Discard*/ CancellationToken _) =>
		n is TypeDeclarationSyntax { AttributeLists.Count: not 0 };

	/// <inheritdoc cref="ISyntaxHandling{TStruct}.Transform(GeneratorSyntaxContext, CancellationToken)"/>
	private Gathered? Transform(GeneratorSyntaxContext gsc, CancellationToken c)
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

		var attributeSymbol = compilation.GetTypeByMetadataName(typeof(AutoDeconstructAttribute).FullName)!;
		var attributesData =
			from attributeData in typeSymbol.GetAttributes()
			where !attributeData.ConstructorArguments.IsDefaultOrEmpty
			let tempSymbol = attributeData.AttributeClass
			where SymbolEqualityComparer.Default.Equals(tempSymbol, attributeSymbol)
			select attributeData;
		if (!attributesData.Any())
		{
			return null;
		}

		return (typeSymbol, attributeSymbol, attributesData, compilation);
	}

	/// <inheritdoc cref="ISyntaxHandling{TStruct}.SourceProduce(SourceProductionContext, TStruct?)"/>
	private void SourceProduce(SourceProductionContext spc, Gathered? gathered)
	{
		if (gathered is not (var typeSymbol, var attributeSymbol, var attributesData, var compilation))
		{
			return;
		}

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
	public {readonlyKeyword}void Deconstruct({paramNamesStr})
	{{
		{assignmentsStr}
	}}"
		);

		string sourceCode = $@"{Configuration.GetNullableEnableString(GetType())}

namespace {namespaceName};

partial {typeKind}{typeSymbol.Name}{genericParametersList}
{{
	{methods}
}}
";
		spc.AddSource(typeSymbol.ToFileName(), GeneratedFileShortcuts.DeconstructionMethod, sourceCode);
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	bool ISyntaxHandling<Gathered>.WithAttributes(SyntaxNode n, CancellationToken c) =>
		WithAttributes(n, c);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	Gathered? ISyntaxHandling<Gathered>.Transform(GeneratorSyntaxContext gsc, CancellationToken c) =>
		Transform(gsc, c);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	void ISyntaxHandling<Gathered>.SourceProduce(SourceProductionContext spc, Gathered? gathered) =>
		SourceProduce(spc, gathered);
}
