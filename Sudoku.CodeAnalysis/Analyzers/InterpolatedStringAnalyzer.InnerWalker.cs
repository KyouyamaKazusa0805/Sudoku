using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;

namespace Sudoku.CodeAnalysis.Analyzers
{
	partial class InterpolatedStringAnalyzer
	{
		/// <summary>
		/// Indicates the searcher that searches for function pointer syntax node.
		/// </summary>
		private sealed class InnerWalker : CSharpSyntaxWalker
		{
			/// <summary>
			/// Indicates the semantic model.
			/// </summary>
			private readonly SemanticModel _semanticModel;


			/// <summary>
			/// Initializes an instance with the specified semantic model.
			/// </summary>
			/// <param name="semanticModel">The semantic model.</param>
			public InnerWalker(SemanticModel semanticModel) => _semanticModel = semanticModel;


			/// <summary>
			/// Indicates the result list.
			/// </summary>
			public IList<InterpolationSyntax>? Collection { get; private set; }


			/// <inheritdoc/>
			public override void VisitInterpolation(InterpolationSyntax node)
			{
				if
				(
					_semanticModel.GetOperation(node) is not IInterpolationOperation
					{
						Kind: OperationKind.Interpolation,
						Expression: { Type: { IsValueType: true } }
					}
				)
				{
					return;
				}

				Collection ??= new List<InterpolationSyntax>();

				Collection.Add(node);
			}
		}
	}
}
