using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
#if DEBUG && SOURCE_GENERATOR_DEBUG
using System.Diagnostics;
#endif

namespace Sudoku.CodeAnalysis.Analyzers
{
	/// <summary>
	/// Indicates the analyzer that check the property named '<c>Properties</c>' in a step searcher.
	/// </summary>
	/// <remarks>
	/// All supported diagnostics:
	/// <list type="bullet">
	/// <item><a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=3599824&amp;doc_id=633030">SUDOKU001</a> (A property named 'Properties' expected)</item>
	/// <item><a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=3599808&amp;doc_id=633030">SUDOKU002</a> (The property 'Properties' must be <see langword="public"/>)</item>
	/// <item><a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=3621783&amp;doc_id=633030">SUDOKU003</a> (The property 'Properties' must be <see langword="static"/>)</item>
	/// <item><a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=3599816&amp;doc_id=633030">SUDOKU004</a> (The property 'Properties' must be <see langword="readonly"/>)</item>
	/// <item><a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=3599818&amp;doc_id=633030">SUDOKU005</a> (The property 'Properties' has a wrong type)</item>
	/// <item><a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=3599826&amp;doc_id=633030">SUDOKU006</a> (The property 'Properties' can't be <see langword="null"/>)</item>
	/// <item><a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=3602787&amp;doc_id=633030">SUDOKU007</a> (The property 'Properties' must contain an initializer)</item>
	/// <item><a href="https://gitee.com/SunnieShine/Sudoku/wikis/pages?sort_id=3607697&amp;doc_id=633030">SUDOKU008</a> (The property 'Properties' must be initialized by a new clause)</item>
	/// </list>
	/// </remarks>
	[Generator]
	public sealed partial class PropertiesInStepSearcherAnalyzer : ISourceGenerator
	{
		/// <summary>
		/// Indicates the full name of the type of the property technique properties.
		/// </summary>
		private const string TechniquePropertiesTypeFullName = "Sudoku.Solving.Manual.TechniqueProperties";


		/// <inheritdoc/>
		public void Execute(GeneratorExecutionContext context)
		{
			var compilation = context.Compilation;
			foreach (var syntaxTree in compilation.SyntaxTrees)
			{
				if (!syntaxTree.TryGetRoot(out var root))
				{
					continue;
				}

				// Create the semantic model and the property list.
				var semanticModel = compilation.GetSemanticModel(syntaxTree);
				var collector = new InnerWalker(compilation, semanticModel);
				collector.Visit(root);

				// If none, skip it.
				if (collector.TargetPropertyInfo is not { } targetPropertyInfos)
				{
					continue;
				}

				// Iterate on each information quadruple.
				foreach (var (hasTargetProperty, classNode, propertyNode, propertySymbol) in targetPropertyInfos)
				{
					if (hasTargetProperty)
					{
						CheckSudoku002(context, propertySymbol);
						CheckSudoku003(context, propertySymbol);
						CheckSudoku004(context, propertySymbol);
						CheckSudoku005(context, compilation, propertySymbol!);
						CheckSudoku006(context, propertySymbol);
						CheckSudoku007(context, propertyNode);
						CheckSudoku008(context, propertyNode);
					}
					else
					{
						CheckSudoku001(context, classNode);
					}
				}
			}
		}

		/// <inheritdoc/>
		public void Initialize(GeneratorInitializationContext context)
		{
#if DEBUG && SOURCE_GENERATOR_DEBUG
			if (!Debugger.IsAttached)
			{
				Debugger.Launch();
			}
#endif
		}

		partial void CheckSudoku001(GeneratorExecutionContext context, ClassDeclarationSyntax classNode);
		partial void CheckSudoku002(GeneratorExecutionContext context, IPropertySymbol? propertySymbol);
		partial void CheckSudoku003(GeneratorExecutionContext context, IPropertySymbol? propertySymbol);
		partial void CheckSudoku004(GeneratorExecutionContext context, IPropertySymbol? propertySymbol);
		partial void CheckSudoku005(GeneratorExecutionContext context, Compilation compilation, IPropertySymbol propertySymbol);
		partial void CheckSudoku006(GeneratorExecutionContext context, IPropertySymbol? propertySymbol);
		partial void CheckSudoku007(GeneratorExecutionContext context, PropertyDeclarationSyntax? propertyNode);
		partial void CheckSudoku008(GeneratorExecutionContext context, PropertyDeclarationSyntax? propertyNode);
	}
}
