namespace SudokuStudio.Interop;

/// <summary>
/// Provides with some deprecated P/Invoke methods. Such methods may be used in the future.
/// </summary>
internal static class PInvoke_Deprecated
{
	/// <summary>
	/// Retrieves the window handle to the active window attached to the calling thread's message queue.
	/// </summary>
	/// <returns>
	/// The return value is the handle to the active window attached to the calling thread's message queue.
	/// Otherwise, the return value is <see langword="null"/>.
	/// </returns>
	/// <remarks>
	/// <para>
	/// To get the handle to the foreground window, you can use
	/// <see href="https://learn.microsoft.com/en-us/windows/desktop/api/winuser/nf-winuser-getforegroundwindow">GetForegroundWindow</see>.
	/// </para>
	/// <para>
	/// To get the window handle to the active window in the message queue for another thread, use
	/// <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getguithreadinfo">GetGUIThreadInfo</see>.
	/// </para>
	/// </remarks>
	[DllImport("user32")]
	[SuppressMessage("Interoperability", "SYSLIB1054:Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time", Justification = "<Pending>")]
	public static extern nint GetActiveWindow();

	/// <summary>
	/// Retrieves information about the active window or a specified GUI thread.
	/// </summary>
	/// <param name="idThread">
	/// The identifier for the thread for which information is to be retrieved. To retrieve this value, use the
	/// <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getwindowthreadprocessid">GetWindowThreadProcessId</see> function.
	/// If this parameter is <see langword="null"/>, the function returns information for the foreground thread.
	/// </param>
	/// <param name="lpgui">
	/// A pointer to a <see cref="GUITHREADINFO"/> structure that receives information describing the thread.
	/// Note that you must set the cbSize member to <see langword="sizeof"/>(<see cref="GUITHREADINFO"/>) before calling this function.
	/// </param>
	/// <returns>
	/// <para>If the function succeeds, the return value is nonzero.</para>
	/// <para>
	/// If the function fails, the return value is zero. To get extended error information, call
	/// <see href="https://learn.microsoft.com/en-us/windows/win32/api/errhandlingapi/nf-errhandlingapi-getlasterror">GetLastError</see>.
	/// </para>
	/// </returns>
	/// <remarks>
	/// <para>
	/// This function succeeds even if the active window is not owned by the calling process.
	/// If the specified thread does not exist or have an input queue, the function will fail.
	/// </para>
	/// <para>
	/// This function is useful for retrieving out-of-context information about a thread.
	/// The information retrieved is the same as if an application retrieved the information about itself.
	/// </para>
	/// <para>
	/// For an edit control, the returned rcCaret rectangle contains the caret plus information on text direction and padding.
	/// Thus, it may not give the correct position of the cursor. The Sans Serif font uses four characters for the cursor:
	/// <list type="table">
	/// <listheader>
	/// <term>Cursor character</term>
	/// <description>Unicode code point</description>
	/// </listheader>
	/// <item>
	/// <term><c>CURSOR_LTR</c></term>
	/// <description>0xf00c</description>
	/// </item>
	/// <item>
	/// <term><c>CURSOR_RTL</c></term>
	/// <description>0xf00d</description>
	/// </item>
	/// <item>
	/// <term><c>CURSOR_THAI</c></term>
	/// <description>0xf00e</description>
	/// </item>
	/// <item>
	/// <term><c>CURSOR_USA</c></term>
	/// <description>0xfff (this is a marker value with no associated glyph)</description>
	/// </item>
	/// </list>
	/// </para>
	/// <para>
	/// To get the actual insertion point in the rcCaret rectangle, perform the following steps.
	/// <list type="number">
	/// <item>
	/// Call <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getkeyboardlayout">GetKeyboardLayout</see>
	/// to retrieve the current input language.
	/// </item>
	/// <item>Determine the character used for the cursor, based on the current input language.</item>
	/// <item>
	/// Call <see href="https://learn.microsoft.com/en-us/windows/win32/api/wingdi/nf-wingdi-createfonta">CreateFont</see>
	/// using Sans Serif for the font, the height given by <see cref="GUITHREADINFO.rcCaret"/>, and a width of zero.
	/// For <c>fnWeight</c>, call <c>SystemParametersInfo(SPI_GETCARETWIDTH, 0, pvParam, 0)</c>.
	/// If <c>pvParam</c> is greater than 1, set <c>fnWeight</c> to 700, otherwise set <c>fnWeight</c> to 400.
	/// </item>
	/// <item>
	/// Select the font into a device context (DC) and use
	/// <see href="https://learn.microsoft.com/en-us/windows/win32/api/wingdi/nf-wingdi-getcharabcwidthsa">GetCharABCWidths</see>
	/// to get the B width of the appropriate cursor character.
	/// </item>
	/// <item>
	/// Add the B width to <see cref="DrawingRectangle.Left"/> in <see cref="GUITHREADINFO.rcCaret"/>
	/// to obtain the actual insertion point.
	/// </item>
	/// </list>
	/// </para>
	/// <para>
	/// The function may not return valid window handles in the <see cref="GUITHREADINFO"/>
	/// structure when called to retrieve information for the foreground thread, such as when a window is losing activation.
	/// </para>
	/// </remarks>
	/// <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getguithreadinfo">GetGUIThreadInfo function (winuser.h)</see>
	[DllImport("user32")]
	[SuppressMessage("Interoperability", "SYSLIB1054:Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time", Justification = "<Pending>")]
	public static extern bool GetGUIThreadInfo(uint idThread, ref GUITHREADINFO lpgui);
}
