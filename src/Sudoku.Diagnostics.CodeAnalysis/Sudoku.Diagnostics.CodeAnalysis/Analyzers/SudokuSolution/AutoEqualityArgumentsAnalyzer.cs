using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Diagnostics.Extensions;
using Sudoku.CodeGenerating;
using Sudoku.CodeGenerating.Extensions;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	[CodeAnalyzer("SD0405")]
	public sealed partial class AutoEqualityArgumentsAnalyzer : DiagnosticAnalyzer
	{
		/// <summary>
		/// Indicates the full type name of type <c>AutoEqualityAttribute</c>.
		/// </summary>
		private const string AutoEqualityAttributeTypeName = "Sudoku.CodeGenerating.AutoEqualityAttribute";


		/// <inheritdoc/>
		public override void Initialize(AnalysisContext context)
		{
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
			context.EnableConcurrentExecution();

			context.RegisterSyntaxNodeAction(
				AnalyzeSyntaxNode,
				new[]
				{
					SyntaxKind.ClassDeclaration,
					SyntaxKind.StructDeclaration,
					SyntaxKind.RecordDeclaration
				}
			);
		}


		private static void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
		{
			var (semanticModel, compilation, originalNode, _, cancellationToken) = context;

			if (
				originalNode is not TypeDeclarationSyntax
				{
					AttributeLists: { Count: not 0 } attributeLists,
					Members: { Count: not 0 } members
				}
			)
			{
				return;
			}

			var attributesData = semanticModel
				.GetDeclaredSymbol(originalNode, cancellationToken)!
				.GetAttributes();

			SyntaxNode? attribute = null;
			foreach (var attributeData in attributesData)
			{
				if (
					attributeData.AttributeClass is { } s
					&& SymbolEqualityComparer.Default.Equals(
						s,
						compilation.GetTypeByMetadataName(AutoEqualityAttributeTypeName)
					)
				)
				{
					foreach (var attributeList in attributeLists)
					{
						foreach (var currentAttribute in attributeList.Attributes)
						{
							if (
								currentAttribute is
								{
									Name: IdentifierNameSyntax
									{
										Identifier: { ValueText: "AutoEqualityAttribute" or "AutoEquality" }
									}
								}
							)
							{
								attribute = currentAttribute;

								goto DetermineSyntaxNode;
							}
						}
					}
				}
			}

		DetermineSyntaxNode:
			if (
				attribute is not AttributeSyntax
				{
					ArgumentList: { Arguments: { Count: not 0 } arguments }
				}
			)
			{
				return;
			}

			// Get built-in type symbols.
			var builtInTypes = GetBuiltInTypesWithoutOperatorEquality(compilation);

			// Iterate on each argument.
			foreach (var argument in arguments)
			{
				// Check the type symbol of the argument.
				if (
					argument is not
					{
						Expression: InvocationExpressionSyntax
						{
							Expression: IdentifierNameSyntax { Identifier: { ValueText: "nameof" } },
							ArgumentList: { Arguments: { Count: 1 } nameofArgs }
						} expression
					}
				)
				{
					continue;
				}

				var nameofArgExpr = nameofArgs[0].Expression;

				var nameofArgTypeSymbol = semanticModel.GetOperation(nameofArgExpr, cancellationToken)?.Type;
				if (nameofArgTypeSymbol is not INamedTypeSymbol argTypeSymbol)
				{
					continue;
				}

				// Check whether the type contains the operator '=='.
				bool containsOperatorEquality = argTypeSymbol.MemberNames.Contains("op_Equality");
				if (
					containsOperatorEquality
					|| !containsOperatorEquality && builtInTypes.Any(
						builtInType => SymbolEqualityComparer.Default.Equals(
							builtInType,
							argTypeSymbol
						)
					)
				)
				{
					continue;
				}

				// If not, report it.
				context.ReportDiagnostic(
					Diagnostic.Create(
						descriptor: SD0405,
						location: nameofArgExpr.GetLocation(),
						messageArgs: null
					)
				);
			}
		}

		/// <summary>
		/// Get all built in types that don't contain <c>operator ==</c>.
		/// </summary>
		/// <param name="compilation">The compilation.</param>
		/// <returns>The symbols.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static INamedTypeSymbol[] GetBuiltInTypesWithoutOperatorEquality(Compilation compilation) => new[]
		{
			compilation.GetSpecialType(SpecialType.System_Object),
			compilation.GetSpecialType(SpecialType.System_Boolean),
			compilation.GetSpecialType(SpecialType.System_SByte),
			compilation.GetSpecialType(SpecialType.System_Byte),
			compilation.GetSpecialType(SpecialType.System_Char),
			compilation.GetSpecialType(SpecialType.System_Single),
			compilation.GetSpecialType(SpecialType.System_Double),
			compilation.GetSpecialType(SpecialType.System_Int16),
			compilation.GetSpecialType(SpecialType.System_Int32),
			compilation.GetSpecialType(SpecialType.System_Int64),
			compilation.GetSpecialType(SpecialType.System_UInt16),
			compilation.GetSpecialType(SpecialType.System_UInt32),
			compilation.GetSpecialType(SpecialType.System_UInt64)
		};
	}
}
