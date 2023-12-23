namespace Sudoku.SourceGeneration;

/// <summary>
/// Represents a source generator type that runs multiple different usage of source output services on compiling code.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class Generator : IIncrementalGenerator
{
	/// <inheritdoc/>
	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		// Elementary generators
		ActionExtension(context);
		PrimaryConstructor(context);
		ObjectOverridden(context);
		DuckTyping(context);
		ImplicitField(context);
		ExplicitInterfaceImpl(context);

		// Advanced generators
		StepSearcherImports(context);
		SudokuStudioXamlBindings(context);
	}

	private void ActionExtension(IncrementalGeneratorInitializationContext context)
		=> context.RegisterSourceOutput(
			context.CompilationProvider,
			static (spc, c) => { if (c.AssemblyName == "SystemExtensions") { ActionExtensionHandler.Generate(spc); } }
		);

	private void PrimaryConstructor(IncrementalGeneratorInitializationContext context)
		=> context.RegisterSourceOutput(
			context.SyntaxProvider
				.ForAttributeWithMetadataName(
					"System.SourceGeneration.DataAttribute",
					SyntaxNodeTypePredicate<ParameterSyntax>,
					PrimaryConstructorMemberHandler.Transform
				)
				.Where(NotNullPredicate)
				.Select(NotNullSelector)
				.Collect(),
			PrimaryConstructorMemberHandler.Output
		);

	private void ObjectOverridden(IncrementalGeneratorInitializationContext context)
	{
		const string equalsAttributeName = "System.SourceGeneration.EqualsAttribute";
		context.RegisterSourceOutput(
			context.SyntaxProvider
				.ForAttributeWithMetadataName(equalsAttributeName, IsPartialTypePredicate, EqualsHandler.Transform)
				.Where(NotNullPredicate)
				.Select(NotNullSelector)
				.Collect(),
			EqualsHandler.Output
		);

		const string getHashCodeAttributeName = "System.SourceGeneration.GetHashCodeAttribute";
		context.RegisterSourceOutput(
			context.SyntaxProvider
				.ForAttributeWithMetadataName(getHashCodeAttributeName, IsPartialTypePredicate, GetHashCodeHandler.Transform)
				.Where(NotNullPredicate)
				.Select(NotNullSelector)
				.Collect(),
			GetHashCodeHandler.Output
		);

		const string toStringAttributeName = "System.SourceGeneration.ToStringAttribute";
		context.RegisterSourceOutput(
			context.SyntaxProvider
				.ForAttributeWithMetadataName(toStringAttributeName, IsPartialTypePredicate, ToStringHandler.Transform)
				.Where(NotNullPredicate)
				.Select(NotNullSelector)
				.Collect(),
			ToStringHandler.Output
		);
	}

	private void DuckTyping(IncrementalGeneratorInitializationContext context)
	{
		const string equalityOperatorsAttributeName = "System.SourceGeneration.EqualityOperatorsAttribute";
		context.RegisterSourceOutput(
			context.SyntaxProvider
				.ForAttributeWithMetadataName(equalityOperatorsAttributeName, IsPartialTypePredicate, EqualityOperatorsHandler.Transform)
				.Where(NotNullPredicate)
				.Select(NotNullSelector)
				.Collect(),
			EqualityOperatorsHandler.Output
		);

		const string comparisonOperatorsAttributeName = "System.SourceGeneration.ComparisonOperatorsAttribute";
		context.RegisterSourceOutput(
			context.SyntaxProvider
				.ForAttributeWithMetadataName(comparisonOperatorsAttributeName, IsPartialTypePredicate, ComparisonOperatorsHandler.Transform)
				.Where(NotNullPredicate)
				.Select(NotNullSelector)
				.Collect(),
			ComparisonOperatorsHandler.Output
		);

		const string inlineArrayFieldAttributeName = "System.SourceGeneration.InlineArrayFieldAttribute`1";
		context.RegisterSourceOutput(
			context.SyntaxProvider
				.ForAttributeWithMetadataName(inlineArrayFieldAttributeName, IsPartialTypePredicate, InlineArrayFieldHandler.Transform)
				.Where(NotNullPredicate)
				.Select(NotNullSelector)
				.Collect(),
			InlineArrayFieldHandler.Output
		);
	}

	private void ImplicitField(IncrementalGeneratorInitializationContext context)
		=> context.RegisterSourceOutput(
			context.SyntaxProvider
				.ForAttributeWithMetadataName(
					"System.SourceGeneration.ImplicitFieldAttribute",
					SyntaxNodeTypePredicate<PropertyDeclarationSyntax>,
					ImplicitFieldHandler.Transform
				)
				.Where(NotNullPredicate)
				.Select(NotNullSelector)
				.Collect(),
			ImplicitFieldHandler.Output
		);

	private void ExplicitInterfaceImpl(IncrementalGeneratorInitializationContext context)
		=> context.RegisterSourceOutput(
			context.SyntaxProvider
				.CreateSyntaxProvider(SyntaxNodeTypePredicate<TypeDeclarationSyntax>, ExplicitInterfaceImplHandler.Transform)
				.Where(NotNullPredicate)
				.Select(NotNullSelector)
				.Collect(),
			ExplicitInterfaceImplHandler.Output
		);

	private void StepSearcherImports(IncrementalGeneratorInitializationContext context)
		=> context.RegisterSourceOutput(
			context.CompilationProvider,
			static (spc, c) => { if (c.AssemblyName == "Sudoku.Analytics") { StepSearcherDefaultImportingHandler.Output(spc, c); } }
		);

	private void SudokuStudioXamlBindings(IncrementalGeneratorInitializationContext context)
	{
		context.RegisterSourceOutput(
			context.CompilationProvider
				.Combine(
					context.SyntaxProvider
						.ForAttributeWithMetadataName(
							"SudokuStudio.ComponentModel.DependencyPropertyAttribute`1",
							static (n, _) => n is ClassDeclarationSyntax { TypeParameterList: null, Modifiers: var m and not [] } && m.Any(SyntaxKind.PartialKeyword),
							DependencyPropertyHandler.Transform
						)
						.Where(NotNullPredicate)
						.Select(NotNullSelector)
						.Collect()
				),
			static (spc, c) => { if (c.Left.AssemblyName == "SudokuStudio") { DependencyPropertyHandler.Output(spc, c.Right); } }
		);

		context.RegisterSourceOutput(
			context.CompilationProvider
				.Combine(
					context.SyntaxProvider
						.ForAttributeWithMetadataName(
							"SudokuStudio.ComponentModel.AttachedPropertyAttribute`1",
							static (n, _) => n is ClassDeclarationSyntax
							{
								TypeParameterList: null,
								Modifiers: var m and not []
							} && m.Any(SyntaxKind.StaticKeyword) && m.Any(SyntaxKind.PartialKeyword),
							AttachedPropertyHandler.Transform
						)
						.Where(NotNullPredicate)
						.Select(NotNullSelector)
						.Collect()
				),
			static (spc, c) => { if (c.Left.AssemblyName == "SudokuStudio") { AttachedPropertyHandler.Output(spc, c.Right); } }
		);
	}
}
