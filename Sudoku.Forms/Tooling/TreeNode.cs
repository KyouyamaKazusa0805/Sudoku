using System.Collections.Generic;
using System.Windows.Controls;
using Sudoku.Data.Extensions;

namespace Sudoku.Forms.Tooling
{
	public sealed class TreeNode
	{
		public object? Header { get; set; } = null;

		public List<TreeViewItem> Children { get; set; } = new List<TreeViewItem>();


		public override string ToString() => Header.NullableToString();
	}
}
