namespace Sudoku.UI.Data;

/// <summary>
/// Provides a binding relation that binds a method that sets a value in the type <see cref="Preference"/>,
/// and a <see cref="Preference"/> instance.
/// </summary>
/// <param name="Control">
/// The control. If you don't bind with a control (e.g. <see cref="Application.RequestedTheme"/>),
/// you can pass the value <see langword="null"/>; otherwise, the value shouldn't be <see langword="null"/>.
/// </param>
/// <param name="ItemSetter">
/// Indicates a serial of methods that sets an item in <see cref="Preference"/>.
/// </param>
/// <param name="ControlRestoration">
/// Indicates a serial of methods that restores the state of a control bound with the setting item.
/// </param>
/// <seealso cref="Preference"/>
internal readonly record struct PreferenceBinding(
	FrameworkElement? Control,
	Action ItemSetter,
	Action<FrameworkElement?> ControlRestoration
)
{
	/// <summary>
	/// Initializes a <see cref="PreferenceBinding"/> instance via the <see cref="TextBlock"/> instance
	/// and the item setter.
	/// </summary>
	/// <param name="control">The <see cref="TextBlock"/> instance.</param>
	/// <param name="itemSetter">
	/// Indicates a serial of methods that sets an item in <see cref="Preference"/>.
	/// </param>
	/// <param name="callback">The callback method that invoked after the constructor called.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public PreferenceBinding(TextBlock control, Action itemSetter, Action? callback = null)
	: this(
		control,
		itemSetter,
		([IsDiscard] _) => control.Foreground = new SolidColorBrush(ApplicationRequestedThemes.GetForegroundColor())
	)
	{
		control.SetValue(TextBlock.ForegroundProperty, new SolidColorBrush(Colors.Gold));

		callback?.Invoke();
	}



	/// <summary>
	/// Creates a <see cref="PreferenceBinding"/> instance via the specified information.
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
	/// <param name="preference">The <see cref="Preference"/> instance to assign the value.</param>
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
	/// <exception cref="NullReferenceException">
	/// Throws when the bound control is <see langword="null"/>.
	/// </exception>
	/// <returns>The result <see cref="PreferenceBinding"/> instance.</returns>
	public static PreferenceBinding Create(
		string propertyName,
		object value,
		FrameworkElement control,
		Preference preference
	)
	{
		const string foregroundPropertyName = "Foreground";
		var foregroundPropertyInfo = control.GetType().GetProperty(foregroundPropertyName);

		return typeof(Preference).GetProperty(propertyName) switch
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
					() => propertyInfo.SetValue(preference, value),
					control => foregroundPropertyInfo.SetValue(
						control ?? throw new NullReferenceException("Here the control can't be null."),
						new SolidColorBrush(ApplicationRequestedThemes.GetForegroundColor())
					)
				)
			}
		};
	}
}
