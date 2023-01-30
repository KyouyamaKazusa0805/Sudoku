namespace SudokuStudio.Views.Controls;

/// <summary>
/// Represents a header control that is used by <see cref="ExtendedExpander"/>.
/// </summary>
/// <seealso cref="ExtendedExpander"/>
[ContentProperty(Name = nameof(SettingActionableElement))]
public sealed partial class ExtendedExpanderHeader : UserControl
{
	/// <summary>
	/// Defines a dependency property that binds with the property <see cref="Title"/>.
	/// </summary>
	/// <seealso cref="Title"/>
	public static readonly DependencyProperty TitleProperty =
		DependencyProperty.Register(
			nameof(Title),
			typeof(string),
			typeof(ExtendedExpanderHeader),
			new(string.Empty, static (d, e) => AutomationProperties.SetName(d, (string)e.NewValue))
		);

	/// <summary>
	/// Defines a dependency property that binds with the property <see cref="Description"/>.
	/// </summary>
	/// <seealso cref="Description"/>
	public static readonly DependencyProperty DescriptionProperty =
		DependencyProperty.Register(
			nameof(Description),
			typeof(string),
			typeof(ExtendedExpanderHeader),
			new(string.Empty, static (d, e) => AutomationProperties.SetHelpText(d, (string)e.NewValue))
		);

	/// <summary>
	/// Defines a dependency property that binds with the property <see cref="Icon"/>.
	/// </summary>
	/// <seealso cref="Icon"/>
	public static readonly DependencyProperty IconProperty =
		DependencyProperty.Register(
			nameof(Icon),
			typeof(IconElement),
			typeof(ExtendedExpanderHeader),
			new(null)
		);


	/// <summary>
	/// Initializes an <see cref="ExtendedExpanderHeader"/> instance.
	/// </summary>
	public ExtendedExpanderHeader()
	{
		InitializeComponent();

		VisualStateManager.GoToState(this, "NormalState", false);
	}


	/// <summary>
	/// Indicates the title of the control.
	/// </summary>
	public string Title
	{
		get => (string)GetValue(TitleProperty);

		set => SetValue(TitleProperty, value);
	}

	/// <summary>
	/// Indicates the description of the control.
	/// </summary>
	public string Description
	{
		get => (string)GetValue(DescriptionProperty);

		set => SetValue(DescriptionProperty, value);
	}

	/// <summary>
	/// Indicates the icon.
	/// </summary>
	public IconElement Icon
	{
		get => (IconElement)GetValue(IconProperty);

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
		if (e.NewSize.Width == e.PreviousSize.Width || ActionableElement is null)
		{
			return;
		}

		VisualStateManager.GoToState(
			this,
			ActionableElement.ActualWidth > e.NewSize.Width / 3 ? "CompactState" : "NormalState",
			false
		);
	}
}
