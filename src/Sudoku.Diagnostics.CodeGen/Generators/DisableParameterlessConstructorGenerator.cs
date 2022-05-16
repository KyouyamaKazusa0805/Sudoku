namespace Sudoku.Diagnostics.CodeGen.Generators;

/// <summary>
/// Indicates the generator that generates the parameterless constructor on <see langword="struct"/> types,
/// to disallow any invokes for them from user.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class DisableParameterlessConstructorGenerator : IIncrementalGenerator
{
	private const string AttributeFullName = "System.Diagnostics.CodeGen.DisableParameterlessConstructorAttribute";


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
		if (gsc is not { Node: StructDeclarationSyntax n, SemanticModel: { Compilation: { } compilation } semanticModel })
		{
			return null;
		}

		var typeSymbol = semanticModel.GetDeclaredSymbol(n, ct);
		if (typeSymbol is not { ContainingType: null, InstanceConstructors: var instanceConstructors })
		{
			return null;
		}

		var attributeTypeSymbol = compilation.GetTypeByMetadataName(AttributeFullName);
		bool predicate(AttributeData e) => SymbolEqualityComparer.Default.Equals(e.AttributeClass, attributeTypeSymbol);
		if (typeSymbol.GetAttributes().FirstOrDefault(predicate) is not { } attributeData)
		{
			return null;
		}

		// Check whether the type contains a user-defined parameterless constructor.
		if (instanceConstructors.Any(static e => e is { Parameters: [], IsImplicitlyDeclared: false }))
		{
			return null;
		}

		return (typeSymbol, attributeData);
	}

	private static void OutputSource(SourceProductionContext spc, ImmutableArray<(INamedTypeSymbol, AttributeData)?> list)
	{
		var recordedList = new List<INamedTypeSymbol>();
		foreach (var v in list)
		{
			if (v is not var (type, attributeData))
			{
				continue;
			}

			if (recordedList.FindIndex(e => SymbolEqualityComparer.Default.Equals(e, type)) != -1)
			{
				continue;
			}

			var (_, _, namespaceName, genericParameterList, _, _, _, _, _, _) = SymbolOutputInfo.FromSymbol(type);

			string? rawMessage = attributeData.GetNamedArgument<string>("Message");
			string? memberName = attributeData.GetNamedArgument<string>("SuggestedMemberName");
			if (
				(rawMessage, memberName) switch
				{
					(not null, not null) => null,
					(null, null) => null,
					(null, not null) => $"Please use the member '{memberName}' instead.",
					_ => rawMessage
				} is not { } message
			)
			{
				continue;
			}

			spc.AddSource(
				$"{type.ToFileName()}.g.{Shortcuts.DisableParametelessConstructor}.cs",
				$$"""
				#nullable enable
				
				namespace {{namespaceName}};
				
				partial struct {{type.Name}}{{genericParameterList}}
				{
					/// <summary>
					/// Throws a <see cref="global::System.NotSupportedException"/>.
					/// </summary>
					/// <exception cref="global::System.NotSupportedException">
					/// The exception will always be thrown.
					/// </exception>
					/// <remarks>
					/// The main idea of the paramterless constructor is to create a new instance
					/// without any extra information, but the current type is special:
					/// the author wants to make you use another member instead of it to get a better experience.
					/// Therefore, the paramterless constructor is disallowed to be invoked
					/// no matter what kind of invocation, reflection or strongly reference.
					/// </remarks>
					[global::System.Runtime.CompilerServices.CompilerGenerated]
					[global::System.CodeDom.Compiler.GeneratedCode("{{typeof(DisableParameterlessConstructorGenerator).FullName}}", "{{VersionValue}}")]
					[global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
					[global::System.Obsolete("{{message}}", true)]
					public {{type.Name}}() => throw new global::System.NotSupportedException();
				}
				"""
			);

			recordedList.Add(type);
		}
	}
}
