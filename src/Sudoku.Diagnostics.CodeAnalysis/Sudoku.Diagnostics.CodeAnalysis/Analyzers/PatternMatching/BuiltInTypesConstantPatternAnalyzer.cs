using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Diagnostics.Extensions;
using Sudoku.CodeGen;
using Sudoku.Diagnostics.CodeAnalysis.Extensions;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	[CodeAnalyzer("SS0604")]
	public sealed partial class BuiltInTypesConstantPatternAnalyzer : DiagnosticAnalyzer
	{
		/// <summary>
		/// Indicates the equals token and not equals token.
		/// </summary>
		private const string EqualsToken = "==", NotEqualsToken = "!=";


		/// <inheritdoc/>
		public override void Initialize(AnalysisContext context)
		{
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
			context.EnableConcurrentExecution();

			context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode, new[] { SyntaxKind.IsPatternExpression });
		}


		private static void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
		{
			var (semanticModel, _, originalNode, _, cancellationToken) = context;

			if (
				originalNode is not IsPatternExpressionSyntax
				{
					Expression: var expressionToCheck,
					Pattern: var pattern
				}
			)
			{
				return;
			}

			switch (pattern)
			{
				// o is constantValue
				case ConstantPatternSyntax
				{
					Expression: { RawKind: not (int)SyntaxKind.NullLiteralExpression } constantExpression
				}
				when semanticModel.TypeEquals(
					expressionToCheck, constantExpression, cancellationToken: cancellationToken
				):
				{
					context.ReportDiagnostic(
						Diagnostic.Create(
							descriptor: SS0604,
							location: originalNode.GetLocation(),
							messageArgs: new[]
							{
								expressionToCheck.ToString(),
								EqualsToken,
								constantExpression.ToString()
							},
							properties: ImmutableDictionary.CreateRange(
								new KeyValuePair<string, string?>[]
								{
									new("EqualityToken", EqualsToken)
								}
							),
							additionalLocations: new[]
							{
								expressionToCheck.GetLocation(),
								constantExpression.GetLocation()
							}
						)
					);

					break;
				}

				// o is not constantValue
				case UnaryPatternSyntax
				{
					RawKind: (int)SyntaxKind.NotPattern,
					Pattern: ConstantPatternSyntax
					{
						Expression: { RawKind: not (int)SyntaxKind.NullLiteralExpression } constantExpression
					}
				}
				when semanticModel.TypeEquals(
					expressionToCheck, constantExpression, cancellationToken: cancellationToken
				):
				{
					context.ReportDiagnostic(
						Diagnostic.Create(
							descriptor: SS0604,
							location: originalNode.GetLocation(),
							messageArgs: new[]
							{
								expressionToCheck.ToString(),
								NotEqualsToken,
								constantExpression.ToString()
							},
							properties: ImmutableDictionary.CreateRange(
								new KeyValuePair<string, string?>[]
								{
									new("EqualityToken", NotEqualsToken)
								}
							),
							additionalLocations: new[]
							{
								expressionToCheck.GetLocation(),
								constantExpression.GetLocation()
							}
						)
					);

					break;
				}
			}
		}
	}
}
