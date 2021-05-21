using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	/// <summary>
	/// Indicates an analyzer that analyzes the code for closed <see langword="enum"/> types.
	/// </summary>
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public sealed partial class ForLoopBadConditionAnalyzer : DiagnosticAnalyzer
	{
		/// <summary>
		/// Indicates the regular expression to match a member access expression.
		/// </summary>
		private static readonly Regex MemberAccessExpressionRegex = new(
			@"[\w\.]+", RegexOptions.Compiled, TimeSpan.FromSeconds(5)
		);


		/// <inheritdoc/>
		public override void Initialize(AnalysisContext context)
		{
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
			context.EnableConcurrentExecution();

			context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode, new[] { SyntaxKind.ForStatement });
		}


		private static void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
		{
			if (
				context.Node is not ForStatementSyntax
				{
					Condition: BinaryExpressionSyntax
					{
						RawKind:
							(int)SyntaxKind.EqualsExpression or (int)SyntaxKind.NotEqualsExpression
							or (int)SyntaxKind.GreaterThanExpression or (int)SyntaxKind.GreaterThanEqualsToken
							or (int)SyntaxKind.LessThanExpression or (int)SyntaxKind.LessThanEqualsToken,
						Left: var leftExpr,
						Right: var rightExpr
					}
				} node
			)
			{
				return;
			}

			// Many times we write complex expression and place them right, such as the following expression:
			//   i > arr.Length
			// where the expression 'arr.Length' is a complex expression that is at the right side.
			// Therefore, the analyzer will check the right expression at first.
			foreach (var (expressionNode, anotherNode) in new[] { (rightExpr, leftExpr), (leftExpr, rightExpr) })
			{
				if (!isSimpleExpression(expressionNode))
				{
					string? suggestedName = MemberAccessExpressionRegex.Match(expressionNode.ToString()) is
					{
						Success: true
					} match && match.ToString() is var matchStr && matchStr.LastIndexOf('.') is var pos
						? matchStr.Substring(pos + 1)
						: null;

					context.ReportDiagnostic(
						Diagnostic.Create(
							descriptor: SS9001,
							location: expressionNode.GetLocation(),
							properties: ImmutableDictionary.CreateRange(
								new KeyValuePair<string, string?>[]
								{
									new("VariableName", anotherNode.ToString()),
									new("SuggestedName", ToLowerCase(suggestedName))
								}
							),
							additionalLocations: new[] { node.GetLocation() },
							messageArgs: null
						)
					);

					break;
				}
			}


			static bool isSimpleExpression(ExpressionSyntax expression) =>
				expression is LiteralExpressionSyntax or DefaultExpressionSyntax or IdentifierNameSyntax;
		}

		private static unsafe string? ToLowerCase(string? s)
		{
			if (s is null)
			{
				return null;
			}

			char* ptr = stackalloc char[s.Length];
			fixed (char* pString = s)
			{
				Unsafe.CopyBlock(ptr, pString, (uint)(sizeof(char) * s.Length));
			}

			ptr[0] = (char)(ptr[0] + ' ');

			return new string(ptr);
		}
	}
}
