using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Rename;
using System;

namespace Sudoku.CodeAnalysis
{
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SudokuCodeAnalysisCodeFixProvider)), Shared]
	public class SudokuCodeAnalysisCodeFixProvider : CodeFixProvider
	{
		public sealed override ImmutableArray<string> FixableDiagnosticIds =>
			ImmutableArray<string>.Empty;
			//ImmutableArray.Create("SUDOKU001");

		public sealed override FixAllProvider GetFixAllProvider()
		{
			// See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/FixAllProvider.md for more information on Fix All Providers
			return WellKnownFixAllProviders.BatchFixer;
		}

		public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
		{
			var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
			if (root is null)
			{
				return;
			}

			// TODO: Replace the following code with your own analysis, generating a CodeAction for each fix to suggest
			var diagnostic = context.Diagnostics.First();
			var diagnosticSpan = diagnostic.Location.SourceSpan;

			// Find the type declaration identified by the diagnostic.
			var parent = root.FindToken(diagnosticSpan.Start).Parent;
			if (parent is null)
			{
				return;
			}

			var declaration = parent.AncestorsAndSelf().OfType<TypeDeclarationSyntax>().First();

			// Register a code action that will invoke the fix.
			context.RegisterCodeFix(
				CodeAction.Create(
					title: CodeFixResources.CodeFixTitle,
					createChangedSolution: c => MakeUppercaseAsync(context.Document, declaration, c),
					equivalenceKey: nameof(CodeFixResources.CodeFixTitle)),
				diagnostic);
		}

		private async Task<Solution> MakeUppercaseAsync(Document document, TypeDeclarationSyntax typeDecl, CancellationToken cancellationToken)
		{
			// Compute new uppercase name.
			var identifierToken = typeDecl.Identifier;
			var newName = identifierToken.Text.ToUpperInvariant();

			// Get the symbol representing the type to be renamed.
			var semanticModel = await document.GetSemanticModelAsync(cancellationToken);
			var typeSymbol = semanticModel.GetDeclaredSymbol(typeDecl, cancellationToken);
			if (typeSymbol is null)
			{
				throw new Exception();
			}

			// Produce a new solution that has all references to that type renamed, including the declaration.
			var originalSolution = document.Project.Solution;
			var optionSet = originalSolution.Workspace.Options;
			var newSolution = await Renamer.RenameSymbolAsync(document.Project.Solution, typeSymbol, newName, optionSet, cancellationToken).ConfigureAwait(false);

			// Return the new solution with the now-uppercase type name.
			return newSolution;
		}
	}
}
