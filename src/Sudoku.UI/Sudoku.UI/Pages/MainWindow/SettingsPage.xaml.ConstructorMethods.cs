namespace Sudoku.UI.Pages.MainWindow;

partial class SettingsPage
{
	/// <summary>
	/// <para>Initializes fields.</para>
	/// <para><i>This method is only called by constructor.</i></para>
	/// </summary>
	private void InitializeFields()
	{
		_boundSteps.ElementAdded += (sender, _) =>
		{
			if (sender is NotifyChangedList<PreferenceBinding> { Count: not 0 } i)
			{
				Button_Save.IsEnabled = true;
				Button_Discard.IsEnabled = true;
			}
		};

		Button_Save.Click += (_, _) =>
		{
			foreach (var (control, setter, restore) in _boundSteps)
			{
				setter();
				restore(control);
			}

			_boundSteps.Clear();
			Button_Save.IsEnabled = false;
			Button_Discard.IsEnabled = false;
		};

		Button_Discard.Click += (_, _) =>
		{
			foreach (var (control, _, restore) in _boundSteps)
			{
				restore(control);
			}

			_boundSteps.Clear();
			Button_Save.IsEnabled = false;
			Button_Discard.IsEnabled = false;
		};
	}

	/// <summary>
	/// <para>Initializes the status of the controls via the <see cref="Preference"/> instance.</para>
	/// <para><i>This method is only called by constructor.</i></para>
	/// </summary>
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
}
