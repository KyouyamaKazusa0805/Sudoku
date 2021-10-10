global using System;
global using System.Collections.Generic;
global using System.Collections.Immutable;
global using System.Linq;
global using System.Threading;
global using Microsoft.CodeAnalysis;
global using Microsoft.CodeAnalysis.CSharp;
global using Microsoft.CodeAnalysis.CSharp.Syntax;
global using Microsoft.CodeAnalysis.Operations;
global using Sudoku.CodeGenerating;
global using Sudoku.Diagnostics.CodeAnalysis.Extensions;
global using static Sudoku.Diagnostics.CodeAnalysis.Constants;

#if SUPPORT_CODE_FIXER
global using System.Collections.Immutable;
#endif

[assembly: AutoDeconstructExtension<GeneratorSyntaxContext>(nameof(GeneratorSyntaxContext.Node), nameof(GeneratorSyntaxContext.SemanticModel))]
[assembly: AutoDeconstructExtensionLambda<GeneratorSyntaxContext, GeneratorSyntaxContext_Dap>(nameof(GeneratorSyntaxContext.Node), nameof(GeneratorSyntaxContext.SemanticModel), $"{nameof(GeneratorSyntaxContext_Dap)}.{nameof(GeneratorSyntaxContext_Dap.Operation)}")]