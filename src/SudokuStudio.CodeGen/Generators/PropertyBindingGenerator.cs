namespace Sudoku.Diagnostics.CodeGen.Generators;

/// <summary>
/// Defines a source generator that is used for binding properties.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class PropertyBindingGenerator : IIncrementalGenerator
{
	/// <inheritdoc/>
	public void Initialize(IncrementalGeneratorInitializationContext context)
		=> context.RegisterSourceOutput(
			context.SyntaxProvider
				.ForAttributeWithMetadataName(
					"System.Diagnostics.CodeGen.NotifyPropertyChangedBackingFieldAttribute",
					static (node, _)
						=> node is VariableDeclaratorSyntax
						{
							Parent: VariableDeclarationSyntax
							{
								Variables.Count: 1,
								Parent: FieldDeclarationSyntax
								{
									Modifiers: var fieldModifiers,
									Parent: ClassDeclarationSyntax { Modifiers: var typeModifiers and not [] }
								}
							},
						}
						&& !fieldModifiers.Any(SyntaxKind.StaticKeyword)
						&& !fieldModifiers.Any(SyntaxKind.ReadOnlyKeyword)
						&& typeModifiers.Any(SyntaxKind.PartialKeyword),
					static (gasc, ct) =>
					{
						if (gasc is not
							{
								Attributes.Length: 1,
								TargetSymbol: IFieldSymbol
								{
									ContainingType: { MemberNames: var memberNames, Interfaces: var impledInterfaces } type,
									Name: var fieldName
								} fieldSymbol,
								SemanticModel.Compilation: var compilation
							})
						{
							return (Data?)null;
						}

						var controlType = compilation.GetTypeByMetadataName("Microsoft.UI.Xaml.Controls.Control");
						if (controlType is null)
						{
							return null;
						}

						if (!type.IsDerivedFrom(controlType))
						{
							return null;
						}

						var notifyPropertyChangedType = compilation.GetTypeByMetadataName(typeof(INotifyPropertyChanged).FullName)!;
						if (!impledInterfaces.Contains(notifyPropertyChangedType, SymbolEqualityComparer.Default))
						{
							return null;
						}

						var propertyChangedEventHandlerType = compilation.GetTypeByMetadataName(typeof(PropertyChangedEventHandler).FullName)!;
						if (!type.GetMembers().OfType<IEventSymbol>().Any(containsPropertyChangedEvent))
						{
							return null;
						}

						var propertyName = fieldName.ToPascalCasing();
						if (memberNames.Contains(propertyName))
						{
							return null;
						}

						return new(fieldName, propertyName, fieldSymbol, type, compilation);


						bool containsPropertyChangedEvent(IEventSymbol e)
							=> e is
							{
								Name: nameof(INotifyPropertyChanged.PropertyChanged),
								ExplicitInterfaceImplementations: [],
								Type: var eventType
							} && SymbolEqualityComparer.Default.Equals(propertyChangedEventHandlerType, eventType);
					}
				)
				.Where(static data => data is not null)
				.Collect(),
			(spc, data) =>
			{
				foreach (var group in data.CastToNotNull().GroupBy<Data, INamedTypeSymbol>(keySelector, SymbolEqualityComparer.Default))
				{
					var type = group.Key;

					var propertyDeclarations = new List<string>();
					foreach (var (field, property, fieldSymbol, _, compilation) in group)
					{
						if (fieldSymbol is not
							{
								Type:
								{
									AllInterfaces: var allInterfaces,
									SpecialType: var fieldSpecialType,
									TypeKind: var fieldTypeKind
								} fieldType
							})
						{
							continue;
						}

						var fieldTypeStr = fieldType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
						var equalityOperatorsType = compilation.GetTypeByMetadataName("System.Numerics.IEqualityOperators`3")!;
						var equatableType = compilation.GetTypeByMetadataName(typeof(IEquatable<>).FullName)!;
						var comparableType = compilation.GetTypeByMetadataName(typeof(IComparable<>).FullName)!;
						var valueComparisonCode = containsEqualityOperators()
							? $$"""{{field}} == value"""
							: containsEqualsMethod()
								? $$"""{{field}}.Equals(value)"""
								: containsCompareToMethod()
									? $$"""{{field}}.CompareTo(value) == 0"""
									: $$"""global::System.Collections.Generic.EqualityComparer<{{fieldTypeStr}}>.Default.Equals({{field}}, value)""";

						propertyDeclarations.Add(
							$$"""
							/// <inheritdoc cref="{{field}}"/>
								[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute]
								[global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{GetType().FullName}}", "{{VersionValue}}")]
								public {{fieldTypeStr}} {{property}}
								{
									get => {{field}};

									set
									{
										if ({{valueComparisonCode}})
										{
											return;
										}

										{{field}} = value;

										PropertyChanged?.Invoke(this, new(nameof({{property}})));
									}
								}
							"""
						);


						bool containsEqualityOperators()
							=> allInterfaces.Contains(equalityOperatorsType, SymbolEqualityComparer.Default)
							|| fieldSpecialType is >= SpecialType.System_Object and <= SpecialType.System_UIntPtr and not (SpecialType.System_ValueType or SpecialType.System_Void)
							|| fieldTypeKind == TypeKind.Enum
							|| fieldType.GetMembers().OfType<IMethodSymbol>().Any(static m => m.Name == "op_Equality");

						bool containsEqualsMethod() => allInterfaces.Contains(equatableType, SymbolEqualityComparer.Default);

						bool containsCompareToMethod() => allInterfaces.Contains(comparableType, SymbolEqualityComparer.Default);
					}

					var @namespace = type.ContainingNamespace.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
					spc.AddSource(
						$"{type.ToFileName()}.g.{Shortcuts.PropertyBinding}.cs",
						$$"""
						// <auto-generated />

						#nullable enable

						namespace {{@namespace["global::".Length..]}};

						partial {{type.GetTypeKindModifier()}} {{type.Name}}
						{
							{{string.Join("\r\n\r\n\t", propertyDeclarations)}}
						}
						"""
					);
				}


				static INamedTypeSymbol keySelector(Data data) => data.Type;
			}
		);
}

/// <summary>
/// Defines a gathered data tuple.
/// </summary>
/// <param name="OriginalFieldName">Indicates the original field name.</param>
/// <param name="PropertyName">The property name.</param>
/// <param name="FieldSymbol">Indicates the field symbol.</param>
/// <param name="Type">Indicates the containing type symbol.</param>
/// <param name="Compilation">Indicates the compilation.</param>
file readonly record struct Data(string OriginalFieldName, string PropertyName, IFieldSymbol FieldSymbol, INamedTypeSymbol Type, Compilation Compilation);

/// <include file='../../global-doc-comments.xml' path='g/csharp11/feature[@name="file-local"]/target[@name="class" and @when="extension"]'/>
file static class Extensions
{
	/// <summary>
	/// Converts the specified string in camel-casing into pascal-casing.
	/// </summary>
	/// <param name="this">The string.</param>
	/// <returns>The converted result.</returns>
	public static string ToPascalCasing(this string @this)
		=> @this switch
		{
			[] => @this,
			['_', .. var slice] => ToPascalCasing(slice),
			[var first and >= 'a' and <= 'z', .. var slice] => $"{char.ToUpper(first)}{slice}",
			[>= 'A' and <= 'Z', ..] => @this,
			[var first and >= '0' and <= '9', .. var slice] => $"_{first}{slice}",
			_ => throw new ArgumentException("The specified string is invalid.", nameof(@this))
		};
}
