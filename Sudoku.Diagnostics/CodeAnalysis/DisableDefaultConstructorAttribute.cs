using System;

namespace Sudoku.Diagnostics.CodeAnalysis
{
	/// <summary>
	/// To disable using default constructor in structs.
	/// </summary>
	[AttributeUsage(AttributeTargets.Struct)]
	public sealed class DisableDefaultConstructorAttribute : Attribute
	{
		/// <summary>
		/// Specifies an action when calling the default constructor
		/// in structs. If the value is <c>true</c>, it will report
		/// a compiling error; otherwise, a warning.
		/// </summary>
		public bool IsError { get; set; } = false;
	}
}
