using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Sudoku.CodeGenerating;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	[CodeAnalyzer("SS0634")]
	public sealed partial class AvailableEmptyBracePropertyPatternAnalyzer : DiagnosticAnalyzer
	{
		/// <summary>
		/// Indicates the property name that is used in <see cref="KeyValuePair{TKey, TValue}.Key"/>.
		/// </summary>
		private const string VariableNamePropertyName = "VariableName";

		/// <summary>
		/// Indicates a pair of braces (i.e. <c>{ }</c>).
		/// </summary>
		private const string EmptyBrace = "{ }";


		/// <inheritdoc/>
		public override void Initialize(AnalysisContext context)
		{
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
			context.EnableConcurrentExecution();

			context.RegisterSyntaxNodeAction(
				AnalyzeSyntaxNode,
				new[] { SyntaxKind.IsPatternExpression, SyntaxKind.LogicalAndExpression }
			);
		}


		private static void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
		{
			switch (context.Node)
			{
				// o is object variable
				case IsPatternExpressionSyntax
				{
					Expression: var expr,
					Pattern: DeclarationPatternSyntax
					{
						Type: PredefinedTypeSyntax { Keyword: { RawKind: (int)SyntaxKind.ObjectKeyword } },
						Designation: SingleVariableDesignationSyntax
						{
							Identifier: { ValueText: var variableName }
						}
					}
				} node:
				{
					context.ReportDiagnostic(
						Diagnostic.Create(
							descriptor: SS0634,
							location: node.GetLocation(),
							messageArgs: new[] { expr.ToString(), variableName, EmptyBrace },
							properties: ImmutableDictionary.CreateRange(
								new KeyValuePair<string, string?>[]
								{
									new(VariableNamePropertyName, variableName)
								}
							),
							additionalLocations: new[] { expr.GetLocation() }
						)
					);

					break;
				}

				// o is not null and var variable
				case IsPatternExpressionSyntax
				{
					Expression: var expr,
					Pattern: BinaryPatternSyntax
					{
						RawKind: (int)SyntaxKind.AndPattern,
						Left: var leftPattern,
						Right: var rightPattern
					}
				} node:
				{
					switch ((leftPattern, rightPattern))
					{
						case (
							UnaryPatternSyntax
							{
								Pattern: ConstantPatternSyntax
								{
									Expression: LiteralExpressionSyntax
									{
										RawKind: (int)SyntaxKind.NullLiteralExpression
									}
								}
							} or RecursivePatternSyntax
							{
								PositionalPatternClause: null,
								PropertyPatternClause: { Subpatterns: { Count: 0 } },
								Designation: null
							} or TypePatternSyntax
							{
								Type: PredefinedTypeSyntax
								{
									Keyword: { RawKind: (int)SyntaxKind.ObjectKeyword }
								}
							},
							VarPatternSyntax
							{
								Designation: SingleVariableDesignationSyntax
								{
									Identifier: { ValueText: var variableName }
								}
							}
						):
						{
							context.ReportDiagnostic(
								Diagnostic.Create(
									descriptor: SS0634,
									location: node.GetLocation(),
									messageArgs: new[] { expr.ToString(), variableName, EmptyBrace },
									properties: ImmutableDictionary.CreateRange(
										new KeyValuePair<string, string?>[]
										{
											new(VariableNamePropertyName, variableName)
										}
									),
									additionalLocations: new[] { expr.GetLocation() }
								)
							);

							break;
						}
						case (
							VarPatternSyntax
							{
								Designation: SingleVariableDesignationSyntax
								{
									Identifier: { ValueText: var variableName }
								}
							},
							UnaryPatternSyntax
							{
								Pattern: ConstantPatternSyntax
								{
									Expression: LiteralExpressionSyntax
									{
										RawKind: (int)SyntaxKind.NullLiteralExpression
									}
								}
							} or RecursivePatternSyntax
							{
								PositionalPatternClause: null,
								PropertyPatternClause: { Subpatterns: { Count: 0 } },
								Designation: null
							} or TypePatternSyntax
							{
								Type: PredefinedTypeSyntax
								{
									Keyword: { RawKind: (int)SyntaxKind.ObjectKeyword }
								}
							}
						):
						{
							context.ReportDiagnostic(
								Diagnostic.Create(
									descriptor: SS0634,
									location: node.GetLocation(),
									messageArgs: new[] { expr.ToString(), variableName, EmptyBrace },
									properties: ImmutableDictionary.CreateRange(
										new KeyValuePair<string, string?>[]
										{
											new(VariableNamePropertyName, variableName)
										}
									),
									additionalLocations: new[] { expr.GetLocation() }
								)
							);

							break;
						}
					}

					break;
				}
			}
		}
	}
}
