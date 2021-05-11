#if false

using System;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeFixes;

namespace Sudoku.Diagnostics.CodeAnalysis
{
	/// <summary>
	/// Indicates the code fixer.
	/// </summary>
	//[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SudokuDiagnosticsCodeAnalysisTempCodeFixProvider)), Shared]
	public sealed class Sudoku001CodeFixProvider : CodeFixProvider
	{
		/// <inheritdoc/>
		public override ImmutableArray<string> FixableDiagnosticIds =>
			ImmutableArray.Create(DiagnosticIds.Sudoku001);

		/// <inheritdoc/>
		public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;


		/// <inheritdoc/>
		public override Task RegisterCodeFixesAsync(CodeFixContext context)
		{
			throw new NotImplementedException();
			//var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
			//
			//var diagnostic = context.Diagnostics.First();
			//var diagnosticSpan = diagnostic.Location.SourceSpan;
			//
			//// Find the type declaration identified by the diagnostic.
			//var declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<TypeDeclarationSyntax>().First();
			//
			//// Register a code action that will invoke the fix.
			//context.RegisterCodeFix(
			//	CodeAction.Create(
			//		title: CodeFixResources.CodeFixTitle,
			//		createChangedSolution: c => MakeUppercaseAsync(context.Document, declaration, c),
			//		equivalenceKey: nameof(CodeFixResources.CodeFixTitle)),
			//	diagnostic);
		}

		//private async Task<Solution> MakeUppercaseAsync(
		//	Document document, TypeDeclarationSyntax typeDecl, CancellationToken cancellationToken)
		//{
		//	// Compute new uppercase name.
		//	var identifierToken = typeDecl.Identifier;
		//	string newName = identifierToken.Text.ToUpperInvariant();
		//
		//	// Get the symbol representing the type to be renamed.
		//	var semanticModel = await document.GetSemanticModelAsync(cancellationToken);
		//	var typeSymbol = semanticModel.GetDeclaredSymbol(typeDecl, cancellationToken);
		//
		//	// Produce a new solution that has all references to that type renamed, including the declaration.
		//	var originalSolution = document.Project.Solution;
		//	var optionSet = originalSolution.Workspace.Options;
		//	var newSolution = await Renamer.RenameSymbolAsync(document.Project.Solution, typeSymbol, newName, optionSet, cancellationToken).ConfigureAwait(false);
		//
		//	// Return the new solution with the now-uppercase type name.
		//	return newSolution;
		//}
	}
}

#endif