using System;
using Android.App;
using Android.Runtime;
using Microsoft.Maui;

namespace Sudoku.UI
{
	/// <summary>
	/// Indicates the main application.
	/// </summary>
	[Application]
	public class MainApplication : MauiApplication<Startup>
	{
		/// <summary>
		/// Initializes a <see cref="MainApplication"/> instance with the specified handle and the ownership.
		/// </summary>
		/// <param name="handle">The handle.</param>
		/// <param name="ownership">The ownership.</param>
		public MainApplication(IntPtr handle, JniHandleOwnership ownership) : base(handle, ownership)
		{
		}
	}
}