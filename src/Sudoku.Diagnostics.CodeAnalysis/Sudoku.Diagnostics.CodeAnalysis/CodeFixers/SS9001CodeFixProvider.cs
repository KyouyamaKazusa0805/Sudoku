using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Extensions;
using Microsoft.CodeAnalysis.Text.Extensions;
using Sudoku.CodeGenerating;

namespace Sudoku.Diagnostics.CodeAnalysis.CodeFixers
{
	[CodeFixProvider("SS9001")]
	public sealed partial class SS9001CodeFixProvider : CodeFixProvider
	{
		/// <inheritdoc/>
		public override async Task RegisterCodeFixesAsync(CodeFixContext context)
		{
			var document = context.Document;
			var diagnostic = context.Diagnostics.First(static d => d.Id == DiagnosticIds.SS9001);
			var root = (await document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false))!;
			var ((_, badExprSpan), descriptor) = diagnostic;
			var badExpression = (ExpressionSyntax)root.FindNode(badExprSpan, getInnermostNodeForTie: true);
			var (_, forLoopSpan) = diagnostic.AdditionalLocations[0];
			var forLoop = (ForStatementSyntax)root.FindNode(forLoopSpan, getInnermostNodeForTie: true);

			context.RegisterCodeFix(
				CodeAction.Create(
					title: CodeFixTitles.SS9001,
					createChangedDocument: async c =>
					{
						// Check the for loop statement node is valid to replace.
						if (
							forLoop is not
							{
								Declaration:
								{
									Type: var declarationType,
									Variables: var variableDeclarators
								} declaration
							}
						)
						{
							throw new InvalidOperationException("The specified syntax node is invalid.");
						}

						// Now define a temporary name to be replaced.
						const string systemSuggestedDefaultVariableName = "variableName";
						string suggestedName = diagnostic.Properties["SuggestedName"]!;
						string resultDefaultVariableName = suggestedName ?? systemSuggestedDefaultVariableName;

						// Check whether the result variable name is duplicate in the old variable declarator list.
						if (
							variableDeclarators.Any(
								variableDeclarator =>
									variableDeclarator.Identifier.ValueText == resultDefaultVariableName
							)
						)
						{
							for (uint trial = 1; ; trial = checked(trial + 1))
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
						var editor = await DocumentEditor.CreateAsync(document, c);
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
						editor.ReplaceNode(
							badExpression,
							SyntaxFactory.IdentifierName(resultDefaultVariableName)
						);

						// Returns the changed document.
						return editor.GetChangedDocument();
					},
					equivalenceKey: nameof(CodeFixTitles.SS9001)
				),
				diagnostic
			);
		}
	}
}
