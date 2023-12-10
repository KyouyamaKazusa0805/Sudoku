using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SudokuStudio.ComponentModel;
using SudokuStudio.Interaction;

namespace SudokuStudio.Views.Controls;

/// <summary>
/// Represents a running strategy control.
/// </summary>
[DependencyProperty<string>("StrategyName", DefaultValue = "", DocSummary = "Indicates the strategy name.")]
[DependencyProperty<IRunningStrategyItemsProvider>("ItemsProvider?", DocSummary = "An instance that can create a list of items that can be used for the displaying.")]
public sealed partial class RunningStrategy : UserControl
{
	/// <summary>
	/// Initializes a <see cref="RunningStrategy"/> instance.
	/// </summary>
	public RunningStrategy() => InitializeComponent();


	/// <summary>
	/// Hidden content presenters.
	/// </summary>
	internal void HideContentPresenters()
	{
		foreach (var element in InternalListView.ItemsPanelRoot.Children)
		{
			if (element is ListViewItem { Content: StackPanel { Children: [.., ContentPresenter presenter, _] } })
			{
				presenter.Opacity = 0;
			}
		}
	}

	/// <summary>
	/// Indicates the value updated.
	/// </summary>
	internal void UpdateValues()
	{
		foreach (var element in InternalListView.ItemsPanelRoot.Children)
		{
			if (element is ListViewItem
				{
					Tag: RunningStrategyItem { Updater.InitializedValueDisplayer: var displayer },
					Content: StackPanel { Children: [_, TextBlock valuePresenter, ContentPresenter { Content: FrameworkElement control }, _] }
				})
			{
				valuePresenter.Text = displayer();
			}
		}
	}


	[Callback]
	private static void ItemsProviderPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> ((RunningStrategy)d).RunningStrategyItems.Source = ((IRunningStrategyItemsProvider)e.NewValue).Items;
}
