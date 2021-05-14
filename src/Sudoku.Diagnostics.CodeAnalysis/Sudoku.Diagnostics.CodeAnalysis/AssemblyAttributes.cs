using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Sudoku.CodeGen;

[assembly: InternalsVisibleTo("Sudoku.Diagnostics.CodeAnalysis.CodeFixes")]

[assembly: AutoDeconstructExtension(typeof(SyntaxNodeAnalysisContext), nameof(SyntaxNodeAnalysisContext.SemanticModel), nameof(SyntaxNodeAnalysisContext.Compilation))]
[assembly: AutoDeconstructExtension(typeof(SyntaxNodeAnalysisContext), nameof(SyntaxNodeAnalysisContext.SemanticModel), nameof(SyntaxNodeAnalysisContext.Compilation), nameof(SyntaxNodeAnalysisContext.Node))]
[assembly: AutoDeconstructExtension(typeof(SyntaxNodeAnalysisContext), nameof(SyntaxNodeAnalysisContext.SemanticModel), nameof(SyntaxNodeAnalysisContext.Compilation), nameof(SyntaxNodeAnalysisContext.Node), nameof(SyntaxNodeAnalysisContext.ContainingSymbol))]
[assembly: AutoDeconstructExtension(typeof(TextSpan), nameof(TextSpan.Start), nameof(TextSpan.End))]
[assembly: AutoDeconstructExtension(typeof(TextSpan), nameof(TextSpan.Start), nameof(TextSpan.End), nameof(TextSpan.Length))]
[assembly: AutoDeconstructExtension(typeof(Diagnostic), nameof(Diagnostic.Location), nameof(Diagnostic.Descriptor))]
[assembly: AutoDeconstructExtension(typeof(Diagnostic), nameof(Diagnostic.Location), nameof(Diagnostic.Descriptor), nameof(Diagnostic.Severity))]
[assembly: AutoDeconstructExtension(typeof(Diagnostic), nameof(Diagnostic.Location), nameof(Diagnostic.Descriptor), nameof(Diagnostic.Severity), nameof(Diagnostic.DefaultSeverity))]