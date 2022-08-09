namespace Microsoft.UI.Xaml;

/// <summary>
/// Provides with extension methods on <see cref="Panel"/>.
/// </summary>
/// <seealso cref="Panel"/>
public static class PanelExtensions
{
	/// <summary>
	/// Sets the property <see cref="Panel.Background"/> with the specified value.
	/// </summary>
	/// <typeparam name="TPanel">The real type of the panel.</typeparam>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TPanel WithBackground<TPanel>(this TPanel @this, Color background) where TPanel : Panel
		=> @this.WithBackground(new SolidColorBrush(background));

	/// <summary>
	/// Sets the property <see cref="Panel.Background"/> with the specified value.
	/// </summary>
	/// <typeparam name="TPanel">The real type of the panel.</typeparam>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TPanel WithBackground<TPanel>(this TPanel @this, Brush background) where TPanel : Panel
	{
		@this.Background = background;
		return @this;
	}

	/// <summary>
	/// Adds the specified control into the children collection of the current panel.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TPanel AddChildren<TPanel, TUIElement>(this TPanel @this, TUIElement control)
		where TPanel : Panel
		where TUIElement : UIElement
	{
		@this.Children.Add(control);
		return @this;
	}

	/// <summary>
	/// Sets the property <see cref="Panel.ChildrenTransitions"/> with the specified value.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TPanel WithChildrenTransitions<TPanel>(this TPanel @this, TransitionCollection transitions)
		where TPanel : Panel
	{
		@this.ChildrenTransitions = transitions;
		return @this;
	}
}
