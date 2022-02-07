#pragma warning disable CS1591

namespace Microsoft.UI.Xaml.Controls;

/// <summary>
/// Provides extension methods on <see cref="GridLayout"/>.
/// </summary>
/// <seealso cref="GridLayout"/>
public static class GridExtensions
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TFrameworkElement WithGridRow<TFrameworkElement>(this TFrameworkElement @this, int value)
		where TFrameworkElement : FrameworkElement
	{
		GridLayout.SetRow(@this, value);
		return @this;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TFrameworkElement WithGridColumn<TFrameworkElement>(this TFrameworkElement @this, int value)
		where TFrameworkElement : FrameworkElement
	{
		GridLayout.SetColumn(@this, value);
		return @this;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TFrameworkElement WithGridRowSpan<TFrameworkElement>(this TFrameworkElement @this, int value)
		where TFrameworkElement : FrameworkElement
	{
		GridLayout.SetRowSpan(@this, value);
		return @this;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TFrameworkElement WithGridColumnSpan<TFrameworkElement>(this TFrameworkElement @this, int value)
		where TFrameworkElement : FrameworkElement
	{
		GridLayout.SetColumnSpan(@this, value);
		return @this;
	}
}
