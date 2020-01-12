using System.Runtime.CompilerServices;

namespace Sudoku.Data.Extensions
{
	public static class Int16Ex
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int FindFirstSet(this short @this) => Int32Ex.FindFirstSet(@this);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int CountSet(this short @this) => Int32Ex.CountSet(@this);
	}
}
