namespace Sudoku.Diagnostics.CodeGen.Generators;

/// <summary>
/// Defines a source generator that generates source code as operator overloading statements.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class AutoOverloadingOperatorGenerator : IIncrementalGenerator
{
	/// <inheritdoc/>
	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		context.RegisterSourceOutput(
			context.SyntaxProvider
				.ForAttributeWithMetadataName("System.Diagnostics.CodeGen.GeneratedOverloadingOperatorAttribute", nodePredicate, transform)
				.Where(static data => data is not null)
				.Collect(),
			output
		);


		static bool nodePredicate(SyntaxNode node, CancellationToken _)
			=> node is TypeDeclarationSyntax { Modifiers: var m and not [] } and (StructDeclarationSyntax or ClassDeclarationSyntax)
			&& m.Any(SyntaxKind.PartialKeyword);

		static bool typeIsLargeStruct(INamedTypeSymbol typeSymbol, INamedTypeSymbol largeStructAttribute)
			=> (
				from attributeData in typeSymbol.GetAttributes()
				where SymbolEqualityComparer.Default.Equals(attributeData.AttributeClass, largeStructAttribute)
				select attributeData
			).Any();

		static string f(INamedTypeSymbol typeSymbol) => typeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

		static Data? transform(GeneratorAttributeSyntaxContext gasc, CancellationToken _)
		{
			return gasc switch
			{
				{
					Attributes: var attributes and not [],
					TargetSymbol: INamedTypeSymbol typeSymbol,
					SemanticModel.Compilation: var compilation
				} => new((Operator)attributes.Aggregate(0, aggregateFunc), typeSymbol, compilation),
				_ => null,
			};


			static int aggregateFunc(int interim, AttributeData next)
				=> interim | next.ConstructorArguments switch { [{ Value: int rawValue and not 0 }] => rawValue, _ => 0 };
		}

		void output(SourceProductionContext spc, ImmutableArray<Data?> data)
		{
			var codeSnippet = new List<string>();
			foreach (var (operatorKinds, typeSymbol, compilation) in data.CastToNotNull())
			{
				var largeStructAttribute = compilation.GetTypeByMetadataName("System.Diagnostics.CodeAnalysis.IsLargeStructAttribute")!;
				var @namespace = typeSymbol.ContainingNamespace.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
				var typeName = typeSymbol.Name;
				var typeKind = typeSymbol.GetTypeKindModifier();

				var operatorsCodeSnippet = new List<string>();
				foreach (var element in operatorKinds)
				{
					var action = element switch
					{
						Operator.Equality => op_Equality,
						Operator.Inequality => op_Inequality,
						Operator.GreaterThan => op_GreaterThan,
						Operator.GreaterThanOrEqual => op_GreaterThanOrEqual,
						Operator.LessThan => op_LessThan,
						Operator.LessThanOrEqual => op_LessThanOrEqual,
						_ => default(Func<INamedTypeSymbol, string>?)
					};

					if (action is not null)
					{
						operatorsCodeSnippet.Add(action(typeSymbol));
					}
				}

				codeSnippet.Add(
					$$"""
					namespace {{@namespace["global::".Length..]}}
					{
						partial {{typeKind}} {{typeName}}
						{
						{{string.Join("\r\n\r\n\t", operatorsCodeSnippet)}}
						}
					}
					"""
				);


				string op_Equality(INamedTypeSymbol typeSymbol)
				{
					var isLargeStruct = typeIsLargeStruct(typeSymbol, largeStructAttribute);
					var fullName = isLargeStruct
						? $"scoped in {f(typeSymbol)}"
						: typeSymbol.IsRefLikeType
							? $"scoped {f(typeSymbol)}"
							: typeSymbol.TypeKind == TypeKind.Class ? $"{f(typeSymbol)}?" : f(typeSymbol);
					var crefText = isLargeStruct
						? $@" cref=""IEqualityOperators{{TSelf, TOther, TResult}}.{nameof(op_Equality)}(TSelf, TOther)"""
						: string.Empty;
					var executingCode = typeSymbol.TypeKind == TypeKind.Class
						? "(left, right) switch { (null, null) => true, (not null, not null) => left.Equals(right), _ => false }"
						: "left.Equals(right)";

					return
						$"""
							/// <inheritdoc{crefText}/>
								[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute]
								[global::System.CodeDom.Compiler.GeneratedCodeAttribute("{GetType().FullName}", "{VersionValue}")]
								[global::System.Runtime.CompilerServices.MethodImplAttribute(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
								public static bool operator ==({fullName} left, {fullName} right)
									=> {executingCode};
						""";
				}

				string op_Inequality(INamedTypeSymbol typeSymbol)
				{
					var isLargeStruct = typeIsLargeStruct(typeSymbol, largeStructAttribute);
					var fullName = isLargeStruct
						? $"scoped in {f(typeSymbol)}"
						: typeSymbol.IsRefLikeType
							? $"scoped {f(typeSymbol)}"
							: typeSymbol.TypeKind == TypeKind.Class ? $"{f(typeSymbol)}?" : f(typeSymbol);
					var crefText = isLargeStruct || typeSymbol.IsRefLikeType
						? $@" cref=""IEqualityOperators{{TSelf, TOther, TResult}}.{nameof(op_Inequality)}(TSelf, TOther)"""
						: string.Empty;

					return
						$"""
							/// <inheritdoc{crefText}/>
								[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute]
								[global::System.CodeDom.Compiler.GeneratedCodeAttribute("{GetType().FullName}", "{VersionValue}")]
								[global::System.Runtime.CompilerServices.MethodImplAttribute(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
								public static bool operator !=({fullName} left, {fullName} right)
									=> !(left == right);
						""";
				}

				string op_GreaterThan(INamedTypeSymbol typeSymbol)
				{
					var isLargeStruct = typeIsLargeStruct(typeSymbol, largeStructAttribute);
					var fullName = isLargeStruct
						? $"scoped in {f(typeSymbol)}"
						: typeSymbol.IsRefLikeType ? $"scoped {f(typeSymbol)}" : f(typeSymbol);
					var crefText = isLargeStruct
						? $@" cref=""IComparisonOperators{{TSelf, TOther, TResult}}.{nameof(op_GreaterThan)}(TSelf, TOther)"""
						: string.Empty;

					return
						$"""
							/// <inheritdoc{crefText}/>
								[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute]
								[global::System.CodeDom.Compiler.GeneratedCodeAttribute("{GetType().FullName}", "{VersionValue}")]
								[global::System.Runtime.CompilerServices.MethodImplAttribute(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
								public static bool operator >({fullName} left, {fullName} right)
									=> left.CompareTo(right) > 0;
						""";
				}

				string op_GreaterThanOrEqual(INamedTypeSymbol typeSymbol)
				{
					var isLargeStruct = typeIsLargeStruct(typeSymbol, largeStructAttribute);
					var fullName = isLargeStruct
						? $"scoped in {f(typeSymbol)}"
						: typeSymbol.IsRefLikeType ? $"scoped {f(typeSymbol)}" : f(typeSymbol);
					var crefText = isLargeStruct
						? $@" cref=""IComparisonOperators{{TSelf, TOther, TResult}}.{nameof(op_GreaterThanOrEqual)}(TSelf, TOther)"""
						: string.Empty;

					return
						$"""
							/// <inheritdoc{crefText}/>
								[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute]
								[global::System.CodeDom.Compiler.GeneratedCodeAttribute("{GetType().FullName}", "{VersionValue}")]
								[global::System.Runtime.CompilerServices.MethodImplAttribute(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
								public static bool operator >=({fullName} left, {fullName} right)
									=> left.CompareTo(right) >= 0;
						""";
				}

				string op_LessThan(INamedTypeSymbol typeSymbol)
				{
					var isLargeStruct = typeIsLargeStruct(typeSymbol, largeStructAttribute);
					var fullName = isLargeStruct
						? $"scoped in {f(typeSymbol)}"
						: typeSymbol.IsRefLikeType ? $"scoped {f(typeSymbol)}" : f(typeSymbol);
					var crefText = isLargeStruct
						? $@" cref=""IComparisonOperators{{TSelf, TOther, TResult}}.{nameof(op_LessThan)}(TSelf, TOther)"""
						: string.Empty;

					return
						$"""
							/// <inheritdoc{crefText}/>
								[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute]
								[global::System.CodeDom.Compiler.GeneratedCodeAttribute("{GetType().FullName}", "{VersionValue}")]
								[global::System.Runtime.CompilerServices.MethodImplAttribute(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
								public static bool operator <({fullName} left, {fullName} right)
									=> left.CompareTo(right) < 0;
						""";
				}

				string op_LessThanOrEqual(INamedTypeSymbol typeSymbol)
				{
					var isLargeStruct = typeIsLargeStruct(typeSymbol, largeStructAttribute);
					var fullName = isLargeStruct
						? $"scoped in {f(typeSymbol)}"
						: typeSymbol.IsRefLikeType ? $"scoped {f(typeSymbol)}" : f(typeSymbol);
					var crefText = isLargeStruct
						? $@" cref=""IComparisonOperators{{TSelf, TOther, TResult}}.{nameof(op_LessThanOrEqual)}(TSelf, TOther)"""
						: string.Empty;

					return
						$"""
							/// <inheritdoc{crefText}/>
								[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute]
								[global::System.CodeDom.Compiler.GeneratedCodeAttribute("{GetType().FullName}", "{VersionValue}")]
								[global::System.Runtime.CompilerServices.MethodImplAttribute(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
								public static bool operator <=({fullName} left, {fullName} right)
									=> left.CompareTo(right) <= 0;
						""";
				}
			}

			spc.AddSource(
				$"OperatorsOverloading.g.{Shortcuts.GeneratedOverloadingOperator}.cs",
				$$"""
				// <auto-generated />

				#nullable enable

				{{string.Join("\r\n\r\n", codeSnippet)}}
				"""
			);
		}
	}
}

/// <summary>
/// Defines a kind of an operator.
/// </summary>
[Flags]
file enum Operator : int
{
	/// <summary>
	/// Indicates <c><see langword="operator"/> ==</c>.
	/// </summary>
	Equality = 1,

	/// <summary>
	/// Indicates <c><see langword="operator"/> !=</c>.
	/// </summary>
	Inequality = 1 << 1,

	/// <summary>
	/// Indicates <c><see langword="operator"/> <![CDATA[>]]></c>.
	/// </summary>
	GreaterThan = 1 << 2,

	/// <summary>
	/// Indicates <c><see langword="operator"/> <![CDATA[>=]]></c>.
	/// </summary>
	GreaterThanOrEqual = 1 << 3,

	/// <summary>
	/// Indicates <c><see langword="operator"/> <![CDATA[<]]></c>.
	/// </summary>
	LessThan = 1 << 4,

	/// <summary>
	/// Indicates <c><see langword="operator"/> <![CDATA[<=]]></c>.
	/// </summary>
	LessThanOrEqual = 1 << 5,
}

/// <summary>
/// Defines a gathered data tuple.
/// </summary>
/// <param name="OperatorKinds">The operator kinds.</param>
/// <param name="TypeSymbol">The type symbol.</param>
/// <param name="Compilation">Indicates the compilation.</param>
file readonly record struct Data(Operator OperatorKinds, INamedTypeSymbol TypeSymbol, Compilation Compilation);

/// <include file='../../global-doc-comments.xml' path='g/csharp11/feature[@name="file-local"]/target[@name="class" and @when="extension"]'/>
file static class Extensions
{
	/// <summary>
	/// Get an enumerator instance that is used by <see langword="foreach"/> loop.
	/// </summary>
	/// <typeparam name="T">The type of an enumeration.</typeparam>
	/// <param name="this">The current field to be iterated.</param>
	/// <returns>An <see cref="IEnumerator{T}"/> instance.</returns>
	public static IEnumerator<T> GetEnumerator<T>(this T @this) where T : unmanaged, Enum
	{
		foreach (var field in Enum.GetValues(typeof(T)).Cast<T>())
		{
			if (@this.HasFlag(field))
			{
				yield return field;
			}
		}
	}
}
