namespace SudokuStudio.Views.Attached;

/// <summary>
/// Defines a bind behaviors on <see cref="TextBlock"/> instances.
/// </summary>
/// <seealso cref="TextBlock"/>
public static partial class TextBlockBindable
{
	[DependencyProperty]
	public static partial IEnumerable<Inline> Inlines { get; set; }


	[Callback]
	private static void InlinesPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if ((d, e) is not (TextBlock target, { NewValue: IEnumerable<Inline> inlines }))
		{
			return;
		}

		var originalCollectionValues = new Inline[target.Inlines.Count];
		target.Inlines.CopyTo(originalCollectionValues, 0);

		try
		{
			target.Inlines.Clear();
			foreach (var inline in inlines)
			{
				target.Inlines.Add(inline);
			}
		}
		catch (COMException ex) when (ex.HResult == -2146496512)
		{
#if false
			// Rollback.
			target.Inlines.Clear();
			foreach (var inline in originalCollectionValues)
			{
				target.Inlines.Add(inline);
			}
#endif
		}
	}
}
