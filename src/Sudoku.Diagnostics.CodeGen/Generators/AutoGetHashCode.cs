namespace Sudoku.Diagnostics.CodeGen.Generators;

/// <summary>
/// Defines a source generator that generates the source code for getting hash code.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class AutoGetHashCode : ISourceGenerator
{
	/// <inheritdoc/>
	public void Execute(GeneratorExecutionContext context)
	{
		foreach (
			var (
				typeSymbol, attributeData, (
					typeName, _, namespaceName, genericParameterList, _,
					typeKind, readOnlyKeyword, _, _, _
				)
			) in ((Receiver)context.SyntaxContextReceiver!).Collection
		)
		{
			string hashCodeStr = string.Join(
				" ^ ",
				from member in attributeData.ConstructorArguments[0].Values
				let memberValue = ((string)member.Value!).Trim()
				select $"{memberValue}.GetHashCode()"
			);

			context.AddSource(
				typeSymbol.ToFileName(),
				GeneratedFileShortcuts.GetHashCode,
				$@"#nullable enable

namespace {namespaceName};

partial {typeKind}{typeSymbol.Name}{genericParameterList}
{{
	/// <inheritdoc cref=""object.GetHashCode""/>
	[global::System.CodeDom.Compiler.GeneratedCode(""{GetType().FullName}"", ""{VersionValue}"")]
	[global::System.Runtime.CompilerServices.CompilerGenerated]
	[global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
	public override {readOnlyKeyword}int GetHashCode() => {hashCodeStr};
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
	private sealed record Receiver(CancellationToken CancellationToken) : IResultCollectionReceiver<AutoGetHashCodeInfo>
	{
		/// <inheritdoc/>
		public ICollection<AutoGetHashCodeInfo> Collection { get; } = new List<AutoGetHashCodeInfo>();


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

			var attribute = compilation.GetTypeByMetadataName(typeof(AutoGetHashCodeAttribute).FullName)!;
			var attributesData = typeSymbol.GetAttributes();
			var attributeData = attributesData.FirstOrDefault(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, attribute));
			if (attributeData is not { ConstructorArguments.IsDefaultOrEmpty: false })
			{
				return;
			}

			Collection.Add((typeSymbol, attributeData, SymbolOutputInfo.FromSymbol(typeSymbol)));
		}
	}
}
