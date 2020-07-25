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
		private void InitializeTechniqueBoxes() =>
			_listTechniques.ItemsSource =
				from pair in
					from technique in EnumEx.GetValues<TechniqueCode>()
					let NullableCategory = LangSource[$"Group{technique}"] as string
					where !(NullableCategory is null)
					select (
						_techniqueName: CoreResources.GetValue(technique.ToString()),
						_category: NullableCategory)
				select new TechniqueBox
				{
					TechniqueName = pair._techniqueName,
					Comment = $"{LangSource["Category"]}{pair._category}"
				};
	}
}
