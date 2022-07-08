namespace Microsoft.UI.Xaml;

/// <summary>
/// Provides with extension methods on <see cref="FrameworkElement"/>.
/// </summary>
/// <seealso cref="FrameworkElement"/>
public static class FrameworkElementExtensions
{
	/// <summary>
	/// Sets the info on <see cref="GridLayout"/>, and returns the reference
	/// of the argument <paramref name="this"/>.
	/// </summary>
	/// <typeparam name="TFrameworkElement">The type of the control.</typeparam>
	/// <param name="this">The <typeparamref name="TFrameworkElement"/>-typed control.</param>
	/// <param name="row">
	/// The row value that is used for <see cref="GridLayout.SetRow(FrameworkElement, int)"/>.
	/// </param>
	/// <param name="column">
	/// The row value that is used for <see cref="GridLayout.SetColumn(FrameworkElement, int)"/>.
	/// </param>
	/// <param name="rowSpan">
	/// The row value that is used for <see cref="GridLayout.SetRowSpan(FrameworkElement, int)"/>.
	/// </param>
	/// <param name="columnSpan">
	/// The row value that is used for <see cref="GridLayout.SetColumnSpan(FrameworkElement, int)"/>.
	/// </param>
	/// <returns>The reference that is same as the argument <paramref name="this"/>.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TFrameworkElement WithGridLayout<TFrameworkElement>(
		this TFrameworkElement @this, int row = 0, int column = 0, int rowSpan = 1, int columnSpan = 1)
		where TFrameworkElement : FrameworkElement
	{
		GridLayout.SetRow(@this, row);
		GridLayout.SetColumn(@this, column);
		GridLayout.SetRowSpan(@this, rowSpan);
		GridLayout.SetColumnSpan(@this, columnSpan);
		return @this;
	}

	/// <summary>
	/// Sets the property <see cref="FrameworkElement.Width"/> with the specified value.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TFrameworkElement WithWidth<TFrameworkElement>(this TFrameworkElement @this, double width)
		where TFrameworkElement : FrameworkElement
	{
		@this.Width = width;
		return @this;
	}

	/// <summary>
	/// Sets the property <see cref="FrameworkElement.Height"/> with the specified value.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TFrameworkElement WithHeight<TFrameworkElement>(this TFrameworkElement @this, double height)
		where TFrameworkElement : FrameworkElement
	{
		@this.Height = height;
		return @this;
	}

	/// <summary>
	/// Sets the property <see cref="FrameworkElement.HorizontalAlignment"/> with the specified value.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TFrameworkElement WithHorizontalAlignment<TFrameworkElement>(this TFrameworkElement @this, HorizontalAlignment value)
		where TFrameworkElement : FrameworkElement
	{
		@this.HorizontalAlignment = value;
		return @this;
	}

	/// <summary>
	/// Sets the property <see cref="FrameworkElement.VerticalAlignment"/> with the specified value.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TFrameworkElement WithVerticalAlignment<TFrameworkElement>(this TFrameworkElement @this, VerticalAlignment value)
		where TFrameworkElement : FrameworkElement
	{
		@this.VerticalAlignment = value;
		return @this;
	}

	/// <summary>
	/// Sets the property <see cref="FrameworkElement.Margin"/> with the specified value.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TFrameworkElement WithMargin<TFrameworkElement>(this TFrameworkElement @this, double margin)
		where TFrameworkElement : FrameworkElement => @this.WithMargin(new Thickness(margin));

	/// <summary>
	/// Sets the property <see cref="FrameworkElement.Margin"/> with the specified value.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TFrameworkElement WithMargin<TFrameworkElement>(
		this TFrameworkElement @this, double left, double top, double right, double bottom)
		where TFrameworkElement : FrameworkElement => @this.WithMargin(new Thickness(left, top, right, bottom));

	/// <summary>
	/// Sets the property <see cref="FrameworkElement.Margin"/> with the specified value.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TFrameworkElement WithMargin<TFrameworkElement>(this TFrameworkElement @this, Thickness margin)
		where TFrameworkElement : FrameworkElement
	{
		@this.Margin = margin;
		return @this;
	}

	/// <summary>
	/// Sets the binding on the specified dependency property.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TFrameworkElement WithBinding<TFrameworkElement>(
		this TFrameworkElement @this, DependencyProperty dp, Binding binding)
		where TFrameworkElement : FrameworkElement
	{
		@this.SetBinding(dp, binding);
		return @this;
	}
}
