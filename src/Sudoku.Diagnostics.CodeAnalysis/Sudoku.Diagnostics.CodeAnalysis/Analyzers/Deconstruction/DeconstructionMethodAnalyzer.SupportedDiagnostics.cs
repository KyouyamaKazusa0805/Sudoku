using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	partial class DeconstructionMethodAnalyzer
	{
		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=4025282&amp;doc_id=633030">
		/// SS0501
		/// </a>
		/// diagnostic result (Deconstruction methods should contain at least 2 parameters).
		/// </summary>
		private static readonly DiagnosticDescriptor SS0501 = new(
			DiagnosticIds.SS0501, Titles.SS0501, Messages.SS0501, Categories.SS0501,
			DiagnosticSeverities.SS0501, true, helpLinkUri: HelpLinks.SS0501
		);

		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=4025301&amp;doc_id=633030">
		/// SS0502
		/// </a>
		/// diagnostic result (Deconstruction methods must be instance ones).
		/// </summary>
		private static readonly DiagnosticDescriptor SS0502 = new(
			DiagnosticIds.SS0502, Titles.SS0502, Messages.SS0502, Categories.SS0502,
			DiagnosticSeverities.SS0502, true, helpLinkUri: HelpLinks.SS0502
		);

		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=4025305&amp;doc_id=633030">
		/// SS0503
		/// </a>
		/// diagnostic result (Deconstruction methods must return void).
		/// </summary>
		private static readonly DiagnosticDescriptor SS0503 = new(
			DiagnosticIds.SS0503, Titles.SS0503, Messages.SS0503, Categories.SS0503,
			DiagnosticSeverities.SS0503, true, helpLinkUri: HelpLinks.SS0503
		);

		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=4025793&amp;doc_id=633030">
		/// SS0504
		/// </a>
		/// diagnostic result (Deconstruction methods must be public).
		/// </summary>
		private static readonly DiagnosticDescriptor SS0504 = new(
			DiagnosticIds.SS0504, Titles.SS0504, Messages.SS0504, Categories.SS0504,
			DiagnosticSeverities.SS0504, true, helpLinkUri: HelpLinks.SS0504
		);

		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=4025794&amp;doc_id=633030">
		/// SS0505
		/// </a>
		/// diagnostic result (All parameters in deconstruction methods should be out parameters).
		/// </summary>
		private static readonly DiagnosticDescriptor SS0505 = new(
			DiagnosticIds.SS0505, Titles.SS0505, Messages.SS0505, Categories.SS0505,
			DiagnosticSeverities.SS0505, true, helpLinkUri: HelpLinks.SS0505
		);

		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=4056812&amp;doc_id=633030">
		/// SS0506
		/// </a>
		/// diagnostic result (The assignment statement isn't a simple variable one, but an expression).
		/// </summary>
		private static readonly DiagnosticDescriptor SS0506 = new(
			DiagnosticIds.SS0506, Titles.SS0506, Messages.SS0506, Categories.SS0506,
			DiagnosticSeverities.SS0506, true, helpLinkUri: HelpLinks.SS0506
		);

		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=4056821&amp;doc_id=633030">
		/// SS0507
		/// </a>
		/// diagnostic result (The parameter should be corresponded to a certain instance field
		/// or instance property in this type).
		/// </summary>
		private static readonly DiagnosticDescriptor SS0507 = new(
			DiagnosticIds.SS0507, Titles.SS0507, Messages.SS0507, Categories.SS0507,
			DiagnosticSeverities.SS0507, true, helpLinkUri: HelpLinks.SS0507
		);

		/// <summary>
		/// Indicates the
		/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=4064196&amp;doc_id=633030">
		/// SS0508
		/// </a>
		/// diagnostic result (The type has already contained a deconstruction method that
		/// holds the same number of parameters of this method, so this method won't work).
		/// </summary>
		private static readonly DiagnosticDescriptor SS0508 = new(
			DiagnosticIds.SS0508, Titles.SS0508, Messages.SS0508, Categories.SS0508,
			DiagnosticSeverities.SS0508, true, helpLinkUri: HelpLinks.SS0508
		);


		/// <inheritdoc/>
		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
			SS0501, SS0502, SS0503, SS0504, SS0505, SS0506, SS0507, SS0508
		);
	}
}
