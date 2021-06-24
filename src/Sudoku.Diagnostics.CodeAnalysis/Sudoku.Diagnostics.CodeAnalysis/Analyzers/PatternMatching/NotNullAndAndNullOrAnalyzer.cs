using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Diagnostics.Extensions;
using Sudoku.CodeGen;
using Sudoku.Diagnostics.CodeAnalysis.Extensions;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	[CodeAnalyzer("SS0618F", "SS0619F")]
	public sealed partial class NotNullAndAndNullOrAnalyzer : DiagnosticAnalyzer
	{
		/// <inheritdoc/>
		public override void Initialize(AnalysisContext context)
		{
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
			context.EnableConcurrentExecution();

			context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode, new[] { SyntaxKind.IsPatternExpression });
		}


		private static void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
		{
			var (semanticModel, _, originalNode) = context;

			if (
				originalNode is not IsPatternExpressionSyntax
				{
					Expression: var expr,
					Pattern: BinaryPatternSyntax pattern
				} node
			)
			{
				return;
			}

			if (semanticModel.GetOperation(expr) is not { Type: (_, _, isNullable: true) })
			{
				return;
			}

			switch (pattern)
			{
				case { RawKind: (int)SyntaxKind.AndPattern, Left: var leftPattern, Right: var rightPattern }:
				{
					checkNotNullAndRecursively(context, new[] { leftPattern, rightPattern });

					break;
				}
				case { RawKind: (int)SyntaxKind.OrPattern, Left: var leftPattern, Right: var rightPattern }:
				{
					checkNullOrRecursively(context, new[] { leftPattern, rightPattern });

					break;
				}
			}


			static void checkNotNullAndRecursively(
				in SyntaxNodeAnalysisContext context, PatternSyntax[] nestedPatterns)
			{
				foreach (var nestedPattern in nestedPatterns)
				{
					switch (nestedPattern)
					{
						case UnaryPatternSyntax
						{
							RawKind: (int)SyntaxKind.NotPattern,
							Pattern: ConstantPatternSyntax
							{
								Expression: LiteralExpressionSyntax
								{
									RawKind: (int)SyntaxKind.NullLiteralExpression
								}
							}
						}:
						{
							context.ReportDiagnostic(
								Diagnostic.Create(
									descriptor: SS0618,
									location: nestedPattern.GetLocation(),
									messageArgs: null
								)
							);

							break;
						}
						case BinaryPatternSyntax
						{
							RawKind: var kind,
							Left: var nestedLeftPattern,
							Right: var nestedRightPattern
						}:
						{
							Recursion? action = kind switch
							{
								(int)SyntaxKind.AndPattern => checkNotNullAndRecursively,
								(int)SyntaxKind.OrPattern => checkNullOrRecursively,
								_ => null
							};

							action?.Invoke(context, new[] { nestedLeftPattern, nestedRightPattern });

							break;
						}
					}
				}
			}

			static void checkNullOrRecursively(
				in SyntaxNodeAnalysisContext context, PatternSyntax[] nestedPatterns)
			{
				foreach (var nestedPattern in nestedPatterns)
				{
					switch (nestedPattern)
					{
						case ConstantPatternSyntax
						{
							Expression: LiteralExpressionSyntax
							{
								RawKind: (int)SyntaxKind.NullLiteralExpression
							}
						}:
						{
							context.ReportDiagnostic(
								Diagnostic.Create(
									descriptor: SS0619,
									location: nestedPattern.GetLocation(),
									messageArgs: null
								)
							);

							break;
						}
						case BinaryPatternSyntax
						{
							RawKind: var kind,
							Left: var nestedLeftPattern,
							Right: var nestedRightPattern
						}:
						{
							Recursion? action = kind switch
							{
								(int)SyntaxKind.AndPattern => checkNotNullAndRecursively,
								(int)SyntaxKind.OrPattern => checkNullOrRecursively,
								_ => null
							};

							action?.Invoke(context, new[] { nestedLeftPattern, nestedRightPattern });

							break;
						}
					}
				}
			}
		}


#pragma warning disable IDE1006
		/// <summary>
		/// Delegates a method that invokes to handle the sub-patterns in an <see langword="and"/>
		/// or <see langword="or"/> pattern recursively.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="nestedPatterns">The nested patterns to iterate.</param>
		private delegate void Recursion(in SyntaxNodeAnalysisContext context, PatternSyntax[] nestedPatterns);
#pragma warning restore IDE1006
	}
}
