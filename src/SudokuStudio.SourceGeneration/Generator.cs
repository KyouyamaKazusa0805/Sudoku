namespace Sudoku.SourceGeneration;

using static CommonMethods;

/// <summary>
/// Represents the source generator.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class Generator : IIncrementalGenerator
{
	/// <inheritdoc/>
	public void Initialize(IncrementalGeneratorInitializationContext context)
		=> SudokuStudioXamlBindings(context);

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

		context.RegisterSourceOutput(
			context.CompilationProvider
				.Combine(
					context.SyntaxProvider
						.ForAttributeWithMetadataName(
							"SudokuStudio.ComponentModel.AutoDependencyPropertyAttribute",
							static (n, _) => n is PropertyDeclarationSyntax { Modifiers: var m and not [] }
								&& m.Any(SyntaxKind.PartialKeyword),
							DependencyPropertyAutoImplementationHandler.Transform
						)
						.Where(NotNullPredicate)
						.Select(NotNullSelector)
						.Collect()
				),
			static (spc, c) =>
			{
				if (c.Left.AssemblyName == "SudokuStudio")
				{
					DependencyPropertyAutoImplementationHandler.Output(spc, c.Right);
				}
			}
		);
	}
}

/// <summary>
/// Represents a set of methods that can be used by the types in this file.
/// </summary>
file static class CommonMethods
{
	/// <summary>
	/// Determine whether the value is not <see langword="null"/>.
	/// </summary>
	/// <typeparam name="T">The type of the value.</typeparam>
	/// <param name="value">The value.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	public static bool NotNullPredicate<T>(T value) => value is not null;

	/// <summary>
	/// Try to get the internal value without nullability checking.
	/// </summary>
	/// <typeparam name="T">The type of the value.</typeparam>
	/// <param name="value">The value with <c>?</c> token being annotated, but not <see langword="null"/> currently.</param>
	/// <param name="_"/>
	/// <returns>The value.</returns>
	public static T NotNullSelector<T>(T? value, CancellationToken _) where T : class => value!;

	/// <summary>
	/// Try to get the internal value without nullability checking.
	/// </summary>
	/// <typeparam name="T">The type of the value.</typeparam>
	/// <param name="value">The value with <c>?</c> token being annotated, but not <see langword="null"/> currently.</param>
	/// <param name="_"/>
	/// <returns>The value.</returns>
	public static T NotNullSelector<T>(T? value, CancellationToken _) where T : struct => value!.Value;
}
