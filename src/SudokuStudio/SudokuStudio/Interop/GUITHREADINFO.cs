namespace SudokuStudio.Interop;

/// <summary>
/// Contains information about a GUI thread.
/// </summary>
/// <remarks>
/// This structure is used with the
/// <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getguithreadinfo">GetGUIThreadInfo</see>
/// function to retrieve information about the active window or a specified GUI thread.
/// </remarks>
[StructLayout(LayoutKind.Sequential)]
internal struct GUITHREADINFO
{
	/// <summary>
	/// The size of this structure, in bytes. The caller must set this member to <see langword="sizeof"/>(<see cref="GUITHREADINFO"/>).
	/// </summary>
	public int cbSize;

	/// <summary>
	/// The thread state. This member can be one or more of the following values.
	/// <list type="table">
	/// <listheader>
	/// <term>Value</term>
	/// <description>Meaning</description>
	/// </listheader>
	/// <item>
	/// <term>
	/// <c>GUI_CARETBLINKING</c><br/>
	/// 0x00000001
	/// </term>
	/// <description>The caret's blink state. This bit is set if the caret is visible.</description>
	/// </item>
	/// <item>
	/// <term>
	/// <c>GUI_INMENUMODE</c><br/>
	/// 0x00000004
	/// </term>
	/// <description>The thread's menu state. This bit is set if the thread is in menu mode.</description>
	/// </item>
	/// <item>
	/// <term>
	/// <c>GUI_INMOVESIZE</c><br/>
	/// 0x00000002
	/// </term>
	/// <description>The thread's move state. This bit is set if the thread is in a move or size loop.</description>
	/// </item>
	/// <item>
	/// <term>
	/// <c>GUI_POPUPMENUMODE</c><br/>
	/// 0x00000010
	/// </term>
	/// <description>The thread's pop-up menu state. This bit is set if the thread has an active pop-up menu.</description>
	/// </item>
	/// <item>
	/// <term>
	/// <c>GUI_SYSTEMMENUMODE</c><br/>
	/// 0x00000008
	/// </term>
	/// <description>The thread's system menu state. This bit is set if the thread is in a system menu mode.</description>
	/// </item>
	/// </list>
	/// </summary>
	public GuiThreadInfoFlags flags;

	/// <summary>
	/// A handle to the active window within the thread.
	/// </summary>
	public nint hwndActive;

	/// <summary>
	/// A handle to the window that has the keyboard focus.
	/// </summary>
	public nint hwndFocus;

	/// <summary>
	/// A handle to the window that has captured the mouse.
	/// </summary>
	public nint hwndCapture;

	/// <summary>
	/// A handle to the window that owns any active menus.
	/// </summary>
	public nint hwndMenuOwner;

	/// <summary>
	/// A handle to the window in a move or size loop.
	/// </summary>
	public nint hwndMoveSize;

	/// <summary>
	/// A handle to the window that is displaying the caret.
	/// </summary>
	public nint hwndCaret;

	/// <summary>
	/// The caret's bounding rectangle, in client coordinates, relative to the window specified by the hwndCaret member.
	/// </summary>
	public DrawingRectangle rcCaret;
}
