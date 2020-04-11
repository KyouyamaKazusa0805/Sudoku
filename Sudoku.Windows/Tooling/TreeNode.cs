using System.Collections.Generic;
using System.Windows.Controls;
using Sudoku.Extensions;

namespace Sudoku.Windows.Tooling
{
	public sealed class TreeNode
	{
		public object? DisplayName { get; set; } = null;

		public List<TreeViewItem> Children { get; set; } = new List<TreeViewItem>();


		public override string ToString() => DisplayName.NullableToString();
	}
}
