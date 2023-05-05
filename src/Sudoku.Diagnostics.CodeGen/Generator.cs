namespace Sudoku.Diagnostics.CodeGen;

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
			const string name = "System.Diagnostics.CodeGen.PrimaryConstructorParameterAttribute";
			context.Register<PrimaryConstructorHandler, PrimaryConstructorCollectedResult>(name, &SyntaxNodeTypePredicate<ParameterSyntax>);
		}

		// Default Overridden
		{
			const string name = "System.Diagnostics.CodeGen.GeneratedOverridingMemberAttribute";
			context.Register<EqualsOverriddenHandler, EqualsOverriddenCollectedResult>(name, &IsPartialMethodPredicate);
			context.Register<GetHashCodeOveriddenHandler, GetHashCodeCollectedResult>(name, &IsPartialMethodPredicate);
			context.Register<ToStringOverriddenHandler, ToStringCollectedResult>(name, &IsPartialMethodPredicate);
		}

		// Instance Deconstruction Methods
		{
			const string name = "System.Diagnostics.CodeGen.DeconstructionMethodAttribute";
			context.Register<InstanceDeconstructionMethodHandler, InstanceDeconstructionMethodCollectedResult>(name, &IsPartialMethodPredicate);
		}
		#endregion

		//
		// Advanced generators
		//
		#region Advanced generators
		// StepSearcher Default Imports
		{
			context.Register<StepSearcherDefaultImportingHandler>("Sudoku.Analytics");
		}
		#endregion
	}
}
