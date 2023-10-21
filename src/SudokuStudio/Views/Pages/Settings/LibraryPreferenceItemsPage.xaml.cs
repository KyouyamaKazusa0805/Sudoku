using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SudokuStudio.Interaction;

namespace SudokuStudio.Views.Pages.Settings;

/// <summary>
/// Represents a page that displays for preference items used in puzzle library page and its related pages.
/// </summary>
public sealed partial class LibraryPreferenceItemsPage : Page
{
	/// <summary>
	/// Initializes a <see cref="LibraryPreferenceItemsPage"/> instance.
	/// </summary>
	public LibraryPreferenceItemsPage()
	{
		InitializeComponent();

		InitializeFields();
	}


	/// <summary>
	/// Initializes for fields.
	/// </summary>
	private void InitializeFields()
		=> CandidateDisplayingComboBox.SelectedIndex = (Offset)((App)Application.Current).Preference.LibraryPreferences.LibraryCandidatesVisibility;


	private void CandidateDisplayingComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		=> ((App)Application.Current).Preference.LibraryPreferences.LibraryCandidatesVisibility = (LibraryCandidatesVisibility)CandidateDisplayingComboBox.SelectedIndex;
}
