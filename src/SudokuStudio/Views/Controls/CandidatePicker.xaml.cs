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
			string.Format(ResourceDictionary.Get("CandidatePicker_RowLabel", App.CurrentCulture), cell / 9 + 1);
		ColumnDisplayer.Text = SelectedCandidate == -1
			? "column"
			: string.Format(ResourceDictionary.Get("CandidatePicker_ColumnLabel", App.CurrentCulture), cell % 9 + 1);
		DigitDisplayer.Text = SelectedCandidate == -1
			? "digit"
			: string.Format(ResourceDictionary.Get("CandidatePicker_DigitLabel", App.CurrentCulture), digit + 1);
	}


	/// <summary>
	/// Indicates the event that is triggered when the selected candidate is changed.
	/// </summary>
	public event CandidatePickerSelectedCandidateChangedEventHandler? SelectedCandidateChanged;


	[Callback]
	private static void SelectedCandidatePropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> ((CandidatePicker)d).UpdateControlViaSelectedCandidate((Candidate)e.NewValue);


	private void DigitSet_SelectedDigitChanged(DigitSet sender, DigitSetSelectedDigitChangedEventArgs e)
	{
		SelectedCandidate = (RowIndexSelector.SelectedDigit * 9 + ColumnIndexSelector.SelectedDigit) * 9 + DigitSelector.SelectedDigit;

		SelectedCandidateChanged?.Invoke(this, new(SelectedCandidate));

		UpdateControlViaSelectedCandidate(SelectedCandidate);
	}
}
