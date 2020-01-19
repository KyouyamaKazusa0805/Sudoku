using System;

namespace Sudoku.Diagnostics.CodeAnalysis
{
	/// <summary>
	/// It means the method marked this attribute is only used for deconstruction
	/// to <see cref="ValueTuple"/>s.
	/// </summary>
	/// <seealso cref="ValueTuple"/>
	[AttributeUsage(AttributeTargets.Method)]
	public sealed class OnDeconstructionAttribute : SyntaxContractAttribute
	{
	}
}
