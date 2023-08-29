namespace SudokuStudio.Views.Controls;

/// <summary>
/// Represents a running strategy control.
/// </summary>
[DependencyProperty<string>("StrategyName", DefaultValue = "", DocSummary = "Indicates the strategy name.")]
[DependencyProperty<IRunningStrategyItemsProvider>("ItemsProvider", IsNullable = true, DocSummary = "An instance that can create a list of items that can be used for the displaying.")]
public sealed partial class RunningStrategy : UserControl
{
	/// <summary>
	/// Initializes a <see cref="RunningStrategy"/> instance.
	/// </summary>
	public RunningStrategy() => InitializeComponent();


	[Callback]
	private static void ItemsProviderPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> ((RunningStrategy)d).RunningStrategyItems.Source = ((IRunningStrategyItemsProvider)e.NewValue).Items;
}
