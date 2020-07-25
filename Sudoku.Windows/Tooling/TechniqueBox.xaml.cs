using System;
using System.Windows.Controls;

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
		/// Indicates whether the specified control shows <see cref="CheckBox"/>.
		/// </summary>
		public bool ShowCheckBox { get; set; }

		/// <summary>
		/// Indicates the name of the technique.
		/// </summary>
		public string TechniqueName { get; set; } = string.Empty;

		/// <summary>
		/// Indicates the comment.
		/// </summary>
		public string Comment { get; set; } = string.Empty;


		/// <summary>
		/// Indicates whether the check box changed the status.
		/// </summary>
		public event EventHandler? CheckingChanged;


		private void CheckBox_Click(object sender, System.Windows.RoutedEventArgs e) =>
			CheckingChanged?.Invoke(sender, EventArgs.Empty);
	}
}
