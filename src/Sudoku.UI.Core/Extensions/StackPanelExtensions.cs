namespace Microsoft.UI.Xaml;

/// <summary>
/// Provides extension methods on <see cref="StackPanel"/>.
/// </summary>
/// <seealso cref="StackPanel"/>
public static class StackPanelExtensions
{
	/// <summary>
	/// Sets the property <see cref="StackPanel.Orientation"/> with the specified value.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static StackPanel WithOrientation(this StackPanel @this, Orientation orientation)
	{
		@this.Orientation = orientation;
		return @this;
	}

	/// <summary>
	/// Sets the property <see cref="StackPanel.Padding"/> with the specified value.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static StackPanel WithPadding(this StackPanel @this, double padding)
		=> WithPadding(@this, new Thickness(padding));

	/// <summary>
	/// Sets the property <see cref="StackPanel.Padding"/> with the specified value.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static StackPanel WithPadding(this StackPanel @this, double left, double top, double right, double bottom)
		=> WithPadding(@this, new Thickness(left, top, right, bottom));

	/// <summary>
	/// Sets the property <see cref="StackPanel.Padding"/> with the specified value.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static StackPanel WithPadding(this StackPanel @this, Thickness padding)
	{
		@this.Padding = padding;
		return @this;
	}
}
