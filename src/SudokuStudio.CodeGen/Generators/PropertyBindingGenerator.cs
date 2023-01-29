namespace Sudoku.Diagnostics.CodeGen.Generators;

/// <summary>
/// Defines a source generator that is used for binding properties.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class PropertyBindingGenerator : IIncrementalGenerator
{
	/// <inheritdoc/>
	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		context.RegisterSourceOutput(
			context.SyntaxProvider
				.ForAttributeWithMetadataName("System.Diagnostics.CodeGen.NotifyBackingFieldAttribute", nodePredicate, transform)
				.Where(static data => data is not null)
				.Collect(),
			output
		);


		static bool nodePredicate(SyntaxNode node, CancellationToken _)
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
			&& typeModifiers.Any(SyntaxKind.PartialKeyword);

		static Data? transform(GeneratorAttributeSyntaxContext gasc, CancellationToken _)
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
				return null;
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

			var propertyName = fieldName.ToPascalCasing();
			if (memberNames.Contains(propertyName))
			{
				return null;
			}

			var callbackAttributeType = compilation.GetTypeByMetadataName("System.Diagnostics.CodeGen.NotifyCallbackAttribute")!;
			if (callbackAttributeType is null)
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

			var doNotEmitDebuggerStepThroughAttribute = false;
			var doNotEmitPropertyChangedEventTrigger = false;
			var accessibility = (Accessibility?)null;
			foreach (var (name, value) in namedArguments)
			{
				switch (name, value)
				{
					case (nameof(Accessibility), { Value: int v }):
					{
						accessibility = (Accessibility)v;
						break;
					}
					case ("DoNotEmitPropertyChangedEventTrigger", { Value: bool v }):
					{
						doNotEmitPropertyChangedEventTrigger = v;
						break;
					}
					case ("DoNotEmitDebuggerStepThroughAttribute", { Value: bool v }):
					{
						doNotEmitDebuggerStepThroughAttribute = v;
						break;
					}
					case ("ComparisonMode", { Value: int v and >= 0 and <= 6 and not 1 }):
					{
						mode = v switch
						{
							0 => EqualityComparisonMode.None,
							2 => EqualityComparisonMode.EqualityOperator,
							3 => EqualityComparisonMode.InstanceEqualsMethod,
							4 => EqualityComparisonMode.InstanceCompareToMethod,
							5 => EqualityComparisonMode.EqualityComparerDefaultInstance,
							6 => EqualityComparisonMode.ObjectReference
						};
						break;
					}
				}
			}

			if (!doNotEmitPropertyChangedEventTrigger)
			{
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


				bool containsPropertyChangedEvent(IEventSymbol e)
					=> e is
					{
						Name: nameof(INotifyPropertyChanged.PropertyChanged),
						ExplicitInterfaceImplementations: [],
						Type: var eventType
					} && SymbolEqualityComparer.Default.Equals(propertyChangedEventHandlerType, eventType);
			}

			return new(
				propertyName, fieldSymbol, type, mode, accessibility ?? Accessibility.Public,
				doNotEmitPropertyChangedEventTrigger, doNotEmitDebuggerStepThroughAttribute, callbackAttributeType
			);


			bool containsEqualityOperators()
				=> allInterfaces.Contains(equalityOperatorsType, SymbolEqualityComparer.Default)
				|| fieldSpecialType is >= SpecialType.System_Object and <= SpecialType.System_UIntPtr and not (SpecialType.System_ValueType or SpecialType.System_Void)
				|| fieldTypeKind == TypeKind.Enum
				|| fieldType.GetMembers().OfType<IMethodSymbol>().Any(static m => m.Name == "op_Equality");

			bool containsEqualsMethod() => allInterfaces.Contains(equatableType, SymbolEqualityComparer.Default);

			bool containsCompareToMethod() => allInterfaces.Contains(comparableType, SymbolEqualityComparer.Default);
		}

		void output(SourceProductionContext spc, ImmutableArray<Data?> data)
		{
			foreach (var group in data.CastToNotNull().GroupBy<Data, INamedTypeSymbol>(keySelector, SymbolEqualityComparer.Default))
			{
				var type = group.Key;
				var methodsInType = type.GetMembers().OfType<IMethodSymbol>();

				var propertyDeclarations = new List<string>();
				foreach (var (property, fieldSymbol, _, mode, accessibility, doNotEmitPropertyChangedEventTrigger, doNotEmitDebuggerStepThroughAttribute, callbackAttributeType) in group)
				{
					if (fieldSymbol is not { Name: var field, Type: { NullableAnnotation: var nullability } fieldType })
					{
						continue;
					}

					var nullableAnnotation = nullability == NullableAnnotation.Annotated ? "?" : string.Empty;
					var fieldTypeStr = $"{fieldType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}{nullableAnnotation}";
					var valueComparisonCode = mode switch
					{
						EqualityComparisonMode.None => $$"""object.ReferenceEquals({{field}}, value)""",
						EqualityComparisonMode.EqualityOperator => $$"""{{field}} == value""",
						EqualityComparisonMode.InstanceEqualsMethod => $$"""{{field}}.Equals(value)""",
						EqualityComparisonMode.InstanceCompareToMethod => $$"""{{field}}.CompareTo(value) == 0""",
						EqualityComparisonMode.EqualityComparerDefaultInstance => $$"""global::System.Collections.Generic.EqualityComparer<{{fieldTypeStr}}>.Default.Equals({{field}}, value)""",
						EqualityComparisonMode.ObjectReference => $$"""object.ReferenceEquals({{field}}, value)""",
						_ => throw new InvalidOperationException($"The value '{nameof(mode)}' is invalid.")
					};

					var ifStatement = mode switch
					{
						not EqualityComparisonMode.None
							=> $$"""
							if ({{valueComparisonCode}})
										{
											return;
										}

										
							""",
						_ => string.Empty
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

					var eventTrigger = doNotEmitPropertyChangedEventTrigger
						? string.Empty
						:
						$$"""


									PropertyChanged?.Invoke(this, new(nameof({{property}})));
						""";

					var customizedCallback = fieldSymbol.GetAttributes().FirstOrDefault(callbackAttributeTypeExists) switch
					{
						{ ConstructorArguments: [{ Value: string methodName }] }
							=> methodsInType.FirstOrDefault(methodSymbol => methodSymbol.Name == methodName)?.Parameters switch
							{
								[]
									=>
									$$"""


												{{methodName}}();
									""",
								[{ Type: { NullableAnnotation: var parameterNullability } parameterType }]
								when SymbolEqualityComparer.Default.Equals(parameterType, fieldType)
									&& nullabilityCompatibilityChecker(nullability, parameterNullability)
									=>
									$$"""


												{{methodName}}(value);
									""",
								_ => string.Empty
							},
						_ => string.Empty
					};

					var debuggerStepThrough = doNotEmitDebuggerStepThroughAttribute
						? string.Empty
						: $$"""
						[global::System.Diagnostics.DebuggerStepThrough]
								
						""";

					propertyDeclarations.Add(
						$$"""
						/// <inheritdoc cref="{{field}}"/>
							[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute]
							[global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{GetType().FullName}}", "{{VersionValue}}")]
							{{accessibilityStr}} {{fieldTypeStr}} {{property}}
							{
								{{debuggerStepThrough}}[global::System.Runtime.CompilerServices.MethodImplAttribute(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
								get => {{field}};

								{{debuggerStepThrough}}[global::System.Runtime.CompilerServices.MethodImplAttribute(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
								set
								{
									{{ifStatement}}{{field}} = value;{{eventTrigger}}{{customizedCallback}}
								}
							}
						"""
					);


					static bool nullabilityCompatibilityChecker(NullableAnnotation nullability, NullableAnnotation parameterNullability)
						=> nullability == NullableAnnotation.NotAnnotated && parameterNullability == NullableAnnotation.Annotated
						|| nullability == parameterNullability;

					bool callbackAttributeTypeExists(AttributeData a)
						=> SymbolEqualityComparer.Default.Equals(a.AttributeClass, callbackAttributeType);
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
	}
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
	EqualityComparerDefaultInstance,

	/// <summary>
	/// 
	/// </summary>
	ObjectReference
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
/// <param name="DoNotEmitPropertyChangedEventTrigger">
/// Indicates whether the source generator does not emit source code to trigger property changed event.
/// </param>
/// <param name="DoNotEmitDebuggerStepThroughAttribute">
/// Indicates whether the source generator does not emit source code of <see cref="DebuggerStepThroughAttribute"/>.
/// </param>
/// <param name="CallbackAttributeType">Indicates the callback attribute type.</param>
file readonly record struct Data(
	string PropertyName,
	IFieldSymbol FieldSymbol,
	INamedTypeSymbol Type,
	EqualityComparisonMode ComparisonMode,
	Accessibility Accessibility,
	bool DoNotEmitPropertyChangedEventTrigger,
	bool DoNotEmitDebuggerStepThroughAttribute,
	INamedTypeSymbol CallbackAttributeType
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
