global using System;
global using System.Collections;
global using System.Collections.Generic;
global using System.Collections.Immutable;
global using System.ComponentModel;
global using System.Diagnostics.CodeAnalysis;
global using System.IO;
global using System.Linq;
global using System.Runtime.CompilerServices;
global using System.Text;
global using System.Text.RegularExpressions;
global using Microsoft.CodeAnalysis;
global using Microsoft.CodeAnalysis.CSharp;
global using Microsoft.CodeAnalysis.CSharp.Syntax;
global using Microsoft.CodeAnalysis.Operations;
global using Microsoft.CodeAnalysis.Text;
global using Sudoku.CodeGenerating.Collections;
global using static Microsoft.CodeAnalysis.SymbolDisplayGenericsOptions;
global using static Microsoft.CodeAnalysis.SymbolDisplayGlobalNamespaceStyle;
global using static Microsoft.CodeAnalysis.SymbolDisplayMiscellaneousOptions;
global using static Microsoft.CodeAnalysis.SymbolDisplayTypeQualificationStyle;
global using static Sudoku.CodeGenerating.Constants;

#if !NETSTANDARD2_1_OR_GREATER
[module: SuppressMessage("Style", "IDE0057:Use range operator", Justification = "<Pending>")]
#endif
