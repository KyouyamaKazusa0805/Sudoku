using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Sudoku.Analytics.Categorization;
using SudokuStudio.BindableSource;
using SudokuStudio.ComponentModel;
using SudokuStudio.Interaction;

namespace SudokuStudio.Views.Controls;

/// <summary>
/// Represents for a technique view.
/// </summary>
[DependencyProperty<double>("HorizontalSpacing", DocSummary = "Indicates the horizontal spacing.")]
[DependencyProperty<double>("VerticalSpacing", DocSummary = "Indicates the vertical spacing.")]
[DependencyProperty<TechniqueViewSelectionMode>("SelectionMode", DefaultValue = TechniqueViewSelectionMode.Single, DocSummary = "Indicates the selection mode.")]
[DependencyProperty<TechniqueSet>("SelectedTechniques", DocSummary = "Indicates the final selected techniques.")]
public sealed partial class TechniqueView : UserControl
{
	[Default]
	private static readonly TechniqueSet SelectedTechniquesDefaultValue = new();


	/// <summary>
	/// Initializes a <see cref="TechniqueView"/> instance.
	/// </summary>
	public TechniqueView() => InitializeComponent();


	/// <summary>
	/// The items source.
	/// </summary>
	private TechniqueSetTechniqueBindableSource[] ItemsSource
		=>
		from technique in Enum.GetValues<Technique>()[1..]
		where !technique.GetFeature().Flags(TechniqueFeature.NotImplemented)
		select new TechniqueSetTechniqueBindableSource { TechniqueField = technique };


	private void TokenButton_Checked(object sender, RoutedEventArgs e)
	{
		if (SelectionMode == TechniqueViewSelectionMode.None)
		{
			return;
		}

		Func<Technique, bool> f = SelectionMode == TechniqueViewSelectionMode.Single ? SelectedTechniques.Replace : SelectedTechniques.Add;
		f((Technique)((ToggleButton)sender).Tag!);
	}

	private void TokenButton_Unchecked(object sender, RoutedEventArgs e)
	{
		if (SelectionMode == TechniqueViewSelectionMode.None)
		{
			return;
		}

		SelectedTechniques.Remove((Technique)((ToggleButton)sender).Tag!);
	}
}
