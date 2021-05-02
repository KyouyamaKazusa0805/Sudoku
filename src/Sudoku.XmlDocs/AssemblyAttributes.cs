using System;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Sudoku.CodeGen;

[assembly: InternalsVisibleTo("Sudoku.XmlDocs.Console")]

[assembly: CLSCompliant(false)]

[assembly: AutoDeconstructExtension(typeof(TextSpan), nameof(TextSpan.Start), nameof(TextSpan.End), Namespace = "Sudoku.XmlDocs.Extensions")]
[assembly: AutoDeconstructExtension(typeof(TextSpan), nameof(TextSpan.Start), nameof(TextSpan.End), nameof(TextSpan.Length), Namespace = "Sudoku.XmlDocs.Extensions")]
[assembly: AutoDeconstructExtension(typeof(ParameterSyntax), nameof(ParameterSyntax.Type), nameof(ParameterSyntax.Identifier), Namespace = "Sudoku.XmlDocs.Extensions")]