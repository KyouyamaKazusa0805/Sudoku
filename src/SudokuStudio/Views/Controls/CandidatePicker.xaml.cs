namespace SudokuStudio.Views.Controls;

/// <summary>
/// Represents a candidate picker.
/// </summary>
[DependencyProperty<Candidate>("SelectedCandidate", DefaultValue = -1, DocSummary = "Indicates the selected candidate.")]
public sealed partial class CandidatePicker : UserControl
{
	/// <summary>
	/// Initializes a <see cref="CandidatePicker"/> instance.
	/// </summary>
	public CandidatePicker()
	{
		InitializeComponent();
		InitializeControls();
	}


	/// <summary>
	/// Initialize control values.
	/// </summary>
	private void InitializeControls() => UpdateControlViaSelectedCandidate(SelectedCandidate);

	/// <summary>
	/// Update control values via selected candidate.
	/// </summary>
	/// <param name="candidate">Indicates the candidate to be updated.</param>
	private void UpdateControlViaSelectedCandidate(Candidate candidate)
	{
		var cell = candidate / 9;
		var digit = candidate % 9;
		RowDisplayer.Text = SelectedCandidate == -1
			? "row" :
			string.Format(ResourceDictionary.Get("CandidatePicker_RowLabel"), cell / 9 + 1);
		ColumnDisplayer.Text = SelectedCandidate == -1
			? "column"
			: string.Format(ResourceDictionary.Get("CandidatePicker_ColumnLabel"), cell % 9 + 1);
		DigitDisplayer.Text = SelectedCandidate == -1
			? "digit"
			: string.Format(ResourceDictionary.Get("CandidatePicker_DigitLabel"), digit + 1);
	}


	[Callback]
	private static void SelectedCandidatePropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> ((CandidatePicker)d).UpdateControlViaSelectedCandidate((Candidate)e.NewValue);
}
