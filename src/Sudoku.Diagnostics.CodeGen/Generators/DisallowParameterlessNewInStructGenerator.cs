namespace Sudoku.Diagnostics.CodeGen.Generators;

/// <summary>
/// Defines a source generator that generates the source code that will disallow user using
/// <see langword="new"/> clauses to initialize an instance of <see langword="struct"/> types
/// without any parameters in constructor.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class DisallowParameterlessNewInStructGenerator : IIncrementalGenerator
{
	/// <inheritdoc/>
	public void Initialize(IncrementalGeneratorInitializationContext context)
		=> context.RegisterSourceOutput(
			context.SyntaxProvider
				.ForAttributeWithMetadataName(
					"System.Diagnostics.CodeGen.DisallowParameterlessConstructorAttribute",
					static (node, _) => node is StructDeclarationSyntax { TypeParameterList: null },
					static (gasc, ct) => ((INamedTypeSymbol)gasc.TargetSymbol, gasc.Attributes)
				)
				.Where(static gathered => gathered.Attributes.Length == 1)
				.Collect(),
			(spc, collection) =>
			{
				foreach (var (symbol, attributeData) in collection)
				{
					var (typeName, _, @namespace, _, _, _, _, _, _, _) = SymbolOutputInfo.FromSymbol(symbol);
					var message = attributeData switch
					{
						[{ NamedArguments: [] }]
							=> "The parameterless constructor is disallowed on purpose.",
						[{ NamedArguments: [{ Value.Value: string suggestedName }] }]
							=> $"Please use the member '{suggestedName}' instead.",
						_
							=> throw new InvalidOperationException("The argument is invalid.")
					};
					spc.AddSource(
						$"{symbol.ToFileName()}.g.{Shortcuts.DisallowParameterlessConstructorInStruct}.cs",
						$$"""
						namespace {{@namespace}};

						partial struct {{typeName}}
						{
							/// <summary>
							/// Throws a <see cref="global::System.NotSupportedException"/>-typed instance.
							/// </summary>
							/// <exception cref="global::System.NotSupportedException">The exception will always be thrown.</exception>
							/// <remarks>
							/// The main idea of the parameterless constructor is to create a new instance
							/// without any extra information, but the current type is special:
							/// It will be very slow to be initialized.
							/// I only suggest you use constant or static read-only field (or static read-only property)
							/// instead of using this constructor.
							/// </remarks>
							[global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Never)]
							[global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{GetType().FullName}}", "{{VersionValue}}")]
							[global::System.ObsoleteAttribute("{{message}}", true)]
							[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute]
							public {{typeName}}() => throw new global::System.NotSupportedException();
						}
						"""
					);
				}
			}
		);
}
