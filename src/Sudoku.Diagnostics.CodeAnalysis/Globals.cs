global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Threading;
global using Microsoft.CodeAnalysis;
global using Microsoft.CodeAnalysis.CSharp;
global using Microsoft.CodeAnalysis.CSharp.Syntax;
global using Microsoft.CodeAnalysis.Operations;
global using Sudoku.CodeGenerating;
global using static Sudoku.Diagnostics.CodeAnalysis.Constants;

#if SUPPORT_CODE_FIXER
global using System.Collections.Immutable;
#endif