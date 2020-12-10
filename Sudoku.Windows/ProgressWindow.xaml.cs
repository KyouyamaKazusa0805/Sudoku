using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using Sudoku.DocComments;
using Sudoku.Models;

namespace Sudoku.Windows
{
	/// <summary>
	/// Interaction logic for <c>ProgressWindow.xaml</c>.
	/// </summary>
	public partial class ProgressWindow : Window
	{
		/// <summary>
		/// Indictaes the current window style code.
		/// </summary>
		private const int GWL_STYLE = -16;

		/// <summary>
		/// Indicates the system menu state.
		/// </summary>
		private const int WS_SYSMENU = 0x80000;


		/// <summary>
		/// The close value. The form can be closed if and only if the value is <see langword="true"/>.
		/// </summary>
		private bool _closeValue = false;


		/// <inheritdoc cref="DefaultConstructor"/>
		public ProgressWindow() => InitializeComponent();


		/// <summary>
		/// The default progress processing method.
		/// </summary>
		public IProgress<IProgressResult> DefaultReporting =>
			new Progress<IProgressResult>(
				e =>
				{
					// The dispatcher instance will help us to modify the state of
					// controls while using multi-threads.
					var progressBar = _progressBarInfo;
					var textBlock = _textBlockInfo;
					progressBar.Dispatcher.Invoke(() => progressBar.Value = e.Percentage);
					textBlock.Dispatcher.Invoke(() => textBlock.Text = e.ToString());
				});


		/// <inheritdoc/>
		protected override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);

			var hwnd = new WindowInteropHelper(this).Handle;
			SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
		}

		/// <inheritdoc/>
		protected override void OnClosing(CancelEventArgs e)
		{
			if (_closeValue)
			{
				// The form can be closed if and only if '_closeValue' is true.
				base.OnClosing(e);
			}
			else
			{
				e.Cancel = true;
			}
		}

		/// <summary>
		/// Close the window anyway. This method will set <see cref="_closeValue"/> <see langword="true"/>
		/// value without any condition.
		/// </summary>
		public void CloseAnyway()
		{
			_closeValue = true;
			Close();
		}


		/// <summary>
		/// Retrieves information about the specified window.
		/// The function also retrieves the 32-bit (DWORD) value at the specified offset
		/// into the extra window memory.
		/// </summary>
		/// <param name="hWnd">
		/// A handle to the window and, indirectly, the class to which the window belongs.
		/// </param>
		/// <param name="nIndex">
		/// The zero-based offset to the value to be retrieved.
		/// Valid values are in the range zero through the number of bytes of extra window memory,
		/// minus four; for example, if you specified 12 or more bytes of extra memory,
		/// a value of 8 would be an index to the third 32-bit integer.
		/// To retrieve any other value, specify one of the following values.
		/// </param>
		/// <returns>
		/// Returns the value if:
		/// <list type="table">
		/// <item>
		/// <term>succeeds</term>
		/// <description>The return value is the requested value.</description>
		/// </item>
		/// <item>
		/// <term>fails</term>
		/// <description>
		/// The return value is zero. In addition, to get extended error information, call <c>GetLastError</c>.
		/// </description>
		/// </item>
		/// <item>
		/// <term><c>SetWindowLong</c> has not been called previously</term>
		/// <description>
		/// <c>GetWindowLong</c> returns zero for values in the extra window or class memory.
		/// </description>
		/// </item>
		/// </list>
		/// </returns>
		/// <remarks>
		/// Reserve extra window memory by specifying a nonzero value in the <c>cbWndExtra</c> member
		/// of the <c>WNDCLASSEX</c> structure used with the <c>RegisterClassEx</c> function.
		/// </remarks>
		[DllImport("user32.dll", SetLastError = true)]
		private static extern int GetWindowLong(IntPtr hWnd, nint nIndex);

		/// <summary>
		/// Changes an attribute of the specified window.
		/// The function also sets the 32-bit (long) value at the specified offset into the extra window memory.
		/// </summary>
		/// <param name="hWnd">
		/// A handle to the window and, indirectly, the class to which the window belongs.
		/// </param>
		/// <param name="nIndex">
		/// The zero-based offset to the value to be set.
		/// Valid values are in the range zero through the number of bytes of extra window memory,
		/// minus the size of an integer. To set any other value, specify one of the following values.
		/// </param>
		/// <param name="dwNewLong">The replacement value.</param>
		/// <returns>
		/// Return the value if:
		/// <list type="table">
		/// <item>
		/// <term>succeeds</term>
		/// <description>The return value is the previous value of the specified 32-bit integer.</description>
		/// </item>
		/// <item>
		/// <term>fails</term>
		/// <description>
		/// The return value is zero. To get extended error information, call <c>GetLastError</c>.
		/// </description>
		/// </item>
		/// <item>
		/// <term>The previous value of the specified 32-bit integer is zero, and the function succeeds</term>
		/// <description>
		/// The return value is zero, but the function doesn't clear the last error information.
		/// This makes it difficult to determine success or failure.To deal with this,
		/// you should clear the last error information by calling <c>SetLastError</c> with 0 before calling
		/// <c>SetWindowLong</c>.
		/// Then, function failure will be indicated by a return value of zero and a
		/// <c>GetLastError</c> result that is non-zero.
		/// </description>
		/// </item>
		/// </list>
		/// </returns>
		/// <remarks>
		/// <para>
		/// Certain window data is cached, so changes you make using <c>SetWindowLong</c> won't take effect
		/// until you call the <c>SetWindowPos</c> function. Specifically,
		/// if you change any of the frame styles, you must call <c>SetWindowPos</c>
		/// with the <c>SWP_FRAMECHANGED</c> flag for the cache to be updated properly.
		/// </para>
		/// <para>
		/// If you use <c>SetWindowLong</c> with the <c>GWL_WNDPROC</c> index to replace the window procedure,
		/// the window procedure must conform to the guidelines specified in the description
		/// of the <c>WindowProc</c> callback function.
		/// </para>
		/// <para>
		/// If you use <c>SetWindowLong</c> with the <c>DWL_MSGRESULT</c> index to set the return value
		/// for a message processed by a dialog procedure, you should return <see langword="true"/>
		/// directly afterward.
		/// Otherwise, if you call any function that results in your dialog procedure receiving a window message,
		/// the nested window message could overwrite the return value you set using <c>DWL_MSGRESULT</c>.
		/// </para>
		/// <para>
		/// Calling <c>SetWindowLong</c> with the <c>GWL_WNDPROC</c> index creates a subclass
		/// of the window class used to create the window. An application can subclass a system class,
		/// but should not subclass a window class created by another process.
		/// The <c>SetWindowLong</c> function creates the window subclass by changing the window procedure
		/// associated with a particular window class, causing the system to call the new window procedure
		/// instead of the previous one.
		/// An application must pass any messages not processed by the new window procedure
		/// to the previous window procedure by calling <c>CallWindowProc</c>.
		/// This allows the application to create a chain of window procedures.
		/// </para>
		/// <para>
		/// Reserve extra window memory by specifying a non-zero value in the <c>cbWndExtra</c> member
		/// of the <c>WNDCLASSEX</c> structure used with the <c>RegisterClassEx</c> function.
		/// </para>
		/// <para>
		/// You must not call <c>SetWindowLong</c> with the <c>GWL_HWNDPARENT</c> index to change the parent of
		/// a child window. Instead, use the <c>SetParent</c> function.
		/// </para>
		/// <para>
		/// If the window has a class style of <c>CS_CLASSDC</c> or <c>CS_OWNDC</c>,
		/// don't set the extended window styles <c>WS_EX_COMPOSITED</c> or <c>WS_EX_LAYERED</c>.
		/// </para>
		/// <para>
		/// Calling <c>SetWindowLong</c> to set the style on a progressbar will reset its position.
		/// </para>
		/// </remarks>
		[DllImport("user32.dll")]
		private static extern int SetWindowLong(IntPtr hWnd, nint nIndex, int dwNewLong);
	}
}
