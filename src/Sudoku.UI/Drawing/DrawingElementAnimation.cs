namespace Sudoku.UI.Drawing;

/// <summary>
/// Applies the animation on drawing elements.
/// </summary>
internal static class DrawingElementAnimation
{
	/// <summary>
	/// Applies a <see cref="UIElement"/> control the scale animation.
	/// </summary>
	/// <typeparam name="TUIElement">The type of the control.</typeparam>
	/// <param name="this">The control instance.</param>
	public static void ApplyScaleAnimation<TUIElement>(TUIElement @this) where TUIElement : UIElement
	{
		var targetScale = new Vector3(1, 1, 1);
		@this.Scale =
			@this.Scale != targetScale
				? targetScale
				: throw new InvalidOperationException("The scale is currently set 'new(1, 1, 1)'.");
	}
}
