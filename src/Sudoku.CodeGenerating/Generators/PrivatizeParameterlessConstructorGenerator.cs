namespace Sudoku.CodeGenerating.Generators;

/// <summary>
/// Provides a source generator that automatically generates the source code for disability of the
/// usage of the parameterless constructor outside the type range.
/// </summary>
[Generator]
public sealed partial class PrivatizeParameterlessConstructorGenerator : ISourceGenerator
{
	/// <inheritdoc/>
	public void Execute(GeneratorExecutionContext context)
	{
		var receiver = (SyntaxReceiver)context.SyntaxReceiver!;
		var compilation = context.Compilation;
		var attributeSymbol = compilation.GetTypeByMetadataName(typeof(PrivatizeParameterlessConstructorAttribute).FullName);

		foreach (var (typeSymbol, attributeData) in
			from type in receiver.Candidates
			let semanticModel = compilation.GetSemanticModel(type.SyntaxTree)
			select semanticModel.GetDeclaredSymbol(type, context.CancellationToken)! into typeSymbol
			let attributesData = typeSymbol.GetAttributes()
			let attributeData = attributesData.FirstOrDefault(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, attributeSymbol))
			where attributeData is not null
			select (typeSymbol, attributeData))
		{
			typeSymbol.DeconstructInfo(
				true, out _, out string namespaceName, out _,
				out _, out _, out _, out _
			);

			if (typeSymbol is not { TypeArguments.IsDefaultOrEmpty: true, Name: var typeName })
			{
				continue;
			}

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
		context.RegisterForSyntaxNotifications(static () => new SyntaxReceiver());
}
