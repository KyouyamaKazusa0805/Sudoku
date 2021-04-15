using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Sudoku.CodeGen
{
	partial class PrimaryConstructorGenerator
	{
		/// <summary>
		/// Indicates the inner member symbol information quadruple.
		/// </summary>
		/// <param name="Type">Indicates the type name.</param>
		/// <param name="ParameterName">Indicates the parameter name.</param>
		/// <param name="Name">Indicates the name.</param>
		/// <param name="Attributes">Indicates all attributes that the type has marked.</param>
		private sealed record SymbolInfo(
			string Type, string ParameterName, string Name, IEnumerable<AttributeData> Attributes
		);
	}
}
