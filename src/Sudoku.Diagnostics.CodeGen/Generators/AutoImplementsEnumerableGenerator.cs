namespace Sudoku.Diagnostics.CodeGen.Generators;

/// <summary>
/// Defines a source generator that generates the source code on implementation for the type <see cref="IEnumerable{T}"/>.
/// </summary>
/// <seealso cref="IEnumerable{T}"/>
[Generator(LanguageNames.CSharp)]
public sealed class AutoImplementsEnumerableGenerator : IIncrementalGenerator
{
	private const string AttributeFullName = "System.Diagnostics.CodeGen.AutoImplementsEnumerableAttribute";


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
			if (
#pragma warning disable IDE0055
				v is not (
					var type,
					{
						ConstructorArguments: [
							{ Value: INamedTypeSymbol elementType },
							{ Value: var memberName }
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

			string fullName = type.ToDisplayString(TypeFormats.FullName);
			bool @explicit = attributeData.GetNamedArgument<bool>("UseExplicitImplementation");
			string elementTypeFullName = elementType.ToDisplayString(TypeFormats.FullName);
			string[] rawConversionExprs = attributeData.GetNamedArgument("Pattern", "*")!.Split('|');
			string[] conversionExprs = rawConversionExprs is [var first] ? new[] { first, "@.*" } : rawConversionExprs;
			convert(ref conversionExprs[0]);
			convert(ref conversionExprs[1]);

			string genericMethodSignature = @explicit
				? $"{readOnlyKeyword}global::System.Collections.Generic.IEnumerator<{elementTypeFullName}> global::System.Collections.Generic.IEnumerable<{elementTypeFullName}>.GetEnumerator()"
				: $"public {readOnlyKeyword}{elementTypeFullName} GetEnumerator()";
			string genericMethodBody = conversionExprs[0];
			string methodSignature = $"{readOnlyKeyword}global::System.Collections.IEnumerator global::System.Collections.IEnumerable.GetEnumerator()";
			string methodBody = conversionExprs[1];

			spc.AddSource(
				$"{type.ToFileName()}.g.{Shortcuts.AutoImplementsEnumerable}.cs",
				$$"""
				#nullable enable
				
				using global::System.Collections;
				using global::System.Collections.Generic;
				
				namespace {{namespaceName}};
				
				partial {{type.GetTypeKindModifier()}} {{type.Name}}{{genericParameterList}}
				{
					/// <inheritdoc/>
					[global::System.Runtime.CompilerServices.CompilerGenerated]
					[global::System.CodeDom.Compiler.GeneratedCode("{{typeof(AutoImplementsEnumerableGenerator).FullName}}", "{{VersionValue}}")]
					[global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
					{{methodSignature}}
						=> {{methodBody}};
					
					/// <inheritdoc/>
					[global::System.Runtime.CompilerServices.CompilerGenerated]
					[global::System.CodeDom.Compiler.GeneratedCode("{{typeof(AutoImplementsEnumerableGenerator).FullName}}", "{{VersionValue}}")]
					[global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
					{{genericMethodSignature}}
						=> {{genericMethodBody}};
				}
				"""
			);

			recordedList.Add(type);


			void convert(ref string conversionExpr)
				=> conversionExpr = conversionExpr
					.Replace("!", $"global::{elementTypeFullName}")
					.Replace("@", (string)memberName!)
					.Replace("*", "GetEnumerator()");
		}
	}
}
