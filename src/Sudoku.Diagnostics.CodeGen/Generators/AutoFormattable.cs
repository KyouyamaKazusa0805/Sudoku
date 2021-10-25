namespace Sudoku.Diagnostics.CodeGen.Generators;

/// <summary>
/// Defines a source generator that generates the source code for formatting methods.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class AutoFormattable : ISourceGenerator
{
	/// <inheritdoc/>
	public void Execute(GeneratorExecutionContext context)
	{
		foreach (
			var (
				typeSymbol, attributeData, (
					typeName, fullTypeName, namespaceName, genericParameterList,
					genericParameterListWithoutConstraint, typeKind, readOnlyKeyword,
					inKeyword, nullableAnnotation, _
				)
			) in ((Receiver)context.SyntaxContextReceiver!).Collection
		)
		{
			context.AddSource(
				typeSymbol.ToFileName(),
				GeneratedFileShortcuts.FormattedMethods,
				$@"#nullable enable

namespace {namespaceName};

partial {typeKind}{typeSymbol.Name}{genericParameterList}
{{
	/// <inheritdoc cref=""object.ToString""/>
	[global::System.CodeDom.Compiler.GeneratedCode(""{GetType().FullName}"", ""{VersionValue}"")]
	[global::System.Runtime.CompilerServices.CompilerGenerated]
	[global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
	public override {readOnlyKeyword}string ToString() => ToString(null, null);

	/// <summary>
	/// Returns a string that represents the current object with the specified format string.
	/// </summary>
	/// <param name=""format"">
	/// The format. If available, the parameter can be <see langword=""null""/>.
	/// </param>
	/// <returns>The string result.</returns>
	[global::System.CodeDom.Compiler.GeneratedCode(""{GetType().FullName}"", ""{VersionValue}"")]
	[global::System.Runtime.CompilerServices.CompilerGenerated]
	[global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
	public {readOnlyKeyword}string ToString(string? format) => ToString(format, null);
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
	private sealed record Receiver(CancellationToken CancellationToken) : IResultCollectionReceiver<AutoFormattableInfo>
	{
		/// <inheritdoc/>
		public ICollection<AutoFormattableInfo> Collection { get; } = new List<AutoFormattableInfo>();


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

			var attribute = compilation.GetTypeByMetadataName(typeof(AutoFormattableAttribute).FullName)!;
			var attributesData = typeSymbol.GetAttributes();
			var attributeData = attributesData.FirstOrDefault(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, attribute));
			if (attributeData is not { ConstructorArguments.IsDefaultOrEmpty: true })
			{
				return;
			}

			Collection.Add((typeSymbol, attributeData, SymbolOutputInfo.FromSymbol(typeSymbol)));
		}
	}
}
