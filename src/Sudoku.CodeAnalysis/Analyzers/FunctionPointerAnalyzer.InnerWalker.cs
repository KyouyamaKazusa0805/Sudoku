using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Sudoku.CodeAnalysis.Analyzers
{
	partial class FunctionPointerAnalyzer
	{
		/// <summary>
		/// Indicates the searcher that searches for function pointer syntax node.
		/// </summary>
		private sealed class InnerWalker : CSharpSyntaxWalker
		{
			/// <summary>
			/// Indicates the result list.
			/// </summary>
			public IList<FunctionPointerTypeSyntax>? Collection { get; private set; }


			/// <inheritdoc/>
			public override void VisitFunctionPointerType(FunctionPointerTypeSyntax node)
			{
				if (node.CallingConvention is not null)
				{
					return;
				}

				Collection ??= new List<FunctionPointerTypeSyntax>();

				Collection.Add(node);
			}
		}
	}
}
