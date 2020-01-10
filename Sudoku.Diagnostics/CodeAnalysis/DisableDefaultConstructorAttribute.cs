using System;
using Sudoku.Diagnostics.Permissions;

namespace Sudoku.Diagnostics.CodeAnalysis
{
	[AttributeUsage(AttributeTargets.Struct)]
	public sealed class DisableDefaultConstructorAttribute : Attribute
	{
		public AccessLevel AccessLevel { get; set; } = AccessLevel.Private;

		public bool IsError { get; set; } = false;
	}
}
