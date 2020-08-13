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
		/// Indictaes the chosen techniques.
		/// </summary>
		public TechniqueCodeFilter ChosenTechniques { get; } = new TechniqueCodeFilter();


		/// <summary>
		/// Initializes the techniques, and stores them into the list.
		/// </summary>
		private void InitializeTechniqueBoxes()
		{
			var list = new List<TechniqueBox>();
			foreach (var (name, technique, category) in
				from Technique in EnumEx.GetValues<TechniqueCode>()
				let NullableCategory = LangSource[$"Group{Technique}"] as string
				where NullableCategory is not null
				select (
					_techniqueName: CoreResources.GetValue(Technique.ToString()),
					_technique: Technique,
					_category: NullableCategory))
			{
				var box = new TechniqueBox
				{
					Technique = new PrimaryElementTuple<string, TechniqueCode>(name, technique),
					Category = $"{LangSource["Category"]}{category}"
				};

				box.CheckingChanged += (sender, _) =>
				{
					if (sender is CheckBox { Content: PrimaryElementTuple<string, TechniqueCode> pair } box)
					{
						Action<TechniqueCode> f = box.IsChecked switch
						{
							true => ChosenTechniques.Add,
							false => ChosenTechniques.Remove,
							_ => (_) => { }
						};

						f(pair.Value2);
					}
				};

				list.Add(box);
			}

			_listTechniques.ItemsSource = list;
		}
	}
}
