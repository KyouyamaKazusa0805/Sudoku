using System;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Extensions;

namespace Sudoku.Diagnostics.CodeAnalysis.CodeFixers
{
	/// <summary>
	/// Indicates the code fixer for solving the diagnostic result
	/// <a href="https://github.com/SunnieShine/Sudoku/wiki/Rule-SD0102">SD0102</a>.
	/// </summary>
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SD0102CodeFixProvider)), Shared]
	public sealed class SD0102CodeFixProvider : CodeFixProvider
	{
		/// <inheritdoc/>
		public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(
			DiagnosticIds.SD0102
		);

		/// <inheritdoc/>
		public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;


		/// <inheritdoc/>
		public override async Task RegisterCodeFixesAsync(CodeFixContext context)
		{
			var document = context.Document;
			var diagnostic = context.Diagnostics.First(static d => d.Id == nameof(DiagnosticIds.SD0102));
			var root = (await document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false))!;
			var ((_, span), _) = diagnostic;
			var propertyDecl = (PropertyDeclarationSyntax)root.FindNode(span, getInnermostNodeForTie: true);

			context.RegisterCodeFix(
				CodeAction.Create(
					title: CodeFixTitles.SD0102,
					createChangedDocument: c => AppendOrChangeAccessibilityAsync(document, propertyDecl, c),
					equivalenceKey: nameof(CodeFixTitles.SD0102)
				),
				diagnostic
			);
		}


		/// <summary>
		/// Delegated method that is invoked by <see cref="RegisterCodeFixesAsync(CodeFixContext)"/> above.
		/// </summary>
		/// <param name="document">The current document to fix.</param>
		/// <param name="propertyDeclaration">
		/// The method declarartion node that the diagnostic result occurs.
		/// </param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>A task that handles this operation.</returns>
		/// <seealso cref="RegisterCodeFixesAsync(CodeFixContext)"/>
		private static async Task<Document> AppendOrChangeAccessibilityAsync(
			Document document, PropertyDeclarationSyntax propertyDeclaration, CancellationToken cancellationToken)
		{
			var editor = await DocumentEditor.CreateAsync(document, cancellationToken);
			editor.SetAccessibility(propertyDeclaration, Accessibility.Public);

			return document.WithSyntaxRoot(editor.GetChangedRoot());
		}
	}
}
