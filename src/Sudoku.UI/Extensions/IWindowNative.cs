#if WINDOWS
using System;
using System.Runtime.InteropServices;

namespace Microsoft.Maui;

/// <summary>
/// Indicates the window data.
/// </summary>
[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("EECDBF0E-BAE9-4CB6-A68E-9598E1CB57BB")]
internal interface IWindowNative
{
	/// <summary>
	/// The handle of the current window.
	/// </summary>
	IntPtr WindowHandle { get; }
}

#endif