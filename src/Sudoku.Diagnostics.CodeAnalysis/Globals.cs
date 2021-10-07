global using System.Collections.Generic;
global using System.Threading;
global using Microsoft.CodeAnalysis;
global using Microsoft.CodeAnalysis.CSharp;
global using Microsoft.CodeAnalysis.CSharp.Syntax;
global using Microsoft.CodeAnalysis.Operations;
global using Sudoku.CodeGenerating;

#if SUPPORT_CODE_FIXER
global using System.Collections.Immutable;
#endif

#pragma warning disable IDE0005
global using Microsoft.CodeAnalysis.Diagnostics;
#pragma warning restore IDE0005