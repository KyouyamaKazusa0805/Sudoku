using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Diagnostics.Extensions;
using Microsoft.CodeAnalysis.Operations;
using Sudoku.CodeGen;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	[CodeAnalyzer("SS0601", "SS0602", "SS0603")]
	public sealed partial class TypePatternAnalyzer : DiagnosticAnalyzer
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
					SyntaxKind.LogicalNotExpression,
					SyntaxKind.IsExpression,
					SyntaxKind.IsPatternExpression
				}
			);
		}


		private static void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
		{
			var (semanticModel, compilation, originalNode) = context;

			CheckSS0601AndSS0603(context, semanticModel, originalNode);
			CheckSS0602(context, semanticModel, compilation, originalNode);
		}

		private static void CheckSS0601AndSS0603(
			SyntaxNodeAnalysisContext context, SemanticModel semanticModel, SyntaxNode originalNode)
		{
			switch (originalNode)
			{
				case BinaryExpressionSyntax
				{
					RawKind: (int)SyntaxKind.IsExpression,
					Left: var expression,
					Right: var type
				} node:
				{
					switch (type)
					{
						case PredefinedTypeSyntax { Keyword: { RawKind: (int)SyntaxKind.ObjectKeyword } }:
						{
							context.ReportDiagnostic(
								Diagnostic.Create(
									descriptor: SS0603,
									location: node.GetLocation(),
									messageArgs: null
								)
							);

							break;
						}
						case TypeSyntax:
						{
							if (semanticModel.GetOperation(expression) is not { Type: var definedType })
							{
								return;
							}

							if (
								semanticModel.GetOperation(node) is not IIsTypeOperation
								{
									TypeOperand: { } matchedType
								}
							)
							{
								return;
							}

							if (!SymbolEqualityComparer.Default.Equals(definedType, matchedType))
							{
								return;
							}

							context.ReportDiagnostic(
								Diagnostic.Create(
									descriptor: SS0601,
									location: type.GetLocation(),
									messageArgs: null
								)
							);

							break;
						}
					}

					break;
				}
				case IsPatternExpressionSyntax
				{
					Expression: var expression,
					Pattern: DeclarationPatternSyntax { Type: var type } pattern
				} node:
				{
					switch (type)
					{
						case PredefinedTypeSyntax
						{
							Parent: { Parent: not ParenthesizedExpressionSyntax },
							Keyword: { RawKind: (int)SyntaxKind.ObjectKeyword }
						}:
						{
							context.ReportDiagnostic(
								Diagnostic.Create(
									descriptor: SS0603,
									location: node.GetLocation(),
									messageArgs: null
								)
							);

							break;
						}
						case var _:
						{
							if (semanticModel.GetOperation(expression) is not { Type: var definedType })
							{
								return;
							}

							if (
								semanticModel.GetOperation(pattern) is not IDeclarationPatternOperation
								{
									MatchedType: { } matchedType
								}
							)
							{
								return;
							}

							if (!SymbolEqualityComparer.Default.Equals(definedType, matchedType))
							{
								return;
							}

							context.ReportDiagnostic(
								Diagnostic.Create(
									descriptor: SS0601,
									location: type.GetLocation(),
									messageArgs: null
								)
							);

							break;
						}
					}

					break;
				}
			}
		}

		private static void CheckSS0602(
			SyntaxNodeAnalysisContext context, SemanticModel semanticModel, Compilation compilation,
			SyntaxNode originalNode)
		{
			switch (originalNode)
			{
				case PrefixUnaryExpressionSyntax
				{
					Operand: ParenthesizedExpressionSyntax
					{
						Expression: BinaryExpressionSyntax expression
					}
				} node:
				{
					if (
						semanticModel.GetOperation(expression) is not IIsTypeOperation
						{
							TypeOperand: var possibleObjectType
						}
					)
					{
						return;
					}

					var objectType = compilation.GetSpecialType(SpecialType.System_Object);
					if (!SymbolEqualityComparer.Default.Equals(possibleObjectType, objectType))
					{
						return;
					}

					context.ReportDiagnostic(
						Diagnostic.Create(
							descriptor: SS0602,
							location: node.GetLocation(),
							messageArgs: null
						)
					);

					break;
				}
				case IsPatternExpressionSyntax
				{
					Pattern: UnaryPatternSyntax
					{
						RawKind: (int)SyntaxKind.NotPattern,
						Pattern: TypePatternSyntax typePattern
					}
				} pattern:
				{
					if (
						semanticModel.GetOperation(typePattern) is not ITypePatternOperation
						{
							MatchedType: { } possibleObjectType
						}
					)
					{
						return;
					}

					var objectType = compilation.GetSpecialType(SpecialType.System_Object);
					if (!SymbolEqualityComparer.Default.Equals(possibleObjectType, objectType))
					{
						return;
					}

					context.ReportDiagnostic(
						Diagnostic.Create(
							descriptor: SS0602,
							location: pattern.GetLocation(),
							messageArgs: null
						)
					);

					break;
				}
			}
		}
	}
}
