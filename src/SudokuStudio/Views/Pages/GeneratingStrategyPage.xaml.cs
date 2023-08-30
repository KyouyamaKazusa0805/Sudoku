namespace SudokuStudio.Views.Pages;

/// <summary>
/// The generating strategy page.
/// </summary>
[DependencyProperty<bool>("IsEditing", Accessibility = Accessibility.Internal, DocSummary = "Indicates whether the editing operation is processed.")]
[DependencyProperty<bool>("IsEditButtonHovered", Accessibility = Accessibility.Internal, DocSummary = "Indicates whether the edit button is hovered.")]
public sealed partial class GeneratingStrategyPage : Page
{
	/// <summary>
	/// The target item that the corresponding control's tag will set this value.
	/// </summary>
	private const string TargetTagName = "Target";


	/// <summary>
	/// Initializes a <see cref="GeneratingStrategyPage"/> instance.
	/// </summary>
	public GeneratingStrategyPage() => InitializeComponent();


	private void Grid_PointerEntered(object sender, PointerRoutedEventArgs e)
	{
		foreach (var control in ((GridLayout)sender).Children)
		{
			if (control is FrameworkElement { Tag: TargetTagName })
			{
				control.Opacity = 1;
				IsEditButtonHovered = true;
				return;
			}
		}
	}

	private void Grid_PointerExited(object sender, PointerRoutedEventArgs e)
	{
		foreach (var control in ((GridLayout)sender).Children)
		{
			if (control is FrameworkElement { Tag: TargetTagName })
			{
				control.Opacity = 0;
				IsEditButtonHovered = false;
				return;
			}
		}
	}

	private void Button_Click(object sender, RoutedEventArgs e)
	{
		var isEditing = false;
		foreach (var itemControl in RunningStrategy.InternalListView.ItemsPanelRoot.Children)
		{
			if (itemControl is not ListViewItem
				{
					Tag: RunningStrategyItem { Updater.UpdaterControlCreator: var creator },
					Content: StackPanel { Children: [.., ContentPresenter presenter] }
				})
			{
				continue;
			}

			isEditing = true;
			presenter.Content = creator();
			presenter.Opacity = 1;
		}

		IsEditing = isEditing;
	}
}
