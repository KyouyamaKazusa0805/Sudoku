global using System;
global using System.Collections;
global using System.Collections.Generic;
global using System.Collections.Immutable;
global using System.Linq;
global using System.Runtime.CompilerServices;
global using System.Text;
global using System.Threading;
global using Microsoft.CodeAnalysis;
global using Microsoft.CodeAnalysis.CSharp;
global using Microsoft.CodeAnalysis.CSharp.Syntax;
global using Microsoft.CodeAnalysis.Text;
global using Sudoku.Diagnostics.CodeGen.Reflection;
global using static Sudoku.Diagnostics.CodeGen.Constants;

global using AutoFormattableInfo = System.ValueTuple<
	Microsoft.CodeAnalysis.INamedTypeSymbol,
	Microsoft.CodeAnalysis.AttributeData,
	Microsoft.CodeAnalysis.SymbolOutputInfo
>;