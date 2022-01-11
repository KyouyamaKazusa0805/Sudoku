using Android.App;
using Android.Runtime;

namespace Sudoku.UI;

/// <summary>
/// Indicates the main application on android.
/// </summary>
[Application]
public class MainApplication : MauiApplication
{
	/// <inheritdoc/>
	public MainApplication(IntPtr handle, JniHandleOwnership ownership) : base(handle, ownership)
	{
	}


	/// <inheritdoc/>
	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
