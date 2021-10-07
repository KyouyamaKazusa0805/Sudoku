namespace Sudoku.CodeGenerating.Generators;

/// <summary>
/// A generator that generates the code for code analyzer and fix defaults.
/// </summary>
#if SUPPORT_CODE_ANALYZER || SUPPORT_CODE_FIXER
[Generator]
#endif
public sealed partial class CodeAnalyzerOrFixerDefaultsGenerator : ISourceGenerator
{
	/// <summary>
	/// Indicates the name that stores the diagnostic information.
	/// </summary>
	private const string CsvTableName = "DiagnosticResults.csv";


#if SUPPORT_CODE_FIXER
	/// <summary>
	/// Indicates the regular expression for extraction of the information.
	/// The regular expression is <c><![CDATA[(?<=\(")[A-Za-z]{2}\d{4}(?="\))]]></c>.
	/// </summary>
	private static readonly Regex InfoRegex = new(
		@"(?<=\("")[A-Za-z]{2}\d{4}(?=""\))",
		RegexOptions.ExplicitCapture,
		TimeSpan.FromSeconds(5)
	);
#endif


	/// <inheritdoc/>
	public void Execute(GeneratorExecutionContext context)
	{
		if (context.AdditionalFiles is not { Length: not 0 } additionalFiles)
		{
			return;
		}

		if (additionalFiles.GetPath(static f => f.Contains(CsvTableName)) is not { } csvTableFilePath)
		{
			return;
		}

		var compilation = context.Compilation;
		var receiver = (SyntaxReceiver)context.SyntaxReceiver!;
		var attributeAnalyzerSymbol = compilation.GetTypeByMetadataName(typeof(CodeAnalyzerAttribute).FullName);
		var attributeFixerSymbol = compilation.GetTypeByMetadataName(typeof(CodeFixProviderAttribute).FullName);
		string[] list = File.ReadAllLines(csvTableFilePath);
		string[] withoutHeader = new Memory<string>(list, 1, list.Length - 1).ToArray();
		string[][] info = (from line in withoutHeader select line.SplitInfo()).ToArray();

#if SUPPORT_CODE_ANALYZER
		foreach (var (type, attributeData) in
			from type in receiver.Candidates
			let model = compilation.GetSemanticModel(type.SyntaxTree)
			select model.GetDeclaredSymbol(type)! into typeSymbol
			let attributesData = typeSymbol.GetAttributes()
			let attributeData = attributesData.FirstOrDefault(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, attributeAnalyzerSymbol))
			where attributeData is { ConstructorArguments.IsDefaultOrEmpty: false }
			select (typeSymbol, attributeData))
		{
			type.DeconstructInfo(
				false, out string fullTypeName, out string namespaceName,
				out _, out _, out _, out _, out bool isGeneric
			);
			if (isGeneric)
			{
				continue;
			}

			string[] diagnosticIds = (
				from arg in attributeData.ConstructorArguments[0].Values
				select ((string)arg.Value!).Trim()
			).ToArray();
			string diagnosticResults = string.Join(
				"\r\n",
				from diagnosticId in diagnosticIds
				let id = Cut(diagnosticId)
				select $@"/// <item>
/// <term><see href=""https://sunnieshine.github.io/Sudoku/rules/Rule-{id}"">{diagnosticId}</see></term>
/// <description>{GetDescription(info, id)}</description>
/// </item>"
			);
			string descriptors = string.Join(
				"\r\n\r\n\t",
				from diagnosticId in diagnosticIds
				let tags = GetWhetherFadingOutTag(diagnosticId)
				let id = Cut(diagnosticId)
				select $@"/// <summary>
	/// Indicates the <see href=""https://sunnieshine.github.io/Sudoku/rules/Rule-{id}"">{id}</see>
	/// diagnostic result ({GetDescription(info, id)}).
	/// </summary>
	[global::System.CodeDom.Compiler.GeneratedCode(""{GetType().FullName}"", ""{VersionValue}"")]
	[global::System.Runtime.CompilerServices.CompilerGenerated]
	private static readonly DiagnosticDescriptor {id} = new(
		DiagnosticIds.{id}, Titles.{id}, Messages.{id}, Categories.{id},
		DiagnosticSeverities.{id}, true, helpLinkUri: HelpLinks.{id}{tags}
	);"
			);
			string supportedInstances = string.Join(
				", ",
				from diagnosticId in diagnosticIds select Cut(diagnosticId)
			);
			string attributeApplied =
#if SUPPORT_CODE_ANALYZER_VSIX
				@"[DiagnosticAnalyzer(LanguageNames.CSharp)]
";
#else
				string.Empty;
#endif
			string overridenProperties =
#if SUPPORT_CODE_ANALYZER_VSIX
				$@"


	/// <inheritdoc/>
	[global::System.CodeDom.Compiler.GeneratedCode(""{GetType().FullName}"", ""{VersionValue}"")]
	[global::System.Runtime.CompilerServices.CompilerGenerated]
	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
		{supportedInstances}
	);";
#else
				string.Empty;
#endif

			context.AddSource(
				type.ToFileName(),
				GeneratedFileShortcuts.CodeAnalyzer,
				$@"#nullable enable

namespace {namespaceName};

/// <summary>
/// Indicates an analyzer that analyzes the code for the following diagnostic results:
/// <list type=""table"">
{diagnosticResults}
/// </list>
/// </summary>
{attributeApplied}partial class {type.Name}
{{
	{descriptors}{overridenProperties}
}}
"
			);
		}
#endif

#if SUPPORT_CODE_FIXER
		foreach (var type in
			from type in receiver.Candidates
			let model = compilation.GetSemanticModel(type.SyntaxTree)
			select model.GetDeclaredSymbol(type)! into type
			where type.GetAttributes().Any(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, attributeFixerSymbol))
			select type)
		{
			type.DeconstructInfo(
				false, out string fullTypeName, out string namespaceName, out _,
				out _, out _, out _, out bool isGeneric
			);
			if (isGeneric)
			{
				continue;
			}

			string typeName = type.Name;
			string id = (
				from attribute in type.GetAttributes()
				where SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, attributeFixerSymbol)
				select InfoRegex.Match(attribute.ToString()) into match
				where match.Success
				select match.Value
			).First();
			string description = GetDescription(info, id);

			context.AddSource(
				type.ToFileName(),
				GeneratedFileShortcuts.CodeFixer,
				$@"using System.Collections.Immutable;
using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

#nullable enable

namespace {namespaceName};

/// <summary>
/// Indicates the code fixer for solving the diagnostic result
/// <see href=""https://sunnieshine.github.io/Sudoku/rules/Rule-{id}"">{id}</see>
/// ({description}).
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof({typeName})), Shared]
partial class {typeName}
{{
	/// <inheritdoc/>
	[global::System.CodeDom.Compiler.GeneratedCode(""{GetType().FullName}"", ""{VersionValue}"")]
	[global::System.Runtime.CompilerServices.CompilerGenerated]
	public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(
		DiagnosticIds.{id}
	);

	/// <inheritdoc/>
	[global::System.CodeDom.Compiler.GeneratedCode(""{GetType().FullName}"", ""{VersionValue}"")]
	[global::System.Runtime.CompilerServices.CompilerGenerated]
	public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;
}}
"
			);
		}
#endif
	}


	/// <inheritdoc/>
	public void Initialize(GeneratorInitializationContext context) => context.FastRegister<SyntaxReceiver>();


#if SUPPORT_CODE_ANALYZER || SUPPORT_CODE_FIXER
	/// <summary>
	/// Cut the diagnostic ID and get the base ID part. This method will remove the suffix <c>"F"</c>
	/// if exists.
	/// </summary>
	/// <param name="id">The diagnostic ID.</param>
	/// <returns>The result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static string Cut(string id) => id.EndsWith('F') ? id.Substring(0, id.Length - 1) : id;

	/// <summary>
	/// Get the raw code when the ID contains the suffix <c>"F"</c>.
	/// </summary>
	/// <param name="id">The diagnostic ID.</param>
	/// <returns>The raw code for representing the option of fading out the code.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static string GetWhetherFadingOutTag(string id) =>
		id.EndsWith('F') ? ",\r\n\t\t\tcustomTags: new[] { WellKnownDiagnosticTags.Unnecessary }" : string.Empty;

	/// <summary>
	/// Get the description of from the split result.
	/// </summary>
	/// <param name="info">The info.</param>
	/// <param name="id">The diagnostic ID.</param>
	/// <returns>The description.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static string GetDescription(string[][] info, string id) => (
		from line in info select (Id: line[0], Title: line[3])
	).First(pair => pair.Id == id).Title.Replace("{", "&lt;").Replace("}", "&gt;");
#endif
}
