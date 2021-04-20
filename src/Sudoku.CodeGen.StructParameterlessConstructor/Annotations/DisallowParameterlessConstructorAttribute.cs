using System;

namespace Sudoku.CodeGen.StructParameterlessConstructor.Annotations
{
	/// <summary>
	/// Marks on a <see langword="struct"/>, to tell the users the parameterless constructor is disabled
	/// and can't be called or used.
	/// </summary>
	[AttributeUsage(AttributeTargets.Struct)]
	public sealed class DisallowParameterlessConstructorAttribute : Attribute
	{
	}
}
