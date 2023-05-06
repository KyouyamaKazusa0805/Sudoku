namespace Sudoku.SourceGeneration.Handlers;

/// <summary>
/// The generator handler for default overridden.
/// </summary>
internal sealed class DefaultOverriddenHandler
{
	/// <summary>
	/// Output the source.
	/// </summary>
	/// <param name="spc">The context.</param>
	/// <param name="value">The value.</param>
	public static void Output(SourceProductionContext spc, DefaultOverriddenCollectedResult value)
	{
		var codeSnippets = new Dictionary<string, SortedList<GenerateMethodKind, (object Data, string Code)>>();
		foreach (var v in value.DataForEquals)
		{
			if (v is not (var mode, var modifiers, { Name: var typeName, ContainingNamespace: var @namespace } type, var paramName))
			{
				continue;
			}

			var (_, _, _, _, genericParamList, _, _, _, _, _) = SymbolOutputInfo.FromSymbol(type);
			var extraAttributeStr = mode switch
			{
				0 => """
				[global::System.Obsolete(global::System.Runtime.Messages.RefStructDefaultImplementationMessage.OverriddenEqualsMethod, false)]
						[global::System.Diagnostics.DebuggerHiddenAttribute]
						[global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Never)]
						
				""",
				_ => string.Empty
			};
			var targetExpression = mode switch
			{
				0 => "false",
				1 => $"{paramName} is {type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)} comparer && Equals(comparer)",
				2 => $"Equals({paramName} as {type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)})"
			};
			var attributeStr = mode switch
			{
				0 => string.Empty,
				1 or 2 => "[global::System.Diagnostics.CodeAnalysis.NotNullWhenAttribute(true)] "
			};

			codeSnippets.Add(
				type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
				new()
				{
					{
						GenerateMethodKind.Equals,
						(
							v,
							$$"""
							/// <inheritdoc cref="object.Equals(object?)"/>
									[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute]
									[global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{typeof(Generator).FullName}}", "{{Value}}")]
									[global::System.Runtime.CompilerServices.MethodImplAttribute(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
									{{extraAttributeStr}}{{modifiers}} bool Equals({{attributeStr}}object? {{paramName}})
										=> {{targetExpression}};
							"""
						)
					}
				}
			);
		}

		foreach (var v in value.DataForGetHashCode)
		{
			if (v is not (var mode, var modifiers, { Name: var typeName, ContainingNamespace: var @namespace } type, var rawMemberNames))
			{
				continue;
			}

			var (_, _, _, _, genericParamList, _, _, _, _, _) = SymbolOutputInfo.FromSymbol(type);
			var needCast = mode switch
			{
				1 when rawMemberNames.First() is var name
					=> (from m in type.GetMembers() where m.Name == name select m).FirstOrDefault() switch
					{
						IFieldSymbol field => field is not { Type.SpecialType: System_Int32, RefKind: RefKind.None },
						IPropertySymbol property => property is not { Type.SpecialType: System_Int32, RefKind: RefKind.None },
						IMethodSymbol { Parameters: [] } method => method is not { ReturnType.SpecialType: System_Int32, RefKind: RefKind.None },
						_ => null
					},
				_ => default(bool?)
			};

			var extraAttributeStr = mode switch
			{
				0 => """
				[global::System.Obsolete(global::System.Runtime.Messages.RefStructDefaultImplementationMessage.OverriddenGetHashCodeMethod, false)]
						[global::System.Diagnostics.DebuggerHiddenAttribute]
						[global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Never)]
						
				""",
				_ => string.Empty
			};
			var targetExpression = (mode, rawMemberNames.ToArray(), needCast) switch
			{
				(0, [], _)
					=> $"\t=> throw new global::System.NotSupportedException(global::System.Runtime.Messages.RefStructDefaultImplementationMessage.OverriddenGetHashCodeMethod);",
				(1, [var memberName], true or null)
					=> $"\t=> (int){memberName};",
				(1, [var memberName], _)
					=> $"\t=> {memberName};",
				(2, { Length: <= 8 } memberNames, _) when string.Join(", ", from e in memberNames select e) is var a
					=> $"\t=> global::System.HashCode.Combine({a});",
				(2, { Length: > 8 } memberNames, _) when string.Join("\r\n\t\t\t", from e in memberNames select $"result.Add({e});") is var a
					=> $$"""
					{
								var result = new global::System.HashCode();
								{{a}}
								return result.ToHashCode();
							}
					""",
			};

			var fullTypeName = type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
			var code =
				$$"""
				/// <inheritdoc cref="object.GetHashCode"/>
						[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute]
						[global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{typeof(Generator).FullName}}", "{{Value}}")]
						[global::System.Runtime.CompilerServices.MethodImplAttribute(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
						{{extraAttributeStr}}{{modifiers}} int GetHashCode()
						{{targetExpression}}
				""";
			if (codeSnippets.TryGetValue(fullTypeName, out var list))
			{
				list.Add(GenerateMethodKind.GetHashCode, (v, code));
			}
			else
			{
				codeSnippets.Add(fullTypeName, new() { { GenerateMethodKind.GetHashCode, (v, code) } });
			}
		}

		foreach (var v in value.DataForToString)
		{
			if (v is not (var mode, var modifiers, { Name: var typeName, ContainingNamespace: var @namespace } type, var attributeType, var rawMemberNames))
			{
				continue;
			}

			var (_, _, _, _, genericParamList, _, _, _, _, _) = SymbolOutputInfo.FromSymbol(type);

			var needCast = mode switch
			{
				0 => (
					from IMethodSymbol method in type.GetAllMembers().OfType<IMethodSymbol>().Distinct(SymbolEqualityComparer.Default)
					where method is { Name: nameof(ToString), Parameters: [{ Type.IsReferenceType: true }] }
					select method
				).Take(2).Count() == 2,
				_ => default(bool?)
			};

			var targetExpression = (mode, rawMemberNames.ToArray(), needCast) switch
			{
				(0, [], true) => $"\t=> ToString(default(string));",
				(0, [], _) => $"\t=> ToString(null);",
				(1, [var memberName], _) => $"\t=> {memberName};",
				(2, var memberNames, _) when argStr(memberNames) is var a => $$$""""	=> $$"""{{nameof({{{typeName}}})}} { {{{a}}} }""";"""",
			};

			var fullTypeName = type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
			var code =
				$$"""
				/// <inheritdoc cref="object.ToString"/>
						[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute]
						[global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{typeof(Generator).FullName}}", "{{Value}}")]
						[global::System.Runtime.CompilerServices.MethodImplAttribute(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
						{{modifiers}} string ToString()
						{{targetExpression}}
				""";
			if (codeSnippets.TryGetValue(fullTypeName, out var list))
			{
				list.Add(GenerateMethodKind.ToString, (v, code));
			}
			else
			{
				codeSnippets.Add(fullTypeName, new() { { GenerateMethodKind.ToString, (v, code) } });
			}


			string argStr(string[] memberNames)
			{
				bool attributePredicate(AttributeData a) => SymbolEqualityComparer.Default.Equals(a.AttributeClass, attributeType);
				return string.Join(
					", ",
					from memberName in memberNames
					let targetMember = (
						from internalMember in type.GetAllMembers()
						where internalMember is IFieldSymbol or IPropertySymbol
						let internalMemberName = internalMember.Name
						where internalMemberName == memberName
						select internalMember
					).FirstOrDefault()
					let foundAttribute = targetMember?.GetAttributes().FirstOrDefault(attributePredicate)
					let projectedMemberName = foundAttribute switch { { ConstructorArguments: [{ Value: string value }] } => value, _ => null }
					select $$$""""{{{(projectedMemberName is null ? $$$"""{{nameof({{{memberName}}})}}""" : projectedMemberName)}}} = {{{{{memberName}}}}}""""
				);
			}
		}

		var typeDeclarations = new List<string>();
		foreach (var (_, generatedCodeSnippet) in codeSnippets)
		{
			var (data, _) = generatedCodeSnippet.First().Value;
			var (@namespace, type, typeName, genericParamList) = data switch
			{
				EqualsOverriddenCollectedResult { Type: { ContainingNamespace: var namespaceSymbol, Name: var n, TypeParameters: var tp } t }
					=> (ns(namespaceSymbol), t, n, gp(tp)),
				GetHashCodeCollectedResult { Type: { ContainingNamespace: var namespaceSymbol, Name: var n, TypeParameters: var tp } t }
					=> (ns(namespaceSymbol), t, n, gp(tp)),
				ToStringCollectedResult { Type: { ContainingNamespace: var namespaceSymbol, Name: var n, TypeParameters: var tp } t }
					=> (ns(namespaceSymbol), t, n, gp(tp))
			};

			typeDeclarations.Add(
				$$"""
				namespace {{@namespace}}
				{
					partial {{type.GetTypeKindModifier()}} {{typeName}}{{genericParamList}}
					{
						{{string.Join("\r\n\r\n\t\t", from kvp in generatedCodeSnippet select kvp.Value.Code)}}
					}
				}
				"""
			);


			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			static string ns(INamespaceSymbol s) => s.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)["global::".Length..];

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			static string gp(ImmutableArray<ITypeParameterSymbol> typeParams)
				=> typeParams is [] ? string.Empty : $"<{string.Join(", ", typeParams)}>";
		}

		spc.AddSource(
			$"DefaultOverridden.g.{Shortcuts.DefaultOverridden}.cs",
			$$"""
			// <auto-generated/>

			#nullable enable

			{{string.Join("\r\n\r\n", typeDeclarations)}}
			"""
		);
	}
}

/// <summary>
/// Represents a kind of generated method.
/// </summary>
file enum GenerateMethodKind
{
	/// <summary>
	/// Indicates the kind is to generate <c>Equals</c>.
	/// </summary>
	Equals,

	/// <summary>
	/// Indicates the kind is to generate <c>GetHashCode</c>.
	/// </summary>
	GetHashCode,

	/// <summary>
	/// Indicates the kind is to generate <c>ToString</c>.
	/// </summary>
	ToString
}
