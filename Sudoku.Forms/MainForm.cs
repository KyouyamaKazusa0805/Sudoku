using System;
using System.Drawing;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Sudoku.Data;
using Sudoku.Drawing;
using Sudoku.Drawing.Layers;
using PointConverter = Sudoku.Drawing.PointConverter;

namespace Sudoku.Forms
{
	/// <summary>
	/// Indicates the main form.
	/// </summary>
	public partial class MainForm : Form
	{
		/// <summary>
		/// Indicates the settings.
		/// </summary>
		private readonly Settings _settings = Settings.DefaultSetting.Clone();

		/// <summary>
		/// Indicates the layer collection.
		/// </summary>
		private LayerCollection _layerCollection;

		/// <summary>
		/// The sudoku grid.
		/// </summary>
		private Grid _grid;

		/// <summary>
		/// The point converter.
		/// </summary>
		private PointConverter _pointConverter;


		/// <include file='../GlobalDocComments.xml' path='comments/defaultConstructor'/>
		public MainForm() => InitializeComponent();


		/// <summary>
		/// Initialization after the initializer <see cref="MainForm.MainForm"/>.
		/// </summary>
		/// <seealso cref="MainForm.MainForm"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void InitializeAfterBase()
		{
			_pointConverter = new PointConverter(_pictureBoxGrid.Width, _pictureBoxGrid.Height);
			_grid = Grid.Empty.Clone();
			_layerCollection = new LayerCollection
			{
				new BackLayer(_pointConverter, _settings.BackgroundColor),
				new GridLineLayer(
					_pointConverter, _settings.GridLineWidth, _settings.GridLineColor),
				new BlockLineLayer(
					_pointConverter, _settings.BlockLineWidth, _settings.BlockLineColor),
				new ValueLayer(
					_pointConverter, _settings.ValueScale, _settings.CandidateScale,
					_settings.GivenColor, _settings.ModifiableColor, _settings.CandidateColor,
					_settings.GivenFontName, _settings.ModifiableFontName,
					_settings.CandidateFontName, _grid)
			};
		}

		/// <summary>
		/// To show the title using default text (<see cref="Control.Text"/>).
		/// </summary>
		/// <seealso cref="Control.Text"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void ShowTitle()
		{
			var assembly = Assembly.GetExecutingAssembly();
			string version = assembly.GetName().Version.ToString();
			string title = assembly.GetCustomAttribute<AssemblyTitleAttribute>().Title;

			Text = $"{title} Ver {version}";
		}

		/// <summary>
		/// To show the specified form.
		/// </summary>
		/// <typeparam name="TForm">The form type.</typeparam>
		/// <param name="byDialog">Indicates whether the form is shown by dialog.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void ShowForm<TForm>(bool byDialog)
			where TForm : Form, new()
		{
			var form = new TForm();
			if (byDialog)
			{
				form.ShowDialog();
			}
			else
			{
				form.Show();
			}
		}

		/// <summary>
		/// To show the image.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void ShowImage()
		{
			var bitmap = new Bitmap(_pointConverter.PanelSize.Width, _pointConverter.PanelSize.Height);
			_layerCollection.IntegrateTo(bitmap);
			_pictureBoxGrid.Image = bitmap;

			GC.Collect();
		}

		/// <summary>
		/// Rearrange the location of the control.
		/// </summary>
		/// <param name="sender">The sender triggered the event.</param>
		/// <param name="control">The control to rearrange the location.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void RearrangeLocationOf(object sender, Control control)
		{
			if (sender is Control senderControl)
			{
				control.Top = senderControl.Top;
			}
		}

		/// <summary>
		/// To get the mouse point at present.
		/// </summary>
		/// <returns>The point.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private Point GetMousePoint() => _pictureBoxGrid.PointToClient(MousePosition);

		/// <summary>
		/// To get the snapshot of this form.
		/// </summary>
		/// <returns>The image.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private Image GetWindowSnapshot()
		{
			var bitmap = new Bitmap(Width, Height);
			using var g = Graphics.FromImage(bitmap);
			g.CopyFromScreen(Location, Point.Empty, bitmap.Size);
			return bitmap;
		}


		private void MainForm_Load(object sender, EventArgs e)
		{
			InitializeAfterBase();
			ShowTitle();
			ShowImage();
		}

		private void MainForm_MouseDown(object sender, MouseEventArgs e)
		{
			ReleaseCapture();
			SendMessage(Handle, WM_SYSCOMMAND, SC_MOVE + HTCAPTION, 0);
		}

		private void ButtonExit_Click(object sender, EventArgs e) => Close();

		private void ButtonAbout_Click(object sender, EventArgs e) =>
			ShowForm<AboutBox>(false);

		private void ButtonMainGrid_MouseUp(object sender, MouseEventArgs e) =>
			RearrangeLocationOf(sender, _panelSelection);

		private void ButtonExit_MouseUp(object sender, MouseEventArgs e) =>
			RearrangeLocationOf(sender, _panelSelection);

		private void ButtonAbout_MouseUp(object sender, MouseEventArgs e) =>
			RearrangeLocationOf(sender, _panelSelection);


		#region Extern utils
		[DllImport("user32.dll")]
		private static extern bool ReleaseCapture();

		/// <summary>
		/// <para>
		/// Sends the specified message to a window or windows. This function calls
		/// the window procedure for the specified window and does not return until the window
		/// procedure has processed the message.
		/// To send a message and return immediately, use the
		/// <a href="https://docs.microsoft.com/zh-cn/windows/win32/api/winuser/nf-winuser-sendmessagecallbacka">
		/// SendMessageCallback</a> or
		/// <a href="https://docs.microsoft.com/zh-cn/windows/win32/api/winuser/nf-winuser-sendnotifymessagea">
		/// SendNotifyMessage</a> function. To post a message to a thread's message queue
		/// and return immediately, use the
		/// <a href="https://docs.microsoft.com/windows/desktop/api/winuser/nf-winuser-postmessagea">
		/// PostMessage</a> or
		/// <a href="https://docs.microsoft.com/zh-cn/windows/win32/api/winuser/nf-winuser-postthreadmessagea">
		/// PostThreadMessage</a> function.
		/// </para>
		/// <para>For more information, please see this
		/// <a href="https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-sendmessage">
		/// link</a>.
		/// </para>
		/// </summary>
		/// <param name="hwnd">
		/// A handle to the window whose window procedure will receive the message.
		/// If this parameter is <b><c>HWND_BROADCAST</c></b> (<c>(HWND)0xffff</c>), the message is sent
		/// to all top-level windows in the system, including disabled or invisible unowned windows,
		/// overlapped windows, and pop-up windows; but the message is not sent to child windows.
		/// Message sending is subject to UIPI. The thread of a process can send messages only
		/// to message queues of threads in processes of lesser or equal integrity level.
		/// </param>
		/// <param name="wMsg">
		/// The message to be sent.
		/// For lists of the system-provided messages, see
		/// <a href="https://docs.microsoft.com/zh-cn/windows/win32/winmsg/about-messages-and-message-queues">
		/// System-Defined Messages</a>.
		/// </param>
		/// <param name="wParam">Additional message-specific information.</param>
		/// <param name="lParam">Additional message-specific information.</param>
		/// <returns>
		/// The return value specifies the result of the message processing; it depends on
		/// the message sent.
		/// </returns>
		[DllImport("user32.dll")]
		private static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);

		private const int WM_SYSCOMMAND = 0x0112;
		private const int SC_MOVE = 0xF010;
		private const int HTCAPTION = 0x0002;
		#endregion
	}
}
