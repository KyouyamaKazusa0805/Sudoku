namespace SudokuStudio.Interaction.Conversions;

/// <summary>
/// Provides with conversion methods used by XAML designer, about backdrop.
/// </summary>
internal static class BackdropConversion
{
	public static int GetSelectedIndex(Segmented segmented)
	{
		var backdropKind = ((App)Application.Current).Preference.UIPreferences.Backdrop;
		var i = 0;
		foreach (var element in segmented.Items.Cast<SegmentedItem>())
		{
			if (element.Tag is string s && Enum.TryParse<BackdropKind>(s, out var target) && target == backdropKind)
			{
				return i;
			}

			i++;
		}

		return -1;
	}
}
