namespace System.Windows;

/// <summary>
/// Provides extension methods on <see cref="WSize"/>.
/// </summary>
/// <seealso cref="WSize"/>
internal static partial class WindowsSizeExtensions
{
	/// <summary>
	/// Convert the current size instance to another instance of type <see cref="DSizeF"/>.
	/// </summary>
	/// <param name="this">The current size.</param>
	/// <returns>The another instance of type <see cref="DSizeF"/>.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static DSizeF ToDSizeF(this WSize @this) => new((float)@this.Width, (float)@this.Height);

	/// <summary>
	/// Convert the current size instance to another instance of type <see cref="DSizeF"/>.
	/// </summary>
	/// <param name="this">The current size.</param>
	/// <returns>The another instance of type <see cref="DSizeF"/>.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static DSize ToDSize(this WSize @this) => new((int)@this.Width, (int)@this.Height);
}
