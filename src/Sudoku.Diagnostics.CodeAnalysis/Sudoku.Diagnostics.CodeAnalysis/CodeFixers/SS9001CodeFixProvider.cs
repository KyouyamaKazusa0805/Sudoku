using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Extensions;
using Microsoft.CodeAnalysis.Text.Extensions;

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
						badExpression: badExpression,
						forLoop: forLoop,
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
		/// <param name="badExpression">The expression node that the diagnostic result occurs.</param>
		/// <param name="forLoop">The syntax node for that <see langword="for"/> loop.</param>
		/// <param name="suggestedName">Indicates the suggested name.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>A task that handles this operation.</returns>
		/// <seealso cref="RegisterCodeFixesAsync(CodeFixContext)"/>
		/// <exception cref="InvalidOperationException">
		/// Throws when <paramref name="forLoop"/> is invalid.
		/// </exception>
		private static async Task<Document> ExtractVariableAsync(
			Document document, ExpressionSyntax badExpression, SyntaxNode forLoop,
			string suggestedName, CancellationToken cancellationToken = default)
		{
			// Check the for loop statement node is valid to replace.
			if (
				forLoop is not ForStatementSyntax
				{
					Declaration: { Type: var declarationType, Variables: var variableDeclarators } declaration
				}
			)
			{
				throw new InvalidOperationException("The specified syntax node is invalid.");
			}

			// Now define a temporary name to be replaced.
			const string systemSuggestedDefaultVariableName = "variableName";
			string resultDefaultVariableName = suggestedName ?? systemSuggestedDefaultVariableName;

			// Check whether the result variable name is duplicate in the old variable declarator list.
			if (
				variableDeclarators.Any(
					variableDeclarator => variableDeclarator.Identifier.ValueText == resultDefaultVariableName
				)
			)
			{
				for (uint trial = 1; ; trial++)
				{
					bool exists = false;
					string currentPossibleName = $"{resultDefaultVariableName}{trial}";
					foreach (var variableDeclarator in variableDeclarators)
					{
						if (variableDeclarator.Identifier.ValueText == currentPossibleName)
						{
							exists = true;
							break;
						}
					}

					if (!exists)
					{
						resultDefaultVariableName = currentPossibleName;
						break;
					}
				}
			}

			// Now we can change and replace the syntax nodes.
			var editor = await DocumentEditor.CreateAsync(document, cancellationToken);
			editor.ReplaceNode(
				declaration,
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
							.NormalizeWhitespace(indentation: "\t")
						}
					)
				)
			);
			editor.ReplaceNode(badExpression, SyntaxFactory.IdentifierName(resultDefaultVariableName));

			// Returns the changed document.
			return editor.GetChangedDocument();
		}
	}
}
