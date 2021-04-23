using Sudoku.DocComments;
using static System.Numerics.BitOperations;

namespace System.Extensions
{
	partial class BitOperationsEx
	{
		/// <inheritdoc cref="Integer.GetAllSets(Integer)"/>
		public static ReadOnlySpan<int> GetAllSets(this sbyte @this)
		{
			if (@this == 0)
			{
				return ReadOnlySpan<int>.Empty;
			}

			int length = PopCount((uint)@this);
			int[] result = new int[length];
			for (byte i = 0, p = 0; i < sizeof(sbyte) << 3; i++, @this >>= 1)
			{
				if ((@this & 1) != 0)
				{
					result[p++] = i;
				}
			}

			return result;
		}

		/// <inheritdoc cref="Integer.GetAllSets(Integer)"/>
		public static ReadOnlySpan<int> GetAllSets(this byte @this)
		{
			if (@this == 0)
			{
				return ReadOnlySpan<int>.Empty;
			}

			int length = PopCount(@this);
			int[] result = new int[length];
			for (byte i = 0, p = 0; i < sizeof(byte) << 3; i++, @this >>= 1)
			{
				if ((@this & 1) != 0)
				{
					result[p++] = i;
				}
			}

			return result;
		}

		/// <inheritdoc cref="Integer.GetAllSets(Integer)"/>
		public static ReadOnlySpan<int> GetAllSets(this short @this)
		{
			if (@this == 0)
			{
				return ReadOnlySpan<int>.Empty;
			}

			int length = PopCount((uint)@this);
			int[] result = new int[length];
			for (byte i = 0, p = 0; i < sizeof(short) << 3; i++, @this >>= 1)
			{
				if ((@this & 1) != 0)
				{
					result[p++] = i;
				}
			}

			return result;
		}

		/// <inheritdoc cref="Integer.GetAllSets(Integer)"/>
		public static ReadOnlySpan<int> GetAllSets(this ushort @this)
		{
			if (@this == 0)
			{
				return ReadOnlySpan<int>.Empty;
			}

			int length = PopCount(@this);
			int[] result = new int[length];
			for (byte i = 0, p = 0; i < sizeof(ushort) << 3; i++, @this >>= 1)
			{
				if ((@this & 1) != 0)
				{
					result[p++] = i;
				}
			}

			return result;
		}

		/// <inheritdoc cref="Integer.GetAllSets(Integer)"/>
		public static ReadOnlySpan<int> GetAllSets(this int @this)
		{
			if (@this == 0)
			{
				return ReadOnlySpan<int>.Empty;
			}

			int length = PopCount((uint)@this);
			int[] result = new int[length];
			for (byte i = 0, p = 0; i < sizeof(int) << 3; i++, @this >>= 1)
			{
				if ((@this & 1) != 0)
				{
					result[p++] = i;
				}
			}

			return result;
		}

		/// <inheritdoc cref="Integer.GetAllSets(Integer)"/>
		public static ReadOnlySpan<int> GetAllSets(this uint @this)
		{
			if (@this == 0)
			{
				return ReadOnlySpan<int>.Empty;
			}

			int length = PopCount(@this);
			int[] result = new int[length];
			for (byte i = 0, p = 0; i < sizeof(uint) << 3; i++, @this >>= 1)
			{
				if ((@this & 1) != 0)
				{
					result[p++] = i;
				}
			}

			return result;
		}

		/// <inheritdoc cref="Integer.GetAllSets(Integer)"/>
		public static ReadOnlySpan<int> GetAllSets(this long @this)
		{
			if (@this == 0)
			{
				return ReadOnlySpan<int>.Empty;
			}

			int length = PopCount((ulong)@this);
			int[] result = new int[length];
			for (byte i = 0, p = 0; i < sizeof(long) << 3; i++, @this >>= 1)
			{
				if ((@this & 1) != 0)
				{
					result[p++] = i;
				}
			}

			return result;
		}

		/// <inheritdoc cref="Integer.GetAllSets(Integer)"/>
		public static ReadOnlySpan<int> GetAllSets(this ulong @this)
		{
			if (@this == 0)
			{
				return ReadOnlySpan<int>.Empty;
			}

			int length = PopCount(@this);
			int[] result = new int[length];
			for (byte i = 0, p = 0; i < sizeof(ulong) << 3; i++, @this >>= 1)
			{
				if ((@this & 1) != 0)
				{
					result[p++] = i;
				}
			}

			return result;
		}
	}
}
