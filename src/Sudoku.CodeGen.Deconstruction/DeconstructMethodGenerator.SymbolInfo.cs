using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Sudoku.CodeGen.Deconstruction
{
	public sealed partial class DeconstructMethodGenerator
	{
		/// <summary>
		/// Indicates the inner member symbol information quadruple.
		/// </summary>
		/// <param name="Type">Indicates the type name.</param>
		/// <param name="ParameterName">Indicates the parameter name.</param>
		/// <param name="Name">Indicates the member name.</param>
		/// <param name="Attributes">Indicates all attributes that the type has marked.</param>
		private sealed record SymbolInfo(
			string Type, string ParameterName, string Name, IEnumerable<AttributeData> Attributes
		);
	}
}
