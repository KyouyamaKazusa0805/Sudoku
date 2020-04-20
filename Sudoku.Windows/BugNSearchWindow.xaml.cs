using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Sudoku.Data;
using Sudoku.Solving.Checking;
using Sudoku.Solving.Utils;

namespace Sudoku.Windows
{
	/// <summary>
	/// Interaction logic for <c>BugNSearchWindow.xaml</c>.
	/// </summary>
	public partial class BugNSearchWindow : Window
	{
		/// <summary>
		/// The puzzle.
		/// </summary>
		private readonly IReadOnlyGrid _puzzle;


		/// <summary>
		/// Initializes an instance with the specified puzzle.
		/// </summary>
		/// <param name="puzzle">The puzzle.</param>
		public BugNSearchWindow(IReadOnlyGrid puzzle)
		{
			InitializeComponent();

			_puzzle = puzzle;
			_labelGrid.Content = $"Grid: {_puzzle:#}";
		}


		[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
		private async void ButtonStartSearching_Click(object sender, RoutedEventArgs e)
		{
			await internalOperation();

			async ValueTask internalOperation()
			{
				_listBoxTrueCandidates.ClearValue(ItemsControl.ItemsSourceProperty);
				_labelStatus.Content = "Searching... The searching may be slow. Please wait.";

				// To be honest, the true candidates can be searched very fast so
				// that we do not need use 'async' keyword...
				var list = new List<PrimaryElementTuple<int, string>>(
					from candidate in await Task.Run(() => new BugChecker(_puzzle).GetAllTrueCandidates(64))
					orderby candidate
					select new PrimaryElementTuple<int, string>(
						candidate, CandidateUtils.ToString(candidate), 2));

				_labelStatus.ClearValue(ContentProperty);
				int count = list.Count;
				if (count == 0)
				{
					_labelStatus.Content = "This puzzle is not a BUG pattern.";
				}
				else
				{
					_listBoxTrueCandidates.ItemsSource = list;
					string isOrAre = count == 1 ? "is" : "are";
					string singularOrPlural = count == 1 ? string.Empty : "s";
					_labelStatus.Content = $"There {isOrAre} {count} true candidate{singularOrPlural} in total.";
				}
			}
		}
	}
}
