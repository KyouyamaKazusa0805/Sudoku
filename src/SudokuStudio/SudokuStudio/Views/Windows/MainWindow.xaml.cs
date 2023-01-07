namespace SudokuStudio.Views.Windows;

/// <summary>
/// Provides with a <see cref="Window"/> instance that is running as main instance of the program.
/// </summary>
/// <seealso cref="Window"/>
public sealed partial class MainWindow : Window
{
	/// <summary>
	/// Initializes a <see cref="MainWindow"/> instance.
	/// </summary>
	public MainWindow()
	{
		InitializeComponent();
		InitializeWindowTitle();
		TrySetMicaBackdrop();
	}


	/// <summary>
	/// Initializes for property <see cref="Window.Title"/>.
	/// </summary>
	/// <seealso cref="Window.Title"/>
	private void InitializeWindowTitle()
	{
		var version = ((App)Application.Current).RunningContext.AssemblyVersion.ToString(3);

		Title = $"{R["_ProgramName"]!} V{version}";
	}
}
