namespace Sudoku.CodeGenerating.Generators;

/// <summary>
/// Provides a source generator that automatically generates the source code for disability of the
/// usage of the parameterless constructor outside the type range.
/// </summary>
[Generator]
public sealed partial class PrivateParameterlessConstructorGenerator : ISourceGenerator
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
			where attributeData is { ConstructorArguments.IsDefaultOrEmpty: false }
			select (typeSymbol, attributeData))
		{
			typeSymbol.DeconstructInfo(
				true, out _, out string namespaceName, out string genericParametersList,
				out _, out _, out _, out _
			);

			context.AddSource(
				typeSymbol.ToFileName(),
				GeneratedFileShortcuts.GetHashCode,
				$@"#nullable enable

namespace {namespaceName};

partial class {typeSymbol.Name}{genericParametersList}
{{
	/// <summary>
	/// Indicates the parameterless constructor.
	/// </summary>
	/// <remarks>
	/// <i>This constructor can't be used anyway.</i>
	/// </remarks>
	[global::System.CodeDom.Compiler.GeneratedCode(""{GetType().FullName}"", ""{VersionValue}"")]
	[global::System.Runtime.CompilerServices.CompilerGenerated]
	[global::System.Obsolete(""You can't call or invoke this constructor anyway."", true, DiagnosticId = ""BAN"")]
	[global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Never)]
	private {typeSymbol.Name}() {{ }}
}}
"
			);
		}
	}

	/// <inheritdoc/>
	public void Initialize(GeneratorInitializationContext context) =>
		context.RegisterForSyntaxNotifications(static () => new SyntaxReceiver());
}
