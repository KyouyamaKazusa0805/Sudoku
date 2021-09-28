namespace Sudoku.UI.CustomControls;

/// <summary>
/// Defines a customized settings option navigation view.
/// </summary>
public partial class CustomizedSettingsNavigationView : NavigationView
{
	/// <summary>
	/// Indicates the settings navigation view item default name.
	/// </summary>
	private const string SettingsNavigationViewItemName = "SettingsNavPaneItem";


	/// <summary>
	/// Indicates the dependency object that binds with the property <see cref="SettingsItemText"/>.
	/// </summary>
	/// <seealso cref="SettingsItemText"/>
	public static readonly DependencyProperty SettingsItemTextProperty = DependencyProperty.Register(
		nameof(SettingsItemText),
		typeof(string),
		typeof(CustomizedSettingsNavigationView),
		new(default(string), Callback)
	);


	/// <summary>
	/// Indicates the inner settings view item.
	/// </summary>
	private NavigationViewItem? _settingsItem;


	/// <summary>
	/// Initializes a <see cref="CustomizedSettingsNavigationView"/> instance.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public CustomizedSettingsNavigationView() => DefaultStyleKey = typeof(CustomizedSettingsNavigationView);


	/// <summary>
	/// Indicates the text content that displays in the menu item.
	/// </summary>
	public string? SettingsItemText
	{
		get => (string)GetValue(SettingsItemTextProperty);

		set => SetValue(SettingsItemTextProperty, value);
	}


	/// <inheritdoc/>
	protected override void OnApplyTemplate()
	{
		base.OnApplyTemplate();

		_settingsItem = (NavigationViewItem)GetTemplateChild(SettingsNavigationViewItemName);
		UpdateSettingsItemText();
	}

	/// <summary>
	/// Update the property <see cref="SettingsItemText"/>.
	/// </summary>
	/// <seealso cref="SettingsItemText"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void UpdateSettingsItemText()
	{
		if (_settingsItem is not null)
		{
			_settingsItem.Content = SettingsItemText ?? "Settings";
			_settingsItem.Icon = new SymbolIcon(Symbol.Setting);
		}
	}


	private static void Callback(DependencyObject d, [Discard] DependencyPropertyChangedEventArgs e)
	{
		if (d is CustomizedSettingsNavigationView navigationView)
		{
			navigationView.UpdateSettingsItemText();
		}
	}
}
