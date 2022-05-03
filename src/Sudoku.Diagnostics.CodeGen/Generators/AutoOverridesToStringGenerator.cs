namespace Sudoku.Diagnostics.CodeGen.Generators;

/// <summary>
/// Defines a source generator that generates for the code that is for the overriden of a type.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed partial class AutoOverridesToStringGenerator : ISourceGenerator
{
	/// <inheritdoc/>
	public void Execute(GeneratorExecutionContext context)
	{
		// Check values.
		if (
			context is not
			{
				SyntaxContextReceiver: Receiver { Collection: var collection } receiver,
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
			var symbolsRawValue = new List<string>();
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
						targetSymbolsRawString.Add($$$"""{{nameof({{{fieldName}}})}} = {{{{{fieldName}}}}}""");
						symbolsRawValue.Add(fieldName);
						break;
					}
					case IPropertySymbol { GetMethod: not null, Name: var propertyName }:
					{
						targetSymbolsRawString.Add($$$"""{{nameof({{{propertyName}}})}} = {{{{{propertyName}}}}}""");
						symbolsRawValue.Add(propertyName);
						break;
					}
					case IMethodSymbol { Name: var methodName, Parameters: [], ReturnsVoid: false }:
					{
						targetSymbolsRawString.Add($$$"""{{nameof({{{methodName}}})}} = {{{{{methodName}}}()}}""");
						symbolsRawValue.Add($"{methodName}()");
						break;
					}
				}
			}

			string outputStringExpression = attributeData.GetNamedArgument<string>("Pattern") is { } pattern
				? convert(pattern) is var convertedPattern && isSimpleInterpolatedPattern(pattern)
					? convertedPattern[1..^1]
					: $"""
					{interpolatedStringSuffix(pattern)}"{convertedPattern}"
					"""
				: $$""""
				$$"""{{type.Name}} { {{string.Join(", ", targetSymbolsRawString)}} }"""
				"""";
			context.AddSource(
				type.ToFileName(),
				"aot",
				$$"""
				#nullable enable
				
				namespace {{namespaceName}};
				
				partial {{type.GetTypeKindModifier()}} {{type.Name}}{{genericParameterList}}
				{
					/// <inheritdoc cref="object.ToString"/>
					[global::System.Runtime.CompilerServices.CompilerGenerated]
					[global::System.CodeDom.Compiler.GeneratedCode("{{GetType().FullName}}", "{{VersionValue}}")]
					[global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
					public override {{readOnlyKeyword}}string ToString()
						=> {{outputStringExpression}};
				}
				"""
			);


			static bool isSimpleInterpolatedPattern(string pattern)
				=> Regex.Matches(pattern, """(?<={).+?(?=})""").Count == 1 && pattern is ['{', .., '}'];

			string convert(string pattern)
				=> Regex
					.Replace(pattern, """(\[0\]|\[[1-9]\d*\])""", m => symbolsRawValue[int.Parse(m.Value[1..^1])])
					.Replace("*", $"{nameof(ToString)}()");

			static string interpolatedStringSuffix(string pattern)
				=> Regex.IsMatch(pattern, """(?<={).+?(?=})""") ? "$" : string.Empty;
		}
	}

	/// <inheritdoc/>
	public void Initialize(GeneratorInitializationContext context)
		=> context.RegisterForSyntaxNotifications(() => new Receiver(context.CancellationToken));
}
