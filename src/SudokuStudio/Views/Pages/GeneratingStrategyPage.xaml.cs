using System.SourceGeneration;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using SudokuStudio.ComponentModel;
using SudokuStudio.Interaction;
using SudokuStudio.Views.Controls;

namespace SudokuStudio.Views.Pages;

/// <summary>
/// The generating strategy page.
/// </summary>
[DependencyProperty<bool>("IsEditing", Accessibility = Accessibility.Internal, DocSummary = "Indicates whether the editing operation is processed.")]
[DependencyProperty<bool>("IsCardHovered", Accessibility = Accessibility.Internal, DocSummary = "Indicates whether the edit button is hovered.")]
public sealed partial class GeneratingStrategyPage : Page
{
	/// <summary>
	/// The target item that the corresponding control's tag will set this value.
	/// </summary>
	private const string TargetTagName = "Target";


	/// <summary>
	/// Initializes a <see cref="GeneratingStrategyPage"/> instance.
	/// </summary>
	public GeneratingStrategyPage()
	{
		InitializeComponent();
		InitializeEvents();
	}


	/// <summary>
	/// An event triggered when the target value is changed.
	/// </summary>
	internal event EventHandler? ValueChanged;

	/// <summary>
	/// An event triggered when the target value is canceled.
	/// </summary>
	internal event EventHandler? ValueCanceled;


	/// <summary>
	/// Initializes for events.
	/// </summary>
	private void InitializeEvents()
	{
		ValueChanged += (_, _) => { RunningStrategy.UpdateValues(); RunningStrategy.HideContentPresenters(); };
		ValueCanceled += (_, _) => RunningStrategy.HideContentPresenters();
	}

	/// <summary>
	/// Try to get all sub-controls in items panel.
	/// </summary>
	/// <returns>All sub-controls in items panel.</returns>
	private IEnumerable<ListViewItem> EnumerateSubControls()
	{
		foreach (var control in RunningStrategy.InternalListView.ItemsPanelRoot.Children)
		{
			if (control is ListViewItem { Tag: RunningStrategyItem } c)
			{
				yield return c;
			}
		}
	}


	private void Grid_PointerEntered(object sender, PointerRoutedEventArgs e)
	{
		foreach (var control in ((GridLayout)sender).Children)
		{
			if (control is StackPanel { Children: var children })
			{
				foreach (var child in children)
				{
					if (child is FrameworkElement { Tag: TargetTagName })
					{
						control.Opacity = 1;
						IsCardHovered = true;
						return;
					}
				}
			}
		}
	}

	private void Grid_PointerExited(object sender, PointerRoutedEventArgs e)
	{
		foreach (var control in ((GridLayout)sender).Children)
		{
			if (control is StackPanel { Children: var children })
			{
				foreach (var child in children)
				{
					if (child is FrameworkElement { Tag: TargetTagName })
					{
						control.Opacity = 0;
						IsCardHovered = false;
						return;
					}
				}
			}
		}
	}

	private void EditButton_Click(object sender, RoutedEventArgs e)
	{
		var isEditing = false;
		foreach (var itemControl in EnumerateSubControls())
		{
			if (itemControl is not
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

	private void SubmitButton_Click(object sender, RoutedEventArgs e)
	{
		foreach (var itemControl in EnumerateSubControls())
		{
			if (itemControl is not
				{
					Tag: RunningStrategyItem { Updater.ValueRouter: var router },
					Content: StackPanel { Children: [.., ContentPresenter { Content: FrameworkElement content } presenter] }
				})
			{
				continue;
			}

			router(content, RunningStrategy.WarningInfoDisplayer);

			presenter.Opacity = 0;
		}

		IsEditing = false;

		ValueChanged?.Invoke(this, EventArgs.Empty);
	}

	private void CancelButton_Click(object sender, RoutedEventArgs e)
	{
		IsEditing = false;

		ValueCanceled?.Invoke(this, EventArgs.Empty);
	}
}
