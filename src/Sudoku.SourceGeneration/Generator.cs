namespace Sudoku.SourceGeneration;

/// <summary>
/// Represents a source generator type that runs multiple different usage of source output services on compiling code.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class Generator : IIncrementalGenerator
{
	/// <inheritdoc/>
	public unsafe void Initialize(IncrementalGeneratorInitializationContext context)
	{
		//
		// Elementary generators
		//
		#region Elementary generators
		// Primary Constructors
		{
			const string name = "System.SourceGeneration.PrimaryConstructorParameterAttribute";
			context.Register<PrimaryConstructorHandler, PrimaryConstructorCollectedResult>(name, &SyntaxNodeTypePredicate<ParameterSyntax>);
		}

		// Default Overridden
		{
			const string name = "System.SourceGeneration.GeneratedOverridingMemberAttribute";
			context.RegisterSourceOutput(
				context.SyntaxProvider
					.ForAttributeWithMetadataName(name, IsPartialMethodPredicate, EqualsOverriddenHandler.Transform)
					.Where(NotNullPredicate)
					.Select(NotNullSelector)
					.Collect()
					.Combine(
						context.SyntaxProvider
							.ForAttributeWithMetadataName(name, IsPartialMethodPredicate, GetHashCodeOveriddenHandler.Transform)
							.Where(NotNullPredicate)
							.Select(NotNullSelector)
							.Collect()
							.Combine(
								context.SyntaxProvider
									.ForAttributeWithMetadataName(name, IsPartialMethodPredicate, ToStringOverriddenHandler.Transform)
									.Where(NotNullPredicate)
									.Select(NotNullSelector)
									.Collect()
							)
					)
					.Select(static (v, _) => new DefaultOverriddenCollectedResult(v.Left.ToArray(), v.Right.Left.ToArray(), v.Right.Right.ToArray())),
				DefaultOverriddenHandler.Output
			);
		}

		// Instance Deconstruction Methods
		{
			const string name = "System.SourceGeneration.DeconstructionMethodAttribute";
			context.Register<InstanceDeconstructionMethodHandler, InstanceDeconstructionMethodCollectedResult>(name, &IsPartialMethodPredicate);
		}
		#endregion

		//
		// Advanced generators
		//
		#region Advanced generators
		// StepSearcher Default Imports
		{
			const string projectName = "Sudoku.Analytics";
			context.Register<StepSearcherDefaultImportingHandler>(projectName);
		}

		// SudokuStudio XAML Bindings
		{
			const string projectName = "SudokuStudio";

			const string name_Dependency = "SudokuStudio.ComponentModel.DependencyPropertyAttribute`1";
			context.Register<DependencyPropertyHandler, DependencyPropertyCollectedResult>(name_Dependency, projectName, &predicate_Dependency);

			const string name_Attached = "SudokuStudio.ComponentModel.AttachedPropertyAttribute`1";
			context.Register<AttachedPropertyHandler, AttachedPropertyCollectedResult>(name_Attached, projectName, &predicate_Attached);


			static bool predicate_Dependency(SyntaxNode n, CancellationToken _)
				=> n is ClassDeclarationSyntax { TypeParameterList: null, Modifiers: var m and not [] } && m.Any(SyntaxKind.PartialKeyword);

			static bool predicate_Attached(SyntaxNode n, CancellationToken _)
				=> n is ClassDeclarationSyntax { TypeParameterList: null, Modifiers: var m and not [] }
				&& m.Any(SyntaxKind.StaticKeyword) && m.Any(SyntaxKind.PartialKeyword);
		}
		#endregion
	}
}
