namespace Sudoku.UI.Views.Controls;

/// <summary>
/// Defines a control that represents a setting header that uses Win11 setting style.
/// </summary>
[ContentProperty(Name = nameof(SettingActionableElement))]
public sealed partial class Win11SettingHeaderControl : UserControl
{
	/// <summary>
	/// Defines a dependency property that binds with the property <see cref="Title"/>.
	/// </summary>
	/// <seealso cref="Title"/>
	public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
		nameof(Title),
		typeof(string),
		typeof(Win11SettingHeaderControl),
		new(string.Empty, static (d, e) => AutomationProperties.SetName(d, (string)e.NewValue))
	);

	/// <summary>
	/// Defines a dependency property that binds with the property <see cref="Description"/>.
	/// </summary>
	/// <seealso cref="Description"/>
	public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(
		nameof(Description),
		typeof(string),
		typeof(Win11SettingHeaderControl),
		new(string.Empty, static (d, e) => AutomationProperties.SetHelpText(d, (string)e.NewValue))
	);

	/// <summary>
	/// Defines a dependency property that binds with the property <see cref="Icon"/>.
	/// </summary>
	/// <seealso cref="Icon"/>
	public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
		nameof(Icon),
		typeof(IconElement),
		typeof(Win11SettingHeaderControl),
		new(null)
	);


	/// <summary>
	/// Initializes a <see cref="Win11SettingHeaderControl"/> instance.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Win11SettingHeaderControl()
	{
		InitializeComponent();

		VisualStateManager.GoToState(this, "NormalState", false);
	}


	/// <summary>
	/// Indicates the title of the control.
	/// </summary>
	public string Title
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (string)GetValue(TitleProperty);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set => SetValue(TitleProperty, value);
	}

	/// <summary>
	/// Indicates the description of the control.
	/// </summary>
	public string Description
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (string)GetValue(DescriptionProperty);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set => SetValue(DescriptionProperty, value);
	}

	/// <summary>
	/// Indicates the icon.
	/// </summary>
	public IconElement Icon
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (IconElement)GetValue(IconProperty);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set => SetValue(IconProperty, value);
	}

	/// <summary>
	/// Indicates the control that is displayed at the right side,
	/// representing a detail information displayer control.
	/// </summary>
	public FrameworkElement? SettingActionableElement { get; set; }


	/// <summary>
	/// Triggers when the size is changed of the main panel.
	/// </summary>
	/// <param name="sender">The object that triggers this event.</param>
	/// <param name="e">The event arguments provided.</param>
	private void MainPanel_SizeChanged(object sender, SizeChangedEventArgs e)
	{
		if (e.NewSize.Width == e.PreviousSize.Width || _cActionableElement is null)
		{
			return;
		}

		if (_cActionableElement.ActualWidth > e.NewSize.Width / 3)
		{
			VisualStateManager.GoToState(this, "CompactState", false);
		}
		else
		{
			VisualStateManager.GoToState(this, "NormalState", false);
		}
	}
}
