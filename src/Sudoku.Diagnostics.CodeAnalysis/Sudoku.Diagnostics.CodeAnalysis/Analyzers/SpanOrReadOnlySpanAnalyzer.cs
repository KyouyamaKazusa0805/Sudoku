using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Diagnostics.Extensions;
using Microsoft.CodeAnalysis.Operations;
using Sudoku.Diagnostics.CodeAnalysis.Extensions;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	/// <summary>
	/// Indicates the analyzer that analyzes the type <see cref="Span{T}"/> or <see cref="ReadOnlySpan{T}"/>.
	/// </summary>
	/// <seealso cref="Span{T}"/>
	/// <seealso cref="ReadOnlySpan{T}"/>
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public sealed partial class SpanOrReadOnlySpanAnalyzer : DiagnosticAnalyzer
	{
		/// <summary>
		/// Indicates the full type name of <see cref="Span{T}"/> of <see cref="int"/>.
		/// </summary>
		private const string SpanTypeFullName = "System.Span`1";

		/// <summary>
		/// Indicates the full type name of <see cref="ReadOnlySpan{T}"/> of <see cref="int"/>.
		/// </summary>
		private const string ReadOnlySpanTypeFullName = "System.ReadOnlySpan`1";


		/// <inheritdoc/>
		public override void Initialize(AnalysisContext context)
		{
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
			context.EnableConcurrentExecution();

			context.RegisterSyntaxNodeAction(
				static context => CheckSudoku017(context),
				new[] { SyntaxKind.MethodDeclaration, SyntaxKind.LocalFunctionStatement }
			);
		}


		private static void CheckSudoku017(SyntaxNodeAnalysisContext context)
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
			var (semanticModel, compilation) = context;
			foreach (var descendant in node.DescendantNodes().ToArray())
			{
				// Check whether the block contains explicit or implicit new clauses.
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
				var operation = semanticModel.GetOperation(newClauseNode);
				if (
					operation is not IObjectCreationOperation
					{
						Kind: OperationKind.ObjectCreation,
						Constructor: { } constructorSymbol
					}
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
					spanTypeSymbol = compilation
						.GetTypeByMetadataName(SpanTypeFullName)!
						.ConstructUnboundGenericType(),
					readOnlySpanTypeSymbol = compilation
						.GetTypeByMetadataName(ReadOnlySpanTypeFullName)!
						.ConstructUnboundGenericType();
				if (!SymbolEqualityComparer.Default.Equals(typeSymbol, spanTypeSymbol)
					&& !SymbolEqualityComparer.Default.Equals(typeSymbol, readOnlySpanTypeSymbol))
				{
					continue;
				}

				// Check parameters.
				var @params = constructorSymbol.Parameters;
				/*length-pattern*/
				if (@params.Length != 2)
				{
					continue;
				}

				if (
					!SymbolEqualityComparer.Default.Equals(
						@params[0].Type,
						compilation.GetPointerTypeSymbol(SpecialType.System_Void)
					)
					|| !SymbolEqualityComparer.Default.Equals(
						@params[1].Type,
						compilation.GetSpecialType(SpecialType.System_Int32)
					)
				)
				{
					continue;
				}

				// Potential syntax node found.
				// If the first argument is a variable, check the assignment.
				if (
					newClauseNode.ArgumentList?.Arguments[0] is not
					{
						RawKind: (int)SyntaxKind.Argument,
						Expression: IdentifierNameSyntax
						{
							Identifier: { ValueText: var variableName }
						}
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

				context.ReportDiagnostic(
					Diagnostic.Create(
						descriptor: new(
							id: DiagnosticIds.SS0102,
							title: Titles.SS0102,
							messageFormat: Messages.SS0102,
							category: Categories.Performance,
							defaultSeverity: DiagnosticSeverity.Warning,
							isEnabledByDefault: true,
							helpLinkUri: HelpLinks.SS0102
						),
						location: newClauseNode.GetLocation(),
						messageArgs: new[]
						{
							SymbolEqualityComparer.Default.Equals(typeSymbol, spanTypeSymbol)
							? "Span"
							: "ReadOnlySpan"
						}
					)
				);
			}
		}
	}
}
