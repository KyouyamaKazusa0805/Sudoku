namespace System.Text;

/// <summary>
/// Provides extension methods on <see cref="StringBuilder"/>.
/// </summary>
/// <seealso cref="StringBuilder"/>
public static class StringBuilderExtensions
{
	/// <summary>
	/// Remove all characters hehind the character whose index is specified.
	/// </summary>
	/// <param name="this">The instance to remove characters.</param>
	/// <param name="startIndex">The start index.</param>
	/// <returns>The reference of the current instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static StringBuilder RemoveFrom(this StringBuilder @this, int startIndex) =>
		@this.Remove(startIndex, @this.Length - startIndex);

	/// <summary>
	/// Remove all characters behind the character whose index is specified.
	/// </summary>
	/// <param name="this">The instance to remove characters.</param>
	/// <param name="startIndex">The start index.</param>
	/// <returns>The reference of the current instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static StringBuilder RemoveFrom(this StringBuilder @this, in Index startIndex) =>
		@this.Remove(startIndex.GetOffset(@this.Length), startIndex.Value);
}
