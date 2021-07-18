using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Diagnostics.Extensions;
using Microsoft.CodeAnalysis.Operations;
using Sudoku.CodeGenerating;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	[CodeAnalyzer("SD0314")]
	public sealed partial class ValueStringBuilderDisposeAnalyzer : DiagnosticAnalyzer
	{
		/// <inheritdoc/>
		public override void Initialize(AnalysisContext context)
		{
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
			context.EnableConcurrentExecution();

			context.RegisterSyntaxNodeAction(
				AnalyzeSyntaxNode,
				new[]
				{
					SyntaxKind.LocalFunctionStatement,
					SyntaxKind.MethodDeclaration,
					SyntaxKind.GlobalStatement
				}
			);
		}


		private static void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
		{
			var (semanticModel, compilation, originanlNode, _, cancellationToken) = context;

			Func<ISymbol?, ISymbol?, bool> f = SymbolEqualityComparer.Default.Equals;
			Func<string, INamedTypeSymbol?> c = compilation.GetTypeByMetadataName;

			var vsbType = c("System.Text.ValueStringBuilder");
			switch (originanlNode)
			{
				case LocalFunctionStatementSyntax { Body: { Statements: { Count: >= 2 } statements } }:
				{
					checkOnMethod(statements);

					break;
				}
				case MethodDeclarationSyntax { Body: { Statements: { Count: >= 2 } statements } }:
				{
					checkOnMethod(statements);

					break;
				}
				case GlobalStatementSyntax { Parent: CompilationUnitSyntax unit, Statement: var statement }:
				{
					checkOnGlobalStatement(unit, statement);

					break;
				}
			}


			void checkOnMethod(in SyntaxList<StatementSyntax> statements)
			{
				int i = 0, count = statements.Count, iterationCount = count - 1;
				
				for (; i < iterationCount; i++)
				{
					// Step 1: Check whether the statement is a variable declaration of type 'ValueStringBuilder'.
					string? variableNameToCheck = null;
					foreach (var descendant in statements[i].DescendantNodes())
					{
						if (
							descendant is not BaseObjectCreationExpressionSyntax
							{
								Parent: EqualsValueClauseSyntax
								{
									Parent: VariableDeclaratorSyntax { Identifier: { ValueText: var variableName } }
								}
							} newClause
						)
						{
							continue;
						}

						// Check whether the current operation contains a operation instance to check.
						var operation = semanticModel.GetOperation(newClause, cancellationToken);
						if (operation is not IObjectCreationOperation { Type: var possibleVsbType })
						{
							continue;
						}

						// Check whether the object creation operation is of type 'ValueStringBuilder'.
						if (!f(possibleVsbType, vsbType))
						{
							continue;
						}

						variableNameToCheck = variableName;
						break;
					}

					if (variableNameToCheck is null)
					{
						continue;
					}

					// Step 2: Check whether the current statement has used the variable above.
					SyntaxNode? toStringInvocationNode = null, expressionStatement = null;
					for (int j = i + 1; j < iterationCount; j++)
					{
						switch (statements[j])
						{
							case ExpressionStatementSyntax
							{
								Expression: var innerExpression
							} currentExpressionStatement:
							{
								foreach (var innerDescendant in innerExpression.DescendantNodes())
								{
									// Check whether the node is an invocation 'ValueStringBuilder.ToString'.
									if (
										innerDescendant is not InvocationExpressionSyntax
										{
											Expression: MemberAccessExpressionSyntax
											{
												Expression: IdentifierNameSyntax
												{
													Identifier: { ValueText: var possibleVariableName }
												},
												Name: { Identifier: { ValueText: "ToString" } }
											},
											ArgumentList: { Arguments: { Count: 0 } }
										}
									)
									{
										continue;
									}

									// Check whether the variable exists above.
									// By this way we can check the type of that variable.
									if (possibleVariableName != variableNameToCheck)
									{
										continue;
									}

									toStringInvocationNode = innerDescendant;
									expressionStatement = currentExpressionStatement;
									goto CheckWhetherTheCurrentStatementContainsToStringInvocationOfThatType;
								}

								break;
							}
						}

					CheckWhetherTheCurrentStatementContainsToStringInvocationOfThatType:
						if (toStringInvocationNode is null)
						{
							continue;
						}

						// Step 3-1: Check whether the current statement contains the extra usage of that variable.
						// For example, if the expression is like:
						//
						//     sb.Append(sb.ToString());
						//
						// Because of the disposing of the variable 'sb', we can't use 'sb' to surround
						// the invocation 'sb.ToString', because the outer invocation 'sb.Append' will be
						// executed after than 'sb.ToString', and 'sb' has been already disposed now to call
						// 'sb.Append'.
						finalStepPhase1(toStringInvocationNode, expressionStatement!, variableNameToCheck);

						// Step 3-2: Check whether the last statements contains the usage of that variable.
						finalStepPhase2(ref j, count, statements, variableNameToCheck);
					}
				}
			}

			void checkOnGlobalStatement(CompilationUnitSyntax compilationUnit, StatementSyntax statement)
			{
				// Step 1: Check whether the statement is a variable declaration of type 'ValueStringBuilder'.
				string? variableNameToCheck = null;
				foreach (var descendant in statement.DescendantNodes())
				{
					if (
						descendant is not BaseObjectCreationExpressionSyntax
						{
							Parent: EqualsValueClauseSyntax
							{
								Parent: VariableDeclaratorSyntax { Identifier: { ValueText: var variableName } }
							}
						} newClause
					)
					{
						continue;
					}

					// Check whether the current operation contains a operation instance to check.
					var operation = semanticModel.GetOperation(newClause, cancellationToken);
					if (operation is not IObjectCreationOperation { Type: var possibleVsbType })
					{
						continue;
					}

					// Check whether the object creation operation is of type 'ValueStringBuilder'.
					if (!f(possibleVsbType, vsbType))
					{
						continue;
					}

					variableNameToCheck = variableName;
					break;
				}

				if (variableNameToCheck is null)
				{
					return;
				}

				// Step 2: Get all global statements.
				var globalStatements = new List<GlobalStatementSyntax>();
				bool isTheFirstStatement = true;
				foreach (var node in compilationUnit.DescendantNodes().OfType<GlobalStatementSyntax>())
				{
					if (isTheFirstStatement)
					{
						isTheFirstStatement = false;
						continue;
					}

					globalStatements.Add(node);
				}

				// Step 3: Check whether the current statement has used the variable above.
				int i = 0, count = globalStatements.Count;
				SyntaxNode? toStringInvocationNode = null, expressionStatement = null;
				for (int iterationCount = count - 1; i < iterationCount; i++)
				{
					switch (globalStatements[i])
					{
						case
						{
							Statement: ExpressionStatementSyntax
							{
								Expression: var innerExpression
							} currentExpressionStatement
						}:
						{
							foreach (var innerDescendant in innerExpression.DescendantNodes())
							{
								// Check whether the node is an invocation 'ValueStringBuilder.ToString'.
								if (
									innerDescendant is not InvocationExpressionSyntax
									{
										Expression: MemberAccessExpressionSyntax
										{
											Expression: IdentifierNameSyntax
											{
												Identifier: { ValueText: var possibleVariableName }
											},
											Name: { Identifier: { ValueText: "ToString" } }
										},
										ArgumentList: { Arguments: { Count: 0 } }
									}
								)
								{
									continue;
								}

								// Check whether the variable exists above.
								// By this way we can check the type of that variable.
								if (possibleVariableName != variableNameToCheck)
								{
									continue;
								}

								toStringInvocationNode = innerDescendant;
								expressionStatement = currentExpressionStatement;
								goto CheckWhetherTheCurrentStatementContainsToStringInvocationOfThatType;
							}

							break;
						}
					}
				}

			CheckWhetherTheCurrentStatementContainsToStringInvocationOfThatType:
				if (toStringInvocationNode is null)
				{
					return;
				}

				// Step 4-1: Check whether the current statement contains the extra usage of that variable.
				// For example, if the expression is like:
				//
				//     sb.Append(sb.ToString());
				//
				// Because of the disposing of the variable 'sb', we can't use 'sb' to surround
				// the invocation 'sb.ToString', because the outer invocation 'sb.Append' will be
				// executed after than 'sb.ToString', and 'sb' has been already disposed now to call
				// 'sb.Append'.
				finalStepPhase1(toStringInvocationNode, expressionStatement!, variableNameToCheck);

				// Step 4-2: Check whether the last statements contains the usage of that variable.
				finalStepPhase2(ref i, count, globalStatements, variableNameToCheck);
			}


			void finalStepPhase1(
				SyntaxNode toStringInvocationNode, SyntaxNode expressionStatement, string? variableNameToCheck)
			{
				foreach (var ancestor in toStringInvocationNode.Ancestors())
				{
					// The node span is out of range.
					if (ancestor.SpanStart < expressionStatement.SpanStart)
					{
						continue;
					}

					if (
						semanticModel.GetOperation(ancestor, cancellationToken) is not IInvocationOperation
						{
							Instance: ILocalReferenceOperation { Local: { Name: var possibleName } }
						}
					)
					{
						continue;
					}

					if (possibleName != variableNameToCheck)
					{
						continue;
					}

					context.ReportDiagnostic(
						Diagnostic.Create(
							descriptor: SD0314,
							location: toStringInvocationNode.GetLocation(),
							messageArgs: null
						)
					);

					return;
				}
			}

			void finalStepPhase2<TSyntaxNode>(
				ref int i, int count, IReadOnlyList<TSyntaxNode> statements, string? variableNameToCheck)
				where TSyntaxNode : SyntaxNode
			{
				while (++i < count)
				{
					var currentStatement = statements[i];
					foreach (var currentStatementDescendant in currentStatement.DescendantNodes())
					{
						if (
							semanticModel.GetOperation(
								currentStatementDescendant, cancellationToken
							) is not IInvocationOperation
							{
								Instance: ILocalReferenceOperation { Local: { Name: var possibleName } }
							}
						)
						{
							continue;
						}

						if (possibleName != variableNameToCheck)
						{
							continue;
						}

						context.ReportDiagnostic(
							Diagnostic.Create(
								descriptor: SD0314,
								location: currentStatementDescendant.GetLocation(),
								messageArgs: null
							)
						);

						// End the current statement checking.
						break;
					}
				}
			}
		}
	}
}
