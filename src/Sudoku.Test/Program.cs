using System;
using Sudoku.CodeGen.Annotations;

// May be unmeaningful.
Console.WriteLine("Hello, world!");

namespace Sudoku.Test
{
	[AutoGeneratePrimaryConstructor]
	internal sealed partial class TestClass
	{
		private readonly int _field1;
		private readonly long _field2;
		private readonly string? _field3;

		public static readonly int Field4 = default;

		public const string Field5 = "Hello, world!";
	}
}