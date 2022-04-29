namespace Sudoku.Bot.Oicq.Concepts;

/// <summary>
/// Defines the console manager.
/// </summary>
[SuppressUnmanagedCodeSecurity]
public static class ConsoleManager
{
	/// <summary>
	/// Indicates whether the current program has a console.
	/// </summary>
	public static bool HasConsole => GetConsoleWindow() != IntPtr.Zero;

	/// <summary>
	/// Creates a new console instance if the process is not attached to a console already.
	/// </summary>
	public static void Show()
	{
		if (!HasConsole)
		{
			AllocConsole();
			invalidate();


			static void invalidate()
			{
				const BindingFlags privateStatic = BindingFlags.Static | BindingFlags.NonPublic;

				var type = typeof(Console);
				var consoleOut = type.GetField("_out", privateStatic);
				var consoleError = type.GetField("_error", privateStatic);
				var consoleInitializeStdOutError = type.GetMethod("InitializeStdOutError", privateStatic);

				Debug.Assert(consoleOut is not null);
				Debug.Assert(consoleError is not null);
				Debug.Assert(consoleInitializeStdOutError is not null);

				consoleOut.SetValue(null, null);
				consoleError.SetValue(null, null);
				consoleInitializeStdOutError.Invoke(null, new object[] { true });
			}
		}
	}

	/// <summary>
	/// If the process has a console attached to it, it will be detached and no longer visible.
	/// Writing to the System.Console is still possible, but no output will be shown.
	/// </summary>
	public static void Hide()
	{
		if (HasConsole)
		{
			Console.SetOut(TextWriter.Null);
			Console.SetError(TextWriter.Null);

			FreeConsole();
		}
	}

	/// <summary>
	/// Toggles the console.
	/// </summary>
	public static void Toggle()
	{
		var hiding = Hide;
		var showing = Show;
		(HasConsole ? hiding : showing)();
	}

	/// <summary>
	/// Allocates a new console for the calling process.
	/// </summary>
	/// <returns>
	/// <para>If the function succeeds, the return value is nonzero.</para>
	/// <para>
	/// If the function fails, the return value is zero. To get extended error information, call
	/// <see href="https://docs.microsoft.com/en-us/windows/win32/api/errhandlingapi/nf-errhandlingapi-getlasterror">GetLastError</see>.
	/// </para>
	/// </returns>
	/// <remarks>
	/// <para>
	/// A process can be associated with only one console, so the <c>AllocConsole</c> function fails
	/// if the calling process already has a console. A process can use the <see cref="FreeConsole"/> function
	/// to detach itself from its current console,
	/// then it can call <see cref="AllocConsole"/> to create a new console
	/// or <see href="https://docs.microsoft.com/en-us/windows/console/attachconsole">AttachConsole</see>
	/// to attach to another console.
	/// </para>
	/// <para>If the calling process creates a child process, the child inherits the new console.</para>
	/// <para>
	/// <see cref="AllocConsole"/> initializes standard input, standard output,
	/// and standard error handles for the new console.
	/// The standard input handle is a handle to the console's input buffer,
	/// and the standard output and standard error handles are handles to the console's screen buffer.
	/// To retrieve these handles, use the
	/// <see href="https://docs.microsoft.com/en-us/windows/console/getstdhandle">GetStdHandle</see> function.
	/// </para>
	/// <para>
	/// This function is primarily used by a graphical user interface (GUI) application to create a console window.
	/// GUI applications are initialized without a console. Console applications are initialized with a console,
	/// unless they are created as detached processes (by calling
	/// the <see href="https://docs.microsoft.com/en-us/windows/win32/api/processthreadsapi/nf-processthreadsapi-createprocessa">CreateProcess</see>
	/// function with the <c>DETACHED_PROCESS</c> flag).
	/// </para>
	/// </remarks>
	[DllImport("kernel32")]
	private static extern bool AllocConsole();

	/// <summary>
	/// Detaches the calling process from its console.
	/// </summary>
	/// <returns>
	/// <para>If the function succeeds, the return value is nonzero.</para>
	/// <para>
	/// If the function fails, the return value is zero. To get extended error information, call
	/// <see href="https://docs.microsoft.com/en-us/windows/win32/api/errhandlingapi/nf-errhandlingapi-getlasterror">GetLastError</see>.
	/// </para>
	/// </returns>
	/// <remarks>
	/// <para>
	/// A process can be attached to at most one console. If the calling process is not already attached to a console,
	/// the error code returned is <c>ERROR_INVALID_PARAMETER</c> (87).
	/// </para>
	/// <para>
	/// A process can use the <see cref="FreeConsole"/> function to detach itself from its console.
	/// If other processes share the console, the console is not destroyed,
	/// but the process that called <see cref="FreeConsole"/> cannot refer to it.
	/// A console is closed when the last process attached to it terminates or calls <see cref="FreeConsole"/>.
	/// After a process calls FreeConsole, it can call the <see cref="AllocConsole"/> function
	/// to create a new console
	/// or <see href="https://docs.microsoft.com/en-us/windows/console/attachconsole">AttachConsole</see>
	/// to attach to another console.
	/// </para>
	/// </remarks>
	[DllImport("kernel32")]
	private static extern bool FreeConsole();

	/// <summary>
	/// Retrieves the output code page used by the console associated with the calling process.
	/// A console uses its output code page to translate the character values written by the various output functions
	/// into the images displayed in the console window.
	/// </summary>
	/// <returns>
	/// <para>
	/// The return value is a code that identifies the code page. For a list of identifiers, see
	/// <see href="https://docs.microsoft.com/en-us/windows/win32/intl/code-page-identifiers">Code Page Identifiers</see>.
	/// </para>
	/// <para>
	/// If the return value is zero, the function has failed. To get extended error information, call
	/// <see href="https://docs.microsoft.com/en-us/windows/win32/api/errhandlingapi/nf-errhandlingapi-getlasterror">GetLastError</see>.
	/// </para>
	/// </returns>
	/// <remarks>
	/// <para>
	/// A code page maps 256 character codes to individual characters.
	/// Different code pages include different special characters, typically customized for a language or a group
	/// of languages. To retrieve more information about a code page, including it's name, see
	/// the <see href="https://docs.microsoft.com/en-us/windows/win32/api/winnls/nf-winnls-getcpinfoexa">GetCPInfoEx</see>
	/// function.
	/// </para>
	/// <para>
	/// To set a console's output code page, use
	/// the <see href="https://docs.microsoft.com/en-us/windows/console/setconsoleoutputcp">SetConsoleOutputCP</see>
	/// function. To set and query a console's input code page, use
	/// the <see href="https://docs.microsoft.com/en-us/windows/console/setconsolecp">SetConsoleCP</see> and
	/// <see href="https://docs.microsoft.com/en-us/windows/console/getconsolecp">GetConsoleCP</see> functions.
	/// </para>
	/// </remarks>
	[DllImport("kernel32")]
	private static extern int GetConsoleOutputCP();

	/// <summary>
	/// Retrieves the window handle used by the console associated with the calling process.
	/// </summary>
	/// <returns>
	/// The return value is a handle to the window used by the console associated with the calling process or
	/// <see langword="null"/> (or <see cref="IntPtr.Zero"/> in C#) if there is no such associated console.
	/// </returns>
	/// <remarks>
	/// <para><i><b>
	/// This document describes console platform functionality that is no longer a part of our
	/// <see href="https://docs.microsoft.com/en-us/windows/console/ecosystem-roadmap">ecosystem roadmap</see>.
	/// We do not recommend using this content in new products, but we will continue to support existing usages
	/// for the indefinite future. Our preferred modern solution focuses on
	/// <see href="https://docs.microsoft.com/en-us/windows/console/console-virtual-terminal-sequences">virtual terminal sequences</see>
	/// for maximum compatibility in cross-platform scenarios. You can find more information
	/// about this design decision in our
	/// <see href="https://docs.microsoft.com/en-us/windows/console/classic-vs-vt">classic console vs. virtual terminal</see>
	/// document.
	/// </b></i></para>
	/// <para>
	/// To compile an application that uses this function, define <c>_WIN32_WINNT</c> as <c>0x0500</c> or later.
	/// For more information, see
	/// <see href="https://docs.microsoft.com/en-us/windows/win32/winprog/using-the-windows-headers">Using the Windows Headers</see>.
	/// </para>
	/// <para><b>
	/// This API is not recommended and does not have a
	/// <see href="https://docs.microsoft.com/en-us/windows/console/console-virtual-terminal-sequences">virtual terminal</see>
	/// equivalent.
	/// This decision intentionally aligns the Windows platform with other operating systems.
	/// This state is only relevant to the local user, session, and privilege context.
	/// Applications remoting via cross-platform utilities and transports like SSH
	/// may not work as expected if using this API.
	/// </b></para>
	/// <para>
	/// For an application that is hosted inside a
	/// <see href="https://docs.microsoft.com/en-us/windows/console/pseudoconsoles">pseudoconsoles</see> session,
	/// this function returns a window handle for message queue purposes only.
	/// The associated window is not displayed locally as the pseudoconsole is serializing all actions
	/// to a stream for presentation on another terminal window elsewhere.
	/// </para>
	/// </remarks>
	[DllImport("kernel32")]
	private static extern IntPtr GetConsoleWindow();
}
