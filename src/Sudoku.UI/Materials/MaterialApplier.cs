using System.Drawing;

namespace Sudoku.UI.Materials;

/// <summary>
/// Provides a marshalling type that holds the method that operates the switching on materials.
/// </summary>
internal static unsafe class MaterialApplier
{
	private const int WM_ERASEBKGND = 0x0014;

	private const int WS_EX_LAYERED = 0x00080000;

	private const int GWL_EXSTYLE = -20;

	private const uint LWA_COLORKEY = 0x00000001;


	/// <summary>
	/// Make the window be transparent.
	/// </summary>
	/// <param name="hWnd">
	/// The handle of the window. You can use the method <see cref="WindowNative.GetWindowHandle(object)"/>
	/// to get the target value. Please note that the return value will be an <see cref="IntPtr"/> value,
	/// which is equivalent to the type <see langword="nint"/> having been introduced in C# 9.
	/// </param>
	public static void MakeTransparent(nint hWnd)
	{
		SetWindowSubclass(hWnd, windowSubClass, 0, 0);

		long nExStyle = GetWindowLong(hWnd, GWL_EXSTYLE);
		if ((nExStyle & WS_EX_LAYERED) == 0)
		{
			SetWindowLong(hWnd, GWL_EXSTYLE, (nint)(nExStyle | WS_EX_LAYERED));
			SetLayeredWindowAttributes(hWnd, (uint)ColorTranslator.ToWin32(SystemDrawingColor.Magenta), 255, LWA_COLORKEY);
		}


		static int windowSubClass(nint hWnd, uint uMsg, nint wParam, nint lParam, nint uIdSubclass, uint dwRefData)
		{
			switch (uMsg)
			{
				case WM_ERASEBKGND:
				{
					GetClientRect(hWnd, out var rect);
					rect.right = 0;

					nint hBrush = CreateSolidBrush(ColorTranslator.ToWin32(SystemDrawingColor.Magenta));
					FillRect(wParam, ref rect, hBrush);
					DeleteObject(hBrush);
					return 1;
				}
			}

			return DefSubclassProc(hWnd, uMsg, wParam, lParam);
		}
	}

	/// <summary>
	/// Applies or removes the specified window for Mica material.
	/// </summary>
	/// <param name="hWnd">
	/// The handle of the window. You can use the method <see cref="WindowNative.GetWindowHandle(object)"/>
	/// to get the target value. Please note that the return value will be an <see cref="IntPtr"/> value,
	/// which is equivalent to the type <see langword="nint"/> having been introduced in C# 9.
	/// </param>
	/// <param name="enable">Indicates whether the operation is applying. If <see langword="false"/>, removing.</param>
	/// <param name="darkMode">Indicates whether the current window uses the dark mode currently.</param>
	/// <seealso cref="WindowNative.GetWindowHandle(object)"/>
	/// <seealso cref="IntPtr"/>
	public static void SetMica(nint hWnd, bool enable, bool darkMode)
	{
		int isMicaEnabled = enable ? 1 : 0;
		_ = DwmSetWindowAttribute(hWnd, (int)DWMWINDOWATTRIBUTE.DWMWA_MICA_EFFECT, ref isMicaEnabled, sizeof(int));

		int isDarkEnabled = darkMode ? 1 : 0;
		_ = DwmSetWindowAttribute(hWnd, (int)DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE, ref isDarkEnabled, sizeof(int));
	}

	/// <summary>
	/// Applies or removes the specified window for acrylic material.
	/// </summary>
	/// <param name="hWnd">
	/// The handle of the window. You can use the method <see cref="WindowNative.GetWindowHandle(object)"/>
	/// to get the target value. Please note that the return value will be an <see cref="IntPtr"/> value,
	/// which is equivalent to the type <see langword="nint"/> having been introduced in C# 9.
	/// </param>
	/// <param name="enable">Indicates whether the operation is applying. If <see langword="false"/>, removing.</param>
	/// <param name="darkMode">Indicates whether the current window uses the dark mode currently.</param>
	/// <seealso cref="WindowNative.GetWindowHandle(object)"/>
	/// <seealso cref="IntPtr"/>
	public static void SetAcrylic(nint hWnd, bool enable, bool darkMode)
		=> SetComposition(hWnd, AccentState.ACCENT_ENABLE_ACRYLICBLURBEHIND, enable, darkMode);

	/// <summary>
	/// Applies or removes the specified window for blur material.
	/// </summary>
	/// <param name="hWnd">
	/// The handle of the window. You can use the method <see cref="WindowNative.GetWindowHandle(object)"/>
	/// to get the target value. Please note that the return value will be an <see cref="IntPtr"/> value,
	/// which is equivalent to the type <see langword="nint"/> having been introduced in C# 9.
	/// </param>
	/// <param name="enable">Indicates whether the operation is applying. If <see langword="false"/>, removing.</param>
	/// <param name="darkMode">Indicates whether the current window uses the dark mode currently.</param>
	/// <seealso cref="WindowNative.GetWindowHandle(object)"/>
	/// <seealso cref="IntPtr"/>
	public static void SetBlur(nint hWnd, bool enable, bool darkMode)
		=> SetComposition(hWnd, AccentState.ACCENT_ENABLE_BLURBEHIND, enable, darkMode);

	private static void SetComposition(nint hWnd, AccentState accentState, bool enable, bool darkMode)
	{
		var accent = enable switch
		{
			true => new AccentPolicy
			{
				AccentState = accentState,
				GradientColor = (uint)(darkMode ? 0x990000 : 0xFFFFFF)
			},
			_ => new AccentPolicy { AccentState = AccentState.ACCENT_DISABLED }
		};
		int structSize = Marshal.SizeOf(accent);

		nint ptr = 0;
		try
		{
			ptr = Marshal.AllocHGlobal(structSize);
			Marshal.StructureToPtr(accent, ptr, false);

			var data = new WindowCompositionAttributeData
			{
				Attribute = WindowCompositionAttribute.WCA_ACCENT_POLICY,
				SizeOfData = structSize,
				Data = ptr
			};

			_ = SetWindowCompositionAttribute(hWnd, ref data);
		}
		finally
		{
			if (ptr != 0)
			{
				Marshal.FreeHGlobal(ptr);
			}
		}
	}

	private static unsafe long GetWindowLong(nint hWnd, int nIndex)
		=> sizeof(nint) == 4 ? GetWindowLong32(hWnd, nIndex) : GetWindowLongPtr64(hWnd, nIndex);

	private static unsafe nint SetWindowLong(nint hWnd, int nIndex, nint dwNewLong)
		=> sizeof(nint) == 4 ? SetWindowLongPtr32(hWnd, nIndex, dwNewLong) : SetWindowLongPtr64(hWnd, nIndex, dwNewLong);

	[DllImport("User32", SetLastError = true, CharSet = CharSet.Auto)]
	private static extern bool GetClientRect(nint hWnd, out RECT lpRect);

	[DllImport("User32", SetLastError = true, CharSet = CharSet.Auto)]
	private static extern bool FillRect(nint hdc, [In] ref RECT rect, nint hbrush);

	[DllImport("Gdi32", SetLastError = true, CharSet = CharSet.Auto)]
	private static extern bool DeleteObject([In] nint hObject);

	[DllImport("Comctl32", SetLastError = true)]
	private static extern bool SetWindowSubclass(nint hWnd, SUBCLASSPROC pfnSubclass, uint uIdSubclass, uint dwRefData);

	[DllImport("User32", SetLastError = true, CharSet = CharSet.Unicode)]
	private static extern bool SetLayeredWindowAttributes(nint hwnd, uint crKey, byte bAlpha, uint dwFlags);

	[DllImport("User32", CharSet = CharSet.Auto, SetLastError = true)]
	private static extern bool GetCursorPos(out PointInt32 lpPoint);

	[DllImport("user32")]
	private static extern bool ShowWindow(nint hWnd, int nCmdShow);

	[DllImport("dwmapi", PreserveSig = true)]
	private static extern int DwmSetWindowAttribute(nint hwnd, int attr, ref int attrValue, int attrSize);

	[DllImport("user32")]
	private static extern int SetWindowCompositionAttribute(nint hwnd, ref WindowCompositionAttributeData data);

	[DllImport("Comctl32", SetLastError = true)]
	private static extern int DefSubclassProc(nint hWnd, uint uMsg, nint wParam, nint lParam);

	[DllImport("User32", CharSet = CharSet.Auto, EntryPoint = "SetWindowLong")]
	private static extern nint SetWindowLongPtr32(nint hWnd, int nIndex, nint dwNewLong);

	[DllImport("User32", CharSet = CharSet.Auto, EntryPoint = "SetWindowLongPtr")]
	private static extern nint SetWindowLongPtr64(nint hWnd, int nIndex, nint dwNewLong);

	[DllImport("Gdi32", SetLastError = true, CharSet = CharSet.Auto)]
	private static extern nint CreateSolidBrush(int crColor);

	[DllImport("User32", EntryPoint = "GetWindowLong", CharSet = CharSet.Auto)]
	private static extern long GetWindowLong32(nint hWnd, int nIndex);

	[DllImport("User32", EntryPoint = "GetWindowLongPtr", CharSet = CharSet.Auto)]
	private static extern long GetWindowLongPtr64(nint hWnd, int nIndex);
}
