namespace Sudoku.Diagnostics.CodeGen.Generators;

/// <summary>
/// Defines a source generator that generates for the code that is for the overriden of a type.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class AutoOverridesToStringGenerator : ISourceGenerator
{
	/// <inheritdoc/>
	public void Execute(GeneratorExecutionContext context)
	{
		// Check values.
		if (
			context is not
			{
				SyntaxContextReceiver: AutoOverridesToStringReceiver { Collection: var collection } receiver,
				Compilation: { Assembly: var assembly } compilation
			}
		)
		{
			return;
		}

		// Iterates on each pair in the collection.
		foreach (var (type, attributeData) in collection)
		{
			var members = type.GetAllMembers();
			var methods = members.OfType<IMethodSymbol>().ToArray();
			if (
				!type.IsRecord && Array.Exists(
					methods,
					static symbol => symbol is
					{
						ContainingType.SpecialType: not (SpecialType.System_Object or SpecialType.System_ValueType),
						IsStatic: false,
						IsAbstract: false,
						Name: nameof(ToString),
						Parameters: [],
						ReturnType.SpecialType: SpecialType.System_String,
						IsImplicitlyDeclared: false
					}
				)
			)
			{
				continue;
			}

			var (_, _, namespaceName, genericParameterList, _, _, readOnlyKeyword, _, _, _) =
				SymbolOutputInfo.FromSymbol(type);

			var targetSymbolsRawString = new List<string>();
			foreach (var typedConstant in attributeData.ConstructorArguments[0].Values)
			{
				string memberName = (string)typedConstant.Value!;

				// Checks whether the specified member is in the target type.
				var selectedMembers = (from member in members where member.Name == memberName select member).ToArray();
				if (selectedMembers is not [var memberSymbol, ..])
				{
					continue;
				}

				switch (memberSymbol)
				{
					case IFieldSymbol { Name: var fieldName }:
					{
						targetSymbolsRawString.Add($"{{nameof({fieldName})}} = {{{fieldName}}}");
						break;
					}
					case IPropertySymbol { GetMethod: not null, Name: var propertyName }:
					{
						targetSymbolsRawString.Add($"{{nameof({propertyName})}} = {{{propertyName}}}");
						break;
					}
					case IMethodSymbol { Name: var methodName, Parameters: [], ReturnsVoid: false }:
					{
						targetSymbolsRawString.Add($"{{nameof({methodName})}} = {{{methodName}()}}");
						break;
					}
				}
			}

			string typeKindString = type.GetTypeKindModifier();
			string finalString = $"{type.Name} {{{{ {string.Join(", ", targetSymbolsRawString)} }}}}";
			context.AddSource(
				type.ToFileName(),
				"aot",
				$$"""
				#nullable enable
				
				namespace {{namespaceName}};
				
				partial {{typeKindString}} {{type.Name}}{{genericParameterList}}
				{
					/// <inheritdoc cref="object.ToString"/>
					[global::System.Runtime.CompilerServices.CompilerGenerated]
					[global::System.CodeDom.Compiler.GeneratedCode("{{GetType().FullName}}", "{{VersionValue}}")]
					[global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
					public override {{readOnlyKeyword}}string ToString()
						=> $"{{finalString}}";
				}
				"""
			);
		}
	}

	/// <inheritdoc/>
	public void Initialize(GeneratorInitializationContext context)
		=> context.RegisterForSyntaxNotifications(() => new AutoOverridesToStringReceiver(context.CancellationToken));
}
