namespace Sudoku.UI.Pages.MainWindow;

/// <summary>
/// Indicates the page that contains the settings.
/// </summary>
public sealed partial class SettingsPage : Page
{
	/// <summary>
	/// Indicates the list of possible searching option values that is provided to be searched for later.
	/// </summary>
	/// <remarks>
	/// The collection is <see cref="ICollection{T}"/> of type (<see cref="string"/>, <see cref="string"/>[]).
	/// Each items will be a <see cref="string"/>[] because the details will be searched by each word.
	/// </remarks>
	private readonly List<(string Key, string[] Keywords)> _valuesToBeSearched = new();

	/// <summary>
	/// Indicates the queue of steps used as temporary records.
	/// </summary>
	private readonly NotifyChangedList<PreferenceBinding> _boundSteps = new();

	/// <summary>
	/// Indicates the preferences.
	/// </summary>
	private readonly Preference _preference = new();


	/// <summary>
	/// Initializes a <see cref="SettingsPage"/> instance.
	/// </summary>
	public SettingsPage()
	{
		InitializeComponent();
		InitializeFields();

		AddPossibleSearchValues(_valuesToBeSearched);
	}


	/// <summary>
	/// Triggers when the value of the property <see cref="AutoSuggestBox.Text"/> is changed.
	/// </summary>
	/// <param name="sender">The <see cref="AutoSuggestBox"/> instance to trigger the event.</param>
	/// <param name="args">The event arguments provided.</param>
	private void AutoSuggestBox_OptionSearcher_TextChanged(
		AutoSuggestBox sender,
		AutoSuggestBoxTextChangedEventArgs args
	) => sender.SetValue(ItemsControl.ItemsSourceProperty, (Sender: sender, Args: args) switch
	{
		(_, Args: { Reason: not AutoSuggestionBoxTextChangeReason.UserInput }) => null,
		(Sender: { Text: var q }, _) when !string.IsNullOrWhiteSpace(q) => (
			from pair in _valuesToBeSearched
			where Array.Exists(pair.Keywords, k => k.Contains(q, StringComparison.OrdinalIgnoreCase))
			select pair.Key
		).ToArray(),
		_ => null
	});

	/// <summary>
	/// Triggers when the one element found is chosen.
	/// </summary>
	/// <param name="sender">The <see cref="AutoSuggestBox"/> instance that triggers this event.</param>
	/// <param name="args">The event arguments provided.</param>
	private void AutoSuggestBox_OptionSearcher_SuggestionChosen(
		AutoSuggestBox sender,
		AutoSuggestBoxSuggestionChosenEventArgs args
	)
	{
		switch ((Sender: sender, Args: args, SettingsPanel))
		{
			case (
				Sender: { Text: var senderText },
				Args: { SelectedItem: string itemValue },
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
							sender.ClearValue(TextBlock.TextProperty);
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
	/// Triggers when a <see cref="Button"/> instance is clicked.
	/// </summary>
	/// <param name="sender">The <see cref="Button"/> instance triggered the event.</param>
	/// <param name="e"></param>
	private void Button_Save_Click(object sender, [Discard] RoutedEventArgs e)
	{
		if (sender is not Button button)
		{
			return;
		}

		foreach (var (control, setter, restore) in _boundSteps)
		{
			setter();
			restore(control);
		}

		_boundSteps.Clear();
		button.SetValue(IsEnabledProperty, false);
	}

	/// <summary>
	/// Triggers when the specified <see cref="ToggleSwitch"/> instance is toggled.
	/// </summary>
	/// <param name="sender">The <see cref="ToggleSwitch"/> instance toggled.</param>
	/// <param name="e"></param>
	private void ToggleSwitch_SashimiFishContainsKeywordFinned_Toggled(object sender, [Discard] RoutedEventArgs e)
	{
		if (sender is not ToggleSwitch { IsOn: var isOn })
		{
			return;
		}

		_boundSteps.Add(
			new(
				OptionItem_SashimiFishContainsKeywordFinned,
				() => _preference.SashimiFishContainsKeywordFinned = isOn
			)
		);
	}

	/// <summary>
	/// Triggers when the specified <see cref="ToggleSwitch"/> instance is toggled.
	/// </summary>
	/// <param name="sender">The <see cref="ToggleSwitch"/> instance toggled.</param>
	/// <param name="e"></param>
	private void ToggleSwitch_UseSizedFishName_Toggled(object sender, [Discard] RoutedEventArgs e)
	{
		if (sender is not ToggleSwitch { IsOn: var isOn })
		{
			return;
		}

		_boundSteps.Add(new(OptionItem_UseSizedFishName, () => _preference.UseSizedFishName = isOn));
	}

	/// <summary>
	/// Triggers when the specified <see cref="ToggleSwitch"/> instance is toggled.
	/// </summary>
	/// <param name="sender">The <see cref="ToggleSwitch"/> instance toggled.</param>
	/// <param name="e"></param>
	private void ToggleSwitch_ApplicationTheme_Toggled(object sender, [Discard] RoutedEventArgs e)
	{
		if (sender is not ToggleSwitch { IsOn: var isOn })
		{
			return;
		}

		_boundSteps.Add(
			new(
				OptionItem_ApplicationTheme,
				() =>
				{
					var resultTheme = isOn ? ApplicationTheme.Dark : ApplicationTheme.Light;

					_preference.ApplicationTheme = resultTheme;
#if !DEBUG
					Application.Current.RequestedTheme = resultTheme;
#endif
				}
			)
		);
	}
}
