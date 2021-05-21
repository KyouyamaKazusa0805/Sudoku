using System;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Extensions;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text.Extensions;
using System.Collections.Generic;

namespace Sudoku.Diagnostics.CodeAnalysis.CodeFixers
{
	/// <summary>
	/// Indicates the code fixer for solving the diagnostic result
	/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/SS9001?sort_id=4042356">
	/// SS9001
	/// </a>.
	/// </summary>
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SS9001CodeFixProvider)), Shared]
	public sealed class SS9001CodeFixProvider : CodeFixProvider
	{
		/// <inheritdoc/>
		public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(
			DiagnosticIds.SS9001
		);

		/// <inheritdoc/>
		public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;


		/// <inheritdoc/>
		public override async Task RegisterCodeFixesAsync(CodeFixContext context)
		{
			var document = context.Document;
			var diagnostic = context.Diagnostics.First(static d => d.Id == DiagnosticIds.SS9001);
			var root = (await document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false))!;
			var ((_, badExprSpan), descriptor) = diagnostic;
			var badExpression = (ExpressionSyntax)root.FindNode(badExprSpan, getInnermostNodeForTie: true);
			var (_, forLoopSpan) = diagnostic.AdditionalLocations[0];
			var forLoop = root.FindNode(forLoopSpan, getInnermostNodeForTie: true);

			context.RegisterCodeFix(
				CodeAction.Create(
					title: CodeFixTitles.SS9001,
					createChangedDocument: c => ExtractVariableAsync(
						document: document,
						root: root,
						badExpression: badExpression,
						forLoop: forLoop,
						variableName: diagnostic.Properties["VariableName"]!,
						suggestedName: diagnostic.Properties["SuggestedName"]!,
						cancellationToken: c
					),
					equivalenceKey: nameof(CodeFixTitles.SS9001)
				),
				diagnostic
			);
		}


		/// <summary>
		/// Delegated method that is invoked by <see cref="RegisterCodeFixesAsync(CodeFixContext)"/> above.
		/// </summary>
		/// <param name="document">The current document to fix.</param>
		/// <param name="root">The syntax root node.</param>
		/// <param name="badExpression">
		/// The interpolted string expression node that the diagnostic result occurs.
		/// </param>
		/// <param name="forLoop">The syntax node for that <see langword="for"/> loop.</param>
		/// <param name="variableName">The variable name to assign.</param>
		/// <param name="suggestedName">Indicates the suggested name.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>A task that handles this operation.</returns>
		/// <seealso cref="RegisterCodeFixesAsync(CodeFixContext)"/>
		/// <exception cref="InvalidOperationException">
		/// Throws when <paramref name="forLoop"/> is invalid.
		/// </exception>
		private static async Task<Document> ExtractVariableAsync(
			Document document, SyntaxNode root, ExpressionSyntax badExpression, SyntaxNode forLoop,
			string variableName, string suggestedName, CancellationToken cancellationToken = default) =>
			await Task.Run(() =>
			{
				if (
					forLoop is not ForStatementSyntax
					{
						Declaration: { Type: var declarationType, Variables: var variableDeclarators },
						Condition: { RawKind: var conditionKind },
						Incrementors: var incrementors,
						Statement: var statement,
					}
				)
				{
					throw new InvalidOperationException("The specified syntax node is invalid.");
				}

				const string systemSuggestedDefaultVariableName = "variableName";
				string resultDefaultVariableName = suggestedName ?? systemSuggestedDefaultVariableName;

				var newRoot = root.ReplaceNode(
					forLoop,
					SyntaxFactory.ForStatement(statement)
					.WithDeclaration(
						SyntaxFactory.VariableDeclaration(declarationType)
						.WithVariables(
							SyntaxFactory.SeparatedList(
								new List<VariableDeclaratorSyntax>(variableDeclarators)
								{
									SyntaxFactory.VariableDeclarator(
										SyntaxFactory.Identifier(resultDefaultVariableName)
									)
									.WithInitializer(
										SyntaxFactory.EqualsValueClause(badExpression)
									)
								}
							)
						)
					)
					.WithCondition(
						SyntaxFactory.BinaryExpression(
							(SyntaxKind)conditionKind,
							SyntaxFactory.IdentifierName(variableName),
							SyntaxFactory.IdentifierName(resultDefaultVariableName)
						)
					)
					.WithIncrementors(incrementors)
					.NormalizeWhitespace()
				);

				return document.WithSyntaxRoot(newRoot);
			}, cancellationToken);
	}
}
