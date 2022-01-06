namespace Sudoku.Diagnostics.CodeGen.Generators;

/// <summary>
/// Indicates a generator that generates the code about the equality method.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class ProxyEqualsMethod : ISourceGenerator
{
	/// <inheritdoc/>
	public void Execute(GeneratorExecutionContext context)
	{
		foreach (
			var (
				typeSymbol, (
					typeName, fullTypeName, namespaceName, genericParameterList,
					genericParameterListWithoutConstraint,
					typeKind, readOnlyKeyword, inKeyword, nullableAnnotation, _
				), methodName
			)
			in ((Receiver)context.SyntaxContextReceiver!).Collection
		)
		{
			string objectEqualityMethod = typeSymbol.IsRefLikeType
				? "// This type is a ref struct, so 'bool Equals(object?)' is useless."
				: $@"/// <inheritdoc/>
	[global::System.CodeDom.Compiler.GeneratedCode(""{GetType().FullName}"", ""{VersionValue}"")]
	[global::System.Runtime.CompilerServices.CompilerGenerated]
	[global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
	public override {readOnlyKeyword}bool Equals(object? obj) => obj is {typeName}{genericParameterListWithoutConstraint} comparer && {methodName}(this, comparer);";

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
		}
	}

	/// <inheritdoc/>
	public void Initialize(GeneratorInitializationContext context) =>
		context.RegisterForSyntaxNotifications(() => new Receiver(context.CancellationToken));


	/// <summary>
	/// Defines a syntax context receiver.
	/// </summary>
	/// <param name="CancellationToken">The cancellation token to cancel the operation.</param>
	private sealed record Receiver(CancellationToken CancellationToken) : IResultCollectionReceiver<ProxyEqualsMethodInfo>
	{
		/// <summary>
		/// Indicates the processed list that records the type symbols had been handled,
		/// which is used for removing duplicate cases.
		/// </summary>
		private readonly List<INamedTypeSymbol> _processedList = new();


		/// <inheritdoc/>
		public ICollection<ProxyEqualsMethodInfo> Collection { get; } = new List<ProxyEqualsMethodInfo>();


		/// <inheritdoc/>
		public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
		{
			if (
				context is not
				{
					Node: TypeDeclarationSyntax n and not InterfaceDeclarationSyntax,
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

			var autoEqualityAttribute = compilation.GetTypeByMetadataName(typeof(AutoEqualityAttribute).FullName)!;
			var typeAttributesData = typeSymbol.GetAttributes();
			if (typeAttributesData.Any(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, autoEqualityAttribute)))
			{
				// To avoid the contradiction for the generation of 'Equals' method and here.
				return;
			}

			var @bool = compilation.GetSpecialType(SpecialType.System_Boolean);
			var attribute = compilation.GetTypeByMetadataName(typeof(ProxyEqualityAttribute).FullName);
			var methodSymbol = typeSymbol.GetMembers().OfType<IMethodSymbol>().SingleOrDefault(
				methodSymbol =>
					methodSymbol is { ReturnType: var returnType, Parameters: [_, _] parameters }
						&& methodSymbol.GetAttributes() is var attributesData
						&& attributesData.Any(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, attribute))
						&& SymbolEqualityComparer.Default.Equals(returnType, @bool)
						&& parameters.All(p => SymbolEqualityComparer.Default.Equals(p.Type, typeSymbol))
						&& !_processedList.Exists(t => SymbolEqualityComparer.Default.Equals(t, typeSymbol))
						&& !(
							typeSymbol.IsReferenceType
							&& !parameters.NullableMatches(NullableAnnotation.Annotated)
						)
			);
			if (methodSymbol is not { Name: var methodName })
			{
				return;
			}

			Collection.Add((typeSymbol, SymbolOutputInfo.FromSymbol(typeSymbol), methodName));
			_processedList.Add(typeSymbol);
		}
	}
}
