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
	private readonly ICollection<(string Key, string[] Keywords)> _valuesToBeSearched = new List<(string, string[])>();


	/// <summary>
	/// Initializes a <see cref="SettingsPage"/> instance.
	/// </summary>
	public SettingsPage()
	{
		InitializeComponent();

		AddPossibleSearchValues(_valuesToBeSearched);
	}


	/// <summary>
	/// Add setting property items into the specified collection.
	/// </summary>
	/// <param name="collection">The collection.</param>
	/// <remarks><i>This method is only called by constructor.</i></remarks>
	private void AddPossibleSearchValues(ICollection<(string, string[])> collection)
	{
		const string p = "SettingsPage_Option_"; // Of length 20.
		const int l = 20; // The length of 'p'.

		foreach (var mergedDictionary in
			from mergedDictionary in Application.Current.Resources.MergedDictionaries
			where DictionaryNameRegex.IsMatch(mergedDictionary.Source.ToString())
			select mergedDictionary)
		{
			foreach (var kvp in mergedDictionary)
			{
				if (kvp is (key: string { Length: >= l } k, value: string v) && k[..l] == p)
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
	) => sender.ItemsSource = (Sender: sender, Args: args) switch
	{
		(_, Args: { Reason: not AutoSuggestionBoxTextChangeReason.UserInput }) => null,
		(Sender: { Text: var q }, _) when !string.IsNullOrWhiteSpace(q) => (
			from pair in _valuesToBeSearched
			where Array.Exists(pair.Keywords, k => k.Contains(q, StringComparison.OrdinalIgnoreCase))
			select pair.Key
		).ToArray(),
		_ => null
	};

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
		if (args.SelectedItem is string itemValue)
		{
			foreach (var control in SettingsPanel.Children)
			{
				switch (control)
				{
					case TextBlock { Text: var text } tb when text == itemValue:
					{
						sender.ClearValue(AutoSuggestBox.TextProperty);

						tb.Focus(FocusState.Programmatic);
						break;
					}
				}
			}
		}
	}
}
