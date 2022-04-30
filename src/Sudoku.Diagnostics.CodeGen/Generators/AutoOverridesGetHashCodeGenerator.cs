namespace Sudoku.Diagnostics.CodeGen.Generators;

/// <summary>
/// Defines a source generator that generates for the code that is for the overriden of a type.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class AutoOverridesGetHashCodeGenerator : ISourceGenerator
{
	/// <inheritdoc/>
	public void Execute(GeneratorExecutionContext context)
	{
		// Check values.
		if (
			context is not
			{
				SyntaxContextReceiver: AutoOverridesGetHashCodeReceiver
				{
					Diagnostics: var diagnostics,
					Collection: var collection
				} receiver,
				Compilation: { Assembly: var assembly } compilation
			}
		)
		{
			return;
		}

		// Report diagnostics if worth.
		if (diagnostics.Count != 0)
		{
			diagnostics.ForEach(context.ReportDiagnostic);
			return;
		}

		// Iterates on each pair in the collection.
		foreach (var (type, attributeData, identifier) in collection)
		{
			if (attributeData.ApplicationSyntaxReference is not { Span: var textSpan, SyntaxTree: var syntaxTree })
			{
				continue;
			}

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
						Name: nameof(GetHashCode),
						Parameters: [],
						ReturnType.SpecialType: SpecialType.System_Int32,
						IsImplicitlyDeclared: false
					}
				)
			)
			{
				context.ReportDiagnostic(Diagnostic.Create(SCA0011, identifier.GetLocation(), messageArgs: null));
				continue;
			}

			var (_, _, namespaceName, genericParameterList, _, _, readOnlyKeyword, _, _, _) =
				SymbolOutputInfo.FromSymbol(type);

			var targetSymbolsRawString = new List<string>();
			var location = Location.Create(syntaxTree, textSpan);
			foreach (var typedConstant in attributeData.ConstructorArguments[0].Values)
			{
				string memberName = (string)typedConstant.Value!;

				// Checks whether the specified member is in the target type.
				var selectedMembers = (from member in members where member.Name == memberName select member).ToArray();
				if (selectedMembers is not [var memberSymbol, ..])
				{
					context.ReportDiagnostic(Diagnostic.Create(SCA0008, location, messageArgs: null));
					continue;
				}

				switch (memberSymbol)
				{
					case IFieldSymbol { Name: var fieldName }:
					{
						targetSymbolsRawString.Add(fieldName);
						break;
					}
					case IPropertySymbol { GetMethod: not null, Name: var propertyName }:
					{
						targetSymbolsRawString.Add(propertyName);
						break;
					}
					case IMethodSymbol { Name: var methodName, Parameters: [], ReturnsVoid: false }:
					{
						targetSymbolsRawString.Add($"{methodName}()");
						break;
					}
					default:
					{
						context.ReportDiagnostic(Diagnostic.Create(SCA0009, location, messageArgs: null));
						break;
					}
				}
			}

			string sealedKeyword = attributeData.NamedArguments is [{ Value.Value: var realValue } namedArg, ..]
				&& ((bool?)realValue ?? false)
				&& type.TypeKind != TypeKind.Struct ? "sealed " : string.Empty;

			string typeKindString = type.GetTypeKindModifier();
			string methodBody = targetSymbolsRawString.Count switch
			{
				<= 8 => $"\t\t=> global::System.HashCode.Combine({string.Join(", ", targetSymbolsRawString)});",
				_ => $$"""
					{
						var final = new global::System.HashCode();
						{{string.Join("\r\n\t\t", from e in targetSymbolsRawString select $"final.Add({e});")}}
						return final.ToHashCode();
					}
				"""
			};

			context.AddSource(
				type.ToFileName(),
				"aog",
				$$"""
				#nullable enable
				
				namespace {{namespaceName}};
				
				partial {{typeKindString}} {{type.Name}}{{genericParameterList}}
				{
					/// <inheritdoc cref="object.ToString"/>
					[global::System.Runtime.CompilerServices.CompilerGenerated]
					[global::System.CodeDom.Compiler.GeneratedCode("{{GetType().FullName}}", "{{VersionValue}}")]
					[global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
					public {{sealedKeyword}}override {{readOnlyKeyword}}int GetHashCode()
				{{methodBody}}
				}
				"""
			);
		}
	}

	/// <inheritdoc/>
	public void Initialize(GeneratorInitializationContext context)
		=> context.RegisterForSyntaxNotifications(() => new AutoOverridesGetHashCodeReceiver(context.CancellationToken));
}
