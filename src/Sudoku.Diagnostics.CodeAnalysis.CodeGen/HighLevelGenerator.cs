namespace Sudoku.Diagnostics.CodeAnalysis.CodeGen;

/// <summary>
/// Indicates the high-level source generator that generates the source generator.
/// </summary>
[Generator]
public sealed class HighLevelGenerator : ISourceGenerator
{
	/// <inheritdoc/>
	public void Execute(GeneratorExecutionContext context)
	{
		foreach (string shortName in ((Receiver)context.SyntaxContextReceiver!).Result)
		{
			context.AddSource(
				$"{shortName}Analyzer.g.cs",
				$@"namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers;

[Generator(LanguageNames.CSharp)]
[CompilerGenerated]
public sealed class {shortName}Analyzer : ISourceGenerator
{{
	/// <inheritdoc/>
	[CompilerGenerated]	
	public void Execute(GeneratorExecutionContext context) =>
		(({shortName}SyntaxChecker)context.SyntaxContextReceiver!).Diagnostics.ForEach(context.ReportDiagnostic);

	/// <inheritdoc/>
	[CompilerGenerated]
	public void Initialize(GeneratorInitializationContext context) =>
		context.RegisterForSyntaxNotifications(() => new {shortName}SyntaxChecker(context.CancellationToken));
}}
"
			);
		}
	}

	/// <inheritdoc/>
	public void Initialize(GeneratorInitializationContext context) =>
		context.RegisterForSyntaxNotifications(() => new Receiver(context.CancellationToken));


	private sealed class Receiver : ISyntaxContextReceiver
	{
		private readonly CancellationToken _cancellationToken;


		public Receiver(CancellationToken cancellationToken) => _cancellationToken = cancellationToken;


		public ICollection<string> Result { get; } = new List<string>();


		public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
		{
			if (
				context is not
				{
					Node: ClassDeclarationSyntax node,
					SemanticModel: { Compilation: var compilation } semanticModel
				}
			)
			{
				return;
			}

			var receiverSymbol = compilation.GetTypeByMetadataName(typeof(ISyntaxContextReceiver).FullName);
			var symbol = semanticModel.GetDeclaredSymbol(node, _cancellationToken);
			if (symbol is not INamedTypeSymbol { Name: var name, AllInterfaces: var interfaces })
			{
				return;
			}

			if (interfaces.All(t => !SymbolEqualityComparer.Default.Equals(t, receiverSymbol)))
			{
				return;
			}

			Result.Add(name.Substring(0, name.IndexOf("SyntaxChecker")));
		}
	}
}
