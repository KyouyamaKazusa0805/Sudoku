namespace Microsoft.UI.Xaml.Controls;

/// <summary>
/// Provides with the extension method on <see cref="Grid"/>.
/// </summary>
/// <seealso cref="Grid"/>
internal static class GridExtensions
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T WithGridRow<T>(this T @this, int gridRow) where T : FrameworkElement
	{
		Grid.SetRow(@this, gridRow);
		return @this;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T WithGridColumn<T>(this T @this, int gridColumn) where T : FrameworkElement
	{
		Grid.SetColumn(@this, gridColumn);
		return @this;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T WithGridRowSpan<T>(this T @this, int gridRowSpan) where T : FrameworkElement
	{
		Grid.SetRowSpan(@this, gridRowSpan);
		return @this;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T WithGridColumnSpan<T>(this T @this, int gridColumnSpan) where T : FrameworkElement
	{
		Grid.SetColumnSpan(@this, gridColumnSpan);
		return @this;
	}
}
