namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers;

[CodeAnalyzer("SS0313")]
public sealed partial class LetClauseAnalyzer : DiagnosticAnalyzer
{
	/// <inheritdoc/>
	public override void Initialize(AnalysisContext context)
	{
		context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
		context.EnableConcurrentExecution();

		context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode, new[] { SyntaxKind.QueryExpression });
	}


	private static void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
	{
		var (semanticModel, _, originalNode, _, cancellationToken) = context;

		// BUG: Here we don't check the property Continuation part, which may also be a variable to check.
		if (
			originalNode is not QueryExpressionSyntax
			{
				FromClause.Identifier.ValueText: var variableInFromClause,
				Body:
				{
					Clauses: { Count: var count and not 0 } clauses,
					SelectOrGroup: var selectOrGroupClause
				}
			}
		)
		{
			return;
		}

		for (int i = 0; i < count; i++)
		{
			if (
				clauses[i] is not LetClauseSyntax
				{
					Identifier: { ValueText: var variableName } identifier,
					Expression: var expression
				} clause
			)
			{
				continue;
			}

			// Possible let clause found.
			// Check whether the variables declared above of this clause exist in the clauses after this clause.
			var variableNames = new List<string> { variableInFromClause };
			for (int j = 0; j < i; j++)
			{
				if (clauses[j] is LetClauseSyntax { Identifier.ValueText: var currentVariableName })
				{
					variableNames.Add(currentVariableName);
				}
			}

			bool variableExists = false;
			for (int j = i + 1; j < count; j++)
			{
				switch (clauses[j])
				{
					case WhereClauseSyntax { Condition: var whereExpr } when descendantsExistsVariableName(whereExpr):
					case LetClauseSyntax { Expression: var letExpr } when descendantsExistsVariableName(letExpr):
					{
						variableExists = true;
						goto CheckWhetherVariableExistsInList;
					}
				}
			}

			switch (selectOrGroupClause)
			{
				case SelectClauseSyntax { Expression: var selectExpr } when descendantsExistsVariableName(selectExpr):
				case GroupClauseSyntax { GroupExpression: var groupExpr, ByExpression: var byExpr }
				when descendantsExistsVariableName(groupExpr) || descendantsExistsVariableName(byExpr):
				{
					variableExists = true;
					break;
				}
			}

		CheckWhetherVariableExistsInList:
			if (variableExists)
			{
				continue;
			}

			context.ReportDiagnostic(
				Diagnostic.Create(
					descriptor: SS0313,
					location: identifier.GetLocation(),
					messageArgs: null
				)
			);


			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			bool descendantsExistsVariableName(SyntaxNode node) =>
				node.DescendantNodesAndSelf().Any(
					node =>
						node is IdentifierNameSyntax { Identifier.ValueText: var tempVariableName }
						&& variableNames.Contains(tempVariableName)
				);
		}
	}
}
