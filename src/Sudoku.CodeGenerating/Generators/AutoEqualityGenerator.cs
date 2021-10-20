using Gathered = System.ValueTuple<
	Microsoft.CodeAnalysis.INamedTypeSymbol, // typeSymbol
	Microsoft.CodeAnalysis.AttributeData // attributeData
>;

namespace Sudoku.CodeGenerating.Generators;

/// <summary>
/// Indicates the generator that generates the methods about the equality checking. The methods below
/// will be generated:
/// <list type="bullet">
/// <item><c>bool Equals(object? obj)</c></item>
/// <item><c>bool Equals(T comparer)</c></item>
/// <item><c>bool ==(T left, T right)</c></item>
/// <item><c>bool !=(T left, T right)</c></item>
/// </list>
/// </summary>
/// <remarks>
/// Please note that if the type is a <see langword="ref struct"/>, the first one won't be generated
/// because this method is useless in the by-ref-like types.
/// </remarks>
[Generator(LanguageNames.CSharp)]
[EmitNullableEnable]
public sealed class AutoEqualityGenerator : IIncrementalGenerator, ISyntaxHandling<Gathered>
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

		var attribute = compilation.GetTypeByMetadataName(typeof(AutoEqualityAttribute).FullName)!;
		var attributeData = typeSymbol.FirstAttributeData(attribute);
		if (attributeData is not { ConstructorArguments.IsDefaultOrEmpty: false })
		{
			return null;
		}

		return (typeSymbol, attributeData);
	}

	/// <inheritdoc cref="ISyntaxHandling{TStruct}.SourceProduce(SourceProductionContext, TStruct?)"/>
	private void SourceProduce(SourceProductionContext spc, Gathered? gathered)
	{
		if (gathered is not (var typeSymbol, var attributeData))
		{
			return;
		}

		typeSymbol.DeconstructInfo(
			false, out string fullTypeName, out string namespaceName, out string genericParametersList,
			out string genericParametersListWithoutConstraint, out string typeKind,
			out string readonlyKeyword, out _
		);
		string inKeyword = typeSymbol.TypeKind == TypeKind.Struct ? "in " : string.Empty;
		string nullableAnnotation = typeSymbol.TypeKind == TypeKind.Class ? "?" : string.Empty;
		string nullCheck = typeSymbol.TypeKind == TypeKind.Class ? "other is not null && " : string.Empty;
		string memberCheck = string.Join(
			" && ",
			from arg in attributeData.ConstructorArguments[0].Values
			select ((string)arg.Value!).Trim() into member
			select $"{member} == other.{member}"
		);

		string typeName = typeSymbol.Name;
		string objectEqualsMethod = typeSymbol switch
		{
			{ IsRefLikeType: true } => "// Ref structs disables the usage on method 'bool Equals(object?)'.",
			{ IsRecord: true, TypeKind: TypeKind.Struct } => "// Record structs can't syntheize the method 'bool Equals(object?)'.",
			_ => $@"/// <inheritdoc cref=""object.Equals(object?)""/>
	[global::System.CodeDom.Compiler.GeneratedCode(""{GetType().FullName}"", ""{VersionValue}"")]
	[global::System.Runtime.CompilerServices.CompilerGenerated]
	[global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
	public override {readonlyKeyword}bool Equals(object? other) => other is {typeName}{genericParametersList} comparer && Equals(comparer);"
		};

		var memberSymbols = typeSymbol.GetMembers().OfType<IMethodSymbol>();
		string opEquality = existsOperatorOverloading(memberSymbols, OperatorNames.Equality)
			? "// 'operator ==' already exists in the type."
			: $@"/// <summary>
	/// Determines whether the two instances contain a same value.
	/// </summary>
	/// <param name=""left"">The left-side-operator instance to compare.</param>
	/// <param name=""right"">The right-side-operator instance to compare.</param>
	/// <returns>A <see cref=""bool""/> result.</returns>
	[global::System.CodeDom.Compiler.GeneratedCode(""{GetType().FullName}"", ""{VersionValue}"")]
	[global::System.Runtime.CompilerServices.CompilerGenerated]
	[global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
	public static bool operator ==({inKeyword}{typeName}{genericParametersListWithoutConstraint} left, {inKeyword}{typeName}{genericParametersListWithoutConstraint} right) => left.Equals(right);";

		string opInequality = existsOperatorOverloading(memberSymbols, OperatorNames.Inequality)
			? "// 'operator !=' already exists in the type."
			: $@"/// <summary>
	/// Determines whether the two instances don't contain a same value.
	/// </summary>
	/// <param name=""left"">The left-side-operator instance to compare.</param>
	/// <param name=""right"">The right-side-operator instance to compare.</param>
	/// <returns>A <see cref=""bool""/> result.</returns>
	[global::System.CodeDom.Compiler.GeneratedCode(""{GetType().FullName}"", ""{VersionValue}"")]
	[global::System.Runtime.CompilerServices.CompilerGenerated]
	[global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
	public static bool operator !=({inKeyword}{typeName}{genericParametersListWithoutConstraint} left, {inKeyword}{typeName}{genericParametersListWithoutConstraint} right) => !(left == right);";

		string sourceCode = $@"{Configuration.GetNullableEnableString(GetType())}

namespace {namespaceName};

partial {typeKind}{typeName}{genericParametersList}
{{
	{objectEqualsMethod}

	/// <summary>
	/// Indicates whether the current object is equal to another object of the same type.
	/// </summary>
	/// <param name=""other"">An object to compare with this object.</param>
	/// <returns>
	/// <see langword=""true""/> if the current object is equal to the other parameter;
	/// otherwise, <see langword=""false""/>.
	/// </returns>
	[global::System.CodeDom.Compiler.GeneratedCode(""{GetType().FullName}"", ""{VersionValue}"")]
	[global::System.Runtime.CompilerServices.CompilerGenerated]
	[global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
	public {readonlyKeyword}bool Equals({inKeyword}{typeName}{genericParametersListWithoutConstraint}{nullableAnnotation} other) => {nullCheck}{memberCheck};


	{opEquality}

	{opInequality}
}}
";
		spc.AddSource(typeSymbol.ToFileName(), GeneratedFileShortcuts.EqualsMethod, sourceCode);


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static bool existsOperatorOverloading(IEnumerable<IMethodSymbol> methods, string operatorName) =>
			methods.Any(method => method.Name == operatorName);
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
