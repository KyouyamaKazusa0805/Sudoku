using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Sudoku.Forms.Subpages;

namespace Sudoku.Forms
{
	public partial class MainForm : Form
	{
		/// <summary>
		/// Indicates the subpage panel.
		/// </summary>
		private readonly IDictionary<Button, SubpagePanel> _subpageDic = new Dictionary<Button, SubpagePanel>();


		public MainForm() => InitializeComponent();


		private void InitializeAfterBase()
		{
			_subpageDic.Add(_buttonMainGrid, new GridPanel { Dock = DockStyle.Fill });
			_subpageDic.Add(_buttonAbout, new AboutPanel { Dock = DockStyle.Fill });

			ShowSubpage(_buttonMainGrid);
		}

		private void ShowTitle()
		{
			var assembly = Assembly.GetExecutingAssembly();
			string version = assembly.GetName().Version.ToString();
			string title = assembly.GetCustomAttribute<AssemblyTitleAttribute>().Title;

			Text = $"{title} Ver {version}";
		}

		/// <summary>
		/// Rearrange the location of the control.
		/// </summary>
		/// <param name="sender">The sender triggered the event.</param>
		/// <param name="control">The control to rearrange the location.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void RearrangeLocationOf(object sender, Control control)
		{
			if (sender is Control senderControl)
			{
				control.Top = senderControl.Top;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void ShowSubpage(Button button)
		{
			var subpageView = _panelSubpage.Controls;
			var control = _subpageDic[button];
			control.Dock = DockStyle.Fill;

			subpageView.Clear();
			subpageView.Add(control);
		}


		private void MainForm_Load(object sender, EventArgs e)
		{
			InitializeAfterBase();
			ShowTitle();
		}

		private void ButtonMainGrid_Click(object sender, EventArgs e)
		{
			RearrangeLocationOf(sender, _panelSelection);
			ShowSubpage(_buttonMainGrid);
		}

		private void ButtonFile_Click(object sender, EventArgs e)
		{
			RearrangeLocationOf(sender, _panelSelection);
		}

		private void ButtonAbout_Click(object sender, EventArgs e)
		{
			RearrangeLocationOf(sender, _panelSelection);
			ShowSubpage(_buttonAbout);
		}
	}
}
