namespace Sudoku.Diagnostics.CodeGen.Generators;

/// <summary>
/// Defines a source generator that generates deconstruction methods of a type.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class AutoDeconstructionGenerator : ISourceGenerator
{
	/// <summary>
	/// Defines the pattern that matches for an expression.
	/// </summary>
	private static readonly Regex ExpressionPattern = new(
		"""\w+""",
		RegexOptions.ExplicitCapture,
		TimeSpan.FromSeconds(5)
	);


	/// <inheritdoc/>
	public void Execute(GeneratorExecutionContext context)
	{
		// Check values.
		if (
			context is not
			{
				SyntaxContextReceiver: AutoDeconstructionReceiver
				{
					Diagnostics: var diagnostics,
					Collection: var collection
				} receiver,
				Compilation: var compilation
			}
		)
		{
			return;
		}

		// Report diagnostics if worth.
		if (diagnostics.Count != 0)
		{
			diagnostics.ForEach(context.ReportDiagnostic);
			return;
		}

		// Iterates on each value.
		foreach (var (type, attributesData, location) in collection)
		{
			var (_, _, namespaceName, genericParameterList, _, _, readOnlyKeyword, _, _, _) = SymbolOutputInfo.FromSymbol(type);
			string typeKindName = (type.TypeKind, type.IsRecord) switch
			{
				(TypeKind.Struct, false) => "struct",
				(TypeKind.Struct, true) => "record struct",
				(TypeKind.Class, false) => "class",
				(TypeKind.Class, true) => "record class",
				(TypeKind.Interface, _) => "interface",
				_ => default!
			};

			foreach (var group in
				from attributeData in attributesData
				let namedArg = attributeData.NamedArguments.FirstOrDefault(static e => e.Key == "GenerateAsExtension")
				group attributeData by ((bool?)namedArg.Value.Value) ?? false)
			{
				if (group.Key)
				{
					var listOfMethodsRawCode = GetForInstance(ref context, group, location, type, readOnlyKeyword);
					string finalCode = string.Join("\r\n\r\n\t", listOfMethodsRawCode);
					context.AddSource(
						type.ToFileName(),
						"d",
						$$"""
						#nullable enable
						
						namespace {{namespaceName}};
						
						partial {{typeKindName}} {{type.Name}}
						{
						{{finalCode}}
						}
						"""
					);
				}
				else
				{
					// TODO: Get for extension methods.
				}
			}
		}
	}

	/// <inheritdoc/>
	public void Initialize(GeneratorInitializationContext context)
		=> context.RegisterForSyntaxNotifications(() => new AutoDeconstructionReceiver(context.CancellationToken));

	/// <summary>
	/// Gets the raw code parts for instance deconstruction methods via the specified list of attributes data.
	/// </summary>
	/// <param name="context">The context to report diagnostics.</param>
	/// <param name="attributesData">The attributes data.</param>
	/// <param name="location">
	/// The <see cref="Location"/> instance to be reported if any diagnostic warnings or errors has been encountered.
	/// </param>
	/// <param name="type">The type to be used.</param>
	/// <param name="readOnlyKeyword">The read-only keyword token.</param>
	/// <returns>The collection of raw code parts for instance deconstruction methods.</returns>
	private IReadOnlyCollection<string> GetForInstance(
		ref GeneratorExecutionContext context, IEnumerable<AttributeData> attributesData,
		Location location, INamedTypeSymbol type, string? readOnlyKeyword)
	{
		var result = new List<string>();

		foreach (var attributeData in attributesData)
		{
			if (attributeData.ConstructorArguments is not [{ Values: var typedConstants }])
			{
				// Invalid case.
				continue;
			}

			if (typedConstants is [])
			{
				context.ReportDiagnostic(Diagnostic.Create(SCA0007, location, messageArgs: null));
				continue;
			}

			var pairs = new List<(ITypeSymbol Symbol, string Name)>();
			foreach (var typedConstant in typedConstants)
			{
				if (typedConstant.Value is not string s)
				{
					continue;
				}

				if (!ExpressionPattern.IsMatch(s))
				{
					context.ReportDiagnostic(Diagnostic.Create(SCA0008, location, messageArgs: null));
					continue;
				}

				switch (type.GetMembers(s).FirstOrDefault())
				{
					case IPropertySymbol { Type: var propertyType, Name: var propertyName }:
					{
						if (propertyType is IPointerTypeSymbol or IFunctionPointerTypeSymbol)
						{
							context.ReportDiagnostic(Diagnostic.Create(SCA0010, location, messageArgs: null));
							continue;
						}

						pairs.Add((propertyType, propertyName));
						break;
					}
					default:
					{
						// TODO: Support expression member.
						context.ReportDiagnostic(Diagnostic.Create(SCA0009, location, messageArgs: null));
						continue;
					}
				}
			}

			string args = string.Join(
				", ",
				from element in pairs
				select $"out {element.Symbol.ToDisplayString(TypeFormats.FullName)} {element.Name.ToCamelCase()}"
			);
			string assignments = string.Join(
				"\r\n\t\t",
				from element in pairs
				select $"{element.Name.ToCamelCase()} = {element.Name};"
			);
			result.Add(
				// Here we should insert an extra indentation, on purpose.
				$$"""
					/// <summary>
					/// Deconstruct the current instance into multiple values, which means you can use
					/// the value-tuple syntax to define your own deconstruction logic.
					/// </summary>
					/// <remarks>
					/// <para>
					/// For example,
					/// if you have defined a deconstruction method <c>Deconstruct</c> with no return value:
					/// <code><![CDATA[
					/// public void Deconstruct(out string name, out int age)
					/// {
					///     name = Name;
					///     age = Age;
					/// }
					/// ]]></code>
					/// The following code will be legal.
					/// <code><![CDATA[
					/// // Use explicitly typed variables to get a deconstruction.
					/// (string name, int age) = student;
					/// 
					/// // Or use the type inferring to omit the type of each variable having been deconstructed.
					/// var (name, age) = student;
					/// 
					/// // Of course, you can also use the pure invocation to get values.
					/// student.Deconstruct(out string name, out int age);
					/// ]]></code>
					/// </para>
					/// <para>
					/// Deconstruction method can also help you use deconstruction pattern, like this:
					/// <code><![CDATA[
					/// if (student is (name: var name, age: >= 18))
					/// {
					///     Console.WriteLine(name);
					/// }
					/// ]]></code>
					/// </para>
					/// </remarks>
					[global::System.Runtime.CompilerServices.CompilerGenerated]
					[global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
					[global::System.CodeDom.Compiler.GeneratedCode("{{GetType().FullName}}", "{{VersionValue}}")]
					public {{readOnlyKeyword}}void Deconstruct({{args}})
					{
						{{assignments}}
					}
				"""
			);
		}

		return result;
	}
}
