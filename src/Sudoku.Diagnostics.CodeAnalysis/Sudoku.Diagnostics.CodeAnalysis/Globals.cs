global using System;
global using System.Collections.Generic;
global using System.Collections.Immutable;
global using System.Linq;
global using System.Runtime.CompilerServices;
global using System.Runtime.InteropServices;
global using System.Text;
global using System.Text.RegularExpressions;
global using System.Threading;
global using System.Threading.Tasks;
global using Microsoft.CodeAnalysis;
global using Microsoft.CodeAnalysis.CodeActions;
global using Microsoft.CodeAnalysis.CodeFixes;
global using Microsoft.CodeAnalysis.CSharp;
global using Microsoft.CodeAnalysis.CSharp.Syntax;
global using Microsoft.CodeAnalysis.Diagnostics;
global using Microsoft.CodeAnalysis.Editing;
global using Microsoft.CodeAnalysis.Operations;
global using Microsoft.CodeAnalysis.Text;
global using Sudoku.CodeGenerating;
global using Sudoku.Diagnostics.CodeAnalysis;
global using ERef = Microsoft.CodeAnalysis.Operations.IEventReferenceOperation;
global using FRef = Microsoft.CodeAnalysis.Operations.IFieldReferenceOperation;
global using LRef = Microsoft.CodeAnalysis.Operations.ILocalReferenceOperation;
global using MRef = Microsoft.CodeAnalysis.Operations.IMethodReferenceOperation;
global using PRef = Microsoft.CodeAnalysis.Operations.IPropertyReferenceOperation;

[assembly: InternalsVisibleTo("Sudoku.Diagnostics.CodeAnalysis.CodeFixes")]

[assembly: AutoDeconstructExtension(typeof(SyntaxNodeAnalysisContext), nameof(SyntaxNodeAnalysisContext.SemanticModel), nameof(SyntaxNodeAnalysisContext.Compilation))]
[assembly: AutoDeconstructExtension(typeof(SyntaxNodeAnalysisContext), nameof(SyntaxNodeAnalysisContext.SemanticModel), nameof(SyntaxNodeAnalysisContext.Compilation), nameof(SyntaxNodeAnalysisContext.Node))]
[assembly: AutoDeconstructExtension(typeof(SyntaxNodeAnalysisContext), nameof(SyntaxNodeAnalysisContext.SemanticModel), nameof(SyntaxNodeAnalysisContext.Compilation), nameof(SyntaxNodeAnalysisContext.Node), nameof(SyntaxNodeAnalysisContext.ContainingSymbol))]
[assembly: AutoDeconstructExtension(typeof(SyntaxNodeAnalysisContext), nameof(SyntaxNodeAnalysisContext.SemanticModel), nameof(SyntaxNodeAnalysisContext.Compilation), nameof(SyntaxNodeAnalysisContext.Node), nameof(SyntaxNodeAnalysisContext.ContainingSymbol), nameof(SyntaxNodeAnalysisContext.CancellationToken))]
[assembly: AutoDeconstructExtension(typeof(OperationAnalysisContext), nameof(OperationAnalysisContext.Compilation), nameof(OperationAnalysisContext.Operation))]
[assembly: AutoDeconstructExtension(typeof(SymbolAnalysisContext), nameof(SymbolAnalysisContext.Symbol), nameof(SymbolAnalysisContext.Compilation), nameof(SymbolAnalysisContext.CancellationToken))]
[assembly: AutoDeconstructExtension(typeof(TextSpan), nameof(TextSpan.Start), nameof(TextSpan.End))]
[assembly: AutoDeconstructExtension(typeof(TextSpan), nameof(TextSpan.Start), nameof(TextSpan.End), nameof(TextSpan.Length))]
[assembly: AutoDeconstructExtension(typeof(Diagnostic), nameof(Diagnostic.Location), nameof(Diagnostic.Descriptor))]
[assembly: AutoDeconstructExtension(typeof(Diagnostic), nameof(Diagnostic.Location), nameof(Diagnostic.Descriptor), nameof(Diagnostic.Severity))]
[assembly: AutoDeconstructExtension(typeof(Diagnostic), nameof(Diagnostic.Location), nameof(Diagnostic.Descriptor), nameof(Diagnostic.Severity), nameof(Diagnostic.DefaultSeverity))]
[assembly: AutoDeconstructExtension(typeof(DiagnosticDescriptor), nameof(DiagnosticDescriptor.Id), nameof(DiagnosticDescriptor.Title), nameof(DiagnosticDescriptor.Description), nameof(DiagnosticDescriptor.HelpLinkUri), nameof(DiagnosticDescriptor.MessageFormat), nameof(DiagnosticDescriptor.Category), nameof(DiagnosticDescriptor.DefaultSeverity), nameof(DiagnosticDescriptor.IsEnabledByDefault), nameof(DiagnosticDescriptor.CustomTags))]
[assembly: AutoDeconstructExtension(typeof(Location), nameof(Location.SourceTree), nameof(Location.SourceSpan))]
[assembly: AutoDeconstructExtension(typeof(CompilationStartAnalysisContext), nameof(CompilationStartAnalysisContext.Compilation), nameof(CompilationStartAnalysisContext.Options))]
[assembly: AutoDeconstructExtension(typeof(IParameterSymbol), nameof(IParameterSymbol.Type), nameof(IParameterSymbol.Name))]