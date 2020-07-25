using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using Sudoku.Extensions;
using Sudoku.Solving.Manual;
using static Sudoku.Windows.Constants.Processings;
using CoreResources = Sudoku.Windows.Resources;

namespace Sudoku.Windows.Tooling
{
	/// <summary>
	/// Interaction logic for <c>TechniqueView.xaml</c>.
	/// </summary>
	public partial class TechniqueView : UserControl
	{
		/// <include file='..\GlobalDocComments.xml' path='comments/defaultConstructor'/>
		public TechniqueView()
		{
			InitializeComponent();
			InitializeTechniqueBoxes();
		}


		/// <summary>
		/// Initializes the techniques, and stores them into the list.
		/// </summary>
		private void InitializeTechniqueBoxes()
		{
			var list = new List<TechniqueBox>();
			foreach (var (techniqueName, technique, category) in
				from technique in EnumEx.GetValues<TechniqueCode>()
				let NullableCategory = LangSource[$"Group{technique}"] as string
				where !(NullableCategory is null)
				select (
					_techniqueName: CoreResources.GetValue(technique.ToString()),
					_technique: technique,
					_category: NullableCategory))
			{
				var box = new TechniqueBox
				{
					Content = techniqueName,
					Comment = $"{LangSource["Category"]}{category}"
				};

				box.CheckingChanged += TechniqueBox_CheckingChanged;

				list.Add(box);
			}

			_listTechniques.ItemsSource = list;
		}

		private void TechniqueBox_CheckingChanged(object? sender, EventArgs e)
		{
			if (sender is TechniqueBox box)
			{

			}
		}
	}
}
