global using System;
global using System.Collections;
global using System.Collections.Generic;
global using System.Collections.Immutable;
global using System.Diagnostics.CodeAnalysis;
global using System.Linq;
global using System.Runtime.CompilerServices;
global using System.Text;
global using System.Threading;
global using Microsoft.CodeAnalysis;
global using Microsoft.CodeAnalysis.CSharp;
global using Microsoft.CodeAnalysis.CSharp.Syntax;
global using Microsoft.CodeAnalysis.Text;
global using static Sudoku.Diagnostics.CodeGen.Constants;
global using AutoGetEnumeratorInfo = System.ValueTuple<
	Microsoft.CodeAnalysis.INamedTypeSymbol,
	Microsoft.CodeAnalysis.AttributeData,
	Microsoft.CodeAnalysis.SymbolOutputInfo
>;
global using AutoLambdaedDeconstructInfo = System.ValueTuple<
	Microsoft.CodeAnalysis.INamedTypeSymbol,
	System.Collections.Generic.IEnumerable<Microsoft.CodeAnalysis.AttributeData>,
	Microsoft.CodeAnalysis.SymbolOutputInfo,
	Sudoku.Diagnostics.CodeGen.MemberDetail[]
>;
