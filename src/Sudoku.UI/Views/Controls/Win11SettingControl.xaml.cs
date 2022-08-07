namespace Sudoku.UI.Views.Controls;

/// <summary>
/// Defines a control that represents a setting that uses Win11 setting style.
/// </summary>
[ContentProperty(Name = nameof(SettingActionableElement))]
public sealed partial class Win11SettingControl : UserControl
{
	/// <summary>
	/// Defines a dependency property that binds with the property <see cref="IsExpanded"/>.
	/// </summary>
	/// <seealso cref="IsExpanded"/>
	public static readonly DependencyProperty IsExpandedProperty =
		DependencyProperty.Register(nameof(IsExpanded), typeof(bool), typeof(Win11SettingControl), new(false));

	/// <summary>
	/// Defines a dependency property that binds with the property <see cref="ExpandableContent"/>.
	/// </summary>
	/// <seealso cref="ExpandableContent"/>
	public static readonly DependencyProperty ExpandableContentProperty =
		DependencyProperty.Register(
			nameof(ExpandableContent),
			typeof(FrameworkElement),
			typeof(Win11SettingControl),
			new(null)
		);

	/// <summary>
	/// Defines a dependency property that binds with the property <see cref="Icon"/>.
	/// </summary>
	/// <seealso cref="Icon"/>
	public static readonly DependencyProperty IconProperty =
		DependencyProperty.Register(nameof(Icon), typeof(IconElement), typeof(Win11SettingControl), new(null));

	/// <summary>
	/// Defines a dependency property that binds with the property <see cref="Description"/>.
	/// </summary>
	/// <seealso cref="Description"/>
	public static readonly DependencyProperty DescriptionProperty =
		DependencyProperty.Register(nameof(Description), typeof(string), typeof(Win11SettingControl), new(string.Empty));

	/// <summary>
	/// Defines a dependency property that binds with the property <see cref="Title"/>.
	/// </summary>
	/// <seealso cref="Title"/>
	public static readonly DependencyProperty TitleProperty =
		DependencyProperty.Register(nameof(Title), typeof(string), typeof(Win11SettingControl), new(string.Empty));


	/// <summary>
	/// Initializes a <see cref="Win11SettingControl"/> instance.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Win11SettingControl() => InitializeComponent();


	/// <summary>
	/// Indicates whether the setting control is expanded.
	/// </summary>
	public bool IsExpanded
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (bool)GetValue(IsExpandedProperty);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set => SetValue(IsExpandedProperty, value);
	}

	/// <inheritdoc cref="Win11SettingHeaderControl.Title"/>
	public string Title
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (string)GetValue(TitleProperty);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set => SetValue(TitleProperty, value);
	}

	/// <inheritdoc cref="Win11SettingHeaderControl.Description"/>
	public string Description
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (string)GetValue(DescriptionProperty);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set => SetValue(DescriptionProperty, value);
	}

	/// <summary>
	/// Indicates the expandable content.
	/// </summary>
	public FrameworkElement ExpandableContent
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (FrameworkElement)GetValue(ExpandableContentProperty);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set => SetValue(ExpandableContentProperty, value);
	}

	/// <inheritdoc cref="Win11SettingHeaderControl.Icon"/>
	public IconElement Icon
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (IconElement)GetValue(IconProperty);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set => SetValue(IconProperty, value);
	}

	/// <inheritdoc cref="Win11SettingHeaderControl.SettingActionableElement"/>
	public FrameworkElement? SettingActionableElement { get; set; }


	/// <summary>
	/// Triggers when the expander is loaded.
	/// </summary>
	/// <param name="sender">The object that triggers this event.</param>
	/// <param name="e">The event arguments provided.</param>
	private void Expander_Loaded(object sender, RoutedEventArgs e)
	{
		AutomationProperties.SetName(Expander, Title);
		AutomationProperties.SetHelpText(Expander, Description);
	}
}
