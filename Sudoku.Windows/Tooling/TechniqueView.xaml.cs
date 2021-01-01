using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using Sudoku.DocComments;
using Sudoku.Generating;
using static Sudoku.Windows.MainWindow;
using CoreResources = Sudoku.Windows.Resources;

namespace Sudoku.Windows.Tooling
{
	/// <summary>
	/// Interaction logic for <c>TechniqueView.xaml</c>.
	/// </summary>
	public partial class TechniqueView : UserControl
	{
		/// <inheritdoc cref="DefaultConstructor"/>
		public TechniqueView()
		{
			InitializeComponent();
			InitializeTechniqueBoxes();
		}


		/// <summary>
		/// Indictaes the chosen techniques.
		/// </summary>
		public TechniqueCodeFilter ChosenTechniques { get; } = new();


		/// <summary>
		/// Initializes the techniques, and stores them into the list.
		/// </summary>
		private void InitializeTechniqueBoxes()
		{
			var list = new List<TechniqueBox>();
			foreach (var (name, technique, category) in
				from technique in Enum.GetValues<TechniqueCode>()
				let nullableCategory = LangSource[$"Group{technique}"] as string
				where nullableCategory is not null
				select (CoreResources.GetValue(technique.ToString()), technique, nullableCategory))
			{
				var box = new TechniqueBox
				{
					Technique = new(name, technique),
					Category = $"{LangSource["Category"]}{category}"
				};

				box.CheckingChanged += (sender, _) =>
				{
					if (sender is CheckBox { Content: KeyedTuple<string, TechniqueCode> pair } box)
					{
						Func<TechniqueCode, TechniqueCodeFilter?> f = box.IsChecked switch
						{
							true => ChosenTechniques.Add,
							false => ChosenTechniques.Remove,
							_ => static _ => default
						};

						f(pair.Item2);
					}
				};

				list.Add(box);
			}

			_listTechniques.ItemsSource = list;
		}
	}
}
