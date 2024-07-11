namespace Sudoku.SourceGeneration;

/// <summary>
/// Represents the source generator.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class Generator : IIncrementalGenerator
{
	/// <inheritdoc/>
	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		context.RegisterSourceOutput(
			context.CompilationProvider
				.Combine(
					context.SyntaxProvider
						.ForAttributeWithMetadataName(
							"SudokuStudio.ComponentModel.DependencyPropertyAttribute",
							static (n, _) => n is PropertyDeclarationSyntax { Modifiers: var m and not [] }
								&& m.Any(SyntaxKind.StaticKeyword) && m.Any(SyntaxKind.PartialKeyword),
							AttachedPropertyHandler.Transform
						)
						.Where(NotNullPredicate)
						.Select(NotNullSelector)
						.Collect()
				),
			static (spc, c) =>
			{
				if (c.Left.AssemblyName == "SudokuStudio")
				{
					AttachedPropertyHandler.Output(spc, c.Right);
				}
			}
		);

		context.RegisterSourceOutput(
			context.CompilationProvider
				.Combine(
					context.SyntaxProvider
						.ForAttributeWithMetadataName(
							"SudokuStudio.ComponentModel.DependencyPropertyAttribute",
							static (n, _) => n is PropertyDeclarationSyntax { Modifiers: var m and not [] }
								&& !m.Any(SyntaxKind.StaticKeyword) && m.Any(SyntaxKind.PartialKeyword),
							DependencyPropertyHandler.Transform
						)
						.Where(NotNullPredicate)
						.Select(NotNullSelector)
						.Collect()
				),
			static (spc, c) =>
			{
				if (c.Left.AssemblyName == "SudokuStudio")
				{
					DependencyPropertyHandler.Output(spc, c.Right);
				}
			}
		);
	}


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
