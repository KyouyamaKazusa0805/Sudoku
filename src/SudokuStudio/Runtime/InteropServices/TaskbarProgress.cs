namespace SudokuStudio.Runtime.InteropServices;

/// <summary>
/// Represents a way to change progress in taskbar icon.
/// </summary>
public static class TaskbarProgress
{
	/// <summary>
	/// Indicates the backing instance casted to <see cref="ITaskbarList3"/>.
	/// </summary>
	private static readonly ITaskbarList3 EntryInstance = (ITaskbarList3)new TaskbarInstance();


	/// <summary>
	/// Indicates whether the application supports for changing in task bar.
	/// </summary>
	public static bool IsTaskbarSupported => Environment.OSVersion.Version >= new Version(6, 1);


	/// <summary>
	/// Try to set state on the specified window.
	/// </summary>
	/// <param name="windowHandle">The handle of the window.</param>
	/// <param name="taskbarState">The taskbar state to be updated.</param>
	public static void SetState(nint windowHandle, TBPFLAG taskbarState)
		=> EntryInstance.SetProgressState(windowHandle, taskbarState);

	/// <summary>
	/// Try to set progress value on the specified window.
	/// </summary>
	/// <param name="windowHandle">The handle of the window.</param>
	/// <param name="progressValue">The progress value currently to be shown.</param>
	/// <param name="progressMax">The maximum value of the progress to be shown.</param>
	public static void SetValue(nint windowHandle, double progressValue, double progressMax)
		=> EntryInstance.SetProgressValue(windowHandle, (ulong)(progressValue * 100), (ulong)(progressMax * 100));
}
