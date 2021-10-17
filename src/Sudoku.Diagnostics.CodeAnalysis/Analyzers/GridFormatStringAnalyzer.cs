namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers;

[Generator]
[CodeAnalyzer("SD0310", "SD0311")]
public sealed partial class GridFormatStringAnalyzer : ISourceGenerator
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
		/// All possible format strings to check.
		/// </summary>
		private static readonly string[] PossibleFormats = new[]
		{
			".", "+", ".+", "+.", "0", ":", "!", ".!", "!.", "0!", "!0",
			".:", "0:", "0+", "+0", "+:", "+.:", ".+:", "#", "#.", "0+:",
			"+0:", "#0", ".!:", "!.:", "0!:", "!0:", "@", "@.", "@0", "@!",
			"@.!", "@!.", "@0!", "@!0", "@*", "@*.", "@.*", "@0*", "@*0",
			"@!*", "@*!", "@:", "@:!", "@!:", "@*:", "@:*", "@!*:", "@*!:",
			"@!:*", "@:!*", "@:!*", "@:*!", "~", "~0", "~.", "@~", "~@", "@~0",
			"@0~", "~@0", "~0@", "@~.", "@.~", "~@.", "~.@", "%", "^"
		};


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

			if (
				originalNode is not InvocationExpressionSyntax
				{
					Expression: MemberAccessExpressionSyntax
					{
						RawKind: (int)SyntaxKind.SimpleMemberAccessExpression,
						Expression: var expression,
						Name: { Identifier.ValueText: MemberNames.ToString } nameNode
					},
					ArgumentList.Arguments: { Count: var count } arguments
				} node
			)
			{
				return;
			}

			if (semanticModel.GetOperation(expression) is not { Type: var possibleSudokuGridType })
			{
				return;
			}

			if (compilation.GetTypeByMetadataName(TypeNames.SudokuGrid) is not { } sudokuGridType
				|| compilation.GetTypeByMetadataName(TypeNames.Grid) is not { } gridType)
			{
				return;
			}

			if (!SymbolEqualityComparer.Default.Equals(possibleSudokuGridType, sudokuGridType)
				&& !SymbolEqualityComparer.Default.Equals(possibleSudokuGridType, gridType))
			{
				return;
			}

			if (count == 0)
			{
				DiagnosticList.Add(
					Diagnostic.Create(
						descriptor: SD0310,
						location: nameNode.GetLocation(),
						messageArgs: null,
						additionalLocations: new[] { node.GetLocation() }
					)
				);
			}
			else if (
				arguments[0] is { Expression: var expr } argument
				&& semanticModel.GetOperation(expr, CancellationToken) is
				{
					ConstantValue: { HasValue: true, Value: string format }
				}
			)
			{
				if (Array.IndexOf(PossibleFormats, format) != -1)
				{
					return;
				}

				DiagnosticList.Add(
					Diagnostic.Create(
						descriptor: SD0311,
						location: argument.GetLocation(),
						messageArgs: null
					)
				);
			}
		}
	}
}
