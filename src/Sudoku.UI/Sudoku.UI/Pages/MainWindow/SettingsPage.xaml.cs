namespace Sudoku.UI.Pages.MainWindow;

/// <summary>
/// Indicates the page that contains the settings.
/// </summary>
public sealed partial class SettingsPage : Page
{
	/// <summary>
	/// Indicates the regular expression that matches a dictionary file name (a <see cref="Uri"/> string).
	/// </summary>
	private static readonly Regex DictionaryNameRegex = new(
		@"Dic_\d{4,5}\.xaml",
		RegexOptions.IgnoreCase,
		TimeSpan.FromSeconds(5)
	);


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
	private readonly List<PreferenceBinding> _boundSteps = new();

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

		AddPossibleSearchValues(_valuesToBeSearched);
	}


	/// <summary>
	/// Register the specified step into the step collection. When the <see cref="Button_Save"/>
	/// is clicked, all steps will be executed.
	/// </summary>
	/// <param name="propertyName">
	/// The name to set. For example, if you want to assign the property <see cref="Preference.ShowCandidates"/>
	/// to <see langword="true"/>, this argument will be
	/// <c><see langword="nameof"/>(<see cref="Preference"/>.ShowCandidates)</c> or just
	/// <c>"ShowCandidates"</c>.
	/// </param>
	/// <param name="value">
	/// The value to set. For example, if you want to assign the property <see cref="Preference.ShowCandidates"/>
	/// to <see langword="true"/>, this argument will be <c><see langword="true"/></c>.
	/// </param>
	/// <param name="control">The control.</param>
	/// <param name="effect">
	/// The effect color that <paramref name="control"/> will set its foreground.
	/// </param>
	/// <exception cref="ArgumentException">
	/// Throws when the property name <paramref name="propertyName"/> doesn't exist in the type.
	/// </exception>
	/// <exception cref="InvalidOperationException">
	/// Throws when the property found but can't write (i.e. The property <see cref="PropertyInfo.CanWrite"/>
	/// returns <see langword="false"/>).
	/// </exception>
	/// <exception cref="MissingMemberException">
	/// Throws when the type <see cref="Preference"/> doesn't exist such property
	/// specified in <paramref name="propertyName"/>.
	/// </exception>
	private void BindItem(string propertyName, object value, FrameworkElement control, Color effect)
	{
		const string foregroundPropertyName = "Foreground";
		var foregroundPropertyInfo = control.GetType().GetProperty(foregroundPropertyName);

		_boundSteps.Add(typeof(Preference).GetProperty(propertyName) switch
		{
			null => throw new ArgumentException(
				$"The property '{propertyName}' doesn't exist in the type {typeof(Preference)}.",
				nameof(propertyName)
			),
			{ CanWrite: false } => throw new InvalidOperationException("The property found but can't write."),
			var propertyInfo => foregroundPropertyInfo switch
			{
				null => throw new MissingMemberException(typeof(Preference).FullName, foregroundPropertyName),
				_ => new PreferenceBinding(
					control,
					() => propertyInfo.SetValue(_preference, value),
					control => foregroundPropertyInfo.SetValue(
						control,
						new SolidColorBrush(ApplicationRequestedThemes.GetForegroundColor())
					)
				)
			}
		});

		Button_Save.SetValue(IsEnabledProperty, true);
		foregroundPropertyInfo.SetValue(control, new SolidColorBrush(effect));
	}

	/// <summary>
	/// Add setting property items into the specified collection.
	/// </summary>
	/// <param name="collection">The collection.</param>
	/// <remarks><i>This method is only called by constructor.</i></remarks>
	private void AddPossibleSearchValues(ICollection<(string, string[])> collection)
	{
		const string p = "SettingsPage_Option_"; // Of length 20.
		const string q = "_Intro"; // Of length 6.
		const int l = 20, m = 6; // The length of 'p' and 'q'.

		foreach (var mergedDictionary in
			from mergedDictionary in Application.Current.Resources.MergedDictionaries
			where DictionaryNameRegex.IsMatch(mergedDictionary.Source.ToString())
			select mergedDictionary)
		{
			foreach (var kvp in mergedDictionary)
			{
				if (kvp is (key: string { Length: >= l } k, value: string v) && k[..l] == p && k[^m..] != q)
				{
					collection.Add((v, v.Contains(' ') ? v.Split(' ') : new[] { v }));
				}
			}
		}
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

		BindItem(
			nameof(Preference.SashimiFishContainsKeywordFinned),
			isOn,
			OptionItem_SashimiFishContainsKeywordFinned,
			Colors.Gold
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

		BindItem(
			nameof(Preference.UseSizedFishName),
			isOn,
			OptionItem_UseSizedFishName,
			Colors.Gold
		);
	}
}
