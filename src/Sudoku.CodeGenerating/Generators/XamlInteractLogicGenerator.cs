using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Sudoku.CodeGenerating.Extensions;
using static Sudoku.CodeGenerating.Constants;

namespace Sudoku.CodeGenerating.Generators
{
	/// <summary>
	/// Indicates the XAML interact logic generator.
	/// </summary>
	[Generator]
	public sealed partial class XamlInteractLogicGenerator : ISourceGenerator
	{
		/// <inheritdoc/>
		public void Execute(GeneratorExecutionContext context)
		{
			if (context.IsNotInProject(ProjectNames.Maui, ProjectNames.Maui_WinUI))
			{
				return;
			}

			var receiver = (SyntaxReceiver)context.SyntaxReceiver!;

			var compilation = context.Compilation;
			var attributeSymbol = compilation.GetTypeByMetadataName<XamlInteractLogicAttribute>();
			Func<ISymbol?, ISymbol?, bool> f = SymbolEqualityComparer.Default.Equals;
			foreach (var type in
				from type in receiver.Candidates
				let model = compilation.GetSemanticModel(type.SyntaxTree)
				select (INamedTypeSymbol)model.GetDeclaredSymbol(type)! into symbol
				where symbol.GetAttributes().Any(a => f(a.AttributeClass, attributeSymbol))
				select symbol)
			{
				type.DeconstructInfo(
					true, out string fullTypeName, out string namespaceName, out string genericParametersList,
					out _, out _, out _, out _
				);
				if (genericParametersList != string.Empty)
				{
					continue;
				}

				// Here we separate the site into three parts... The whole site is too long...
				const string
					csharpDocsSite = "https://docs.microsoft.com/en-us/dotnet/csharp/",
					subpageSite = "whats-new/tutorials/upgrade-to-nullable-references",
					section = "#warnings-help-discover-original-design-intent";

				context.AddSource(
					type.ToFileName(),
					"InteractLogicDefaults",
					$@"#nullable enable

namespace {namespaceName}
{{
	/// <summary>
	/// The basic interactions about the <c><see cref=""{type.Name}""/>.xaml</c>.
	/// </summary>
	partial class {type.Name}
	{{
		/// <summary>
		/// Initializes a <see cref=""MainPage""/> instance with no parameters.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This constructor will calls the inner method called <c>InitializeComponent</c>,
		/// but this method will automatically iniatilize all controls in this page implicitly,
		/// but Roslyn don't know this behavior, so it'll report a
		/// <see href=""{csharpDocsSite}{subpageSite}{section}"">
		/// CS8618
		/// </see>
		/// compiler warning.
		/// </para>
		/// <para>
		/// <i>
		/// To be honest, I suggest the team will append
		/// <see cref=""global::System.Diagnostics.CodeAnalysis.MemberNotNullAttribute""/>
		/// onto the method <c>InitializeComponent</c> in order to avoid Roslyn making the error wave
		/// mark on this method.
		/// </i>
		/// </para>
		/// </remarks>
		/// <seealso cref=""global::System.Diagnostics.CodeAnalysis.MemberNotNullAttribute""/>
		[global::System.CodeDom.Compiler.GeneratedCode(""{GetType().FullName}"", ""{VersionValue}"")]
		[global::System.Runtime.CompilerServices.CompilerGenerated]
#if NULLABLE
#nullable disable
		public MainPage() => InitializeComponent();
#nullable restore
#else
#error You must append the compilation symbol 'NULLABLE'.
#endif
	}}
}}"
				);
			}
		}

		/// <inheritdoc/>
		public void Initialize(GeneratorInitializationContext context) => context.FastRegister<SyntaxReceiver>();
	}
}
