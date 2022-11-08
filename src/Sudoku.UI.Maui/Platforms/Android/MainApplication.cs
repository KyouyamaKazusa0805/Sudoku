using Android.App;
using Android.Runtime;

namespace Sudoku.UI.Maui;

/// <summary>
/// The main application entry point class.
/// </summary>
[Application]
public class MainApplication : MauiApplication
{
	/// <summary>
	/// Initializes a <see cref="MainApplication"/> instance via handle and ownership.
	/// </summary>
	/// <param name="handle">The handle.</param>
	/// <param name="ownership">The ownership.</param>
	public MainApplication(nint handle, JniHandleOwnership ownership) : base(handle, ownership)
	{
	}


	/// <inheritdoc/>
	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
