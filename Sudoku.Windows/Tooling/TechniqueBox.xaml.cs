using System;
using System.Windows;
using System.Windows.Controls;
using Sudoku.Solving.Manual;

namespace Sudoku.Windows.Tooling
{
	/// <summary>
	/// Interaction logic for <c>TechniqueBox.xaml</c>.
	/// </summary>
	public partial class TechniqueBox : UserControl
	{
		/// <include file='..\GlobalDocComments.xml' path='comments/defaultConstructor'/>
		public TechniqueBox() => InitializeComponent();


		/// <summary>
		/// Indicates the gategory.
		/// </summary>
		public string Category { get; set; } = string.Empty;

		/// <summary>
		/// Indicates the technique.
		/// </summary>
		public PrimaryElementTuple<string, TechniqueCode> Technique { get; set; }


		/// <summary>
		/// Indicates whether the check box changed the status.
		/// </summary>
		public event EventHandler? CheckingChanged;


		private void CheckBox_Click(object sender, RoutedEventArgs e) =>
			CheckingChanged?.Invoke(sender, EventArgs.Empty);
	}
}
