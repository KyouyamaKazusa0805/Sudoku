namespace Sudoku.UI.Pages.MainWindow;

partial class SettingsPage
{
	/// <summary>
	/// Initializes fields.
	/// </summary>
	/// <remarks><i>This method is only called by constructor.</i></remarks>
	private void InitializeFields() => _boundSteps.ElementAdded += (sender, [Discard] _) =>
	{
		if (sender is NotifyChangedList<PreferenceBinding> { Count: not 0 } i)
		{
			Button_Save.SetValue(IsEnabledProperty, true);
		}
	};

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

		foreach (var mergedDictionary in UiResources.Dictionaries)
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
}
