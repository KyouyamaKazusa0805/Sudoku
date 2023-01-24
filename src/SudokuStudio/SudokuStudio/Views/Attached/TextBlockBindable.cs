namespace SudokuStudio.Views.Attached;

/// <summary>
/// Defines a bind behaviors on <see cref="TextBlock"/> instances for property <see cref="TextBlock.Inlines"/>.
/// </summary>
/// <seealso cref="TextBlock"/>
/// <seealso cref="TextBlock.Inlines"/>
public static class TextBlockBindable
{
	/// <summary>
	/// Defines a dependency property instance that is bindable inline collection.
	/// </summary>
	public static readonly DependencyProperty InlinesProperty =
		DependencyProperty.RegisterAttached(
			nameof(TextBlock.Inlines),
			typeof(IEnumerable<Inline>),
			typeof(TextBlockBindable),
			new(
				null,
				static (d, e) =>
				{
					if (d is not TextBlock target)
					{
						return;
					}

					target.Inlines.Clear();
					target.Inlines.AddRange((IEnumerable<Inline>)e.NewValue);
				}
			)
		);


	/// <summary>
	/// Gets a <see cref="Inline"/> list.
	/// </summary>
	/// <param name="obj">The dependency object.</param>
	/// <returns>A list of <see cref="Inline"/>s.</returns>
	public static IEnumerable<Inline> GetInlines(DependencyObject obj) => (IEnumerable<Inline>)obj.GetValue(InlinesProperty);

	/// <summary>
	/// Sets a <see cref="Inline"/> list. This will trigger the event to append inlines into the target <see cref="TextBlock"/>.
	/// </summary>
	/// <param name="obj">The dependency object.</param>
	/// <param name="value">The inlines to be added.</param>
	public static void SetInlines(DependencyObject obj, IEnumerable<Inline> value) => obj.SetValue(InlinesProperty, value);
}
