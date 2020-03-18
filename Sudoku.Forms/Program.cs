using System;
using System.Windows.Forms;

namespace Sudoku.Forms
{
	/// <summary>
	/// The main class for running the application
	/// (The form of the property <see cref="MainForm"/> points to).
	/// </summary>
	/// <seealso cref="MainForm"/>
	internal static class Program
	{
		/// <summary>
		/// <para>The main form in this application.</para>
		/// <para>
		/// Note that the property is <see langword="static"/>, so it will be initialized later.
		/// </para>
		/// </summary>
		internal static MainForm MainForm { get; private set; }


		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		private static void Main()
		{
			Application.SetHighDpiMode(HighDpiMode.SystemAware);
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(MainForm = new MainForm());
		}
	}
}
