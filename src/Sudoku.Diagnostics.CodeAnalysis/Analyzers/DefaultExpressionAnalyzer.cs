namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers;

[Generator]
[CodeAnalyzer("SD0303", "SD0304")]
public sealed partial class DefaultExpressionAnalyzer : ISourceGenerator
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
					Node: var originalNode,
					SemanticModel: { Compilation: var compilation } semanticModel
				}
			)
			{
				return;
			}

			Func<ISymbol?, ISymbol?, bool> f = SymbolEqualityComparer.Default.Equals;
			string typeName;
			bool isOfTypeSudokuGrid;
			SyntaxNode? parent;
			var cellsSymbol = compilation.GetTypeByMetadataName(TypeNames.Cells);
			var candidatesSymbol = compilation.GetTypeByMetadataName(TypeNames.Candidates);
			var gridSymbol = compilation.GetTypeByMetadataName(TypeNames.Grid);
			var sudokuGridSymbol = compilation.GetTypeByMetadataName(TypeNames.SudokuGrid);
			switch (originalNode)
			{
				case BaseObjectCreationExpressionSyntax
				{
					Parent: var parentNode,
					ArgumentList.Arguments.Count: 0,
					Initializer: null
				} node
				when semanticModel.GetOperation(node) is IObjectCreationOperation
				{
					Kind: OperationKind.ObjectCreation,
					Type: var typeSymbol
				}:
				{
					bool isOfTypeCells = f(typeSymbol, cellsSymbol);
					bool isOfTypeCandidates = f(typeSymbol, candidatesSymbol);
					isOfTypeSudokuGrid = f(typeSymbol, gridSymbol) || f(typeSymbol, sudokuGridSymbol);

					if (!isOfTypeCells && !isOfTypeCandidates && !isOfTypeSudokuGrid)
					{
						return;
					}

					parent = parentNode;
					typeName = isOfTypeCells
						? TypeNames.Cells
						: isOfTypeCandidates
							? TypeNames.Candidates
							: $"{TypeNames.SudokuGrid} or {TypeNames.Grid}";

					break;
				}
				case DefaultExpressionSyntax { Parent: var parentNode } node
				when semanticModel.GetOperation(node) is { Type: var typeSymbol }:
				{
					bool isOfTypeCells = f(typeSymbol, cellsSymbol);
					bool isOfTypeCandidates = f(typeSymbol, candidatesSymbol);
					isOfTypeSudokuGrid = f(typeSymbol, sudokuGridSymbol) || f(typeSymbol, gridSymbol);

					if (!isOfTypeCells && !isOfTypeCandidates && !isOfTypeSudokuGrid)
					{
						return;
					}

					parent = parentNode;
					typeName = isOfTypeCells
						? TypeNames.Cells
						: isOfTypeCandidates
							? TypeNames.Candidates
							: $"{TypeNames.SudokuGrid} or {TypeNames.Grid}";

					break;
				}
				case LiteralExpressionSyntax
				{
					Parent: var parentNode,
					RawKind: (int)SyntaxKind.DefaultLiteralExpression
				} node
				when semanticModel.GetOperation(node) is { Type: var typeSymbol }:
				{
					bool isOfTypeCells = f(typeSymbol, cellsSymbol);
					bool isOfTypeCandidates = f(typeSymbol, candidatesSymbol);
					isOfTypeSudokuGrid = f(typeSymbol, sudokuGridSymbol) || f(typeSymbol, gridSymbol);

					if (!isOfTypeCells && !isOfTypeCandidates && !isOfTypeSudokuGrid)
					{
						return;
					}

					parent = parentNode;
					typeName = isOfTypeCells
						? TypeNames.Cells
						: isOfTypeCandidates
							? TypeNames.Candidates
							: $"{TypeNames.SudokuGrid} or {TypeNames.Grid}";

					break;
				}
				default:
				{
					return;
				}
			}

			switch (parent)
			{
				case BinaryExpressionSyntax
				{
					Left: var expressionOrVariable,
					RawKind: var kind and (
						(int)SyntaxKind.EqualsExpression or (int)SyntaxKind.NotEqualsExpression
					)
				} binaryExpr:
				{
					string propertyName = isOfTypeSudokuGrid ? PropertyNames.IsUndefined : PropertyNames.IsEmpty;
					DiagnosticList.Add(
						Diagnostic.Create(
							descriptor: SD0304,
							location: binaryExpr.GetLocation(),
#if SUPPORT_CODE_FIXER
							properties: ImmutableDictionary.CreateRange(
								new KeyValuePair<string, string?>[]
								{
									new("Variable", expressionOrVariable.ToString()),
									new("PropertyName", propertyName),
									new("Operator", @operator)
								}
							),
#endif
							messageArgs: new[]
							{
								expressionOrVariable.ToString(),
								propertyName,
								kind == (int)SyntaxKind.EqualsExpression ? string.Empty : TokenValues.NegateOperator
							}
						)
					);

					break;
				}
				default:
				{
					string propertyName = isOfTypeSudokuGrid ? PropertyNames.Undefined : PropertyNames.Empty;
					DiagnosticList.Add(
						Diagnostic.Create(
							descriptor: SD0303,
							location: originalNode.GetLocation(),
#if SUPPORT_CODE_FIXER
							properties: ImmutableDictionary.CreateRange(
								new KeyValuePair<string, string?>[]
								{
									new("TypeName", typeName),
									new("PropertyName", propertyName)
								}
							),
#endif
							messageArgs: new[] { typeName, propertyName }
						)
					);

					break;
				}
			}
		}
	}
}
