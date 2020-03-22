using System;
using System.ComponentModel;
using System.Windows;

namespace Sudoku.Forms
{
	/// <summary>
	/// Interaction logic for SettingsWindow.xaml
	/// </summary>
	public partial class SettingsWindow : Window
	{
		/// <summary>
		/// The base window.
		/// </summary>
		private readonly MainWindow _baseWindow;


		/// <include file='../GlobalDocComments.xml' path='comments/defaultConstructor'/>
		public SettingsWindow(MainWindow baseWindow)
		{
			InitializeComponent();

			_baseWindow = baseWindow;

			Callback += () => baseWindow._updateControlStatus.Invoke(this, EventArgs.Empty);
		}


		/// <summary>
		/// Indicates an event triggering when the form closed, the data will be
		/// called back to the main form (i.e. <see cref="MainWindow"/>).
		/// </summary>
		public event CallbackEventHandler Callback;


		/// <inheritdoc/>
		protected override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);

			// TODO: Show controls with the specified settings.
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			Callback.Invoke();

			base.OnClosing(e);
		}
	}
}
