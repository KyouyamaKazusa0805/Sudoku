namespace Sudoku.SourceGeneration.Handlers;

internal static class InlineArrayFieldHandler
{
	public static string? Transform(GeneratorAttributeSyntaxContext gasc, CancellationToken _)
	{
		if (gasc is not
			{
				Attributes: [
					{
						ConstructorArguments: [{ Value: string fieldName and not [] }, { Value: int length and > 0 }],
						AttributeClass.TypeArguments: [var fieldType]
					}
				],
				TargetSymbol: INamedTypeSymbol
				{
					TypeKind: TypeKind.Struct,
					ContainingNamespace: var @namespace,
					ContainingType: null,
					Name: var typeName,
					TypeParameters: var typeParameters,
					IsRecord: false,
					IsReadOnly: false,
					IsFileLocal: false
				} type,
				SemanticModel.Compilation: var compilation
			})
		{
			return null;
		}

		const string largeStructAttributeName = "System.SourceGeneration.LargeStructureAttribute";
		if (compilation.GetTypeByMetadataName(largeStructAttributeName) is not { } largeStructAttribute)
		{
			return null;
		}

		var isLargeStructure = type.GetAttributes().Any(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, largeStructAttribute));
		var namespaceString = @namespace.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)["global::".Length..];
		var typeParametersString = typeParameters is not [] ? $"<{string.Join(", ", typeParameters)}>" : string.Empty;
		var inKeyword = isLargeStructure ? "in " : string.Empty;
		return $$"""
			namespace {{namespaceString}}
			{
				partial struct {{typeName}}{{typeParametersString}}
				{
					/// <summary>
					/// Indicates the internal field that provides the visit entry for fixed-sized buffer type <see cref="__InternalBuffer"/>.
					/// </summary>
					/// <seealso cref="__InternalBuffer"/>
					[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute]
					[global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{typeof(InlineArrayFieldHandler).FullName}}", "{{Value}}")]
					private __InternalBuffer {{fieldName}};


					/// <summary>
					/// Indicates the internal buffer type.
					/// </summary>
					[global::System.Runtime.CompilerServices.InlineArrayAttribute({{length}})]
					[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute]
					[global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{typeof(InlineArrayFieldHandler).FullName}}", "{{Value}}")]
					private struct __InternalBuffer : global::System.IEquatable<__InternalBuffer>, global::System.Numerics.IEqualityOperators<__InternalBuffer, __InternalBuffer, bool>
					{
						/// <summary>
						/// Indicates the first element of the whole buffer.
						/// </summary>
						[global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Style", "IDE0044:Add readonly modifier", Justification = "<Pending>")]
						[global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
						[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute]
						[global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{typeof(InlineArrayFieldHandler).FullName}}", "{{Value}}")]
						private {{fieldType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}} _firstElement;


						/// <inheritdoc/>
						[global::System.Runtime.CompilerServices.MethodImplAttribute(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
						[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute]
						[global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{typeof(InlineArrayFieldHandler).FullName}}", "{{Value}}")]
						public override readonly bool Equals([global::System.Diagnostics.CodeAnalysis.NotNullWhenAttribute(true)] object? obj) => obj is __InternalBuffer comparer && Equals({{inKeyword}}comparer);

						/// <inheritdoc/>
						[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute]
						[global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{typeof(InlineArrayFieldHandler).FullName}}", "{{Value}}")]
						public readonly bool Equals(ref readonly __InternalBuffer other)
						{
							for (var i = 0; i < {{length}}; i++)
							{
								if (this[i] != other[i])
								{
									return false;
								}
							}

							return true;
						}

						/// <inheritdoc/>
						[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute]
						[global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{typeof(InlineArrayFieldHandler).FullName}}", "{{Value}}")]
						public override readonly int GetHashCode()
						{
							var hashCode = new global::System.HashCode();
							for (var i = 0; i < {{length}}; i++)
							{
								hashCode.Add(this[i]);
							}

							return hashCode.ToHashCode();
						}

						/// <inheritdoc/>
						[global::System.Runtime.CompilerServices.MethodImplAttribute(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
						[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute]
						[global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{typeof(InlineArrayFieldHandler).FullName}}", "{{Value}}")]
						readonly bool global::System.IEquatable<__InternalBuffer>.Equals(__InternalBuffer other) => Equals({{inKeyword}}other);


						/// <inheritdoc cref="global::System.Numerics.IEqualityOperators{TSelf, TOther, TResult}.op_Equality(TSelf, TOther)"/>
						[global::System.Runtime.CompilerServices.MethodImplAttribute(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
						[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute]
						[global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{typeof(InlineArrayFieldHandler).FullName}}", "{{Value}}")]
						public static bool operator ==(in __InternalBuffer left, in __InternalBuffer right) => left.Equals({{inKeyword}}right);

						/// <inheritdoc cref="global::System.Numerics.IEqualityOperators{TSelf, TOther, TResult}.op_Inequality(TSelf, TOther)"/>
						[global::System.Runtime.CompilerServices.MethodImplAttribute(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
						[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute]
						[global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{typeof(InlineArrayFieldHandler).FullName}}", "{{Value}}")]
						public static bool operator !=(in __InternalBuffer left, in __InternalBuffer right) => !(left == right);

						/// <inheritdoc/>
						[global::System.Runtime.CompilerServices.MethodImplAttribute(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
						[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute]
						[global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{typeof(InlineArrayFieldHandler).FullName}}", "{{Value}}")]
						static bool global::System.Numerics.IEqualityOperators<__InternalBuffer, __InternalBuffer, bool>.operator ==(__InternalBuffer left, __InternalBuffer right) => left == right;

						/// <inheritdoc/>
						[global::System.Runtime.CompilerServices.MethodImplAttribute(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
						[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute]
						[global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{typeof(InlineArrayFieldHandler).FullName}}", "{{Value}}")]
						static bool global::System.Numerics.IEqualityOperators<__InternalBuffer, __InternalBuffer, bool>.operator !=(__InternalBuffer left, __InternalBuffer right) => left != right;
					}
				}
			}
			""";
	}

	public static void Output(SourceProductionContext spc, ImmutableArray<string> value)
		=> spc.AddSource(
			"InlineArrayField.g.cs",
			$"""
			{Banner.AutoGenerated}
			
			#nullable enable
			
			{string.Join("\r\n\r\n", value)}
			"""
		);
}
