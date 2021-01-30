#nullable enable annotations

using System.Diagnostics.CodeAnalysis;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Sudoku.UI.Pages
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class SettingsBehaviorPage : Page
	{
		/// <summary>
		/// Indicates the preferences used and checked.
		/// </summary>
		private Preferences? _preferences;


		/// <summary>
		/// Initializes a <see cref="SettingsBehaviorPage"/> instance using the default behavior.
		/// </summary>
		public SettingsBehaviorPage() => InitializeComponent();


		/// <summary>
		/// To initialize values.
		/// </summary>
		/// <remarks>
		/// Although this method is <see langword="private"/>, the method will be invoked by reflection
		/// to initialize each values when the settings page is changed.
		/// </remarks>
		[SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
		private void InitializeValues()
		{
			if (_preferences is null)
			{
				return;
			}

			SettingsBehaviorAskBeforeQuitting.IsChecked = _preferences.AskBeforeQuitting;
			SettingsBehaviorUseZeroCharacterWhenCopyCode.IsChecked = _preferences.UseZeroCharacterWhenCopyCode;
			SettingsBehaviorOnlyDisplaySameLevelStepsWhenFindAllSteps.IsChecked = _preferences.OnlyDisplaySameLevelStepsWhenFindAllSteps;
		}


		/// <summary>
		/// Triggers when <see cref="SettingsBehaviorAskBeforeQuitting"/> is clicked.
		/// </summary>
		/// <param name="sender">The object to trigger this event.</param>
		/// <param name="e">The event arguments provided.</param>
		private void SettingsBehaviorAskBeforeQuitting_Click(object sender, RoutedEventArgs e)
		{
			if (_preferences is null)
			{
				return;
			}

			_preferences.AskBeforeQuitting ^= true;
		}

		/// <summary>
		/// Triggers when <see cref="SettingsBehaviorUseZeroCharacterWhenCopyCode"/> is clicked.
		/// </summary>
		/// <param name="sender">The object to trigger this event.</param>
		/// <param name="e">The event arguments provided.</param>
		private void SettingsBehaviorUseZeroCharacterWhenCopyCode_Click(object sender, RoutedEventArgs e)
		{
			if (_preferences is null)
			{
				return;
			}

			_preferences.UseZeroCharacterWhenCopyCode ^= true;
		}

		/// <summary>
		/// Triggers when <see cref="SettingsBehaviorOnlyDisplaySameLevelStepsWhenFindAllSteps"/> is clicked.
		/// </summary>
		/// <param name="sender">The object to trigger this event.</param>
		/// <param name="e">The event arguments provided.</param>
		private void SettingsBehaviorOnlyDisplaySameLevelStepsWhenFindAllSteps_Click(object sender, RoutedEventArgs e)
		{
			if (_preferences is null)
			{
				return;
			}

			_preferences.OnlyDisplaySameLevelStepsWhenFindAllSteps ^= true;
		}
	}
}
