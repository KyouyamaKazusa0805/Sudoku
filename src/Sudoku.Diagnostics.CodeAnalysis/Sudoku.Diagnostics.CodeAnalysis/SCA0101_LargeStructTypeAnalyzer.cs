namespace Sudoku.Diagnostics.CodeAnalysis;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class SCA0101_LargeStructTypeAnalyzer : DiagnosticAnalyzer
{
	/// <inheritdoc/>
	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(SCA0101, SCA0001);


	/// <inheritdoc/>
	public override void Initialize(AnalysisContext context)
	{
		context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
		context.EnableConcurrentExecution();

		context.RegisterOperationAction(
			static oac =>
			{
				if (oac is not
					{
						CancellationToken: var ct,
						Compilation: var compilation,
						Operation: IObjectCreationOperation
						{
							Type: { TypeKind: TypeKind.Struct, Name: var name and not [] } type,
							Arguments: [],
							Syntax: ObjectCreationExpressionSyntax node,
						}
					})
				{
					return;
				}

				var nodeLocation = node.GetLocation();
				var largeStruct = compilation.GetTypeByMetadataName(SpecialFullTypeNames.LargeStructAttribute);
				if (largeStruct is null)
				{
					oac.ReportDiagnostic(Diagnostic.Create(SCA0001, nodeLocation, messageArgs: SpecialFullTypeNames.LargeStructAttribute));
					return;
				}

				var attributesData = type.GetAttributes();
				if (attributesData.Length == 0)
				{
					return;
				}

				if (attributesData.FirstOrDefault(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, largeStruct)) is not { } a)
				{
					return;
				}

				static TypedConstant? f(NamedArguments l) => l.GetValueByName(SpecialNamedArgumentNames.SuggestedMemberName);
				oac.ReportDiagnostic(
					Diagnostic.Create(
						SCA0101,
						nodeLocation,
						ImmutableDictionary.CreateRange(
							new KeyValuePair<string, string?>[]
							{
								new(SpecialNamedArgumentNames.TypeName, name),
								new(
									SpecialNamedArgumentNames.SuggestedMemberName,
									a switch
									{
										{ NamedArguments: var l and not [] } when f(l) is { Value: string value } => value,
										_ => null
									}
								)
							}
						)
					)
				);
			},
			new[] { OperationKind.ObjectCreation }
		);
	}
}
