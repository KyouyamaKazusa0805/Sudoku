namespace System.Numerics;

partial class BitOperationsExtensions
{
	/// <summary>
	/// <para>Get an enumerator to iterate on each bits of the specified integer value.</para>
	/// <para>This method will allow you to use <see langword="foreach"/> loop to iterate on all indices of set bits.</para>
	/// </summary>
	/// <param name="this">The value.</param>
	/// <returns>All indices of set bits.</returns>
	/// <remarks>
	/// This method allows you using <see langword="foreach"/> loop to iterate this value:
	/// <code><![CDATA[
	/// foreach (int bit in 17)
	/// {
	///     // Do something...
	/// }
	/// ]]></code>
	/// </remarks>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static partial BitEnumerator GetEnumerator(this sbyte @this) => @this.GetAllSets().GetEnumerator();

	/// <inheritdoc cref="GetEnumerator(sbyte)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static partial BitEnumerator GetEnumerator(this byte @this) => @this.GetAllSets().GetEnumerator();

	/// <inheritdoc cref="GetEnumerator(sbyte)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static partial BitEnumerator GetEnumerator(this short @this) => @this.GetAllSets().GetEnumerator();

	/// <inheritdoc cref="GetEnumerator(sbyte)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static partial BitEnumerator GetEnumerator(this ushort @this) => @this.GetAllSets().GetEnumerator();

	/// <inheritdoc cref="GetEnumerator(sbyte)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static partial BitEnumerator GetEnumerator(this int @this) => @this.GetAllSets().GetEnumerator();

	/// <inheritdoc cref="GetEnumerator(sbyte)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static partial BitEnumerator GetEnumerator(this uint @this) => @this.GetAllSets().GetEnumerator();

	/// <inheritdoc cref="GetEnumerator(sbyte)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static partial BitEnumerator GetEnumerator(this long @this) => @this.GetAllSets().GetEnumerator();

	/// <inheritdoc cref="GetEnumerator(sbyte)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static partial BitEnumerator GetEnumerator(this ulong @this) => @this.GetAllSets().GetEnumerator();

	/// <inheritdoc cref="GetEnumerator(sbyte)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static unsafe partial BitEnumerator GetEnumerator(this nint @this) => @this.GetAllSets().GetEnumerator();

	/// <inheritdoc cref="GetEnumerator(sbyte)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static unsafe partial BitEnumerator GetEnumerator(this nuint @this) => @this.GetAllSets().GetEnumerator();
}
