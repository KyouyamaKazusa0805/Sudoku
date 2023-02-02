namespace Sudoku.Diagnostics.CodeGen.Generators;

/// <summary>
/// Defines a source generator that generates the source code that are setting properties used in logical solver type.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class LogicalSolverPropertiesGenerator : IIncrementalGenerator
{
	/// <inheritdoc/>
	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		context.RegisterSourceOutput(context.CompilationProvider, output);


		void output(SourceProductionContext spc, Compilation compilation)
		{
			if (compilation is not { Assembly: { GlobalNamespace: var @namespace } assemblySymbol })
			{
				return;
			}

			var manualSolverTypeSymbol = compilation.GetTypeByMetadataName("Sudoku.Solving.Logical.LogicalSolver");
			if (manualSolverTypeSymbol is not { TypeKind: Kind.Class, IsRecord: true, IsSealed: true })
			{
				// The core type cannot be found.
				return;
			}

			var stepSearcherType = compilation.GetTypeByMetadataName("Sudoku.Solving.Logical.IStepSearcher");
			if (stepSearcherType is not { TypeKind: Kind.Interface })
			{
				// Same reason as above.
				return;
			}

			var attributeType = compilation.GetTypeByMetadataName("Sudoku.Solving.Logical.Annotations.StepSearcherPropertyAttribute");
			if (attributeType is not { TypeKind: Kind.Class, IsSealed: true })
			{
				// Same reason as above.
				return;
			}

			// Iterates on all possible types derived from this interface.
			var allTypes = @namespace.GetAllNestedTypes();
			var foundResultInfos = new List<Data>();
			foreach (var searcherType in
				from typeSymbol in allTypes
				where typeSymbol is { TypeKind: Kind.Class, AllInterfaces: not [] }
				let implementedInterfaces = typeSymbol.AllInterfaces
				where implementedInterfaces.Contains(stepSearcherType, SymbolEqualityComparer.Default)
				select typeSymbol)
			{
				foreach (var property in searcherType.GetMembers().OfType<IPropertySymbol>())
				{
					if (!property.ContainsAttribute(attributeType))
					{
						continue;
					}

					if (property is not
						{
							ExplicitInterfaceImplementations: [],
							ContainingType.Name: var searcherTypeName,
							Name: var propertyName
						})
					{
						continue;
					}

					var searcherFullTypeName = $"Sudoku.Solving.Logical.StepSearchers.I{searcherTypeName}";
					var interfaceType = compilation.GetTypeByMetadataName(searcherFullTypeName);
					if (interfaceType is not { AllInterfaces: var interfaceBaseInterfaces })
					{
						continue;
					}

					if (interfacePropertyMatcher(interfaceType))
					{
						foundResultInfos.Add(new(property, interfaceType, interfaceType));
					}
					else if (interfaceBaseInterfaces.FirstOrDefault(interfacePropertyMatcher) is { } baseInterfaceType)
					{
						foundResultInfos.Add(new(property, interfaceType, baseInterfaceType));
					}


					bool interfacePropertyMatcher(INamedTypeSymbol e)
						=> e.GetMembers().OfType<IPropertySymbol>().Any(p => p.Name == property.Name);
				}
			}

			var targetPropertiesCode = string.Join(
				"\r\n\r\n\t",
				from info in foundResultInfos
				let typeStr = info.DerivedInterfaceType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
				let propertyContainedInterfaceTypeStr = info.PropertyContainedInterfaceType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
				let typeStrWithoutInterfacePrefix = info.Property.ContainingType.Name
				let propertyStr = info.Property.Name
				let propertyTypeStr = info.Property.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
				select
					$$"""
					/// <inheritdoc cref="{{propertyContainedInterfaceTypeStr}}.{{propertyStr}}"/>
						[global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{GetType().FullName}}", "{{VersionValue}}")]
						[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute]
						public {{propertyTypeStr}} {{typeStrWithoutInterfacePrefix}}_{{propertyStr}}
						{
							[global::System.Diagnostics.DebuggerStepThroughAttribute]
							[global::System.Runtime.CompilerServices.MethodImplAttribute(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
							get
							{
								if (TargetSearcherCollection.GetOfType<{{typeStr}}>() is { } searcher)
								{
									return searcher.{{propertyStr}};
								}

								throw new global::System.InvalidOperationException($"Property '{nameof(searcher.{{propertyStr}})}' is not found.");
							}

							[global::System.Diagnostics.DebuggerStepThroughAttribute]
							[global::System.Runtime.CompilerServices.MethodImplAttribute(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
							set
							{
								if (TargetSearcherCollection.GetOfType<{{typeStr}}>() is { } searcher)
								{
									searcher.{{propertyStr}} = value;
								}

								//throw new global::System.InvalidOperationException($"Property '{nameof(searcher.{{propertyStr}})}' is not found.");
							}
						}
					"""
			);

			spc.AddSource(
				$"LogicalSolver.g.{Shortcuts.LogicalSolverOptions}.cs",
				$$"""
				// <auto-generated/>

				#nullable enable

				namespace Sudoku.Solving.Logical;

				partial record LogicalSolver
				{
					{{targetPropertiesCode}}
				}

				/// <include
				///     file='../../global-doc-comments.xml'
				///     path='g/csharp11/feature[@name="file-local"]/target[@name="class" and @when="extension"]'/>
				[global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{GetType().FullName}}", "{{VersionValue}}")]
				[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute]
				file static class Extensions
				{
					/// <summary>
					/// Try to fetch a valid <typeparamref name="T"/> instance via the specified pool.
					/// </summary>
					/// <typeparam name="T">The type of the step searcher you want to fetch.</typeparam>
					/// <param name="this">The pool where all possible step searchers are stored.</param>
					/// <returns>
					/// The found step searcher instance.
					/// If the type is marked <see cref="global::Sudoku.Solving.Logical.Annotations.SeparatedStepSearcherAttribute"/>,
					/// the method will return the first found instance.
					/// </returns>
					/// <seealso cref="global::Sudoku.Solving.Logical.Annotations.SeparatedStepSearcherAttribute"/>
					[global::System.Runtime.CompilerServices.MethodImplAttribute(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
					public static T? GetOfType<T>(this global::Sudoku.Solving.Logical.StepSearchers.StepSearcherCollection @this)
						where T : class, global::Sudoku.Solving.Logical.IStepSearcher => @this.OfType<T>().FirstOrDefault();
				}
				"""
			);
		}
	}
}

file readonly record struct Data(IPropertySymbol Property, INamedTypeSymbol DerivedInterfaceType, INamedTypeSymbol PropertyContainedInterfaceType);
