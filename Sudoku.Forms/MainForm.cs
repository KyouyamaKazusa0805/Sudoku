using System;
using System.Drawing;
using System.Reflection;
using System.Runtime.CompilerServices;
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


		private void ToolStripMenuItem_aboutAuthor_Click(object sender, EventArgs e) =>
			ShowForm<AboutBox>(false);

		private void ToolStripMenuItem_fileQuit_Click(object sender, EventArgs e) =>
			Close();

		private void MainForm_Load(object sender, EventArgs e)
		{
			InitializeAfterBase();
			ShowTitle();
			ShowImage();
		}
	}
}
