using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Diagnostics.Extensions;
using Sudoku.CodeGenerating;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	[CodeAnalyzer("SD0401", "SD0402")]
	public sealed partial class AutoAttributePropertiesAnalyzer : DiagnosticAnalyzer
	{
		/// <summary>
		/// Indicates the full type names.
		/// </summary>
		private const string
			AutoDeconstructAttributeTypeName = "AutoDeconstructAttribute",
			AutoDeconstructAttributeTypeShortName = "AutoDeconstruct",
			AutoDeconstructExtensionAttributeTypeName = "AutoDeconstructExtensionAttribute",
			AutoDeconstructExtensionAttributeTypeShortName = "AutoDeconstructExtension",
			AutoEqualityAttributeTypeName = "AutoEqualityAttribute",
			AutoEqualityAttributeTypeShortName = "AutoEquality",
			AutoGetEnumeratorAttributeTypeName = "AutoGetEnumeratorAttribute",
			AutoGetEnumeratorAttributeTypeShortName = "AutoGetEnumerator",
			AutoHashCodeAttributeTypeName = "AutoHashCodeAttribute",
			AutoHashCodeAttributeTypeShortName = "AutoHashCode";


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
			var (semanticModel, _, node) = context;
			if (node is not MemberDeclarationSyntax { AttributeLists: { Count: not 0 } attributeLists })
			{
				return;
			}

			foreach (var attributeList in attributeLists)
			{
				if (attributeList.Attributes is not { Count: not 0 } attributes)
				{
					continue;
				}

				foreach (var attribute in attributes)
				{
					if (
						attribute is not
						{
							Parent: AttributeListSyntax,
							Name: IdentifierNameSyntax { Identifier: { ValueText: var text } } identifierName,
							ArgumentList: { Arguments: var arguments }
						}
					)
					{
						continue;
					}

					switch (text)
					{
						case AutoDeconstructAttributeTypeName or AutoDeconstructAttributeTypeShortName:
						{
							if (arguments.Count < 2)
							{
								context.ReportDiagnostic(
									Diagnostic.Create(
										descriptor: SD0402,
										location: node.GetLocation(),
										messageArgs: new object[]
										{
											$"Sudoku.CodeGenerating.{AutoDeconstructAttributeTypeName}",
											2,
											" at least"
										}
									)
								);

								continue;
							}

							goto ArgumentsChecking_Case1;
						}
						case var name and (
							AutoEqualityAttributeTypeName or AutoEqualityAttributeTypeShortName
							or AutoHashCodeAttributeTypeName or AutoHashCodeAttributeTypeShortName
						):
						{
							if (arguments.Count < 1)
							{
								context.ReportDiagnostic(
									Diagnostic.Create(
										descriptor: SD0402,
										location: node.GetLocation(),
										messageArgs: new object[]
										{
											$"Sudoku.CodeGenerating.{name}",
											1,
											" at least"
										}
									)
								);

								continue;
							}

							goto ArgumentsChecking_Case1;
						}
						case AutoDeconstructExtensionAttributeTypeName
						or AutoDeconstructExtensionAttributeTypeShortName:
						{
							if (arguments.Count < 3)
							{
								context.ReportDiagnostic(
									Diagnostic.Create(
										descriptor: SD0402,
										location: node.GetLocation(),
										messageArgs: new object[]
										{
											$"Sudoku.CodeGenerating.{AutoDeconstructExtensionAttributeTypeName}",
											3,
											" at least"
										}
									)
								);

								continue;
							}

							goto ArgumentsChecking_Case2;
						}
						case AutoGetEnumeratorAttributeTypeName or AutoGetEnumeratorAttributeTypeShortName:
						{
							if (
								arguments[0] is { Expression: var expr } argument
								&& semanticModel.GetOperation(expr) is
								{
									Kind: not OperationKind.NameOf,
									ConstantValue: { HasValue: true, Value: string exprValue }
								}
							)
							{
								context.ReportDiagnostic(
									Diagnostic.Create(
										descriptor: SD0401,
										location: argument.GetLocation(),
										messageArgs: null,
										properties: ImmutableDictionary.CreateRange(
											new KeyValuePair<string, string?>[]
											{
												new("ExpressionValue", exprValue)
											}
										)
									)
								);
							}

							break;
						}

					ArgumentsChecking_Case1:
						{
							foreach (var argument in arguments)
							{
								if (
									semanticModel.GetOperation(argument.Expression) is
									{
										Kind: not OperationKind.NameOf,
										ConstantValue: { HasValue: true, Value: string exprValue }
									}
								)
								{
									context.ReportDiagnostic(
										Diagnostic.Create(
											descriptor: SD0401,
											location: argument.GetLocation(),
											messageArgs: null,
											properties: ImmutableDictionary.CreateRange(
												new KeyValuePair<string, string?>[]
												{
													new("ExpressionValue", exprValue)
												}
											)
										)
									);
								}
							}

							break;
						}

					ArgumentsChecking_Case2:
						{
							for (int i = 1; i < arguments.Count; i++)
							{
								var argument = arguments[i];
								if (
									semanticModel.GetOperation(argument.Expression) is
									{
										Kind: not OperationKind.NameOf,
										ConstantValue: { HasValue: true, Value: string exprValue }
									}
								)
								{
									context.ReportDiagnostic(
										Diagnostic.Create(
											descriptor: SD0401,
											location: argument.GetLocation(),
											messageArgs: null,
											properties: ImmutableDictionary.CreateRange(
												new KeyValuePair<string, string?>[]
												{
													new("ExpressionValue", exprValue)
												}
											)
										)
									);
								}
							}

							break;
						}
					}
				}
			}
		}
	}
}
