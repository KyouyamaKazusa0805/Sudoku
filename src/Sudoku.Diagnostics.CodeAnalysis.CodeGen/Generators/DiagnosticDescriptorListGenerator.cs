namespace Sudoku.Diagnostics.CodeGen.Generators;

/// <summary>
/// Defines a source generator that generates the diagnostic descriptor list
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class DiagnosticDescriptorListGenerator : IIncrementalGenerator
{
	/// <inheritdoc/>
	public void Initialize(IncrementalGeneratorInitializationContext context)
		=> context.RegisterSourceOutput(
			context.AdditionalTextsProvider
				.Where(static a => a.Path.EndsWith("CodeAnalysisDetailList.csv"))
				.Collect(),
			(spc, additionalTextFiles) =>
			{
				if (additionalTextFiles is not [{ Path: var path }])
				{
					return;
				}

				var descriptors = DiagnosticDescriptorSerializer.GetDiagnosticDescriptorsFromFile(path);
				var text = string.Join(
					"\r\n\r\n\t",
					from descriptor in descriptors
					let helpLinkUrl = $"https://sunnieshine.github.io/Sudoku/code-analysis/{descriptor.Id}"
					select
						$$"""
						/// <summary>
							/// Indicates the <see href="{{helpLinkUrl}}">{{descriptor.Id}}</see> diagnostic descriptor.
							/// </summary>
							public static readonly global::Microsoft.CodeAnalysis.DiagnosticDescriptor {{descriptor.Id}} =
								new(
									nameof({{descriptor.Id}}),
									"{{descriptor.Title}}",
									"{{descriptor.MessageFormat}}",
									"{{descriptor.Category}}",
									global::Microsoft.CodeAnalysis.DiagnosticSeverity.{{descriptor.DefaultSeverity}},
									true,
									"{{descriptor.Description}}",
									"{{helpLinkUrl}}"
								);
						"""
				);

				spc.AddSource(
					$"WellKnownDiagnosticDescriptors.g.{Shortcuts.DiagnosticDescriptorList}.cs",
					$$"""
					namespace Sudoku.Diagnostics.CodeAnalysis;

					/// <summary>
					/// Represents with the well-known diagnostic descriptors.
					/// </summary>
					[global::System.CodeDom.Compiler.GeneratedCodeAttribute("{{GetType().FullName}}", "{{VersionValue}}")]
					[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute]
					internal static class WellKnownDiagnosticDescriptors
					{
						{{text}}
					}
					"""
				);
			}
		);
}
