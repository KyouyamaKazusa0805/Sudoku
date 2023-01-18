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
					"System.Diagnostics.CodeGen.NotifyBackingFieldAttribute",
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
								Attributes: [{ NamedArguments: var namedArguments }],
								TargetSymbol: IFieldSymbol
								{
									ContainingType: { MemberNames: var memberNames, Interfaces: var impledInterfaces } type,
									Type:
									{
										AllInterfaces: var allInterfaces,
										SpecialType: var fieldSpecialType,
										TypeKind: var fieldTypeKind
									} fieldType,
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

						var equalityOperatorsType = compilation.GetTypeByMetadataName("System.Numerics.IEqualityOperators`3")!;
						var equatableType = compilation.GetTypeByMetadataName(typeof(IEquatable<>).FullName)!;
						var comparableType = compilation.GetTypeByMetadataName(typeof(IComparable<>).FullName)!;
						var mode = containsEqualityOperators()
							? EqualityComparisonMode.EqualityOperator
							: containsEqualsMethod()
								? EqualityComparisonMode.InstanceEqualsMethod
								: containsCompareToMethod()
									? EqualityComparisonMode.InstanceCompareToMethod
									: EqualityComparisonMode.EqualityComparerDefaultInstance;

						var accessibility = (Accessibility?)null;
						foreach (var (name, value) in namedArguments)
						{
							if (name == nameof(Accessibility))
							{
								accessibility = (Accessibility)(int)value.Value!;
							}
						}

						return new(propertyName, fieldSymbol, type, mode, accessibility ?? Accessibility.Public);


						bool containsPropertyChangedEvent(IEventSymbol e)
							=> e is
							{
								Name: nameof(INotifyPropertyChanged.PropertyChanged),
								ExplicitInterfaceImplementations: [],
								Type: var eventType
							} && SymbolEqualityComparer.Default.Equals(propertyChangedEventHandlerType, eventType);

						bool containsEqualityOperators()
							=> allInterfaces.Contains(equalityOperatorsType, SymbolEqualityComparer.Default)
							|| fieldSpecialType is >= SpecialType.System_Object and <= SpecialType.System_UIntPtr and not (SpecialType.System_ValueType or SpecialType.System_Void)
							|| fieldTypeKind == TypeKind.Enum
							|| fieldType.GetMembers().OfType<IMethodSymbol>().Any(static m => m.Name == "op_Equality");

						bool containsEqualsMethod() => allInterfaces.Contains(equatableType, SymbolEqualityComparer.Default);

						bool containsCompareToMethod() => allInterfaces.Contains(comparableType, SymbolEqualityComparer.Default);
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
					foreach (var (property, fieldSymbol, _, mode, accessibility) in group)
					{
						if (fieldSymbol is not { Name: var field, Type: { NullableAnnotation: var nullability } fieldType })
						{
							continue;
						}

						var nullableAnnotation = nullability == NullableAnnotation.Annotated ? "?" : string.Empty;
						var fieldTypeStr = $"{fieldType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}{nullableAnnotation}";
						var valueComparisonCode = mode switch
						{
							EqualityComparisonMode.EqualityOperator => $$"""{{field}} == value""",
							EqualityComparisonMode.InstanceEqualsMethod => $$"""{{field}}.Equals(value)""",
							EqualityComparisonMode.InstanceCompareToMethod => $$"""{{field}}.CompareTo(value) == 0""",
							EqualityComparisonMode.EqualityComparerDefaultInstance => $$"""global::System.Collections.Generic.EqualityComparer<{{fieldTypeStr}}>.Default.Equals({{field}}, value)""",
							_ => "true"
						};

						var accessibilityStr = accessibility switch
						{
							Accessibility.File => "file",
							Accessibility.Private => "private",
							Accessibility.Protected => "protected",
							Accessibility.PrivateProtected => "private protected",
							Accessibility.Internal => "internal",
							Accessibility.ProtectedInternal => "protected internal",
							Accessibility.Public => "public",
							_ => throw new NotSupportedException("The value is not defined.")
						};

						propertyDeclarations.Add(
							$$"""
							/// <inheritdoc cref="{{field}}"/>
								[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute]
								[global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{GetType().FullName}}", "{{VersionValue}}")]
								{{accessibilityStr}} {{fieldTypeStr}} {{property}}
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
/// Defines a mode to limit value's equality comparison.
/// </summary>
file enum EqualityComparisonMode
{
	/// <summary>
	/// Indicates the mode is none.
	/// </summary>
	None,

	/// <summary>
	/// Indicates the comparison mode is to call <c><see langword="operator"/> ==</c> to check whether two instances are considered equal.
	/// </summary>
	EqualityOperator,

	/// <summary>
	/// Indicates the comparison mode is to call <c><see cref="IEquatable{T}.Equals(T)"/></c>
	/// to check whether two instances are considered equal.
	/// </summary>
	InstanceEqualsMethod,

	/// <summary>
	/// Indicates the comparison mode is to call <c><see cref="IComparable{T}.CompareTo(T)"/></c>
	/// to check whether two instances are considered equal.
	/// </summary>
	InstanceCompareToMethod,

	/// <summary>
	/// Indicates the comparison mode is to call <c><see cref="EqualityComparer{T}.Equals(T, T)"/></c>
	/// via instance <see cref="EqualityComparer{T}.Default"/> to check whether two instances are considered equal.
	/// </summary>
	EqualityComparerDefaultInstance
}

/// <summary>
/// Defines an accessibility kind.
/// </summary>
file enum Accessibility
{
	None,

	/// <summary>
	/// Indicates the accessibility is <see langword="file"/>-scoped.
	/// </summary>
	File,

	/// <summary>
	/// Indicates the accessibility is <see langword="private"/>.
	/// </summary>
	Private,

	/// <summary>
	/// Indicates the accessibility is <see langword="protected"/>.
	/// </summary>
	Protected,

	/// <summary>
	/// Indicates the accessibility is <see langword="private protected"/>.
	/// </summary>
	PrivateProtected,

	/// <summary>
	/// Indicates the accessibility is <see langword="internal"/>.
	/// </summary>
	Internal,

	/// <summary>
	/// Indicates the accessibility is <see langword="protected internal"/>.
	/// </summary>
	ProtectedInternal,

	/// <summary>
	/// Indicates the accessibility is <see langword="public"/>.
	/// </summary>
	Public
}

/// <summary>
/// Defines a gathered data tuple.
/// </summary>
/// <param name="PropertyName">The property name.</param>
/// <param name="FieldSymbol">Indicates the field symbol.</param>
/// <param name="Type">Indicates the containing type symbol.</param>
/// <param name="ComparisonMode">Indicates a mode to compare two instances, used by property setter.</param>
/// <param name="Accessibility">Indicates the accessibility.</param>
file readonly record struct Data(
	string PropertyName,
	IFieldSymbol FieldSymbol,
	INamedTypeSymbol Type,
	EqualityComparisonMode ComparisonMode,
	Accessibility Accessibility
);

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
