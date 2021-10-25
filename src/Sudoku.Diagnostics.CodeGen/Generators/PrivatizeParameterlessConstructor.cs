namespace Sudoku.Diagnostics.CodeGen.Generators;

/// <summary>
/// Provides a source generator that automatically generates the source code for disability of the
/// usage of the parameterless constructor outside the type range.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class PrivatizeParameterlessConstructor : ISourceGenerator
{
	/// <inheritdoc/>
	public void Execute(GeneratorExecutionContext context)
	{
		foreach (
			var (
				typeSymbol, attributeData, (typeName, _, namespaceName, _, _, _, _, _, _, _)
			) in ((Receiver)context.SyntaxContextReceiver!).Collection
		)
		{
			context.AddSource(
				typeSymbol.ToFileName(),
				GeneratedFileShortcuts.PrivateParameterlessConstructor,
				$@"#nullable enable

namespace {namespaceName};

partial class {typeName}
{{
	/// <summary>
	/// <para>Indicates the parameterless constructor.</para>
	/// <para><i>This constructor can't be used anyway.</i></para>
	/// </summary>
	/// <exception cref=""NotSupportedException"">Always throws.</exception>
	[global::System.CodeDom.Compiler.GeneratedCode(""{GetType().FullName}"", ""{VersionValue}"")]
	[global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER || (NETCOREAPP3_0 || NETCOREAPP3_1)
	[global::System.Obsolete(""You can't call or invoke this constructor anyway."", true, DiagnosticId = ""BAN"")]
#else
	[global::System.Obsolete(""You can't call or invoke this constructor anyway."", true)]
#endif
	[global::System.Runtime.CompilerServices.CompilerGenerated]
	private {typeName}() => throw new NotSupportedException();
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
	private sealed record Receiver(CancellationToken CancellationToken) : IResultCollectionReceiver<PrivatizeParameterlessConstructorInfo>
	{
		/// <inheritdoc/>
		public ICollection<PrivatizeParameterlessConstructorInfo> Collection { get; } = new List<PrivatizeParameterlessConstructorInfo>();


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

			if (
				semanticModel.GetDeclaredSymbol(n, CancellationToken) is not
				{
					TypeArguments.IsDefaultOrEmpty: true
				} typeSymbol
			)
			{
				return;
			}

			var attribute = compilation.GetTypeByMetadataName(typeof(PrivatizeParameterlessConstructorAttribute).FullName)!;
			var attributesData = typeSymbol.GetAttributes();
			var attributeData = attributesData.FirstOrDefault(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, attribute));
			if (attributeData is not { ConstructorArguments.IsDefaultOrEmpty: false })
			{
				return;
			}

			Collection.Add((typeSymbol, attributeData, SymbolOutputInfo.FromSymbol(typeSymbol, true)));
		}
	}
}
