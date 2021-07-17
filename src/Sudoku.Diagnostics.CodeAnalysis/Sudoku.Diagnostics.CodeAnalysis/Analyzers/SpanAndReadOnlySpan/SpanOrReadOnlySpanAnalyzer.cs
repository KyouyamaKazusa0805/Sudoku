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
	[CodeAnalyzer("SS0201")]
	public sealed partial class SpanOrReadOnlySpanAnalyzer : DiagnosticAnalyzer
	{
		/// <inheritdoc/>
		public override void Initialize(AnalysisContext context)
		{
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
			context.EnableConcurrentExecution();

			context.RegisterSyntaxNodeAction(
				CheckSS0201,
				new[] { SyntaxKind.MethodDeclaration, SyntaxKind.LocalFunctionStatement }
			);
		}


		private static void CheckSS0201(SyntaxNodeAnalysisContext context)
		{
			switch (context.Node)
			{
				case MethodDeclarationSyntax { Body: { } body }:
				{
					InternalVisit(context, body);

					break;
				}
				case LocalFunctionStatementSyntax { Body: { } body }:
				{
					InternalVisit(context, body);

					break;
				}
			}
		}

		/// <summary>
		/// Visit methods.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="node">The root node of the function.</param>
		private static void InternalVisit(SyntaxNodeAnalysisContext context, SyntaxNode node)
		{
			var (semanticModel, compilation, _, _, cancellationToken) = context;

			Func<ISymbol?, ISymbol?, bool> f = SymbolEqualityComparer.Default.Equals;
			Func<SpecialType, INamedTypeSymbol> s = compilation.GetSpecialType;
			Func<string, INamedTypeSymbol?> c = compilation.GetTypeByMetadataName;
			foreach (var descendant in node.DescendantNodes().ToArray())
			{
				// Check whether the block contains an explicit or implicit new expression.
				if (
					descendant is not BaseObjectCreationExpressionSyntax
					{
						RawKind:
							(int)SyntaxKind.ObjectCreationExpression
							or (int)SyntaxKind.ImplicitObjectCreationExpression
					} newClauseNode
				)
				{
					continue;
				}

				// Check operation and get the constructor symbol.
				if (
					semanticModel.GetOperation(newClauseNode, cancellationToken) is not IObjectCreationOperation
					{
						Kind: OperationKind.ObjectCreation,
						Constructor: { } constructorSymbol
					} operation
				)
				{
					continue;
				}

				// Check generic.
				if (constructorSymbol.ContainingType is not { IsGenericType: true } containingType)
				{
					continue;
				}

				// Check type.
				INamedTypeSymbol
					typeSymbol = containingType.ConstructUnboundGenericType(),
					spanTypeSymbol = c(typeof(Span<>).FullName)!.ConstructUnboundGenericType(),
					readOnlySpanTypeSymbol = c(typeof(ReadOnlySpan<>).FullName)!.ConstructUnboundGenericType();
				if (!f(typeSymbol, spanTypeSymbol) && !f(typeSymbol, readOnlySpanTypeSymbol))
				{
					continue;
				}

				// Check parameters.
				if (constructorSymbol.Parameters is not { Length: 2 } @params)
				{
					continue;
				}

				if (!f(@params[0].Type, compilation.CreatePointerTypeSymbol(s(SpecialType.System_Void)))
					|| !f(@params[1].Type, s(SpecialType.System_Int32)))
				{
					continue;
				}

				// Potential syntax node found.
				// If the first argument is a variable, check the assignment.
				if (
					newClauseNode.ArgumentList?.Arguments[0] is not
					{
						RawKind: (int)SyntaxKind.Argument,
						Expression: IdentifierNameSyntax { Identifier: { ValueText: var variableName } }
					}
				)
				{
					continue;
				}

				// Check the local variable.
				// If the assignment is 'stackalloc' clause, we can report the diagnostic result.
				if (
					Array.Find(
						node.DescendantNodes().ToArray(),
						element => element is VariableDeclaratorSyntax
						{
							Identifier: { ValueText: var localVariableName }
						} && localVariableName == variableName
					) is not VariableDeclaratorSyntax
					{
						Initializer: { Value: StackAllocArrayCreationExpressionSyntax }
					}
				)
				{
					continue;
				}

				// Check whether the Span- or ReadOnlySpan- typed instance is as the result value
				// of an out parameter or a return statement.
				switch (newClauseNode.Parent)
				{
					case AssignmentExpressionSyntax
					{
						Left: IdentifierNameSyntax { Identifier: { ValueText: var identifierName } }
					}:
					{
						var outParameters = new List<string>();
						for (
							IOperation? tempOperation = operation;
							tempOperation is not null;
							tempOperation = tempOperation.Parent
						)
						{
							if (
								tempOperation is IMethodReferenceOperation
								{
									Method: { Parameters: { Length: not 0 } parameters }
								}
							)
							{
								outParameters.AddRange(
									from parameter in parameters
									where parameter.RefKind == RefKind.Out
									select parameter.Name
								);
							}
						}
						if (outParameters.Count != 0 && outParameters.Contains(identifierName))
						{
							r();
						}

						break;
					}

					case ReturnStatementSyntax:
					{
						r();

						break;
					}

					void r() => context.ReportDiagnostic(
						Diagnostic.Create(
							descriptor: SS0201,
							location: newClauseNode.GetLocation(),
							messageArgs: new[]
							{
								(f(typeSymbol, spanTypeSymbol) ? typeof(Span<>) : typeof(ReadOnlySpan<>)).Name
							}
						)
					);
				}
			}
		}
	}
}
