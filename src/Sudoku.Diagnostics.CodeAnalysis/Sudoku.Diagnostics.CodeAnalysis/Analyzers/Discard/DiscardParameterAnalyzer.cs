using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Diagnostics.Extensions;
using Sudoku.CodeGenerating;
using Sudoku.CodeGenerating.Extensions;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	[CodeAnalyzer("SD0413")]
	public sealed partial class DiscardParameterAnalyzer : DiagnosticAnalyzer
	{
		/// <inheritdoc/>
		public override void Initialize(AnalysisContext context)
		{
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
			context.EnableConcurrentExecution();

			context.RegisterSyntaxNodeAction(
				AnalyzeSyntaxNode,
				new[] { SyntaxKind.MethodDeclaration, SyntaxKind.LocalFunctionStatement }
			);
		}


		private static void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
		{
			var (semanticModel, compilation, originalNode, _, cancellationToken) = context;

			Func<ISymbol?, ISymbol?, bool> f = SymbolEqualityComparer.Default.Equals;
			var attribute = compilation.GetTypeByMetadataName("System.Diagnostics.CodeAnalysis.DiscardAttribute");
			switch (originalNode)
			{
				case MethodDeclarationSyntax method when whenClause(method, out string? parameterName):
				{
					traverseDescendants(
						method,
						parameterName
#if !NETSTANDARD2_1_OR_GREATER
						!
#endif
					);

					break;
				}
				case LocalFunctionStatementSyntax function when whenClause(function, out string? parameterName):
				{
					traverseDescendants(
						function,
						parameterName
#if !NETSTANDARD2_1_OR_GREATER
						!
#endif
					);

					break;
				}


				bool whenClause(
					SyntaxNode method,
#if NETSTANDARD2_1_OR_GREATER
					[NotNullWhen(true)]
#endif
					out string? parameterName
				)
				{
					if (
						semanticModel.GetDeclaredSymbol(method, cancellationToken) is IMethodSymbol
						{
							Parameters: { Length: not 0 } parameters
						} && parameters.FirstOrDefault(
							parameter => parameter.GetAttributes().Any(a => f(a.AttributeClass, attribute))
						) is { Name: var possibleParameterName }
					)
					{
						parameterName = possibleParameterName;
						return true;
					}
					else
					{
						parameterName = null;
						return false;
					}
				}

				void traverseDescendants(SyntaxNode method, string parameterName)
				{
					foreach (var usage in method.DescendantNodes())
					{
						if (
							usage is not IdentifierNameSyntax
							{
								Parent: not ArgumentSyntax
								{
									Parent: ArgumentListSyntax
									{
										Parent: InvocationExpressionSyntax
										{
											Expression: IdentifierNameSyntax
											{
												Identifier: { ValueText: "nameof" }
											}
										}
									}
								},
								Identifier: { ValueText: var possibleReference }
							}
						)
						{
							continue;
						}

						if (possibleReference != parameterName)
						{
							continue;
						}

						// BUG: If the variable exists in a static lambda, static anonymous function or a static local function, the variable may not the same variable.
						context.ReportDiagnostic(
							Diagnostic.Create(
								descriptor: SD0413,
								location: usage.GetLocation(),
								messageArgs: null
							)
						);
					}
				}
			}
		}
	}
}
