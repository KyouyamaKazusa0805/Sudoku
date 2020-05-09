using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
#if TARGET_64BIT
using native_uint = System.UInt32;
using native_int = System.Int32;
#else
using native_uint = System.UInt16;
using native_int = System.Int16;
#endif

namespace Sudoku.Windows.Tooling
{
	public partial class ColorDialog
	{
		/// <summary>
		/// Used for call the common dialog "ChooseColor".
		/// </summary>
		[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
		private struct CHOOSECOLOR
		{
			public native_int lStructSize;

			/// <summary>
			/// The base window.
			/// </summary>
			public IntPtr hwndOwner;

			public IntPtr hInstance;

			/// <summary>
			/// The color that user chosen.
			/// </summary>
			public native_int rgbResult;

			/// <summary>
			/// The color that user defined (custom color).
			/// </summary>
			public IntPtr lpCustColors;
			
			/// <summary>
			/// The type of the dialog.
			/// </summary>
			public native_uint Flags;

			public native_int lCustData;

			public IntPtr lpfnHook;

			public IntPtr lpTemplateName;


			/// <summary>
			/// Create an instance of <see cref="CHOOSECOLOR"/> struct.
			/// </summary>
			/// <returns>The result.</returns>
			public static CHOOSECOLOR CreateInstance()
			{
				var cc = new CHOOSECOLOR();
				cc.lStructSize = Marshal.SizeOf(cc);
				cc.hwndOwner = IntPtr.Zero;
				cc.hInstance = IntPtr.Zero;
				cc.rgbResult = 0xFFFFFF;
				cc.lpCustColors = IntPtr.Zero;
				cc.Flags = 0;
				cc.lCustData = 0;
				cc.lpfnHook = IntPtr.Zero;
				cc.lpTemplateName = IntPtr.Zero;

				return cc;
			}
		}
	}
}
