namespace Sudoku.Windows.CustomControls;

/// <summary>
/// Interaction logic for <c>TechniqueBox.xaml</c>.
/// </summary>
public partial class TechniqueBox : UserControl
{
	/// <summary>
	/// Initializes a default <see cref="TechniqueBox"/> instance.
	/// </summary>
	public TechniqueBox() => InitializeComponent();


	/// <summary>
	/// Indicates the category.
	/// </summary>
	public string Category { get; set; } = string.Empty;

	/// <summary>
	/// Indicates the technique.
	/// </summary>
	public KeyedTuple<string, Technique>? Technique { get; set; }


	/// <summary>
	/// Indicates whether the check box changed the status.
	/// </summary>
	public event EventHandler? CheckingChanged;


	private void CheckBox_Click(object sender, RoutedEventArgs e) =>
		CheckingChanged?.Invoke(sender, EventArgs.Empty);
}
