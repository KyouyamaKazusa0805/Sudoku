namespace Sudoku.Diagnostics.CodeGen.Generators;

/// <summary>
/// Indicates a generator that generates the code about the equality method.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class ProxyEqualsMethodGenerator : ISourceGenerator
{
	/// <summary>
	/// The result collection.
	/// </summary>
	private readonly ICollection<INamedTypeSymbol> _resultCollection = new List<INamedTypeSymbol>();


	/// <inheritdoc/>
	public void Execute(GeneratorExecutionContext context)
	{
		var processedList = new List<INamedTypeSymbol>();
		var compilation = context.Compilation;
		var attributeSymbol = compilation.GetTypeByMetadataName(typeof(ProxyEqualityAttribute).FullName);
		var boolean = compilation.GetSpecialType(SpecialType.System_Boolean);
		foreach (var (typeSymbol, method) in
			from typeSymbol in _resultCollection
			from member in typeSymbol.GetMembers().OfType<IMethodSymbol>()
			let attributesData = member.GetAttributes()
			where attributesData.Any(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, attributeSymbol))
				&& SymbolEqualityComparer.Default.Equals(member.ReturnType, boolean)
			let parameters = member.Parameters
			where parameters.Length == 2
				&& parameters.All(p => SymbolEqualityComparer.Default.Equals(p.Type, typeSymbol))
				&& !processedList.Exists(t => SymbolEqualityComparer.Default.Equals(t, typeSymbol))
			let method = (
				from member in typeSymbol.GetMembers().OfType<IMethodSymbol>()
				from attribute in member.GetAttributes()
				where SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, attributeSymbol)
				select member
			).First()
			let methodParams = method.Parameters
			where !typeSymbol.IsReferenceType || methodParams.NullableMatches(NullableAnnotation.Annotated)
			select (typeSymbol, method))
		{
			var (
				typeName, fullTypeName, namespaceName, genericParameterList, genericParameterListWithoutConstraint,
				typeKind, readOnlyKeyword, inKeyword, nullableAnnotation, _
			) = SymbolOutputInfo.FromSymbol(typeSymbol);
			string methodName = method.Name;
			string objectEqualityMethod = typeSymbol.IsRefLikeType
				? "// This type is a ref struct, so 'bool Equals(object?)' is useless."
				: $@"/// <inheritdoc/>
	[global::System.CodeDom.Compiler.GeneratedCode(""{GetType().FullName}"", ""{VersionValue}"")]
	[global::System.Runtime.CompilerServices.CompilerGenerated]
	[global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
	public override {readOnlyKeyword}bool Equals(object? obj) => obj is {typeSymbol.Name}{genericParameterListWithoutConstraint} comparer && {methodName}(this, comparer);";

			context.AddSource(
				typeSymbol.ToFileName(),
				GeneratedFileShortcuts.ProxyEqualsMethod,
				$@"#nullable enable

namespace {namespaceName};

partial {typeKind}{typeName}{genericParameterList}
{{
	{objectEqualityMethod}

	/// <inheritdoc cref=""IEquatable{{T}}.Equals(T)"" />
	[global::System.CodeDom.Compiler.GeneratedCode(""{GetType().FullName}"", ""{VersionValue}"")]
	[global::System.Runtime.CompilerServices.CompilerGenerated]
	[global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
	public bool Equals({inKeyword}{typeName}{genericParameterListWithoutConstraint}{nullableAnnotation} other) => {methodName}(this, other);


	/// <summary>
	/// Determine whether two instances hold a same value.
	/// </summary>
	/// <param name=""left"">The left-side instance to compare.</param>
	/// <param name=""right"">The right-side instance to compare.</param>
	/// <returns>A <see cref=""bool""/> result.</returns>
	[global::System.CodeDom.Compiler.GeneratedCode(""{GetType().FullName}"", ""{VersionValue}"")]
	[global::System.Runtime.CompilerServices.CompilerGenerated]
	[global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
	public static bool operator ==({inKeyword}{typeName}{genericParameterListWithoutConstraint} left, {inKeyword}{typeName}{genericParameterListWithoutConstraint} right) => {methodName}(left, right);

	/// <summary>
	/// Determine whether two instances don't hold a same value.
	/// </summary>
	/// <param name=""left"">The left-side instance to compare.</param>
	/// <param name=""right"">The right-side instance to compare.</param>
	/// <returns>A <see cref=""bool""/> result.</returns>
	[global::System.CodeDom.Compiler.GeneratedCode(""{GetType().FullName}"", ""{VersionValue}"")]
	[global::System.Runtime.CompilerServices.CompilerGenerated]
	[global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
	public static bool operator !=({inKeyword}{typeName}{genericParameterListWithoutConstraint} left, {inKeyword}{typeName}{genericParameterListWithoutConstraint} right) => !(left == right);
}}
"
			);

			processedList.Add(typeSymbol);
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
							SyntaxNode: TypeDeclarationSyntax { AttributeLists.Count: not 0 } n and not InterfaceDeclarationSyntax,
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

					var attribute = compilation.GetTypeByMetadataName(typeof(AutoEqualityAttribute).FullName)!;
					var attributesData = typeSymbol.GetAttributes();
					var attributeData = attributesData.FirstOrDefault(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, attribute));
					if (attributeData is not { ConstructorArguments.IsDefaultOrEmpty: false })
					{
						return;
					}

					_resultCollection.Add(typeSymbol);
				}
			)
		);
}
