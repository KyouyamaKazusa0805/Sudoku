namespace Sudoku.Diagnostics.CodeGen.Generators;

/// <summary>
/// Defines a source generator that generates the source code on implementation for the type <see cref="IEnumerable{T}"/>.
/// </summary>
/// <seealso cref="IEnumerable{T}"/>
[Generator(LanguageNames.CSharp)]
public sealed partial class AutoImplementsEnumerableGenerator : ISourceGenerator
{
	/// <inheritdoc/>
	public void Execute(GeneratorExecutionContext context)
	{
		if (context is not { SyntaxContextReceiver: Receiver { Collection: var collection } })
		{
			return;
		}

		foreach (var (type, attributeData) in collection)
		{
			if (
#pragma warning disable IDE0055
				attributeData is not
				{
					ConstructorArguments: [
						{ Value: INamedTypeSymbol elementType },
						{ Value: var memberName }
					]
				}
#pragma warning restore IDE0055
			)
			{
				return;
			}

			var (_, _, namespaceName, genericParameterList, _, _, readOnlyKeyword, _, _, _) = SymbolOutputInfo.FromSymbol(type);

			string fullName = type.ToDisplayString(TypeFormats.FullName);
			bool @explicit = attributeData.GetNamedArgument<bool>("UseExplicitImplementation");
			string elementTypeFullName = elementType.ToDisplayString(TypeFormats.FullName);
			string[] rawConversionExprs = attributeData.GetNamedArgument("ConversionExpression", "*")!.Split('|');
			string[] conversionExprs = rawConversionExprs is [var first] ? new[] { first, "@.*" } : rawConversionExprs;
			convert(ref conversionExprs[0]);
			convert(ref conversionExprs[1]);

			string genericMethodSignature = @explicit
				? $"{readOnlyKeyword}global::System.Collections.Generic.IEnumerator<{elementTypeFullName}> global::System.Collections.Generic.IEnumerable<{elementTypeFullName}>.GetEnumerator()"
				: $"public {readOnlyKeyword}{elementTypeFullName} GetEnumerator()";
			string genericMethodBody = conversionExprs[0];
			string methodSignature = $"{readOnlyKeyword}global::System.Collections.IEnumerator global::System.Collections.IEnumerable.GetEnumerator()";
			string methodBody = conversionExprs[1];

			context.AddSource(
				type.ToFileName(),
				"ag",
				$$"""
				#nullable enable
				
				using global::System.Collections;
				using global::System.Collections.Generic;
				
				namespace {{namespaceName}};
				
				partial {{type.GetTypeKindModifier()}} {{type.Name}}{{genericParameterList}}
				{
					/// <inheritdoc/>
					[global::System.Runtime.CompilerServices.CompilerGenerated]
					[global::System.CodeDom.Compiler.GeneratedCode("{{GetType().FullName}}", "{{VersionValue}}")]
					[global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
					{{methodSignature}}
						=> {{methodBody}};
					
					/// <inheritdoc/>
					[global::System.Runtime.CompilerServices.CompilerGenerated]
					[global::System.CodeDom.Compiler.GeneratedCode("{{GetType().FullName}}", "{{VersionValue}}")]
					[global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
					{{genericMethodSignature}}
						=> {{genericMethodBody}};
				}
				"""
			);


			void convert(ref string conversionExpr)
				=> conversionExpr = conversionExpr
					.Replace("!", $"global::{elementTypeFullName}")
					.Replace("@", (string)memberName!)
					.Replace("*", "GetEnumerator()");
		}
	}

	/// <inheritdoc/>
	public void Initialize(GeneratorInitializationContext context)
		=> context.RegisterForSyntaxNotifications(() => new Receiver(context.CancellationToken));
}
