using Android.App;
using Android.Runtime;

namespace Sudoku.Maui;

/// <summary>
/// The main application.
/// </summary>
[Application]
public sealed class MainApplication : MauiApplication
{
	/// <summary>
	/// Initializes a <see cref="MainApplication"/> instance via the specified handle and ownership.
	/// </summary>
	/// <param name="handle">The handle.</param>
	/// <param name="ownership">The ownership.</param>
	public MainApplication(nint handle, JniHandleOwnership ownership) : base(handle, ownership)
	{
	}


	/// <inheritdoc/>
	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
