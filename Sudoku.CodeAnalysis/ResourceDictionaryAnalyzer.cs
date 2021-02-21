using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Sudoku.CodeAnalysis
{
	/// <summary>
	/// Indicates the analyzer that is called the resource dictionary values. Both two resource dictionaries
	/// store in the folder <c>..\required\lang</c>.
	/// </summary>
	[Generator]
	public sealed class ResourceDictionaryAnalyzer : ISourceGenerator
	{
		/// <summary>
		/// Indicates the folder where two resource dictionaries store.
		/// </summary>
		private const string Path = @"..\required\lang";


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
				var collector = new DynamicMemberAccessSyntaxWalker(compilation, semanticModel);
				collector.Visit(root);


			}
		}

		/// <inheritdoc/>
		public void Initialize(GeneratorInitializationContext context)
		{
		}

		/// <summary>
		/// Indicates the syntax walker that searches and visits the syntax node that is:
		/// <list type="bullet">
		/// <item><c>TextResources.Current.KeyToGet</c></item>
		/// <item>
		/// <c>Current.KeyToGet</c> (need the directive <c>using static Sudoku.Resources.TextResources;</c>)
		/// </item>
		/// </list>
		/// </summary>
		private sealed class DynamicMemberAccessSyntaxWalker : CSharpSyntaxWalker
		{
			/// <summary>
			/// Indicates the compilation.
			/// </summary>
			private readonly Compilation _compilation;

			/// <summary>
			/// Indicates the semantic model of this syntax tree.
			/// </summary>
			private readonly SemanticModel _semanticModel;


			/// <summary>
			/// Initializes an instance with the specified compilation and the semantic model.
			/// </summary>
			/// <param name="compilation">The compilation.</param>
			/// <param name="semanticModel">The semantic model.</param>
			public DynamicMemberAccessSyntaxWalker(Compilation compilation, SemanticModel semanticModel)
			{
				_compilation = compilation;
				_semanticModel = semanticModel;
			}


			/// <summary>
			/// Indicates whether the walker found the target syntax node.
			/// </summary>
			/// <remarks>
			/// The result value can be:
			/// <list type="table">
			/// <item>
			/// <term><c><see langword="true"/></c></term>
			/// <description>The target syntax node found.</description>
			/// </item>
			/// <item>
			/// <term><c><see langword="false"/></c></term>
			/// <description>The target syntax node isn't found.</description>
			/// </item>
			/// </list>
			/// </remarks>
			public bool HasTargetValue { get; }

			/// <summary>
			/// Indicates the collection that stores those nodes. The value can be used if and only if
			/// <see cref="HasTargetValue"/> is <see langword="true"/>.
			/// </summary>
			/// <seealso cref="HasTargetValue"/>
			public IList<(MemberAccessExpressionSyntax Node, string Value)>? Collection { get; }


			/// <inheritdoc/>
			public override void VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
			{
				if (!node.IsKind(SyntaxKind.SimpleMemberAccessExpression))
				{
					return;
				}

				switch (_semanticModel.GetOperation(node))
				{
					case null:
					case { Kind: not OperationKind.DynamicMemberReference }:
					{
						return;
					}
					case { Children: var children } operation:
					{
						var collection = new List<(MemberAccessExpressionSyntax, string)>();

						// TODO: Implement.

						break;
					}
				}
			}
		}
	}
}
