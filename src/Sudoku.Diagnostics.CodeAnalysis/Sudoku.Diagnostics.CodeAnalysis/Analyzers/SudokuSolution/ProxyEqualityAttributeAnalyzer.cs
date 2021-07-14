using System;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Diagnostics.Extensions;
using Sudoku.CodeGenerating;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	[CodeAnalyzer("SD0406", "SD0407", "SD0408", "SD0409", "SD0410")]
	public sealed partial class ProxyEqualityAttributeAnalyzer : DiagnosticAnalyzer
	{
		/// <summary>
		/// Indicates the type name of the attribute <c>ProxyEqualityAttribute</c>.
		/// </summary>
		private const string ProxyEqualityAttributeTypeName = "Sudoku.CodeGenerating.ProxyEqualityAttribute";


		/// <inheritdoc/>
		public override void Initialize(AnalysisContext context)
		{
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
			context.EnableConcurrentExecution();

			context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode, new[] { SyntaxKind.MethodDeclaration });
		}


		private static void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
		{
			var (semanticModel, compilation, originalNode, _, cancellationToken) = context;

			if (
				originalNode is not MethodDeclarationSyntax
				{
					AttributeLists: { Count: not 0 } attributes,
					Modifiers: var modifiers,
					ReturnType: var returnType,
					ParameterList: { Parameters: { Count: var count } parameters },
					Identifier: var identifier
				} node
			)
			{
				return;
			}

			if (semanticModel.GetDeclaredSymbol(originalNode) is not IMethodSymbol methodSymbol)
			{
				return;
			}

			var attributeSymbol = compilation.GetTypeByMetadataName(ProxyEqualityAttributeTypeName);
			if (
				methodSymbol.GetAttributes().All(
					attribute => !SymbolEqualityComparer.Default.Equals(
						attribute.AttributeClass,
						attributeSymbol
					)
				)
			)
			{
				return;
			}

			CheckSD0406(context, modifiers, identifier);
			CheckSD0407(context, returnType, semanticModel, compilation, cancellationToken);
			CheckSD0408(context, node, parameters, semanticModel, cancellationToken);
			CheckSD0409(context, node, attributeSymbol, semanticModel, cancellationToken);
			CheckSD0410(context, identifier, count);
		}

		private static void CheckSD0406(
			SyntaxNodeAnalysisContext context, SyntaxTokenList modifiers, SyntaxToken identifier)
		{
			if (modifiers.Any(static modifier => modifier.RawKind == (int)SyntaxKind.StaticKeyword))
			{
				return;
			}

			context.ReportDiagnostic(
				Diagnostic.Create(
					descriptor: SD0406,
					location: identifier.GetLocation(),
					messageArgs: null
				)
			);
		}

		private static void CheckSD0407(
			SyntaxNodeAnalysisContext context, TypeSyntax returnType, SemanticModel semanticModel,
			Compilation compilation, CancellationToken cancellationToken)
		{
			if (
				SymbolEqualityComparer.Default.Equals(
					compilation.GetSpecialType(SpecialType.System_Boolean),
					semanticModel.GetSymbolInfo(returnType, cancellationToken).Symbol
				)
			)
			{
				return;
			}

			context.ReportDiagnostic(
				Diagnostic.Create(
					descriptor: SD0407,
					location: returnType.GetLocation(),
					messageArgs: null
				)
			);
		}

		private static void CheckSD0408(
			SyntaxNodeAnalysisContext context, MethodDeclarationSyntax node,
			SeparatedSyntaxList<ParameterSyntax> parameters, SemanticModel semanticModel,
			CancellationToken cancellationToken)
		{
			if (node.Parent is not TypeDeclarationSyntax type)
			{
				return;
			}

			if (semanticModel.GetDeclaredSymbol(type, cancellationToken) is not { } typeSymbol)
			{
				return;
			}

			foreach (var parameter in parameters)
			{
				if (parameter.Type is not { } typeNodeToCheck)
				{
					// Parameter can't empty its type.

					continue;
				}

				var typeToCheck = semanticModel.GetSymbolInfo(typeNodeToCheck, cancellationToken).Symbol;
				if (SymbolEqualityComparer.Default.Equals(typeSymbol, typeToCheck))
				{
					continue;
				}

				context.ReportDiagnostic(
					Diagnostic.Create(
						descriptor: SD0408,
						location: parameter.GetLocation(),
						messageArgs: null
					)
				);
			}
		}

		private static void CheckSD0409(
			SyntaxNodeAnalysisContext context, MethodDeclarationSyntax node,
			INamedTypeSymbol? attributeSymbol, SemanticModel semanticModel,
			CancellationToken cancellationToken)
		{
			if (node.Parent is not TypeDeclarationSyntax type)
			{
				return;
			}

			if (semanticModel.GetDeclaredSymbol(type, cancellationToken) is not { } typeSymbol)
			{
				return;
			}

			if (
				typeSymbol.GetMembers().OfType<IMethodSymbol>().Count(
					method => method.GetAttributes().Any(
						attribute => SymbolEqualityComparer.Default.Equals(
							attribute.AttributeClass,
							attributeSymbol
						)
					)
				) == 1
			)
			{
				return;
			}

			context.ReportDiagnostic(
				Diagnostic.Create(
					descriptor: SD0409,
					location: type.Identifier.GetLocation(),
					messageArgs: null
				)
			);
		}

		private static void CheckSD0410(SyntaxNodeAnalysisContext context, SyntaxToken identifier, int count)
		{
			if (count == 2)
			{
				return;
			}

			context.ReportDiagnostic(
				Diagnostic.Create(
					descriptor: SD0410,
					location: identifier.GetLocation(),
					messageArgs: null
				)
			);
		}
	}
}
