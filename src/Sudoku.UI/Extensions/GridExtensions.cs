namespace Microsoft.UI.Xaml.Controls;

/// <summary>
/// Provides extension methods on <see cref="GridLayout"/>.
/// </summary>
/// <seealso cref="GridLayout"/>
public static class GridExtensions
{
	/// <summary>
	/// Sets the value of the <c>Grid.Row</c> XAML attached property
	/// on the specified <see cref="FrameworkElement"/>.
	/// </summary>
	/// <typeparam name="TFrameworkElement">The type of the element.</typeparam>
	/// <param name="this">The control.</param>
	/// <param name="value">The value of the <c>Grid.Row</c> XAML attached property value.</param>
	/// <returns>The same reference as the argument <paramref name="this"/>.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TFrameworkElement WithGridRow<TFrameworkElement>(this TFrameworkElement @this, int value)
		where TFrameworkElement : notnull, FrameworkElement
	{
		GridLayout.SetRow(@this, value);
		return @this;
	}

	/// <summary>
	/// Sets the value of the <c>Grid.Column</c> XAML attached property
	/// on the specified <see cref="FrameworkElement"/>.
	/// </summary>
	/// <typeparam name="TFrameworkElement">The type of the element.</typeparam>
	/// <param name="this">The control.</param>
	/// <param name="value">The value of the <c>Grid.Column</c> XAML attached property value.</param>
	/// <returns>The same reference as the argument <paramref name="this"/>.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TFrameworkElement WithGridColumn<TFrameworkElement>(this TFrameworkElement @this, int value)
		where TFrameworkElement : notnull, FrameworkElement
	{
		GridLayout.SetColumn(@this, value);
		return @this;
	}

	/// <summary>
	/// Sets the value of the <c>Grid.RowSpan</c> XAML attached property
	/// on the specified <see cref="FrameworkElement"/>.
	/// </summary>
	/// <typeparam name="TFrameworkElement">The type of the element.</typeparam>
	/// <param name="this">The control.</param>
	/// <param name="value">The value of the <c>Grid.RowSpan</c> XAML attached property value.</param>
	/// <returns>The same reference as the argument <paramref name="this"/>.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TFrameworkElement WithGridRowSpan<TFrameworkElement>(this TFrameworkElement @this, int value)
		where TFrameworkElement : notnull, FrameworkElement
	{
		GridLayout.SetRowSpan(@this, value);
		return @this;
	}

	/// <summary>
	/// Sets the value of the <c>Grid.ColumnSpan</c> XAML attached property
	/// on the specified <see cref="FrameworkElement"/>.
	/// </summary>
	/// <typeparam name="TFrameworkElement">The type of the element.</typeparam>
	/// <param name="this">The control.</param>
	/// <param name="value">The value of the <c>Grid.ColumnSpan</c> XAML attached property value.</param>
	/// <returns>The same reference as the argument <paramref name="this"/>.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TFrameworkElement WithGridColumnSpan<TFrameworkElement>(this TFrameworkElement @this, int value)
		where TFrameworkElement : notnull, FrameworkElement
	{
		GridLayout.SetColumnSpan(@this, value);
		return @this;
	}
}
