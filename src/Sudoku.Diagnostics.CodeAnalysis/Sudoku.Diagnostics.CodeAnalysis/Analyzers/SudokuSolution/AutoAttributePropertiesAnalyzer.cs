namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers;

[CodeAnalyzer("SD0401", "SD0402")]
public sealed partial class AutoAttributePropertiesAnalyzer : DiagnosticAnalyzer
{
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
						Name: IdentifierNameSyntax { Identifier.ValueText: var text } identifierName,
						ArgumentList.Arguments: var arguments
					}
				)
				{
					continue;
				}

				switch (text)
				{
					case nameof(AutoDeconstructAttribute) or "AutoDeconstruct":
					{
						if (arguments.Count < 2)
						{
							context.ReportDiagnostic(
								Diagnostic.Create(
									descriptor: SD0402,
									location: node.GetLocation(),
									messageArgs: new object[]
									{
										"Sudoku.CodeGenerating.AutoDeconstructAttribute",
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
						nameof(AutoEqualityAttribute) or "AutoEquality"
						or nameof(AutoHashCodeAttribute) or "AutoHashCode"
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
					case "AutoDeconstructExtensionAttribute" or "AutoDeconstructExtension":
					{
						if (arguments.Count < 3)
						{
							context.ReportDiagnostic(
								Diagnostic.Create(
									descriptor: SD0402,
									location: node.GetLocation(),
									messageArgs: new object[]
									{
										"Sudoku.CodeGenerating.AutoDeconstructExtensionAttribute",
										3,
										" at least"
									}
								)
							);

							continue;
						}

						goto ArgumentsChecking_Case2;
					}
					case nameof(AutoGetEnumeratorAttribute) or "AutoGetEnumerator":
					{
						if (
							arguments[0] is { Expression: var expr } argument
							&& semanticModel.GetOperation(expr) is
							{
								Kind: not OperationKind.NameOf,
								ConstantValue: { HasValue: true, Value: string exprValue and not "@" }
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
						for (int i = 1, count = arguments.Count; i < count; i++)
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
