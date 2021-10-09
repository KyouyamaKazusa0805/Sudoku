namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers;

[Generator]
[CodeAnalyzer("SD0309")]
public sealed partial class StackAllocAndNewClauseInCellsAndCandidatesAnalyzer : ISourceGenerator
{
	/// <inheritdoc/>
	public void Execute(GeneratorExecutionContext context)
	{
		foreach (var diagnostic in ((CodeAnalyzer)context.SyntaxContextReceiver!).DiagnosticList)
		{
			context.ReportDiagnostic(diagnostic);
		}
	}

	/// <inheritdoc/>
	public void Initialize(GeneratorInitializationContext context) =>
		context.RegisterForSyntaxNotifications(() => new CodeAnalyzer(context.CancellationToken));


	/// <summary>
	/// Defines the syntax receiver.
	/// </summary>
	private sealed class CodeAnalyzer : IAnalyzer
	{
		/// <summary>
		/// Initializes a <see cref="CodeAnalyzer"/> instance via the specified cancellation token.
		/// </summary>
		/// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
		public CodeAnalyzer(CancellationToken cancellationToken) => CancellationToken = cancellationToken;


		/// <inheritdoc/>
		public CancellationToken CancellationToken { get; }

		/// <inheritdoc/>
		public IList<Diagnostic> DiagnosticList { get; } = new List<Diagnostic>();


		/// <inheritdoc/>
		public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
		{
			if (
				context is not
				{
					Node: BaseObjectCreationExpressionSyntax
					{
						ArgumentList.Arguments: { Count: 1 } arguments
					} node,
					SemanticModel: { Compilation: var compilation } semanticModel
				}
			)
			{
				return;
			}

			if (
				semanticModel.GetOperation(node, CancellationToken) is not IObjectCreationOperation
				{
					Kind: OperationKind.ObjectCreation,
					Type: var typeSymbol
				}
			)
			{
				return;
			}

			var cellsSymbol = compilation.GetTypeByMetadataName(TypeNames.Cells);
			var candidatesSymbol = compilation.GetTypeByMetadataName(TypeNames.Candidates);
			bool isOfTypeCells = SymbolEqualityComparer.Default.Equals(typeSymbol, cellsSymbol);
			bool isOfTypeCandidates = SymbolEqualityComparer.Default.Equals(typeSymbol, candidatesSymbol);
			if (!isOfTypeCells && !isOfTypeCandidates)
			{
				return;
			}

			switch (arguments[0].Expression)
			{
				case ArrayCreationExpressionSyntax newExpression:
				{
					DiagnosticList.Add(
						Diagnostic.Create(
							descriptor: SD0309,
							location: newExpression.GetLocation(),
							messageArgs: new[] { TokenValues.NewKeyword }
						)
					);

					break;
				}
				case ImplicitArrayCreationExpressionSyntax implicitNewExpression:
				{
					DiagnosticList.Add(
						Diagnostic.Create(
							descriptor: SD0309,
							location: implicitNewExpression.GetLocation(),
							messageArgs: new[] { TokenValues.NewKeyword }
						)
					);

					break;
				}
				case StackAllocArrayCreationExpressionSyntax stackAllocExpression:
				{
					DiagnosticList.Add(
						Diagnostic.Create(
							descriptor: SD0309,
							location: stackAllocExpression.GetLocation(),
							messageArgs: new[] { TokenValues.StackAllocKeyword }
						)
					);

					break;
				}
				case ImplicitStackAllocArrayCreationExpressionSyntax implicitStackAllocExpression:
				{
					DiagnosticList.Add(
						Diagnostic.Create(
							descriptor: SD0309,
							location: implicitStackAllocExpression.GetLocation(),
							messageArgs: new[] { TokenValues.StackAllocKeyword }
						)
					);

					break;
				}
			}
		}
	}
}
