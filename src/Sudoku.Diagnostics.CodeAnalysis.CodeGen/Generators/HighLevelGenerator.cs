namespace Sudoku.Diagnostics.CodeGen.Generators;

/// <summary>
/// Indicates the high-level source generator that generates the source generator.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed partial class HighLevelGenerator : ISourceGenerator
{
	/// <inheritdoc/>
	public void Execute(GeneratorExecutionContext context)
	{
		if (
			context is not
			{
				AdditionalFiles: { Length: 1 } additionalFiles,
				SyntaxContextReceiver: Receiver { Diagnostics: var diagnostics, Result: var shortNames }
			}
		)
		{
			return;
		}

		// Get the compiler diagnostics and insert into the analyzer types.
		var descriptors = 
			from detail in MarkdownHandler.SplitTable(File.ReadAllText(additionalFiles[0].Path))
			select detail.ToDescriptor();

		// Report compiler diagnostics.
		diagnostics.ForEach(context.ReportDiagnostic);

		// Append analzyers.
		foreach (var (shortName, fullName, diagnosticIds) in shortNames)
		{
			context.AddSource(
				$"{shortName}Analyzer.g.cs",
				$@"namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers;

[global::System.CodeDom.Compiler.GeneratedCode(""{GetType().FullName}"", ""{VersionValue}"")]
[global::System.Runtime.CompilerServices.CompilerGenerated]
[global::Microsoft.CodeAnalysis.Generator(global::Microsoft.CodeAnalysis.LanguageNames.CSharp)]
public sealed class {shortName}Analyzer : global::Microsoft.CodeAnalysis.ISourceGenerator
{{
	/// <inheritdoc/>
	[global::System.CodeDom.Compiler.GeneratedCode(""{GetType().FullName}"", ""{VersionValue}"")]
	[global::System.Runtime.CompilerServices.CompilerGenerated]
	public void Execute(global::Microsoft.CodeAnalysis.GeneratorExecutionContext context) =>
		(({shortName}SyntaxChecker)context.SyntaxContextReceiver!).Diagnostics.ForEach(context.ReportDiagnostic);

	/// <inheritdoc/>
	[global::System.CodeDom.Compiler.GeneratedCode(""{GetType().FullName}"", ""{VersionValue}"")]
	[global::System.Runtime.CompilerServices.CompilerGenerated]
	public void Initialize(global::Microsoft.CodeAnalysis.GeneratorInitializationContext context) =>
		context.RegisterForSyntaxNotifications(() => new {shortName}SyntaxChecker(context.CancellationToken));
}}
"
			);

			var selection = (
				from descriptor in descriptors
				where Array.IndexOf(diagnosticIds, descriptor.Id) != -1
				select descriptor
			).ToArray();

			string descriptorsXmlDocStr = string.Join(
				"\r\n",
				from descriptor in selection
				select $@"/// <item>
/// <term>{descriptor.Id}</term>
/// <description>{descriptor.Title}</description>
/// </item>"
			);

			string descriptorsStr = string.Join(
				"\r\n\r\n\t",
				from descriptor in selection
				select $@"/// <summary>
	/// Indicates the descriptor {descriptor.Id} ({descriptor.Title}).
	/// </summary>
	[global::System.CodeDom.Compiler.GeneratedCode(""{GetType().FullName}"", ""{VersionValue}"")]
	[global::System.Runtime.CompilerServices.CompilerGenerated]
	private static readonly global::Microsoft.CodeAnalysis.DiagnosticDescriptor {descriptor.Id} = new(
		id: nameof({descriptor.Id}),
		title: ""{descriptor.Title}"",
		messageFormat: ""{descriptor.MessageFormat}"",
		category: ""{descriptor.Category}"",
		defaultSeverity: global::Microsoft.CodeAnalysis.DiagnosticSeverity.{descriptor.DefaultSeverity},
		isEnabledByDefault: true,
		helpLinkUri: {(descriptor.HelpLinkUri is var s and not "" ? s : "null")}
	);"
			);

			context.AddSource(
				$"{fullName}.g.cs",
				$@"namespace Sudoku.Diagnostics.CodeAnalysis.SyntaxContextReceivers;

/// <summary>
/// Indicates the syntax checker that checks for the code structures and the API usages.
/// The current analyzer will check the following cases:
/// <list type=""bullet"">
/// <listheader>
/// <term>Diagnostic ID</term>
/// <description>Description</description>
/// </listheader>
{descriptorsXmlDocStr}
/// </list>
/// </summary>
partial class {fullName}
{{
	{descriptorsStr}


	/// <summary>
	/// Indicates the cancellation token used.
	/// </summary>
	[global::System.CodeDom.Compiler.GeneratedCode(""{GetType().FullName}"", ""{VersionValue}"")]
	[global::System.Runtime.CompilerServices.CompilerGenerated]
	private readonly global::System.Threading.CancellationToken _cancellationToken;


	/// <summary>
	/// Initializes a <see cref=""{fullName}""/> instance using the cancellation token.
	/// </summary>
	/// <param name=""cancellationToken"">The cancellation token to cancel the operation.</param>
	[global::System.CodeDom.Compiler.GeneratedCode(""{GetType().FullName}"", ""{VersionValue}"")]
	[global::System.Runtime.CompilerServices.CompilerGenerated]
	[global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
	public {fullName}(global::System.Threading.CancellationToken cancellationToken) =>
		_cancellationToken = cancellationToken;


	/// <summary>
	/// Indicates all possible diagnostics types used.
	/// </summary>
	[global::System.CodeDom.Compiler.GeneratedCode(""{GetType().FullName}"", ""{VersionValue}"")]	
	[global::System.Runtime.CompilerServices.CompilerGenerated]
	public global::System.Collections.Generic.List<global::Microsoft.CodeAnalysis.Diagnostic> Diagnostics {{ get; }} = new();
}}
"
			);
		}
	}

	/// <inheritdoc/>
	public void Initialize(GeneratorInitializationContext context) =>
		context.RegisterForSyntaxNotifications(() => new Receiver(context.CancellationToken));
}
