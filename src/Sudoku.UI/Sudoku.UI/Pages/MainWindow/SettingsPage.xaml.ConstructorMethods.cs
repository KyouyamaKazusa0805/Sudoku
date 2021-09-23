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
			Button_Save.IsEnabled = true;
			Button_Discard.IsEnabled = true;
		}
	};

	/// <summary>
	/// Initializes the status of the controls via the <see cref="Preference"/> instance.
	/// </summary>
	/// <remarks><i>This method is only called by constructor.</i></remarks>
	private void InitializeControls()
	{
		_preference.ApplicationTheme = Application.Current.RequestedTheme;

		// Iterate on each writtable property (i.e. with setter).
		foreach (var property in
			from property in _preference.GetType().GetProperties()
			where property.CanWrite
			select property)
		{
			// Iterate on each attribute marked if worth.
			foreach (var customAttribute in property.CustomAttributes)
			{
				if (
					customAttribute is not
					{
						AttributeType: { GenericTypeArguments: { Length: 2 } typeArgs } attrType,
						ConstructorArguments: { Count: 1 } args
					} || attrType.GetGenericTypeDefinition() != typeof(PreferenceItemAttribute<,>)
				)
				{
					continue;
				}

				// Use reflection to set the values, which avoids us creating too much delegated methods,
				// e.g. 'ToggleSwitch_ApplicationTheme.Loaded'.
				object value = property.GetValue(_preference)!;
				object instance = Activator.CreateInstance(typeArgs[1])!;
				object control = FindName((string)args[0].Value!)!;
				bool success = (bool)instance.GetType()
					.GetMethod(nameof(PreferenceItemConverter<FrameworkElement>.Bind))!
					.Invoke(instance, new[] { value, control })!;

				break;
			}
		}
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
