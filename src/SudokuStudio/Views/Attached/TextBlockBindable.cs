namespace SudokuStudio.Views.Attached;

/// <summary>
/// Defines a bind behaviors on <see cref="TextBlock"/> instances.
/// </summary>
/// <seealso cref="TextBlock"/>
[AttachedProperty<IEnumerable<Inline>>(nameof(TextBlock.Inlines))]
public static partial class TextBlockBindable
{
	[Callback]
	private static void InlinesPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if (d is not TextBlock target)
		{
			return;
		}

		if (e.NewValue is not IEnumerable<Inline> inlines)
		{
			return;
		}

		target.Inlines.Clear();
		foreach (var inline in inlines)
		{
			target.Inlines.Add(inline);
		}
	}
}
