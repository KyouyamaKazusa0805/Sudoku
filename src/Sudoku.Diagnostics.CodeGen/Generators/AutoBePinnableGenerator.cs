namespace Sudoku.Diagnostics.CodeGen.Generators;

/// <summary>
/// Defines a source generator that generates the source code on implementation
/// for the method <c>GetPinnableReference</c>.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class AutoBePinnableGenerator : ISourceGenerator
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
				attributeData is not
				{
					ConstructorArguments: [{ Value: ITypeSymbol returnType }, { Value: string pattern }]
				}
			)
			{
				continue;
			}

			var (_, _, namespaceName, genericParameterList, _, _, readOnlyKeyword, _, _, _) =
				SymbolOutputInfo.FromSymbol(type);
			bool returnByReadOnlyRef = attributeData.GetNamedArgument("ReturnsReadOnlyReference", true);
			string refReadOnlyModifier = returnByReadOnlyRef ? "ref readonly" : "ref";
			string returnTypeFullName = returnType.ToDisplayString(TypeFormats.FullName);
			context.AddSource(
				type.ToFileName(),
				Shortcuts.AutoBePinnable,
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
					[global::System.CodeDom.Compiler.GeneratedCode("{{GetType().FullName}}", "{{VersionValue}}")]
					[global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
					public unsafe {{readOnlyKeyword}}{{refReadOnlyModifier}} {{returnTypeFullName}} GetPinnableReference()
						=> ref {{pattern}};
				}
				"""
			);
		}
	}

	/// <inheritdoc/>
	public void Initialize(GeneratorInitializationContext context)
		=> context.RegisterForSyntaxNotifications(() => new Receiver(context.CancellationToken));


	/// <summary>
	/// The inner syntax context receiver instance.
	/// </summary>
	/// <param name="CancellationToken">The cancellation token to cancel the operation.</param>
	private sealed record class Receiver(CancellationToken CancellationToken) : ISyntaxContextReceiver
	{
		/// <summary>
		/// Indicates the result collection.
		/// </summary>
		public ICollection<(INamedTypeSymbol Symbol, AttributeData)> Collection { get; } =
			new List<(INamedTypeSymbol, AttributeData)>();


		/// <inheritdoc/>
		public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
		{
			const string attributeFullName = "System.Diagnostics.CodeGen.AutoBePinnableAttribute";

			if (
				context is not
				{
					Node: TypeDeclarationSyntax { Modifiers: var modifiers and not [] } n and (
						ClassDeclarationSyntax or StructDeclarationSyntax
					),
					SemanticModel: { Compilation: { } compilation } semanticModel
				}
			)
			{
				return;
			}

			if (semanticModel.GetDeclaredSymbol(n, CancellationToken) is not { ContainingType: null } typeSymbol)
			{
				return;
			}

			var attributeTypeSymbol = compilation.GetTypeByMetadataName(attributeFullName);
			var attributeData = (
				from a in typeSymbol.GetAttributes()
				where SymbolEqualityComparer.Default.Equals(a.AttributeClass, attributeTypeSymbol)
				select a
			).FirstOrDefault();
			if (attributeData is null)
			{
				return;
			}

			if (!modifiers.Any(SyntaxKind.PartialKeyword))
			{
				return;
			}

			if (!Collection.Any(e => SymbolEqualityComparer.Default.Equals(e.Symbol, typeSymbol)))
			{
				Collection.Add((typeSymbol, attributeData));
			}
		}
	}
}
