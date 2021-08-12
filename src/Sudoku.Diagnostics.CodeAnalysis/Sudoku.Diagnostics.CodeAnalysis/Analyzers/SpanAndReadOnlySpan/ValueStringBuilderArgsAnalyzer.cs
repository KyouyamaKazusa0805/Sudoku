using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Diagnostics.Extensions;
using Microsoft.CodeAnalysis.Operations;
using Sudoku.CodeGenerating;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers;

[CodeAnalyzer("SD0312", "SD0313")]
public sealed partial class ValueStringBuilderArgsAnalyzer : DiagnosticAnalyzer
{
	/// <inheritdoc/>
	public override void Initialize(AnalysisContext context)
	{
		context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
		context.EnableConcurrentExecution();

		context.RegisterSyntaxNodeAction(
			static context =>
			{
				CheckSD0312(context);
				CheckSD0313(context);
			},
			new[] { SyntaxKind.ObjectCreationExpression, SyntaxKind.ImplicitObjectCreationExpression }
		);
	}


	private static void CheckSD0312(SyntaxNodeAnalysisContext context)
	{
		var (semanticModel, compilation, originalNode, _, cancellationToken) = context;

		var charSymbol = compilation.GetSpecialType(SpecialType.System_Char);
		var charArraySymbol = compilation.CreateArrayTypeSymbol(charSymbol);
		var vsbSymbol = compilation.GetTypeByMetadataName("System.Text.ValueStringBuilder");
		switch (originalNode)
		{
			case ObjectCreationExpressionSyntax
			{
				Type: var type,
				ArgumentList: { Arguments: { Count: 1 } args }
			}
			when semanticModel.GetSymbolInfo(type, cancellationToken) is { Symbol: var possibleVsbSymbol }
			&& SymbolEqualityComparer.Default.Equals(possibleVsbSymbol, vsbSymbol):
			{
				var arg = args[0];
				switch (arg.Expression)
				{
					case ArrayCreationExpressionSyntax
					{
						Type: { ElementType: PredefinedTypeSyntax { Keyword: { ValueText: "char" } } }
					}:
					case ImplicitArrayCreationExpressionSyntax implicitArrayCreation
					when semanticModel.GetOperation(implicitArrayCreation) is IArrayCreationOperation
					{
						Type: var possibleCharArrayTypeSymbol
					} && SymbolEqualityComparer.Default.Equals(charArraySymbol, possibleCharArrayTypeSymbol):
					{
						context.ReportDiagnostic(
							Diagnostic.Create(
								descriptor: SD0312,
								location: arg.GetLocation(),
								messageArgs: null,
								additionalLocations: new[] { arg.Expression.GetLocation() }
							)
						);

						break;
					}
				}

				break;
			}
			case ImplicitObjectCreationExpressionSyntax { ArgumentList: { Arguments: { Count: 1 } args } }
			when semanticModel.GetOperation(originalNode, cancellationToken) is IObjectCreationOperation
			{
				Type: var possibleCharSymbol
			} && SymbolEqualityComparer.Default.Equals(possibleCharSymbol, vsbSymbol):
			{
				var arg = args[0];
				switch (arg.Expression)
				{
					case ArrayCreationExpressionSyntax
					{
						Type: { ElementType: PredefinedTypeSyntax { Keyword: { ValueText: "char" } } }
					}:
					case ImplicitArrayCreationExpressionSyntax implicitArrayCreation
					when semanticModel.GetOperation(implicitArrayCreation) is IArrayCreationOperation
					{
						Type: var possibleCharArrayTypeSymbol
					} && SymbolEqualityComparer.Default.Equals(charArraySymbol, possibleCharArrayTypeSymbol):
					{
						context.ReportDiagnostic(
							Diagnostic.Create(
								descriptor: SD0312,
								location: arg.GetLocation(),
								messageArgs: null,
								additionalLocations: new[] { arg.Expression.GetLocation() }
							)
						);

						break;
					}
				}

				break;
			}
		}
	}

	private static void CheckSD0313(SyntaxNodeAnalysisContext context)
	{
		var (semanticModel, compilation, originalNode, _, cancellationToken) = context;

		var charSymbol = compilation.GetSpecialType(SpecialType.System_Char);
		var charArraySymbol = compilation.CreateArrayTypeSymbol(charSymbol);
		var vsbSymbol = compilation.GetTypeByMetadataName("System.Text.ValueStringBuilder");
		switch (originalNode)
		{
			case ObjectCreationExpressionSyntax
			{
				Type: var type,
				ArgumentList: { Arguments: { Count: 1 } args }
			}
			when semanticModel.GetSymbolInfo(type, cancellationToken) is { Symbol: var possibleVsbSymbol }
			&& SymbolEqualityComparer.Default.Equals(possibleVsbSymbol, vsbSymbol):
			{
				var arg = args[0];
				switch (arg.Expression)
				{
					case StackAllocArrayCreationExpressionSyntax
					{
						Type: ArrayTypeSyntax
						{
							ElementType: PredefinedTypeSyntax { Keyword: { ValueText: "char" } },
							RankSpecifiers: { Count: 1 } rankSpecifiers
						}
					}
					when rankSpecifiers[0] is { Sizes: { Count: 1 } sizes }
					&& sizes[0] is LiteralExpressionSyntax
					{
						RawKind: (int)SyntaxKind.NumericLiteralExpression
					} size
					&& semanticModel.GetConstantValue(size, cancellationToken) is
					{
						HasValue: true,
						Value: int value and >= 300
					}:
					{
						context.ReportDiagnostic(
							Diagnostic.Create(
								descriptor: SD0313,
								location: arg.GetLocation(),
								properties: ImmutableDictionary.CreateRange(
									new KeyValuePair<string, string?>[] { new("Length", value.ToString()) }
								),
								messageArgs: null,
								additionalLocations: new[] { arg.Expression.GetLocation() }
							)
						);

						break;
					}
					case ImplicitStackAllocArrayCreationExpressionSyntax
					{
						Initializer: { Expressions: { Count: var value and >= 300 } }
					} implicitArrayCreation
					when semanticModel.GetOperation(implicitArrayCreation) is IArrayCreationOperation
					{
						Type: var possibleCharArrayTypeSymbol
					} && SymbolEqualityComparer.Default.Equals(charArraySymbol, possibleCharArrayTypeSymbol):
					{
						context.ReportDiagnostic(
							Diagnostic.Create(
								descriptor: SD0313,
								location: arg.GetLocation(),
								properties: ImmutableDictionary.CreateRange(
									new KeyValuePair<string, string?>[] { new("Length", value.ToString()) }
								),
								messageArgs: null,
								additionalLocations: new[] { arg.Expression.GetLocation() }
							)
						);

						break;
					}
				}

				break;
			}
			case ImplicitObjectCreationExpressionSyntax { ArgumentList: { Arguments: { Count: 1 } args } }
			when semanticModel.GetOperation(originalNode, cancellationToken) is IObjectCreationOperation
			{
				Type: var possibleCharSymbol
			} && SymbolEqualityComparer.Default.Equals(possibleCharSymbol, vsbSymbol):
			{
				var arg = args[0];
				switch (arg.Expression)
				{
					case StackAllocArrayCreationExpressionSyntax
					{
						Type: ArrayTypeSyntax
						{
							ElementType: PredefinedTypeSyntax { Keyword: { ValueText: "char" } },
							RankSpecifiers: { Count: 1 } rankSpecifiers
						}
					}
					when rankSpecifiers[0] is { Sizes: { Count: 1 } sizes }
					&& sizes[0] is LiteralExpressionSyntax
					{
						RawKind: (int)SyntaxKind.NumericLiteralExpression
					} size
					&& semanticModel.GetConstantValue(size, cancellationToken) is
					{
						HasValue: true,
						Value: int value and >= 300
					}:
					{
						context.ReportDiagnostic(
							Diagnostic.Create(
								descriptor: SD0313,
								location: arg.GetLocation(),
								properties: ImmutableDictionary.CreateRange(
									new KeyValuePair<string, string?>[] { new("Length", value.ToString()) }
								),
								messageArgs: null,
								additionalLocations: new[] { arg.Expression.GetLocation() }
							)
						);

						break;
					}
					case ImplicitStackAllocArrayCreationExpressionSyntax
					{
						Initializer: { Expressions: { Count: var value and >= 300 } }
					} implicitArrayCreation
					when semanticModel.GetOperation(implicitArrayCreation) is IArrayCreationOperation
					{
						Type: var possibleCharArrayTypeSymbol
					} && SymbolEqualityComparer.Default.Equals(charArraySymbol, possibleCharArrayTypeSymbol):
					{
						context.ReportDiagnostic(
							Diagnostic.Create(
								descriptor: SD0313,
								location: arg.GetLocation(),
								properties: ImmutableDictionary.CreateRange(
									new KeyValuePair<string, string?>[] { new("Length", value.ToString()) }
								),
								messageArgs: null,
								additionalLocations: new[] { arg.Expression.GetLocation() }
							)
						);

						break;
					}
				}

				break;
			}
		}
	}
}
