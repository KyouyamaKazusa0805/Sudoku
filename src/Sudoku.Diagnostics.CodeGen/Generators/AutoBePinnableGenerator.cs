namespace Sudoku.Diagnostics.CodeGen.Generators;

/// <summary>
/// Defines a source generator that generates the source code on implementation
/// for the method <c>GetPinnableReference</c>.
/// </summary>
#if false
[Generator(LanguageNames.CSharp)]
#endif
public sealed class AutoBePinnableGenerator : IIncrementalGenerator
{
	private const string AttributeFullName = "System.Diagnostics.CodeGen.AutoBePinnableAttribute";


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
		if (gsc is not { Node: TypeDeclarationSyntax node, SemanticModel: { Compilation: var compilation } semanticModel })
		{
			return null;
		}

		if (semanticModel.GetDeclaredSymbol(node, ct) is not { ContainingType: null } typeSymbol)
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
			if (
#pragma warning disable IDE0055
				v is not (
					var type,
					{ ConstructorArguments: [{ Value: ITypeSymbol returnType }, { Value: string pattern }] } attributeData
				)
#pragma warning restore IDE0055
			)
			{
				continue;
			}

			if (recordedList.FindIndex(e => SymbolEqualityComparer.Default.Equals(e, type)) != -1)
			{
				continue;
			}

			var (_, _, namespaceName, genericParameterList, _, _, readOnlyKeyword, _, _, _) = SymbolOutputInfo.FromSymbol(type);
			bool returnByReadOnlyRef = attributeData.GetNamedArgument("ReturnsReadOnlyReference", true);
			string refReadOnlyModifier = returnByReadOnlyRef ? "ref readonly" : "ref";
			string returnTypeFullName = returnType.ToDisplayString(TypeFormats.FullName);
			spc.AddSource(
				$"{type.ToFileName()}.g.{Shortcuts.AutoBePinnable}.cs",
				$$"""
				#nullable enable
				
				namespace {{namespaceName}};
				
				partial {{type.GetTypeKindModifier()}} {{type.Name}}{{genericParameterList}}
				{
					/// <include
					///	    file="../../global-doc-comments.xml"
					///	    path="g/csharp7/feature[@name='custom-fixed']/target[@name='method']"/>
					[global::System.Runtime.CompilerServices.CompilerGenerated]
					[global::System.CodeDom.Compiler.GeneratedCode("{{typeof(AutoBePinnableGenerator).FullName}}", "{{VersionValue}}")]
					[global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
					public unsafe {{readOnlyKeyword}}{{refReadOnlyModifier}} {{returnTypeFullName}} GetPinnableReference()
						=> ref {{pattern}};
				}
				"""
			);

			recordedList.Add(type);
		}
	}
}
