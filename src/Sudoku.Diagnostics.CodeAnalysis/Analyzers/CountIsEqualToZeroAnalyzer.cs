namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers;

[Generator]
[CodeAnalyzer("SD0302")]
public sealed partial class CountIsEqualToZeroAnalyzer : ISourceGenerator
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
					Node: BinaryExpressionSyntax
					{
						RawKind: var kind,
						Left: MemberAccessExpressionSyntax
						{
							RawKind: (int)SyntaxKind.SimpleMemberAccessExpression,
							Expression: var exprNode,
							Name.Identifier.ValueText: MemberNames.Count
						},
						Right: LiteralExpressionSyntax
						{
							RawKind: (int)SyntaxKind.NumericLiteralExpression,
							Token.ValueText: TokenValues.Zero
						}
					} node,
					SemanticModel: { Compilation: var compilation } semanticModel
				}
			)
			{
				return;
			}

			if (
				semanticModel.GetOperation(exprNode) is not
				{
					Kind: OperationKind.LocalReference,
					Type: var typeSymbol
				}
			)
			{
				return;
			}

			var cellsSymbol = compilation.GetTypeByMetadataName(TypeNames.Cells);
			var candidatesSymbol = compilation.GetTypeByMetadataName(TypeNames.Candidates);
			if (!SymbolEqualityComparer.Default.Equals(typeSymbol, cellsSymbol)
				&& !SymbolEqualityComparer.Default.Equals(typeSymbol, candidatesSymbol))
			{
				return;
			}

			string equalityToken = kind == (int)SyntaxKind.NotEqualsExpression
				? TokenValues.InequalityOperator
				: TokenValues.EqualityOperator;
			DiagnosticList.Add(
				Diagnostic.Create(
					descriptor: SD0302,
					location: node.GetLocation(),
#if SUPPORT_CODE_FIXER
					properties: ImmutableDictionary.CreateRange(
						new KeyValuePair<string, string?>[]
						{
							new("NodeName", nodeStr),
							new("Operator", @operator)
						}
					),
#endif
					messageArgs: new[]
					{
						exprNode.ToString(),
						equalityToken,
						equalityToken == TokenValues.EqualityOperator ? string.Empty : TokenValues.NegateOperator
					}
				)
			);
		}
	}
}
