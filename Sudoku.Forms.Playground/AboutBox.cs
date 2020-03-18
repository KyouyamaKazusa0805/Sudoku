using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace Sudoku.Forms.Playground
{
	partial class AboutBox : Form
	{
		public AboutBox()
		{
			InitializeComponent();

			Text = $"About {AssemblyTitle}";
			_labelProductName.Text = AssemblyProduct;
			_labelVersion.Text = $"Version {AssemblyVersion}";
			_labelCopyright.Text = AssemblyCopyright;
			_labelCompanyName.Text = AssemblyCompany;
			_textBoxDescription.Text = AssemblyDescription;
		}

		public string AssemblyTitle
		{
			get
			{
				object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
				if (attributes.Length > 0)
				{
					var titleAttribute = (AssemblyTitleAttribute)attributes[0];
					string title = titleAttribute.Title;
					if (string.IsNullOrEmpty(title))
					{
						return title;
					}
				}
				return Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
			}
		}

		public string AssemblyVersion =>
			Assembly.GetExecutingAssembly().GetName().Version.ToString();

		public string AssemblyDescription
		{
			get
			{
				object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
				return attributes.Length == 0
					? string.Empty
					: ((AssemblyDescriptionAttribute)attributes[0]).Description;
			}
		}

		public string AssemblyProduct
		{
			get
			{
				object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
				return attributes.Length == 0
					? string.Empty
					: ((AssemblyProductAttribute)attributes[0]).Product;
			}
		}

		public string AssemblyCopyright
		{
			get
			{
				object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
				return attributes.Length == 0
					? string.Empty
					: ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
			}
		}

		public string AssemblyCompany
		{
			get
			{
				object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
				return attributes.Length == 0
					? string.Empty
					: ((AssemblyCompanyAttribute)attributes[0]).Company;
			}
		}


		private void ButtonOk_Click(object sender, EventArgs e) => Close();
	}
}
