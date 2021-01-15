using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Text;
using System.Windows;
using Microsoft.Win32;
using Sudoku.Data.Stepping;
using Sudoku.DocComments;
using Sudoku.Drawing;
using static System.Drawing.StringAlignment;
using static Sudoku.Windows.Constants;
using static Sudoku.Windows.MainWindow;
using DFontStyle = System.Drawing.FontStyle;

namespace Sudoku.Windows
{
	/// <summary>
	/// Interaction logic for <c>PictureSavingPreferencesWindow.xaml</c>.
	/// </summary>
	/// <remarks>
	/// The conditional compiling symbol <c>ADVANCED_PICTURE_SAVING</c>
	/// is disabled now because the code surrounded with the symbol contains the bug
	/// which makes the quality of the exported picture isn't good enough.
	/// </remarks>
	public partial class PictureSavingPreferencesWindow : Window
	{
		/// <summary>
		/// <para>
		/// Indicates the text that should be output. Sometimes this field may contain
		/// escape characters (such as "<c>$e</c>" means extension).
		/// </para>
		/// <para>If you don't want to add extra text, the field will be <see langword="null"/>.</para>
		/// </summary>
		/// <remarks>
		/// Escape characters use the character <c>$</c> as the header because the symbol
		/// can't be used in file names. Supported formats are below:
		/// <list type="table">
		/// <item>
		/// <term><c>$f</c></term>
		/// <description>The file name.</description>
		/// </item>
		/// <item>
		/// <term><c>$e</c></term>
		/// <description>The file extension.</description>
		/// </item>
		/// <item>
		/// <term><c>$D</c></term>
		/// <description>The date without any extra characters. Such as <c>20200202</c>.</description>
		/// </item>
		/// <item>
		/// <term><c>$d</c></term>
		/// <description>The date with hyphen. Such as <c>2020-02-02</c>.</description>
		/// </item>
		/// </list>
		/// </remarks>
		private string? _format;

		/// <summary>
		/// Indicates the settings in main window.
		/// </summary>
		private readonly WindowsSettings _settings;

		/// <summary>
		/// Indicates the target painter.
		/// </summary>
		private readonly GridPainter _targetPainter;

		/// <summary>
		/// Indicates the grid.
		/// </summary>
		private readonly UndoableGrid _grid;


		/// <summary>
		/// Initializes an instance with the specified size.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="settings">The settings.</param>
		/// <param name="targetPainter">The target painter.</param>
		public PictureSavingPreferencesWindow(
			UndoableGrid grid, WindowsSettings settings, GridPainter targetPainter)
		{
			InitializeComponent();
			_settings = settings;
			_grid = grid;
			_targetPainter = targetPainter;
			_numericUpDownSize.CurrentValue = (decimal)_settings.SavingPictureSize;
			_textBoxFormat.Text = _settings.OutputPictureFormatText ?? string.Empty;
		}


		/// <summary>
		/// Indicates whether the format string is valid.
		/// </summary>
		/// <param name="filePath">The file path.</param>
		/// <param name="text">(<see langword="out"/> parameter) The output text.</param>
		/// <returns>The <see cref="bool"/> value indicating that.</returns>
		private bool IsFormatValid(string filePath, out string text)
		{
			var today = DateTime.Today;
#if AUTHOR_RESERVED
			var thisThursday = today.AddDays(1 - (int)today.DayOfWeek) + TimeSpan.FromDays(3);
#endif

			var dic = new (string Escape, string Replacement)[]
			{
				("$f", Path.GetFileNameWithoutExtension(filePath)),
				("$e", Path.GetExtension(filePath)),
				("$D", today.ToString("yyyyMMdd")),
				("$d", today.ToString("yyyy-MM-dd")),
#if AUTHOR_RESERVED
				("!D", thisThursday.ToString("yyyyMMdd")),
#endif
			};

			var sb = new StringBuilder(_format);
			foreach (var (escape, replacement) in dic)
			{
				sb.Replace(escape, replacement);
			}
			return !(text = sb.ToString()).Contains('$');
		}

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void ButtonSave_Click(object sender, RoutedEventArgs e)
		{
			string originalString = _textBoxFormat.Text;
			_settings.OutputPictureFormatText = _format =
				string.IsNullOrWhiteSpace(originalString) ? null : originalString;

			internalOperation((float)_numericUpDownSize.CurrentValue);
			Close();

			bool internalOperation(float size)
			{
				var saveFileDialog = new SaveFileDialog
				{
					AddExtension = true,
					DefaultExt = "png",
					Filter = (string)LangSource["PictureSavingFilter"],
					Title = (string)LangSource["PictureSavingSaveDialogTitle"]
				};

				if (saveFileDialog.ShowDialog() is not true)
				{
					return !(e.Handled = true);
				}

				var targetPainter = new GridPainter(new(size, size), _settings, _grid)
				{
					View = _targetPainter.View, // May be null.
					CustomView = _targetPainter.CustomView, // May be null.
					Conclusions = _targetPainter.Conclusions
				};

				Bitmap? bitmap = null;
				try
				{
					int selectedIndex = saveFileDialog.FilterIndex;
					string fileName = saveFileDialog.FileName;
					bitmap = targetPainter.Draw();
					switch (selectedIndex)
					{
						case >= -1 and <= 3: // Normal picture formats.
						{
							if (_format is not null)
							{
								if (!IsFormatValid(saveFileDialog.FileName, out string resultText))
								{
									Messagings.CheckFormatString();
									return !(e.Handled = true);
								}

								const int fontSize = 20;
								const string fontName = "Microsoft YaHei UI,Consolas,Times New Roman";
								var result = new Bitmap(bitmap.Width, bitmap.Height + (fontSize << 1));
								using var g = Graphics.FromImage(result);
								using var f = new Font(fontName, fontSize, DFontStyle.Bold);
								using var sf = new StringFormat { Alignment = Center, LineAlignment = Center };
								g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
								g.SmoothingMode = SmoothingMode.HighQuality;
								g.CompositingQuality = CompositingQuality.HighQuality;
								g.InterpolationMode = InterpolationMode.HighQualityBicubic;
								g.Clear(Color.White);
								g.DrawImage(bitmap, 0, 0);
								g.DrawString(
									resultText, f, Brushes.Black, bitmap.Width >> 1,
									bitmap.Height + (fontSize >> 1) + 8, sf);
								SavePicture(result, fileName);
							}
							else
							{
								SavePicture(bitmap, fileName);
							}

							break;
						}
						case 4: // Windows metafile format (WMF).
						{
							using var g = Graphics.FromImage(bitmap);
							using var metaFile = new Metafile(fileName, g.GetHdc());
							using var targetGraphics = Graphics.FromImage(metaFile);
							targetPainter.Draw(null, targetGraphics);
							targetGraphics.Save();

							break;
						}
					}
				}
				catch (Exception ex)
				{
					Messagings.ShowExceptionMessage(ex);
					return false;
				}
				finally
				{
					bitmap?.Dispose();
				}

				_settings.SavingPictureSize = size;
				return true;
			}
		}

		/// <summary>
		/// To save the picture.
		/// </summary>
		/// <param name="bitmap">The bitmap.</param>
		/// <param name="fileName">The file name.</param>
		private static void SavePicture(Bitmap bitmap, string fileName) => bitmap.Save(fileName);

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void ButtonCancel_Click(object sender, RoutedEventArgs e) => Close();
	}
}
