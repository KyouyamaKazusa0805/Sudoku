using System;
using System.Drawing;
using System.Drawing.Text;
using System.Windows;
using System.Windows.Controls;
using Sudoku.Drawing.Extensions;

namespace Sudoku.Windows.Tooling
{
	/// <summary>
	/// Interaction logic for <c>ColorDialog.xaml</c>.
	/// </summary>
	public partial class ColorDialog : Window
	{
		/// <include file='../../GlobalDocComments.xml' path='comments/defaultConstructor'/>
		public ColorDialog() => InitializeComponent();


		/// <summary>
		/// Indicates the selected color.
		/// </summary>
		public Color SelectedColor { get; private set; } = default;


		/// <inheritdoc/>
		protected override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);

			_colorCanvasMain.SelectedColorChanged += (_, e) =>
			{
				if (!(e.NewValue is null))
				{
					SelectedColor = e.NewValue.Value.ToDColor();
				}
			};
		}
	}
}
