using System;
using Microsoft.Maui.Controls;
using System.Diagnostics.CodeAnalysis;
using MemberNotNull = System.Diagnostics.CodeAnalysis.MemberNotNullAttribute;

namespace Sudoku.UI
{
	/// <summary>
	/// The basic interactions about the <c><see cref="MainPage"/>.xaml</c>.
	/// </summary>
	public partial class MainPage : ContentPage
	{
		/// <summary>
		/// Indicates the numbers that the button clicked.
		/// </summary>
		private int _clickedCount;


		/// <summary>
		/// Initializes a <see cref="MainPage"/> instance with no parameters.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This constructor will calls the inner method called <c>InitializeComponent</c>,
		/// but this method will automatically iniatilize all controls in this page implicitly,
		/// but Roslyn don't know this behavior.
		/// </para>
		/// <para>
		/// To be honest, I suggest the team will append <see cref="MemberNotNull"/>
		/// onto the method <c>InitializeComponent</c> in order to avoid Roslyn making the error wave
		/// mark on this method.
		/// </para>
		/// </remarks>
		/// <seealso cref="MemberNotNull"/>
#if NULLABLE
#nullable disable
		public MainPage() => InitializeComponent();
#nullable restore
#endif


		/// <summary>
		/// Executed when the button <see cref="_buttonCounter"/> clicked.
		/// </summary>
		/// <param name="sender">
		/// The sender to trigger the event. The value of <paramref name="sender"/>
		/// should be <see cref="_buttonCounter"/> in general.
		/// </param>
		/// <param name="e">
		/// The event arguments provided. Here this argument will be a discard
		/// because the inner data is unmeaningful.
		/// </param>
		/// <seealso cref="_buttonCounter"/>
		private void OnCounterClicked(object sender, [Discard] EventArgs e)
		{
			if (sender is not Button)
			{
				return;
			}

			_clickedCount++;
			_labelCounter.Text = $"Current count: {_clickedCount.ToString()}";
		}
	}
}
