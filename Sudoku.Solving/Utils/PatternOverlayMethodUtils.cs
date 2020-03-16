using System.Collections.Generic;
using System.Linq;
using Sudoku.Data;

namespace Sudoku.Solving.Utils
{
	/// <summary>
	/// Provides extension method used for pattern overlay method (POM).
	/// </summary>
	public static class PatternOverlayMethodUtils
	{
		/// <summary>
		/// The templates of all placement cases of a single digit.
		/// </summary>
		public static IReadOnlyList<GridMap> Templates
		{
			get
			{
				var templates = new List<GridMap>();
				GenerateMapsRecursively(templates, GridMap.Empty, 0);

				return templates;
			}
		}


		/// <summary>
		/// Generate maps recursively.
		/// </summary>
		/// <param name="templates">All templates.</param>
		/// <param name="template">The current template.</param>
		/// <param name="count">The current count.</param>
		private static void GenerateMapsRecursively(
			IList<GridMap> templates, GridMap template, int count)
		{
			if (count == 9)
			{
				templates.Add(template);
				return;
			}

			for (int i = 0; i < 9; i++)
			{
				int cell = count * 9 + i;
				template.Add(cell);

				if ((new GridMap(cell, false) & template).IsEmpty)
				{
					GenerateMapsRecursively(templates, template, count + 1);
				}

				template.Remove(cell);
			}
		}
	}
}
