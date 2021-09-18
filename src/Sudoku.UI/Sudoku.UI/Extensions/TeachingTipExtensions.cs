namespace Microsoft.UI.Xaml.Controls;

/// <summary>
/// Provides extension methods on <see cref="TeachingTip"/>.
/// </summary>
/// <seealso cref="TeachingTip"/>
internal static class TeachingTipExtensions
{
	/// <summary>
	/// Opens the specified <see cref="TeachingTip"/> instance if worth.
	/// </summary>
	/// <param name="this">
	/// The specified <see cref="TeachingTip"/> instance that may be <see langword="null"/>.
	/// </param>
	/// <returns>
	/// A <see cref="bool"/> value indicating whether the <see cref="TeachingTip"/> instance
	/// can be successfully opened. If <see langword="true"/>, the argument <paramref name="this"/>
	/// shouldn't <see langword="null"/>.
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool Open([NotNullWhen(true)] this TeachingTip? @this) =>
		@this is not null && (@this.IsOpen = true);
}
