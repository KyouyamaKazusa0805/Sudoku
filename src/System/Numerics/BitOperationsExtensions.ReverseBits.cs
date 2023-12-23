namespace System.Numerics;

partial class BitOperationsExtensions
{
	/// <summary>
	/// <para>Reverse all bits in a specified value.</para>
	/// <para>Note that the value is passed by <b>reference</b> though the method is an extension method, and returns nothing.</para>
	/// </summary>
	/// <param name="this">The value.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static partial void ReverseBits(this scoped ref byte @this)
	{
		@this = (byte)(@this >> 1 & 0x55 | (@this & 0x55) << 1);
		@this = (byte)(@this >> 2 & 0x33 | (@this & 0x33) << 2);
		@this = (byte)(@this >> 4 | @this << 4);
	}

	/// <inheritdoc cref="ReverseBits(ref byte)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static partial void ReverseBits(this scoped ref short @this)
	{
		@this = (short)(@this >> 1 & 0x5555 | (@this & 0x5555) << 1);
		@this = (short)(@this >> 2 & 0x3333 | (@this & 0x3333) << 2);
		@this = (short)(@this >> 4 & 0x0F0F | (@this & 0x0F0F) << 4);
		@this = (short)(@this >> 8 | @this << 8);
	}

	/// <inheritdoc cref="ReverseBits(ref byte)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static partial void ReverseBits(this scoped ref int @this)
	{
		@this = @this >> 1 & 0x55555555 | (@this & 0x55555555) << 1;
		@this = @this >> 2 & 0x33333333 | (@this & 0x33333333) << 2;
		@this = @this >> 4 & 0x0F0F0F0F | (@this & 0x0F0F0F0F) << 4;
		@this = @this >> 8 & 0x00FF00FF | (@this & 0x00FF00FF) << 8;
		@this = @this >> 16 | @this << 16;
	}

	/// <inheritdoc cref="ReverseBits(ref byte)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static partial void ReverseBits(this scoped ref long @this)
	{
		@this = @this >> 1 & 0x55555555_55555555L | (@this & 0x55555555_55555555L) << 1;
		@this = @this >> 2 & 0x33333333_33333333L | (@this & 0x33333333_33333333L) << 2;
		@this = @this >> 4 & 0x0F0F0F0F_0F0F0F0FL | (@this & 0x0F0F0F0F_0F0F0F0FL) << 4;
		@this = @this >> 8 & 0x00FF00FF_00FF00FFL | (@this & 0x00FF00FF_00FF00FFL) << 8;
		@this = @this >> 16 & 0x0000FFFF_0000FFFFL | (@this & 0x0000FFFF_0000FFFFL) << 16;
		@this = @this >> 32 | @this << 32;
	}
}
