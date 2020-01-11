using System.Runtime.CompilerServices;

namespace Sudoku.Data.Extensions
{
	public static class Int16Ex
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int FindFirstSet(this short @this)
		{
			return Int32Ex.FindFirstSet(@this);

			#region Deprecated Code
			//if (@this == 0)
			//{
			//	return -1;
			//}
			//
			//for (int i = 0; i < sizeof(short) << 3; i++, @this >>= 1)
			//{
			//	if ((@this & 1) != 0)
			//	{
			//		return i;
			//	}
			//}
			//
			//return -1;
			#endregion
		}
	}
}
