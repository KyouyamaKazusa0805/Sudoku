namespace Sudoku.Diagnostics.CodeGen.Generators;

/// <summary>
/// Defines a source generator that can generate the source code
/// that implements the interface type <c><![CDATA[IDefaultable<T>]]></c>.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class AutoImplementsDefaultableGenerator : IIncrementalGenerator
{
	private const string AttributeFullName = "System.Diagnostics.CodeGen.AutoImplementsDefaultableAttribute";


	/// <inheritdoc/>
	public void Initialize(IncrementalGeneratorInitializationContext context)
		=> context.RegisterSourceOutput(
			context.SyntaxProvider
				.CreateSyntaxProvider(NodePredicate, GetValuesProvider)
				.Where(static element => element is not null)
				.Collect(),
			OutputSource
		);


	private static (INamedTypeSymbol, AttributeData)? GetValuesProvider(GeneratorSyntaxContext gsc, CancellationToken ct)
	{
		if (gsc is not { Node: TypeDeclarationSyntax n, SemanticModel: { Compilation: { } compilation } semanticModel })
		{
			return null;
		}

		if (semanticModel.GetDeclaredSymbol(n, ct) is not { ContainingType: null } typeSymbol)
		{
			return null;
		}

		var attributeTypeSymbol = compilation.GetTypeByMetadataName(AttributeFullName);
		var attributeData = (
			from a in typeSymbol.GetAttributes()
			where SymbolEqualityComparer.Default.Equals(a.AttributeClass, attributeTypeSymbol)
			select a
		).FirstOrDefault();
		return attributeData is null ? null : (typeSymbol, attributeData);
	}

	private static void OutputSource(SourceProductionContext spc, ImmutableArray<(INamedTypeSymbol, AttributeData)?> list)
	{
		var recordedList = new List<INamedTypeSymbol>();
		foreach (var v in list)
		{
			if (v is not (var type, { ConstructorArguments: [{ Value: string defaultFieldName }] } attributeData))
			{
				continue;
			}

			if (recordedList.FindIndex(e => SymbolEqualityComparer.Default.Equals(e, type)) != -1)
			{
				continue;
			}

			var (_, _, namespaceName, genericParameterList, _, _, readOnlyKeyword, _, _, _) =
				SymbolOutputInfo.FromSymbol(type);

			string fullName = type.ToDisplayString(TypeFormats.FullName);
			string? assignment = attributeData.GetNamedArgument<string>("Pattern") is { } patternValue
				? $" = {patternValue}"
				: string.Empty;
			string isDefaultExpression = attributeData.GetNamedArgument<string>("IsDefaultExpression")
				?? $"this == {defaultFieldName}";
			string defaultFieldDescription = attributeData.GetNamedArgument<string>("DefaultFieldDescription")
				?? """<inheritdoc cref="global::System.IDefaultable{T}.Default" />""";

			spc.AddSource(
				$"{type.ToFileName()}.g.{Shortcuts.AutoImplementsDefaultable}.cs",
				$$"""
				#nullable enable
				
				namespace {{namespaceName}};
				
				partial {{type.GetTypeKindModifier()}} {{type.Name}}{{genericParameterList}}
				{
					/// <summary>
					/// {{defaultFieldDescription}}
					/// </summary>
					public static readonly {{fullName}} {{defaultFieldName}}{{assignment}};
					
					
					/// <inheritdoc/>
					{{readOnlyKeyword}}bool IDefaultable<{{fullName}}>.IsDefault => {{isDefaultExpression}};
					
					/// <inheritdoc/>
					static {{fullName}} IDefaultable<{{fullName}}>.Default => {{defaultFieldName}};
				}
				"""
			);

			recordedList.Add(type);
		}
	}
}
