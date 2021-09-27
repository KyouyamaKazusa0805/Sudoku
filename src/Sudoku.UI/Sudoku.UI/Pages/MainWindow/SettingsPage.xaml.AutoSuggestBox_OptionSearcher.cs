namespace Sudoku.UI.Pages.MainWindow;

partial class SettingsPage
{
	/// <summary>
	/// Triggers when the one element found is chosen.
	/// </summary>
	/// <param name="sender">The <see cref="AutoSuggestBox"/> instance that triggers this event.</param>
	/// <param name="args">The event arguments provided.</param>
	private partial void AutoSuggestBox_OptionSearcher_SuggestionChosen(
		AutoSuggestBox sender,
		AutoSuggestBoxSuggestionChosenEventArgs args
	)
	{
		switch ((Sender: sender, EventArgs: args, SettingsPanel))
		{
			case (
				Sender: { Text: var senderText },
				EventArgs: { SelectedItem: string itemValue },
				SettingsPanel: { Children: var controls }
			)
			when !string.IsNullOrWhiteSpace(senderText):
			{
				foreach (var control in controls)
				{
					switch (control)
					{
						case TextBlock { Text: var text, FocusState: FocusState.Unfocused } tb
						when text == itemValue:
						{
							sender.Text = string.Empty;
							tb.Focus(FocusState.Programmatic);

							continue;
						}

						// TODO: Wait for other controls adding.
					}
				}

				break;
			}
		}
	}

	/// <summary>
	/// Triggers when the specified <see cref="AutoSuggestBox"/> instance is submitted the input.
	/// </summary>
	/// <param name="sender">The instance to trigger that event.</param>
	/// <param name="args">The event arguments provided.</param>
	private partial void AutoSuggestBox_OptionSearcher_QuerySubmitted(
		AutoSuggestBox sender,
		AutoSuggestBoxQuerySubmittedEventArgs args
	)
	{
		string queryText = args.QueryText;
		if (string.IsNullOrWhiteSpace(queryText))
		{
			return;
		}

		sender.ItemsSource = (
			from pair in _valuesToBeSearched
			where Array.Exists(pair.Keywords, k => k.Contains(queryText, StringComparison.OrdinalIgnoreCase))
			select pair.Key
		).ToArray();
	}
}
