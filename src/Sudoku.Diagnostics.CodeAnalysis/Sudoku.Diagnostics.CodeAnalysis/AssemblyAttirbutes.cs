using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis.Diagnostics;
using Sudoku.CodeGen;

[assembly: InternalsVisibleTo("Sudoku.Diagnostics.CodeAnalysis.CodeFixes")]

[assembly: AutoDeconstructExtension(typeof(SyntaxNodeAnalysisContext), nameof(SyntaxNodeAnalysisContext.SemanticModel), nameof(SyntaxNodeAnalysisContext.Compilation))]
[assembly: AutoDeconstructExtension(typeof(SyntaxNodeAnalysisContext), nameof(SyntaxNodeAnalysisContext.SemanticModel), nameof(SyntaxNodeAnalysisContext.Compilation), nameof(SyntaxNodeAnalysisContext.Node))]
[assembly: AutoDeconstructExtension(typeof(SyntaxNodeAnalysisContext), nameof(SyntaxNodeAnalysisContext.SemanticModel), nameof(SyntaxNodeAnalysisContext.Compilation), nameof(SyntaxNodeAnalysisContext.Node), nameof(SyntaxNodeAnalysisContext.ContainingSymbol))]