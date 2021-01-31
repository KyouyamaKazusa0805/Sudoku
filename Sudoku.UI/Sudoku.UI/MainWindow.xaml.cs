using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Microsoft.UI.Xaml;
using Sudoku.Painting;
using Sudoku.UI.Data;
using Sudoku.UI.Dictionaries;

namespace Sudoku.UI
{
	/// <summary>
	/// The main window of the program. This program will open this window at first.
	/// </summary>
	public sealed partial class MainWindow : Window
	{
		/// <summary>
		/// Initializes a <see cref="MainWindow"/> instance with the default instantiation behavior.
		/// </summary>
		public MainWindow()
		{
			InitializeComponent();

			LoadPreferences();
			InitializeProgramTitle();
		}


		/// <summary>
		/// Indicates the inner preferences.
		/// </summary>
		public PagePreferences Preferences { get; private set; }


		/// <summary>
		/// To load the preferences from local.
		/// </summary>
		[MemberNotNull(nameof(Preferences))]
		private void LoadPreferences()
		{
			// Check the existence of the directory that stores the configuration file.
			if (!Directory.Exists(Path.GetDirectoryName(Paths.ConfigurationFile)))
			{
				Preferences = new();
				return;
			}

			// Check the existence of the configuration file.
			if (!File.Exists(Paths.ConfigurationFile))
			{
				Preferences = new();
				return;
			}

			// Deserialize the object.
			string dir = File.ReadAllText(Paths.ConfigurationFile);
			Preferences =
				PreferencesBase.Deserialize(dir, out var inst) && inst is PagePreferences preferences
				? preferences
				: new();

			// Then set the instance to the main page. In this way we can get the preferences
			// from this property in the main page.
			MainPage.BaseWindow = this;
		}

		/// <summary>
		/// To save the preferences to local.
		/// </summary>
		private void SavePreferences() => PreferencesBase.Serialize(Preferences, Paths.ConfigurationFile);

		/// <summary>
		/// To initialize the property <see cref="Window.Title"/>.
		/// </summary>
		/// <remarks>
		/// When I filled with this property into the XAML page, it'll give me a complier error
		/// <c>WMC061: Property not found</c>, so I moved that statement to here.
		/// </remarks>
		private void InitializeProgramTitle() => Title = ResourceFinder.Current.ProgramTitle;


		/// <summary>
		/// Triggers when the window is closed.
		/// </summary>
		/// <param name="sender">The object to trigger this event.</param>
		/// <param name="args">The event arguments provided.</param>
		private void Window_Closed(object sender, WindowEventArgs args) => SavePreferences();

		/// <summary>
		/// Triggers when the base window of the main page is closing.
		/// </summary>
		/// <param name="sender">The object to trigger this event.</param>
		/// <param name="e">The event arguments provided.</param>
		private void MainPage_BaseWindowClosing(object sender, EventArgs e) => SavePreferences();
	}
}
