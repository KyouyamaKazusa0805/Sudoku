namespace Microsoft.UI.Xaml;

/// <summary>
/// Provides with extension methods on <see cref="UIElement"/>.
/// </summary>
/// <seealso cref="UIElement"/>
public static class UIElementExtensions
{
	/// <summary>
	/// Sets the property <see cref="UIElement.Visibility"/> with the specified value.
	/// </summary>
	/// <param name="this">The property.</param>
	/// <param name="visibility">The value.</param>
	/// <returns>The instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TUIElement WithVisibility<TUIElement>(this TUIElement @this, Visibility visibility)
		where TUIElement : UIElement
	{
		@this.Visibility = visibility;
		return @this;
	}
}
