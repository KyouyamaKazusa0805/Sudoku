namespace Sudoku.Diagnostics.CodeGen.Generators;

/// <summary>
/// Defines a source generator that generates the source code on implementation
/// for the method <c>GetPinnableReference</c>.
/// </summary>
[Generator(LanguageNames.CSharp)]
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

	private static bool NodePredicate(SyntaxNode node, CancellationToken _)
		=> node is TypeDeclarationSyntax { Modifiers: var modifiers, AttributeLists.Count: > 0 }
			&& modifiers.Any(SyntaxKind.PartialKeyword);

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
		var attributesData =
			from a in typeSymbol.GetAttributes()
			where SymbolEqualityComparer.Default.Equals(a.AttributeClass, attributeTypeSymbol)
			select a;
		return attributesData.FirstOrDefault() switch
		{
			{ } attributeData => (typeSymbol, attributeData),
			_ => null
		};
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
					{
						ConstructorArguments: [
							{ Value: ITypeSymbol returnType },
							{ Value: string pattern }
						]
					} attributeData
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
				$"{type.ToFileName()}.{Shortcuts.AutoBePinnable}.cs",
				$$"""
				#nullable enable
				
				namespace {{namespaceName}};
				
				partial {{type.GetTypeKindModifier()}} {{type.Name}}{{genericParameterList}}
				{
					/// <summary>
					/// Returns a reference as the fixed position of the current instance.
					/// For example, the return value will be the pointer value that points to the zero-indexed
					/// place in an array.
					/// </summary>
					/// <returns>A reference as the fixed position of the current instance.</returns>
					/// <remarks>
					/// Beginning with C# 7, we can customize the return value type of a <see langword="fixed"/> variable
					/// if we implement a parameterless method called <c>GetPinnableReference</c>, returning by
					/// <see langword="ref"/> or <see langword="ref readonly"/>. For example, if we hold a fixed buffer
					/// of element type <see cref="short"/>:
					/// <code><![CDATA[
					/// class ExampleType
					/// {
					///	    private fixed short _maskList[100];
					///	
					///     public ref readonly short GetPinnableReference() => ref _maskList[0];
					/// }
					/// ]]></code>
					/// We can use <see langword="fixed"/> statement to define a variable of type <see langword="short"/>*
					/// as the left-value.
					/// <code>
					/// var instance = new ExampleType();
					/// fixed (short* ptr = instance)
					/// {
					///     // Operation here.
					/// }
					/// </code>
					/// </remarks>
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
