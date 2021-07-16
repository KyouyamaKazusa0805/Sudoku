using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Diagnostics.Extensions;
using Sudoku.CodeGenerating;
using Sudoku.Diagnostics.CodeAnalysis.Extensions;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	[CodeAnalyzer("SS9007")]
	public sealed partial class AttributeSetterAnalyzer : DiagnosticAnalyzer
	{
		/// <inheritdoc/>
		public override void Initialize(AnalysisContext context)
		{
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
			context.EnableConcurrentExecution();

			context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode, new[] { SyntaxKind.ClassDeclaration });
		}


		private static void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
		{
			var (semanticModel, _, originalNode, _, cancellationToken) = context;

			if (
				originalNode is not ClassDeclarationSyntax
				{
					Members: { Count: not 0 } members,
					BaseList: { Types: { Count: not 0 } types }
				}
			)
			{
				return;
			}

			if (types[0] is not SimpleBaseTypeSyntax { Type: var type })
			{
				return;
			}

			if (
				semanticModel.GetSymbolInfo(type, cancellationToken) is not
				{
					Symbol: INamedTypeSymbol symbol
				}
			)
			{
				return;
			}

			bool isAttribute = false;
			for (
				var symbolBaseType = symbol.BaseType;
				symbolBaseType is not null;
				symbolBaseType = symbolBaseType.BaseType
			)
			{
				if (symbolBaseType.ToDisplayString(FormatOptions.TypeFormat) == typeof(Attribute).FullName)
				{
					isAttribute = true;
					break;
				}
			}
			if (!isAttribute)
			{
				return;
			}

			foreach (var member in members)
			{
				if (
					member is not PropertyDeclarationSyntax
					{
						AccessorList: { Accessors: { Count: not 0 } accessors }
					}
				)
				{
					continue;
				}

				foreach (var accessor in accessors)
				{
					if (accessor is not { Keyword: { RawKind: (int)SyntaxKind.SetKeyword } keyword })
					{
						continue;
					}

					context.ReportDiagnostic(
						Diagnostic.Create(
							descriptor: SS9007,
							location: keyword.GetLocation(),
							messageArgs: null,
							additionalLocations: new[] { accessor.GetLocation() }
						)
					);
				}
			}
		}
	}
}
