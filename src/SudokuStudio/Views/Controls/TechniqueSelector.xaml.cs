using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Sudoku.Analytics.Categorization;
using SudokuStudio.BindableSource;
using SudokuStudio.ComponentModel;
using static SudokuStudio.Strings.StringsAccessor;

namespace SudokuStudio.Views.Controls;

/// <summary>
/// Represents a technique selector control.
/// </summary>
[DependencyProperty<int>("SelectedIndex", DocSummary = "Indicates the selected index.")]
public sealed partial class TechniqueSelector : UserControl
{
	/// <summary>
	/// Initializes a <see cref="TechniqueSelector"/> instance.
	/// </summary>
	public TechniqueSelector() => InitializeComponent();


	/// <summary>
	/// Indicates the base items source.
	/// </summary>
	internal TechniqueBindableSource[] ItemsSource
		=>
		from field in Enum.GetValues<Technique>()
		let feature = field.GetFeature()
		where feature is 0 or TechniqueFeature.HardToBeGenerated or TechniqueFeature.DirectTechniques
		let displayName = field == 0 ? GetString("TechniqueSelector_NoTechniqueSelected") : field.GetName()
		select new TechniqueBindableSource { DisplayName = displayName, Technique = field, Feature = feature };


	/// <inheritdoc cref="Selector.SelectionChanged"/>
	public event SelectionChangedEventHandler? SelectionChanged;


	private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) => SelectionChanged?.Invoke(this, e);
}
