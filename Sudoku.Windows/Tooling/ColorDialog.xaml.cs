using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
using Sudoku.Drawing.Extensions;
using DColor = System.Drawing.Color;

namespace Sudoku.Windows.Tooling
{
	/// <summary>
	/// Interaction logic for <c>ColorDialog.xaml</c>.
	/// </summary>
	public partial class ColorDialog : Window
	{
		/// <summary>
		/// The alpha tunnel.
		/// </summary>
		private byte _alpha;


		/// <include file='../../GlobalDocComments.xml' path='comments/defaultConstructor'/>
		public ColorDialog() => InitializeComponent();


		/// <summary>
		/// Indicates the selected color.
		/// </summary>
		public DColor SelectedColor { get; private set; } = default;


		/// <inheritdoc/>
		protected override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);

			_buttonRgb.Background = new SolidColorBrush(SelectedColor.ToWColor());
		}

		private void SliderAlpha_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) =>
			_alpha = (byte)_sliderAlpha.Value;

		private void ButtonRgb_Click(object sender, RoutedEventArgs e)
		{
			int[] rawCustom = new int[0x10];
			var custHandle = GCHandle.Alloc(rawCustom, GCHandleType.Pinned);
			var cc = CHOOSECOLOR.CreateInstance();
			cc.hwndOwner = GetActiveWindow();
			cc.Flags = (uint)ColorFlags.FullOpen;
			cc.lpCustColors = custHandle.AddrOfPinnedObject();
			if (ChooseColorA(ref cc) != 0)
			{
				SelectedColor = DColor.FromArgb(_alpha, ColorTranslator.FromWin32(cc.rgbResult));
				_buttonRgb.Background = new SolidColorBrush(SelectedColor.ToWColor());
			}

			custHandle.Free();
		}

		/// <summary>
		/// Choose a color using system API.
		/// </summary>
		/// <param name="pChoosecolor">The color structure.</param>
		/// <returns>The result.</returns>
		[DllImport("comdlg32.dll", CharSet = CharSet.Ansi)]
		private static extern int ChooseColorA(ref CHOOSECOLOR pChoosecolor);

		/// <summary>
		/// Get the current active window.
		/// </summary>
		/// <returns>The handle of this window (HWND).</returns>
		[DllImport("user32.dll", SetLastError = true)]
		private static extern IntPtr GetActiveWindow();
	}
}
