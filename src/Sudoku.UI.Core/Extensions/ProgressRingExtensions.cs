namespace Microsoft.UI.Xaml;

/// <summary>
/// Provides with extension methods on <see cref="ProgressRing"/>.
/// </summary>
/// <seealso cref="ProgressRing"/>
public static class ProgressRingExtensions
{
	/// <summary>
	/// Sets the property <see cref="ProgressRing.IsActive"/> with the specified value.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ProgressRing WithIsActive(this ProgressRing @this, bool isActive)
	{
		@this.IsActive = isActive;
		return @this;
	}
}
