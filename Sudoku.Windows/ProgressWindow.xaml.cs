using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using Sudoku.DocComments;
using Sudoku.Models;

namespace Sudoku.Windows
{
	/// <summary>
	/// Interaction logic for <c>ProgressWindow.xaml</c>.
	/// </summary>
	public partial class ProgressWindow : Window
	{
		/// <inheritdoc cref="DefaultConstructor"/>
		public ProgressWindow() => InitializeComponent();


		/// <summary>
		/// Indicates the cancellation token source used for cancelling a task.
		/// </summary>
		public CancellationTokenSource? CancellationTokenSource { get; init; }

		/// <summary>
		/// The default progress processing method.
		/// </summary>
		public IProgress<IProgressResult> DefaultReporting =>
			new Progress<IProgressResult>(e =>
			{
				// The dispatcher instance will help us to modify the state of
				// controls while using multi-threads.
				var progressBar = _progressBarInfo;
				var textBlock = _textBlockInfo;
				progressBar.Dispatcher.Invoke(() => progressBar.Value = e.Percentage);
				textBlock.Dispatcher.Invoke(() => textBlock.Text = e.ToString());
			});


		/// <inheritdoc/>
		protected override void OnInitialized(EventArgs e) => base.OnInitialized(e);

		/// <inheritdoc/>
		protected override void OnClosing(CancelEventArgs e)
		{
			if (CancellationTokenSource is null)
			{
				e.Cancel = true;
			}
			else
			{
				CancellationTokenSource.Cancel();
				base.OnClosing(e);
			}
		}
	}
}
