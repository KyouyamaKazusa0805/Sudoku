namespace Sudoku.Diagnostics.CodeGen.Generators;

/// <summary>
/// Defines a generator that generates the source code for mask-styled members on a type.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class MaskStyledDataTypeGenerator : IIncrementalGenerator
{
	/// <inheritdoc/>
	public void Initialize(IncrementalGeneratorInitializationContext context)
		=> context.RegisterSourceOutput(
			context.SyntaxProvider
				.CreateSyntaxProvider(NodePredicate, GetValuesProvider)
				.Where(static pair => pair is not null)
				.Collect(),
			GenerateSource
		);

	private static (INamedTypeSymbol, AttributeData)? GetValuesProvider(GeneratorSyntaxContext gsc, CancellationToken ct)
	{
		if (gsc is not { Node: TypeDeclarationSyntax node, SemanticModel: var semanticModel })
		{
			return null;
		}

		if (semanticModel.GetDeclaredSymbol(node, ct) is not { ContainingType: null } typeSymbol)
		{
			return null;
		}

		const string attributeTypeName = "System.Diagnostics.CodeGen.MaskStyledDataTypeAttribute<>";
		var attributeData = (
			from a in typeSymbol.GetAttributes()
			let attributeClass = a.AttributeClass
			where attributeClass is { IsGenericType: true }
			let unboundedAttributeType = attributeClass.ConstructUnboundGenericType()
			where unboundedAttributeType.ToDisplayString(TypeFormats.FullName) == attributeTypeName
			select a
		).FirstOrDefault();
		return attributeData is null ? null : (typeSymbol, attributeData);
	}

	private void GenerateSource(SourceProductionContext spc, ImmutableArray<(INamedTypeSymbol, AttributeData)?> list)
	{
		var recordedList = new List<INamedTypeSymbol>();
		foreach (var v in list)
		{
			if (
#pragma warning disable IDE0055
				v is not (
					{ Name: var typeName } type,
					{
						AttributeClass.TypeArguments: [var fieldType],
						ConstructorArguments: [{ Values: var ctorArgs and not [] }]
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

			var ctorArgsList = new List<(string Name, INamedTypeSymbol MemberType, int Start, int End)>();
			for (int i = 0, last = 0; i < ctorArgs.Length; i += 3)
			{
				if (ctorArgs[i] is { Value: string s }
					&& i + 1 < ctorArgs.Length && ctorArgs[i + 1] is { Value: int endIndex }
					&& i + 2 < ctorArgs.Length && ctorArgs[i + 2] is { Value: INamedTypeSymbol memberType })
				{
					ctorArgsList.Add((s, memberType, last, endIndex));
					last = endIndex;

					continue;
				}

				ctorArgsList = null;
				break;
			}
			if (ctorArgsList is null)
			{
				continue;
			}

			string ctorParamsStr = string.Join(
				", ",
				from quadruple in ctorArgsList
				let memberTypeStr = quadruple.MemberType.ToDisplayString(TypeFormats.FullName)
				let paramterName = quadruple.Name.ToCamelCase()
				select $"{memberTypeStr} {paramterName}"
			);

			string assignmentStr = string.Join(
				" | ",
				from quadruple in ctorArgsList
				let startIndex = quadruple.Start
				let endIndex = quadruple.End
				let leftShiftingStr = startIndex != 0 ? $" << {startIndex}" : string.Empty
				let name = quadruple.Name.ToCamelCase()
				select $"{name}{leftShiftingStr}"
			);

			string properties = string.Join(
				"\r\n\r\n",
				from quadruple in ctorArgsList
				let startIndex = quadruple.Start
				let endIndex = quadruple.End
				let rightShiftingStr = startIndex != 0 ? $" >> {startIndex}" : string.Empty
				let bitwiseAndStr = $" & {(1 << endIndex - startIndex) - 1}"
				let memberType = quadruple.MemberType.ToDisplayString(TypeFormats.FullName)
				select
					$$"""
						[global::System.Runtime.CompilerServices.CompilerGenerated]
						[global::System.CodeDom.Compiler.GeneratedCode("{{typeof(MaskStyledDataTypeGenerator).FullName}}", "{{VersionValue}}")]
						public {{readOnlyKeyword}}{{memberType}} {{quadruple.Name}}
						{
							[global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
							[global::System.Runtime.CompilerServices.CompilerGenerated]
							[global::System.CodeDom.Compiler.GeneratedCode("{{typeof(MaskStyledDataTypeGenerator).FullName}}", "{{VersionValue}}")]
							get => _mask{{rightShiftingStr}}{{bitwiseAndStr}};
						}
					"""
			);

			spc.AddSource(
				$"{type.ToFileName()}.g.{Shortcuts.MaskStyledDataType}.cs",
				$$"""
				#pragma warning disable CS1591
				
				namespace {{namespaceName}};
				
				partial {{type.GetTypeKindModifier()}} {{typeName}}{{genericParameterList}}
				{
					[global::System.Runtime.CompilerServices.CompilerGenerated]
					[global::System.CodeDom.Compiler.GeneratedCode("{{typeof(MaskStyledDataTypeGenerator).FullName}}", "{{VersionValue}}")]
					private readonly {{fieldType.ToDisplayString(TypeFormats.FullName)}} _mask;
					
					
					[global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
					[global::System.Runtime.CompilerServices.CompilerGenerated]
					[global::System.CodeDom.Compiler.GeneratedCode("{{typeof(MaskStyledDataTypeGenerator).FullName}}", "{{VersionValue}}")]
					public {{typeName}}({{ctorParamsStr}}) => _mask = {{assignmentStr}};
					
					
					{{properties}}
					
					
					/// <inheritdoc/>
					[global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
					[global::System.Runtime.CompilerServices.CompilerGenerated]
					[global::System.CodeDom.Compiler.GeneratedCode("{{typeof(MaskStyledDataTypeGenerator).FullName}}", "{{VersionValue}}")]
					public override {{readOnlyKeyword}}int {{nameof(GetHashCode)}}() => _mask;
				}
				"""
			);

			recordedList.Add(type);
		}
	}
}
