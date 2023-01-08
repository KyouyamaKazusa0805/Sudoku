namespace SudokuStudio.Views.Controls;

/// <summary>
/// Defines a cell displayed in a <see cref="SudokuPane"/>.
/// </summary>
/// <seealso cref="SudokuPane"/>
public sealed partial class SudokuPaneCell : UserControl
{
	public static readonly DependencyProperty ValueFontSizeProperty =
		DependencyProperty.Register(nameof(ValueFontSize), typeof(double), typeof(SudokuPaneCell), new(42.0));

	public static readonly DependencyProperty CandidateFontSizeProperty =
		DependencyProperty.Register(nameof(CandidateFontSize), typeof(double), typeof(SudokuPaneCell), new(14.0));


	/// <summary>
	/// Initializes a <see cref="SudokuPaneCell"/> instance.
	/// </summary>
	public SudokuPaneCell() => InitializeComponent();


	/// <summary>
	/// Indicates the size of value text.
	/// </summary>
	public double ValueFontSize
	{
		get => (double)GetValue(ValueFontSizeProperty);

		set => SetValue(ValueFontSizeProperty, value);
	}

	/// <summary>
	/// Indicates the size of candidate text.
	/// </summary>
	public double CandidateFontSize
	{
		get => (double)GetValue(CandidateFontSizeProperty);

		set => SetValue(CandidateFontSizeProperty, value);
	}
}
