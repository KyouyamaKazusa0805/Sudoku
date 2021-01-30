using System;
using System.Linq;
using Sudoku.UI.Pages;
using Windows.ApplicationModel.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Sudoku.UI
{
	partial class MainPage
	{
		/// <summary>
		/// Triggers when <see cref="NavigationViewMain"/> is loaded.
		/// </summary>
		/// <param name="sender">The object to trigger this event.</param>
		/// <param name="e">The event arguments provided.</param>
		private void NavigationViewMain_Loaded(object sender, RoutedEventArgs e)
		{
			if (sender is not NavigationView view)
			{
				return;
			}

			UpdateSettingsDisplayTextAndItsToolTip(view);
		}

		/// <summary>
		/// Triggers when a item control of <see cref="NavigationViewMain"/> is invoked.
		/// </summary>
		/// <param name="sender">The object to trigger this event.</param>
		/// <param name="args">The event arguments provided.</param>
		private void NavigationViewMain_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
		{
			if (args.IsSettingsInvoked)
			{
				// Settings page.
				FrameToShowTheSpecifiedPage.Navigate(typeof(SettingsPage));
			}
			else
			{
				// Other pages.
				var allViews = sender.MenuItems.OfType<NavigationViewItem>();

				// Then navigate to the specified page.
				switch (allViews.First(findFirstItem).Tag)
				{
					case nameof(NavigationViewItemQuit): // To exit the program.
					{
						CoreApplication.Exit();
						break;
					}
					case string tag
					when Type.GetType(
						$"Sudoku.UI.Pages.{tag.Substring(nameof(NavigationViewItem).Length)}Page"
					) is { } type: // Navigate to the specified page.
					{
						FrameToShowTheSpecifiedPage.Navigate(type);
						break;
					}
				}
			}

			bool findFirstItem(NavigationViewItem x) => (string)x.Content == (string)args.InvokedItem;
		}
	}
}
